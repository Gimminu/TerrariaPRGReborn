using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;
using Rpg.Common.Players;

namespace Rpg.Common.Skills
{
    /// <summary>
    /// Data-driven skill implementation for common effects.
    /// </summary>
    public class GenericSkill : BaseSkill
    {
        private readonly SkillDefinition definition;

        public GenericSkill(SkillDefinition definition)
        {
            this.definition = definition;
        }

        public override string InternalName => definition.InternalName;
        public override string DisplayName => definition.DisplayName;
        public override string Description => definition.Description;
        public override SkillType SkillType => definition.SkillType;
        public override JobType RequiredJob => definition.RequiredJob;
        public override int RequiredLevel => definition.RequiredLevel;
        public override int SkillPointCost => definition.SkillPointCost;
        public override int MaxRank => definition.MaxRank;
        public override string IconTexture => definition.IconTexture;
        public override float CooldownSeconds => definition.CooldownSeconds;
        public override int ResourceCost => definition.ResourceCost;
        public override ResourceType ResourceType => definition.ResourceType;

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0)
                return;

            float scale = 1f + (CurrentRank - 1) * 0.2f;

            if (definition.PassiveDamageBonus > 0f)
                player.GetDamage(DamageClass.Generic) += definition.PassiveDamageBonus * scale;

            if (definition.PassiveCritBonus > 0f)
                player.GetCritChance(DamageClass.Generic) += definition.PassiveCritBonus * scale;

            if (definition.PassiveAttackSpeedBonus > 0f)
                player.GetAttackSpeed(DamageClass.Generic) += definition.PassiveAttackSpeedBonus * scale;

            if (definition.PassiveMoveSpeedBonus > 0f)
                player.moveSpeed += definition.PassiveMoveSpeedBonus * scale;

            if (definition.PassiveDefenseBonus > 0)
                player.statDefense += (int)System.Math.Round(definition.PassiveDefenseBonus * scale);

            if (definition.PassiveMinionSlots > 0)
                player.maxMinions += (int)System.Math.Round(definition.PassiveMinionSlots * scale);

            if (definition.PassiveManaCostReduction > 0f)
            {
                player.manaCost -= definition.PassiveManaCostReduction * scale;
                if (player.manaCost < 0f)
                    player.manaCost = 0f;
            }

            if (definition.PassiveLifeRegenBonus > 0)
                player.lifeRegen += (int)System.Math.Round(definition.PassiveLifeRegenBonus * scale);

            if (definition.PassiveMaxLifeBonus > 0)
                player.statLifeMax2 += (int)System.Math.Round(definition.PassiveMaxLifeBonus * scale);

            if (definition.PassiveMaxManaBonus > 0)
                player.statManaMax2 += (int)System.Math.Round(definition.PassiveMaxManaBonus * scale);
        }

        protected override void OnActivate(Player player)
        {
            float scale = 1f + (CurrentRank - 1) * 0.2f;

            if (definition.BuffIds != null && definition.BuffIds.Length > 0)
            {
                int duration = (int)System.Math.Round(definition.BuffDurationSeconds * 60f * scale);
                foreach (int buffId in definition.BuffIds)
                {
                    player.AddBuff(buffId, duration);
                }
            }

            if (definition.HealAmount > 0)
            {
                int heal = (int)System.Math.Round(definition.HealAmount * scale);
                player.statLife = System.Math.Min(player.statLife + heal, player.statLifeMax2);
                if (Main.myPlayer == player.whoAmI)
                    CombatText.NewText(player.Hitbox, Color.LightGreen, $"+{heal}");
            }

            if (definition.RestoreMana > 0)
            {
                int mana = (int)System.Math.Round(definition.RestoreMana * scale);
                player.statMana = System.Math.Min(player.statMana + mana, player.statManaMax2);
                if (Main.myPlayer == player.whoAmI)
                    CombatText.NewText(player.Hitbox, Color.LightBlue, $"+{mana} Mana");
            }

            if (definition.RestoreStamina > 0)
            {
                int stamina = (int)System.Math.Round(definition.RestoreStamina * scale);
                var rpgPlayer = player.GetModPlayer<RpgPlayer>();
                rpgPlayer.Stamina = System.Math.Min(rpgPlayer.Stamina + stamina, rpgPlayer.MaxStamina);
            }

            if (definition.AoEDamage > 0 && definition.AoERadius > 0f)
            {
                ApplyAreaDamage(player, (int)System.Math.Round(definition.AoEDamage * scale), definition.AoERadius);
            }
        }

        private void ApplyAreaDamage(Player player, int damage, float radius)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5)
                    continue;

                if (Vector2.Distance(npc.Center, player.Center) > radius)
                    continue;

                int hitDirection = npc.Center.X >= player.Center.X ? 1 : -1;
                float critChance = player.GetCritChance(definition.AoEDamageClass);
                bool crit = Main.rand.NextFloat(100f) < critChance;
                player.ApplyDamageToNPC(npc, damage, definition.AoEKnockback, hitDirection, crit, definition.AoEDamageClass, false);
            }
        }
    }
}
