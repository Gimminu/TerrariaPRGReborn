using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common;
using Rpg.Common.Effects;
using Rpg.Common.Jobs;

namespace Rpg.Common.Base
{
    /// <summary>
    /// Abstract base class for all skills - inherit from this for maximum reusability
    /// Active skills override Activate(), Passive skills override ApplyPassive()
    /// </summary>
    public abstract class BaseSkill : ISkill
    {
        #region ISkill Implementation

        public abstract string InternalName { get; }
        public abstract string DisplayName { get; }
        public abstract string Description { get; }
        public abstract SkillType SkillType { get; }
        public abstract JobType RequiredJob { get; }
        public abstract int RequiredLevel { get; }
        public abstract int SkillPointCost { get; }
        public abstract int MaxRank { get; }
        public abstract string IconTexture { get; }
        public abstract float CooldownSeconds { get; }
        public abstract int ResourceCost { get; }
        public abstract ResourceType ResourceType { get; }
        
        /// <summary>
        /// Effect to use for this skill (override to use a different effect)
        /// </summary>
        public virtual BaseSkillEffect SkillEffect => SkillEffectRegistry.GetEffectForSkillType(SkillType);

        /// <summary>
        /// 스킬 전제조건 - 이 스킬을 배우기 전에 필요한 스킬들
        /// 형식: "스킬이름:최소랭크" (예: "PowerStrike:3")
        /// </summary>
        public virtual string[] PrerequisiteSkills => System.Array.Empty<string>();

        public int CurrentRank { get; set; } = 0;
        public float CurrentCooldown { get; set; } = 0f;

        #endregion

        #region Virtual Methods for Overriding

        /// <summary>
        /// Called when player learns/upgrades this skill
        /// </summary>
        public virtual void OnLearn(Player player, int newRank)
        {
            CurrentRank = newRank;
        }

        /// <summary>
        /// Check if player can learn this skill
        /// </summary>
        public virtual bool CanLearn(Player player)
        {
            var rpgPlayer = player.GetModPlayer<Players.RpgPlayer>();
            
            // Check level
            if (rpgPlayer.Level < RequiredLevel)
                return false;

            // Check job
            if (RequiredJob != JobType.None && !JobDatabase.IsJobInLineage(rpgPlayer.CurrentJob, RequiredJob))
                return false;

            // Check if already max rank
            if (CurrentRank >= MaxRank)
                return false;

            // Check skill points
            if (rpgPlayer.SkillPoints < SkillPointCost)
                return false;
            
            // Check prerequisite skills
            if (!CheckPrerequisites(player))
                return false;

            return true;
        }
        
        /// <summary>
        /// Check if prerequisite skills are met
        /// </summary>
        protected virtual bool CheckPrerequisites(Player player)
        {
            if (PrerequisiteSkills == null || PrerequisiteSkills.Length == 0)
                return true;
                
            var skillManager = player.GetModPlayer<Skills.SkillManager>();
            
            foreach (string prereq in PrerequisiteSkills)
            {
                string[] parts = prereq.Split(':');
                string skillName = parts[0];
                int requiredRank = parts.Length > 1 && int.TryParse(parts[1], out int r) ? r : 1;
                
                if (!skillManager.LearnedSkills.TryGetValue(skillName, out var learnedSkill))
                    return false;
                    
                if (learnedSkill.CurrentRank < requiredRank)
                    return false;
            }
            
            return true;
        }
        
