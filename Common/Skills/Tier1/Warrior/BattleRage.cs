using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier1.Warrior
{
    /// <summary>
    /// Battle Rage - Warrior's offensive passive.
    /// Increases melee damage and critical chance.
    /// </summary>
    public class BattleRage : BaseSkill
    {
        public override string InternalName => "BattleRage";
        public override string DisplayName => "Battle Rage";
        public override string Description => "Passively increase melee damage and critical strike chance.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Warrior;
        public override int RequiredLevel => RpgConstants.FIRST_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/BattleRage";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // Scaling per rank
        private static readonly float[] DAMAGE_BONUS = { 0.04f, 0.06f, 0.08f, 0.10f, 0.14f };
        private static readonly float[] CRIT_BONUS = { 3f, 4f, 5f, 7f, 10f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0)
                return;

            int rank = CurrentRank;

            // Apply melee damage bonus
            player.GetDamage(DamageClass.Melee) += DAMAGE_BONUS[rank - 1];

            // Apply critical chance bonus
            player.GetCritChance(DamageClass.Melee) += CRIT_BONUS[rank - 1];

            // Visual effect when attacking with high crit chance
            if (player.itemAnimation > 0 && Main.GameUpdateCount % 10 == 0)
            {
                CreateRageEffect(player);
            }
        }

        private void CreateRageEffect(Player player)
        {
            if (CurrentRank >= 3)
            {
                Dust dust = Dust.NewDustDirect(
                    player.position,
                    player.width,
                    player.height,
                    DustID.Torch,
                    0,
                    -1f,
                    100,
                    Color.OrangeRed,
                    0.8f
                );
                dust.noGravity = true;
            }
        }
    }
}
