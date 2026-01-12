using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Rpg.Common.Effects
{
    /// <summary>
    /// Centralized skill visual and audio effects
    /// </summary>
    public static class SkillEffects
    {
        #region Healing Effects
        
        /// <summary>
        /// Play healing visual effect
        /// </summary>
        public static void PlayHealEffect(Player player, int healAmount)
        {
            // Green particles rising
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-4f, -1f)
                );
                
                Dust dust = Dust.NewDustDirect(
                    player.position,
                    player.width,
                    player.height,
                    DustID.HealingPlus,
                    velocity.X,
                    velocity.Y,
                    100,
                    default,
                    1.5f
                );
                dust.noGravity = true;
            }
            
            // Sound
            SoundEngine.PlaySound(SoundID.Item4, player.position);
            
            // Combat text
            if (Main.myPlayer == player.whoAmI)
            {
                CombatText.NewText(player.Hitbox, Color.LightGreen, $"+{healAmount}");
            }
        }
        
        #endregion
        
        #region Buff Effects
        
        /// <summary>
        /// Play buff activation effect
        /// </summary>
        public static void PlayBuffEffect(Player player, Color color)
        {
            // Sparkle particles
            for (int i = 0; i < 25; i++)
            {
                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-3f, 3f),
                    Main.rand.NextFloat(-3f, 3f)
                );
                
                Dust dust = Dust.NewDustDirect(
                    player.position,
                    player.width,
                    player.height,
                    DustID.MagicMirror,
                    velocity.X,
                    velocity.Y,
                    150,
                    color,
                    1.2f
                );
                dust.noGravity = true;
            }
            
            SoundEngine.PlaySound(SoundID.Item25, player.position);
        }
        
        /// <summary>
        /// Play defensive buff effect
        /// </summary>
        public static void PlayDefenseBuffEffect(Player player)
        {
            // Shield-like particles
            for (int i = 0; i < 30; i++)
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                float distance = Main.rand.NextFloat(40f, 60f);
                Vector2 offset = new Vector2(
                    (float)System.Math.Cos(angle) * distance,
                    (float)System.Math.Sin(angle) * distance
                );
                
                Vector2 pos = player.Center + offset;
                
                Dust dust = Dust.NewDustDirect(
                    pos,
                    1, 1,
                    DustID.Electric,
                    -offset.X * 0.05f,
                    -offset.Y * 0.05f,
                    100,
                    Color.Cyan,
                    1.0f
                );
                dust.noGravity = true;
            }
            
            SoundEngine.PlaySound(SoundID.Item28, player.position);
        }
        
        /// <summary>
        /// Play attack buff effect
        /// </summary>
        public static void PlayAttackBuffEffect(Player player)
        {
            // Red/orange fire particles
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-4f, 0f)
                );
                
                Dust dust = Dust.NewDustDirect(
                    player.position,
                    player.width,
                    player.height,
                    DustID.Torch,
                    velocity.X,
                    velocity.Y,
                    100,
                    default,
                    1.3f
                );
                dust.noGravity = true;
            }
            
            SoundEngine.PlaySound(SoundID.Item45, player.position);
        }
        
        #endregion
        
        #region Attack Effects
        
        /// <summary>
        /// Play melee skill effect
        /// </summary>
        public static void PlayMeleeSkillEffect(Player player, float radius)
        {
            // Slash effect particles in a circle
            for (int i = 0; i < 40; i++)
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                float dist = Main.rand.NextFloat(radius * 0.3f, radius);
                
                Vector2 offset = new Vector2(
                    (float)System.Math.Cos(angle) * dist,
                    (float)System.Math.Sin(angle) * dist
                );
                
                Vector2 velocity = offset * 0.1f;
                
                Dust dust = Dust.NewDustDirect(
                    player.Center + offset,
                    1, 1,
                    DustID.Smoke,
                    velocity.X,
                    velocity.Y,
                    100,
                    default,
                    1.5f
                );
                dust.noGravity = true;
            }
            
            SoundEngine.PlaySound(SoundID.Item1 with { Pitch = -0.3f }, player.position);
        }
        
        /// <summary>
        /// Play magic skill effect
        /// </summary>
        public static void PlayMagicSkillEffect(Player player, float radius, Color color)
        {
            // Magic explosion
            for (int i = 0; i < 50; i++)
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                float dist = Main.rand.NextFloat(0f, radius);
                
                Vector2 offset = new Vector2(
                    (float)System.Math.Cos(angle) * dist,
                    (float)System.Math.Sin(angle) * dist
                );
                
                Dust dust = Dust.NewDustDirect(
                    player.Center + offset,
                    1, 1,
                    DustID.MagicMirror,
                    offset.X * 0.05f,
                    offset.Y * 0.05f,
                    150,
                    color,
                    1.2f
                );
                dust.noGravity = true;
            }
            
            SoundEngine.PlaySound(SoundID.Item8, player.position);
        }
        
        /// <summary>
        /// Play ranged skill effect
        /// </summary>
        public static void PlayRangedSkillEffect(Player player)
        {
            // Arrow trail particles
            for (int i = 0; i < 15; i++)
            {
                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-1f, 1f),
                    Main.rand.NextFloat(-2f, 0f)
                );
                
                Dust dust = Dust.NewDustDirect(
                    player.position,
                    player.width,
                    player.height,
                    DustID.GreenTorch,
                    velocity.X,
                    velocity.Y,
                    100,
                    default,
                    1.0f
                );
                dust.noGravity = true;
            }
            
            SoundEngine.PlaySound(SoundID.Item5, player.position);
        }
        
        /// <summary>
        /// Play summon skill effect
        /// </summary>
        public static void PlaySummonSkillEffect(Player player)
        {
            // Mystical summoning particles
            for (int i = 0; i < 30; i++)
            {
                float angle = (float)i / 30f * MathHelper.TwoPi;
                float dist = 50f;
                
                Vector2 offset = new Vector2(
                    (float)System.Math.Cos(angle) * dist,
                    (float)System.Math.Sin(angle) * dist
                );
                
                Dust dust = Dust.NewDustDirect(
                    player.Center + offset,
                    1, 1,
                    DustID.Enchanted_Pink,
                    0f,
                    -2f,
                    100,
                    default,
                    1.3f
                );
                dust.noGravity = true;
            }
            
            SoundEngine.PlaySound(SoundID.Item44, player.position);
        }
        
        #endregion
        
        #region Level Up Effects
        
        /// <summary>
        /// Play level up celebration effect
        /// </summary>
        public static void PlayLevelUpEffect(Player player, int newLevel)
        {
            // Grand particle explosion
            for (int i = 0; i < 60; i++)
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                float speed = Main.rand.NextFloat(2f, 6f);
                
                Vector2 velocity = new Vector2(
                    (float)System.Math.Cos(angle) * speed,
                    (float)System.Math.Sin(angle) * speed
                );
                
                int dustType = Main.rand.Next(new[] { 
                    DustID.GoldCoin, 
                    DustID.YellowTorch, 
                    DustID.Enchanted_Gold 
                });
                
                Dust dust = Dust.NewDustDirect(
                    player.Center,
                    1, 1,
                    dustType,
                    velocity.X,
                    velocity.Y,
                    100,
                    default,
                    1.5f
                );
                dust.noGravity = true;
            }
            
            // Extra sparkles
            for (int i = 0; i < 20; i++)
            {
                Gore.NewGore(
                    player.GetSource_FromThis(),
                    player.Center,
                    new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-5f, -2f)),
                    Main.rand.Next(16, 18)
                );
            }
            
            SoundEngine.PlaySound(SoundID.Item4 with { Pitch = 0.5f }, player.position);
            SoundEngine.PlaySound(SoundID.Coins, player.position);
            
            // Level up text
            if (Main.myPlayer == player.whoAmI)
            {
                CombatText.NewText(
                    player.Hitbox,
                    Color.Gold,
                    $"LEVEL {newLevel}!",
                    true
                );
            }
        }
        
        #endregion
        
        #region Job Advancement Effects
        
        /// <summary>
        /// Play job advancement effect
        /// </summary>
        public static void PlayJobAdvancementEffect(Player player, string jobName)
        {
            // Massive light burst
            for (int i = 0; i < 100; i++)
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                float speed = Main.rand.NextFloat(3f, 8f);
                
                Vector2 velocity = new Vector2(
                    (float)System.Math.Cos(angle) * speed,
                    (float)System.Math.Sin(angle) * speed
                );
                
                Dust dust = Dust.NewDustDirect(
                    player.Center,
                    1, 1,
                    DustID.RainbowMk2,
                    velocity.X,
                    velocity.Y,
                    0,
                    default,
                    2.0f
                );
                dust.noGravity = true;
            }
            
            // Light pillar effect
            for (int y = 0; y < 20; y++)
            {
                Vector2 pos = player.Center + new Vector2(0, -y * 20);
                
                Dust dust = Dust.NewDustDirect(
                    pos,
                    1, 1,
                    DustID.GoldFlame,
                    0f,
                    -2f,
                    100,
                    default,
                    2.0f
                );
                dust.noGravity = true;
            }
            
            SoundEngine.PlaySound(SoundID.Item119, player.position);
            
            if (Main.myPlayer == player.whoAmI)
            {
                CombatText.NewText(
                    player.Hitbox,
                    Color.Cyan,
                    $"Now: {jobName}!",
                    true
                );
            }
        }
        
        #endregion
        
        #region Skill Learning Effects
        
        /// <summary>
        /// Play skill learned effect
        /// </summary>
        public static void PlaySkillLearnedEffect(Player player, string skillName)
        {
            // Book/scroll particles
            for (int i = 0; i < 25; i++)
            {
                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-3f, 3f),
                    Main.rand.NextFloat(-4f, -1f)
                );
                
                Dust dust = Dust.NewDustDirect(
                    player.position,
                    player.width,
                    player.height,
                    DustID.BlueTorch,
                    velocity.X,
                    velocity.Y,
                    100,
                    default,
                    1.2f
                );
                dust.noGravity = true;
            }
            
            SoundEngine.PlaySound(SoundID.Item35, player.position);
            
            if (Main.myPlayer == player.whoAmI)
            {
                CombatText.NewText(
                    player.Hitbox,
                    Color.LightBlue,
                    $"Learned: {skillName}"
                );
            }
        }
        
        #endregion
        
        #region XP Gain Effects
        
        /// <summary>
        /// Play XP gain floating text
        /// </summary>
        public static void PlayXPGainEffect(Player player, long xpAmount, bool isBonusXP = false)
        {
            if (Main.myPlayer != player.whoAmI)
                return;
            
            Color color = isBonusXP ? Color.Cyan : Color.LightGreen;
            string prefix = isBonusXP ? "+" : "+";
            
            CombatText.NewText(
                player.Hitbox,
                color,
                $"{prefix}{xpAmount} XP"
            );
        }
        
        #endregion
        
        #region Damage Effects
        
        /// <summary>
        /// Play AoE damage effect
        /// </summary>
        public static void PlayAoEDamageEffect(Vector2 center, float radius, Color color)
        {
            // Expanding ring of particles
            int particleCount = (int)(radius / 2);
            
            for (int i = 0; i < particleCount; i++)
            {
                float angle = (float)i / particleCount * MathHelper.TwoPi;
                
                Vector2 offset = new Vector2(
                    (float)System.Math.Cos(angle) * radius,
                    (float)System.Math.Sin(angle) * radius
                );
                
                Dust dust = Dust.NewDustDirect(
                    center + offset,
                    1, 1,
                    DustID.Torch,
                    offset.X * 0.02f,
                    offset.Y * 0.02f,
                    100,
                    color,
                    1.5f
                );
                dust.noGravity = true;
            }
            
            // Impact dust
            for (int i = 0; i < 20; i++)
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                float dist = Main.rand.NextFloat(0, radius * 0.5f);
                
                Vector2 pos = center + new Vector2(
                    (float)System.Math.Cos(angle) * dist,
                    (float)System.Math.Sin(angle) * dist
                );
                
                Dust.NewDustDirect(
                    pos,
                    1, 1,
                    DustID.Smoke,
                    0f, -1f,
                    100,
                    default,
                    1.0f
                );
            }
            
            SoundEngine.PlaySound(SoundID.Item14, center);
        }
        
        #endregion
    }
}