        /// <summary>
        /// Get reason why skill cannot be learned (for UI display)
        /// </summary>
        public virtual string GetCannotLearnReason(Player player)
        {
            var rpgPlayer = player.GetModPlayer<Players.RpgPlayer>();
            
            if (rpgPlayer.Level < RequiredLevel)
                return $"Requires Level {RequiredLevel}";
                
            if (RequiredJob != JobType.None && !JobDatabase.IsJobInLineage(rpgPlayer.CurrentJob, RequiredJob))
                return $"Requires {RequiredJob} job";
                
            if (CurrentRank >= MaxRank)
                return "Max rank reached";
                
            if (rpgPlayer.SkillPoints < SkillPointCost)
                return $"Need {SkillPointCost} SP (have {rpgPlayer.SkillPoints})";
                
            if (!CheckPrerequisites(player))
            {
                foreach (string prereq in PrerequisiteSkills)
                {
                    string[] parts = prereq.Split(':');
                    string skillName = parts[0];
                    int requiredRank = parts.Length > 1 && int.TryParse(parts[1], out int r) ? r : 1;
                    
                    var skillManager = player.GetModPlayer<Skills.SkillManager>();
                    if (!skillManager.LearnedSkills.TryGetValue(skillName, out var learnedSkill) || learnedSkill.CurrentRank < requiredRank)
                    {
                        var prereqSkill = Skills.SkillDatabase.GetSkill(skillName);
                        string prereqName = prereqSkill?.DisplayName ?? skillName;
                        return $"Requires {prereqName} Rank {requiredRank}";
                    }
                }
            }
            
            return "";
        }

        /// <summary>
        /// Check if player can use this skill right now
        /// </summary>
        public virtual bool CanUse(Player player)
        {
            // Not learned
            if (CurrentRank <= 0)
                return false;

            // On cooldown
            if (CurrentCooldown > 0f)
                return false;

            // Check resource cost
            if (!HasEnoughResource(player))
                return false;

            return true;
        }

        /// <summary>
        /// Check if player has enough resource (mana/life/etc)
        /// </summary>
        protected virtual bool HasEnoughResource(Player player)
        {
            int cost = GetEffectiveResourceCost(player);
            return ResourceType switch
            {
                ResourceType.Mana => player.statMana >= cost,
                ResourceType.Life => player.statLife >= cost,
                ResourceType.Stamina => player.GetModPlayer<Players.RpgPlayer>().Stamina >= cost,
                _ => true
            };
        }

        /// <summary>
        /// Consume resource when skill is used
        /// </summary>
        protected virtual void ConsumeResource(Player player)
        {
            int cost = GetEffectiveResourceCost(player);
            switch (ResourceType)
            {
                case ResourceType.Mana:
                    player.statMana -= cost;
                    break;
                case ResourceType.Life:
                    player.statLife -= cost;
                    break;
                case ResourceType.Stamina:
                    player.GetModPlayer<Players.RpgPlayer>().ConsumeStamina(cost);
                    break;
            }
        }

        /// <summary>
        /// Get resource cost after stat-based reductions (e.g., Intelligence mana cost reduction)
        /// </summary>
        public virtual int GetEffectiveResourceCost(Player player)
        {
            int baseCost = ResourceCost;
            if (baseCost <= 0)
                return 0;

            switch (ResourceType)
            {
                case ResourceType.Mana:
                    {
                        float costMultiplier = System.Math.Max(0f, player.manaCost);
                        var rpgPlayer = player.GetModPlayer<Players.RpgPlayer>();
                        float intReduction = 0f;
                        if (rpgPlayer != null)
                        {
                            intReduction = System.Math.Min(
                                rpgPlayer.TotalIntelligence * RpgConstants.INTELLIGENCE_MANA_COST_REDUCTION_PER_POINT,
                                RpgConstants.INTELLIGENCE_MAX_MANA_COST_REDUCTION);
                        }

                        float finalCost = baseCost * costMultiplier * (1f - intReduction);
                        if (finalCost <= 0f)
                            return 0;

                        return System.Math.Max(1, (int)System.Math.Ceiling(finalCost));
                    }
                default:
                    return baseCost;
            }
        }

        /// <summary>
        /// Update cooldown (called every frame)
        /// </summary>
        public virtual void UpdateCooldown()
        {
            if (CurrentCooldown > 0f)
            {
                CurrentCooldown -= 1f / 60f; // 60 FPS
                if (CurrentCooldown < 0f)
                    CurrentCooldown = 0f;
            }
        }

