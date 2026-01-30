using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;
using Rpg.Common.Effects;

namespace Rpg.Common.Skills.Tier1.Warrior
{
    /// <summary>
    /// Ground Slam - 땅을 강타하여 주변 적에게 피해를 입힌다.
    /// 전사의 범위 공격 스킬.
    /// </summary>
    public class GroundSlam : BaseSkill
    {
        public override string InternalName => "GroundSlam";
        public override string DisplayName => "Ground Slam";
        public override string Description => "Slam the ground with tremendous force, damaging nearby enemies.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Warrior;
        public override int RequiredLevel => 22;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/GroundSlam";
        
        public override float CooldownSeconds => 10f - (CurrentRank * 0.4f); // 10 -> 6초
        public override int ResourceCost => 35 - CurrentRank; // 35 -> 25
        public override ResourceType ResourceType => ResourceType.Stamina;
        
        // 전제조건: PowerStrike 3랭크 이상 필요
        public override string[] PrerequisiteSkills => new[] { "PowerStrike:3" };
        
        // Use the GroundSlamEffect from registry
        public override BaseSkillEffect SkillEffect => SkillEffectRegistry.GroundSlam;

        private static readonly float[] DAMAGE_MULTIPLIER = { 1.2f, 1.4f, 1.6f, 1.8f, 2.0f, 2.2f, 2.5f, 2.8f, 3.1f, 3.5f };
        private static readonly float[] RADIUS = { 80f, 90f, 100f, 110f, 120f, 130f, 140f, 150f, 160f, 180f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            float multiplier = DAMAGE_MULTIPLIER[rank - 1];
            float radius = RADIUS[rank - 1];
            
            int baseDamage = (int)(70 * multiplier);
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly)
                {
                    float dist = Vector2.Distance(player.Center, npc.Center);
                    if (dist <= radius)
                    {
                        // 거리에 따른 피해 감소
                        float distFactor = 1f - (dist / radius) * 0.3f;
                        int finalDamage = (int)(Main.DamageVar(baseDamage, player.luck) * distFactor);
                        npc.SimpleStrikeNPC(finalDamage, player.direction, true, 8f, DamageClass.Melee, true);
                    }
                }
            }
            
            PlayEffects(player, radius);
        }

        private void PlayEffects(Player player, float radius)
        {
            // Use new effect system with intensity based on rank
            SpawnEffect(player, CurrentRank / 3 + 1);
            
            // Additional ground impact for larger radius
            SkillEffect?.CreateGroundImpact(player.Center + new Vector2(0, player.height / 2), radius);
        }
    }
}
