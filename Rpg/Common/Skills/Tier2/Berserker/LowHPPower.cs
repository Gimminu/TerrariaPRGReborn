using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Berserker
{
    /// <summary>
    /// Low HP Power - Berserker's passive skill.
    /// Gain bonus damage when wounded.
    /// </summary>
    public class LowHPPower : BaseSkill
    {
        public override string InternalName => "LowHPPower";
        public override string DisplayName => "Low HP Power";
        public override string Description => "Gain bonus damage when below 40% HP.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Berserker;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/LowHPPower";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // Scaling per rank
        private static readonly float[] HP_THRESHOLD = { 0.40f, 0.40f, 0.45f, 0.45f, 0.50f };
        private static readonly float[] DAMAGE_BONUS = { 0.15f, 0.20f, 0.25f, 0.30f, 0.40f };

        private bool wasActive;

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0)
                return;

            int rank = CurrentRank;
            float hpPercent = (float)player.statLife / player.statLifeMax2;
            float threshold = HP_THRESHOLD[rank - 1];

            bool isActive = hpPercent <= threshold;

            if (isActive)
            {
                float damageBonus = DAMAGE_BONUS[rank - 1];
                player.GetDamage(DamageClass.Generic) += damageBonus;

                // Visual effect
                if (Main.GameUpdateCount % 15 == 0)
                {
                    Dust dust = Dust.NewDustDirect(
                        player.position,
                        player.width,
                        player.height,
                        DustID.Blood,
                        0,
                        -1f,
                        100,
                        Color.DarkRed,
                        1f
                    );
                    dust.noGravity = true;
                }

                if (!wasActive && Main.myPlayer == player.whoAmI)
                {
                    CombatText.NewText(player.Hitbox, Color.DarkRed, "Low HP Power!", false, false);
                }
            }

            wasActive = isActive;
        }
    }
}