        #endregion

        #region Skill Execution (Override in derived classes)

        /// <summary>
        /// Active skill activation - OVERRIDE THIS for active skills
        /// </summary>
        public virtual void Activate(Player player)
        {
            if (!CanUse(player))
                return;

            ConsumeResource(player);
            CurrentCooldown = GetActualCooldown(player);

            // Enhanced activation effects for RPG feel
            PlayActivationEffects(player);

            // Play activation sound/visual
            OnActivate(player);

            if (SkillEffect != null)
            {
                SpawnEffect(player, 1);
            }
        }

        /// <summary>
        /// Play enhanced activation effects for skills
        /// </summary>
        private void PlayActivationEffects(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // Different sound effects based on skill type
            switch (SkillType)
            {
                case SkillType.Active:
                    SoundEngine.PlaySound(SoundID.Item43, player.position); // Magic activation sound
                    break;
                case SkillType.Passive:
                    // Passives don't play activation sounds
                    break;
            }

            // Visual effects based on skill type
            Color effectColor = GetSkillEffectColor();
            
            // Create skill activation particles
            for (int i = 0; i < 15; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4f, 4f);
                Dust dust = Dust.NewDustPerfect(
                    player.Center,
                    DustID.MagicMirror,
                    velocity,
                    Scale: Main.rand.NextFloat(0.8f, 1.2f)
                );
                dust.noGravity = true;
                dust.color = effectColor;
                dust.fadeIn = 0.3f;
            }
        }

        /// <summary>
        /// Get color for skill activation effects based on skill type
        /// </summary>
        private Color GetSkillEffectColor()
        {
            switch (SkillType)
            {
                case SkillType.Active:
                    return new Color(100, 200, 255); // Blue for active skills
                case SkillType.Passive:
                    return new Color(255, 255, 100); // Yellow for passive skills
                default:
                    return Color.White;
            }
        }

        /// <summary>
        /// Override this for skill logic
        /// </summary>
        protected virtual void OnActivate(Player player)
        {
            // Implement skill effect here
        }

        /// <summary>
        /// Passive skill effect - OVERRIDE THIS for passive skills
        /// Called every frame or on specific events
        /// </summary>
        public virtual void ApplyPassive(Player player)
        {
            // Implement passive effect here
        }

        /// <summary>
        /// Called when skill hits an enemy
        /// </summary>
        public virtual void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            // Override for on-hit effects
        }

        /// <summary>
        /// Called when player takes damage
        /// </summary>
        public virtual void OnPlayerHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            // Override for defensive skills
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Get skill power scaled by rank
        /// </summary>
        protected float GetScaledPower(float basePower)
        {
            return basePower * (1f + (CurrentRank - 1) * 0.25f); // 25% per rank for sharper scaling
        }

        /// <summary>
        /// Get cooldown with CDR applied
        /// </summary>
        protected float GetActualCooldown(Player player)
        {
            float cdr = player.GetModPlayer<Players.RpgPlayer>().CooldownReduction;
            return CooldownSeconds * (1f - cdr);
        }

        /// <summary>
        /// Spawn visual effect at player position using the skill's effect
        /// </summary>
        protected void SpawnEffect(Player player, int intensity = 1)
        {
            SkillEffect?.PlayOnPlayer(player, intensity);
        }
        
        /// <summary>
        /// Spawn visual effect at a specific position
        /// </summary>
        protected void SpawnEffectAt(Vector2 position, int intensity = 1)
        {
            SkillEffect?.Play(position, intensity);
        }
        
        /// <summary>
        /// Spawn visual effect on an NPC
        /// </summary>
        protected void SpawnEffectOnNPC(NPC npc, int intensity = 1)
        {
            SkillEffect?.PlayOnNPC(npc, intensity);
        }

        #endregion
    }
}
