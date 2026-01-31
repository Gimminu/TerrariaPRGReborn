using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;
using RpgMod.Common.Players;

namespace RpgMod.Common.Skills.Tier1.Warrior
{
    /// <summary>
    /// Endurance - 지구력.
    /// 공격받을 때 피해를 일부 흡수하여 체력으로 회복.
    /// 전사의 생존 버프.
    /// 만렙 시 쿨타임 = 지속시간 (25초)
    /// </summary>
    public class Endurance : BaseSkill
    {
        public override string InternalName => "Endurance";
        public override string DisplayName => "Endurance";
        public override string Description => "Reduce damage taken and gain lifesteal on hits for a short time.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Warrior;
        public override int RequiredLevel => 25;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Endurance";
        
        // 만렙 시 쿨 = 지속 (25초)
        public override float CooldownSeconds => 40f - (CurrentRank * 1.5f); // 40 -> 25초
        public override int ResourceCost => 30;
        public override ResourceType ResourceType => ResourceType.Stamina;
        
        // 전제조건: IronWill 3랭크 이상 필요
        public override string[] PrerequisiteSkills => new[] { "IronWill:3" };

        private static readonly int[] DURATION_SECONDS = { 12, 14, 16, 18, 19, 20, 21, 22, 24, 25 };
        private static readonly float[] LIFESTEAL = { 0.03f, 0.04f, 0.05f, 0.06f, 0.07f, 0.08f, 0.09f, 0.10f, 0.11f, 0.12f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            player.AddBuff(BuffID.Endurance, duration);
            
            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            rpgPlayer.AddTemporaryLifesteal(LIFESTEAL[rank - 1], duration);

            PlayEffects(player);
            ShowMessage(player, "Endurance!", Color.DarkRed);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item4, player.position);
            
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.LifeDrain, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-3f, 0f), 100, Color.DarkRed, 1.2f);
                dust.noGravity = true;
            }
        }

        private void ShowMessage(Player player, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
                CombatText.NewText(player.Hitbox, color, text);
        }
    }
}
