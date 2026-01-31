using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Cleric
{
    /// <summary>
    /// Purify - 정화.
    /// 디버프 제거.
    /// 클레릭의 디버프 클렌즈.
    /// </summary>
    public class Purify : BaseSkill
    {
        public override string InternalName => "Purify";
        public override string DisplayName => "Purify";
        public override string Description => "Remove harmful debuffs from yourself.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Cleric;
        public override int RequiredLevel => 72;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Purify";
        
        public override float CooldownSeconds => 30f - (CurrentRank * 1.5f); // 30 -> 15초
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Mana;

        // 제거할 디버프 목록
        private static readonly int[] DEBUFFS_TO_REMOVE = {
            BuffID.Poisoned, BuffID.OnFire, BuffID.Venom, BuffID.CursedInferno,
            BuffID.Bleeding, BuffID.Confused, BuffID.Slow, BuffID.Weak,
            BuffID.BrokenArmor, BuffID.Silenced, BuffID.Cursed, BuffID.Darkness,
            BuffID.Frozen, BuffID.Chilled, BuffID.Electrified, BuffID.Burning
        };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int maxDebuffsRemoved = 2 + rank / 2; // 2-7개
            int removed = 0;
            
            foreach (int debuffId in DEBUFFS_TO_REMOVE)
            {
                if (removed >= maxDebuffsRemoved) break;
                
                if (player.HasBuff(debuffId))
                {
                    player.ClearBuff(debuffId);
                    removed++;
                }
            }
            
            PlayEffects(player);
            ShowMessage(player, removed > 0 ? "Purified!" : "Pure!", Color.Gold);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item4, player.position);
            
            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.GoldFlame, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-3f, 0f), 150, Color.White, 1.3f);
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
