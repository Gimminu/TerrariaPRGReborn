using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Summoner
{
    /// <summary>
    /// Minion Frenzy - 미니언 광란.
    /// 소환수의 공격력을 높인다.
    /// 소환사의 핵심 공격 버프.
    /// 만렙 시 쿨타임 = 지속시간 (30초)
    /// </summary>
    public class MinionFrenzy : BaseSkill
    {
        public override string InternalName => "MinionFrenzy";
        public override string DisplayName => "Minion Frenzy";
        public override string Description => "Drive your minions into a frenzy, increasing their damage.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Summoner;
        public override int RequiredLevel => 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/MinionFrenzy";
        
        // 만렙 시 쿨 = 지속 (30초)
        public override float CooldownSeconds => 45f - (CurrentRank * 1.5f); // 45 -> 30초
        public override int ResourceCost => 35;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 15, 18, 20, 22, 24, 26, 27, 28, 29, 30 };
        private static readonly float[] DAMAGE_BONUS = { 0.10f, 0.12f, 0.14f, 0.16f, 0.18f, 0.20f, 0.22f, 0.24f, 0.27f, 0.30f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            float damageBonus = DAMAGE_BONUS[rank - 1];

            player.AddBuff(BuffID.Summoning, duration);
            var rpgPlayer = player.GetModPlayer<Players.RpgPlayer>();
            rpgPlayer.AddTemporarySummonDamage(damageBonus, duration);

            PlayEffects(player);
            ShowMessage(player, $"Minion Frenzy! (+{(int)(damageBonus * 100)}% Summon DMG)", Color.MediumPurple);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath6, player.position);
            
            // 소환 이펙트
            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Enchanted_Pink, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 0f), 150, Color.Purple, 1.2f);
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
