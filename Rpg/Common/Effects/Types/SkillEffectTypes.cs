using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Rpg.Common.Effects.Types
{
    #region Healing Effects
    
    /// <summary>
    /// Base healing effect - green/white particles rising upward
    /// </summary>
    public class HealEffect : BaseSkillEffect
    {
        public override string EffectId => "Heal";
        public override Color PrimaryColor => new Color(100, 255, 100);
        public override Color SecondaryColor => Color.White;
        public override int DustType => DustID.HealingPlus;
        public override SoundStyle? EffectSound => SoundID.Item4;
        
        protected override void SpawnParticles(Vector2 position, int intensity)
        {
            int count = 15 * intensity;
            for (int i = 0; i < count; i++)
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-3f, -1f));
                Dust dust = Dust.NewDustPerfect(
                    position + Main.rand.NextVector2Circular(20f, 30f),
                    DustType,
                    velocity,
                    0,
                    PrimaryColor,
                    1.5f * ScaleMultiplier
                );
                dust.noGravity = true;
                dust.fadeIn = 1.2f;
            }
        }
    }
    
    /// <summary>
    /// Divine healing - golden holy light
    /// </summary>
    public class DivineHealEffect : HealEffect
    {
        public override string EffectId => "DivineHeal";
        public override Color PrimaryColor => new Color(255, 215, 0);
        public override Color SecondaryColor => new Color(255, 255, 200);
        public override int DustType => DustID.GoldFlame;
        
        public override void Play(Vector2 position, int intensity = 1)
        {
            base.Play(position, intensity);
            // Add extra holy light spiral
            CreateSpiral(position, 20 * intensity, 50f);
        }
    }
    
    /// <summary>
    /// Mass healing - wide area effect
    /// </summary>
    public class MassHealEffect : HealEffect
    {
        public override string EffectId => "MassHeal";
        public override float ScaleMultiplier => 1.5f;
        
        public override void Play(Vector2 position, int intensity = 1)
        {
            base.Play(position, intensity);
            CreateCircularBurst(position, 24, 30f, 3f);
            CreateCircularBurst(position, 16, 50f, 2f);
        }
    }
    
    #endregion

    #region Damage Effects
    
    /// <summary>
    /// Base physical damage effect
    /// </summary>
    public class PhysicalDamageEffect : BaseSkillEffect
    {
        public override string EffectId => "PhysicalDamage";
        public override Color PrimaryColor => new Color(255, 100, 100);
        public override Color SecondaryColor => new Color(200, 50, 50);
        public override int DustType => DustID.Blood;
        public override SoundStyle? EffectSound => SoundID.Item1;
        
        protected override void SpawnParticles(Vector2 position, int intensity)
        {
            int count = 12 * intensity;
            for (int i = 0; i < count; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(5f, 5f);
                Dust dust = Dust.NewDustPerfect(position, DustType, velocity, 0, PrimaryColor, 1.3f);
                dust.noGravity = false;
            }
        }
    }
    
    /// <summary>
    /// Critical hit effect - more dramatic
    /// </summary>
    public class CriticalHitEffect : PhysicalDamageEffect
    {
        public override string EffectId => "CriticalHit";
        public override Color PrimaryColor => new Color(255, 50, 50);
        public override SoundStyle? EffectSound => SoundID.Item89;
        public override float ScaleMultiplier => 1.5f;
        
        public override void Play(Vector2 position, int intensity = 1)
        {
            base.Play(position, intensity);
            // Star burst for critical
            CreateCircularBurst(position, 8, 10f, 6f);
        }
    }
    
    /// <summary>
    /// Fire damage effect
    /// </summary>
    public class FireDamageEffect : BaseSkillEffect
    {
        public override string EffectId => "FireDamage";
        public override Color PrimaryColor => new Color(255, 100, 0);
        public override Color SecondaryColor => new Color(255, 200, 0);
        public override int DustType => DustID.Torch;
        public override SoundStyle? EffectSound => SoundID.Item45;
        
        protected override void SpawnParticles(Vector2 position, int intensity)
        {
            int count = 20 * intensity;
            for (int i = 0; i < count; i++)
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-4f, -1f));
                int dustType = Main.rand.NextBool() ? DustID.Torch : DustID.Smoke;
                Color color = Color.Lerp(PrimaryColor, SecondaryColor, Main.rand.NextFloat());
                
                Dust dust = Dust.NewDustPerfect(
                    position + Main.rand.NextVector2Circular(15f, 15f),
                    dustType,
                    velocity,
                    0,
                    color,
                    1.5f * ScaleMultiplier
                );
                dust.noGravity = true;
            }
        }
    }
    
    /// <summary>
    /// Ice damage effect
    /// </summary>
    public class IceDamageEffect : BaseSkillEffect
    {
        public override string EffectId => "IceDamage";
        public override Color PrimaryColor => new Color(100, 200, 255);
        public override Color SecondaryColor => Color.White;
        public override int DustType => DustID.IceTorch;
        public override SoundStyle? EffectSound => SoundID.Item28;
        
        protected override void SpawnParticles(Vector2 position, int intensity)
        {
            int count = 18 * intensity;
            for (int i = 0; i < count; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(3f, 3f);
                Dust dust = Dust.NewDustPerfect(
                    position + Main.rand.NextVector2Circular(20f, 20f),
                    DustType,
                    velocity,
                    0,
                    Color.Lerp(PrimaryColor, SecondaryColor, Main.rand.NextFloat()),
                    1.3f * ScaleMultiplier
                );
                dust.noGravity = true;
            }
        }
    }
    
    /// <summary>
    /// Lightning damage effect
    /// </summary>
    public class LightningDamageEffect : BaseSkillEffect
    {
        public override string EffectId => "LightningDamage";
        public override Color PrimaryColor => new Color(255, 255, 100);
        public override Color SecondaryColor => new Color(100, 100, 255);
        public override int DustType => DustID.Electric;
        public override SoundStyle? EffectSound => SoundID.Item122;
        
        protected override void SpawnParticles(Vector2 position, int intensity)
        {
            int count = 15 * intensity;
            for (int i = 0; i < count; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(6f, 6f);
                Dust dust = Dust.NewDustPerfect(position, DustType, velocity, 0, PrimaryColor, 1.0f);
                dust.noGravity = true;
            }
        }
    }
    
    /// <summary>
    /// Dark/Shadow damage effect
    /// </summary>
    public class ShadowDamageEffect : BaseSkillEffect
    {
        public override string EffectId => "ShadowDamage";
        public override Color PrimaryColor => new Color(80, 0, 120);
        public override Color SecondaryColor => new Color(150, 0, 200);
        public override int DustType => DustID.Shadowflame;
        public override SoundStyle? EffectSound => SoundID.Item104;
        
        protected override void SpawnParticles(Vector2 position, int intensity)
        {
            int count = 16 * intensity;
            for (int i = 0; i < count; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4f, 4f);
                Dust dust = Dust.NewDustPerfect(
                    position + Main.rand.NextVector2Circular(15f, 15f),
                    DustType,
                    velocity,
                    100,
                    default,
                    1.4f * ScaleMultiplier
                );
                dust.noGravity = true;
            }
        }
    }
    
    #endregion

    #region Buff Effects
    
    /// <summary>
    /// Base buff application effect
    /// </summary>
    public class BuffEffect : BaseSkillEffect
    {
        public override string EffectId => "Buff";
        public override Color PrimaryColor => new Color(100, 150, 255);
        public override Color SecondaryColor => Color.White;
        public override int DustType => DustID.MagicMirror;
        public override SoundStyle? EffectSound => SoundID.Item25;
        
        public override void PlayOnPlayer(Player player, int intensity = 1)
        {
            // Spiral around player
            CreateSpiral(player.Center, 15 * intensity, 40f);
            PlaySound(player.Center);
        }
    }
    
    /// <summary>
    /// Power/Strength buff effect - red/orange
    /// </summary>
    public class PowerBuffEffect : BuffEffect
    {
        public override string EffectId => "PowerBuff";
        public override Color PrimaryColor => new Color(255, 100, 50);
        public override Color SecondaryColor => new Color(255, 200, 100);
        public override int DustType => DustID.Torch;
    }
    
    /// <summary>
    /// Defense buff effect - grey/blue
    /// </summary>
    public class DefenseBuffEffect : BuffEffect
    {
        public override string EffectId => "DefenseBuff";
        public override Color PrimaryColor => new Color(150, 150, 200);
        public override Color SecondaryColor => new Color(200, 200, 255);
        public override int DustType => DustID.Silver;
        
        public override void PlayOnPlayer(Player player, int intensity = 1)
        {
            // Shield-like circular effect
            CreateCircularBurst(player.Center, 16, 25f, 0.5f);
            PlaySound(player.Center);
        }
    }
    
    /// <summary>
    /// Speed buff effect - cyan/white
    /// </summary>
    public class SpeedBuffEffect : BuffEffect
    {
        public override string EffectId => "SpeedBuff";
        public override Color PrimaryColor => new Color(100, 255, 255);
        public override Color SecondaryColor => Color.White;
        public override int DustType => DustID.IceTorch;
        public override SoundStyle? EffectSound => SoundID.Item66;
    }
    
    /// <summary>
    /// Rage/Berserk buff effect - intense red
    /// </summary>
    public class RageBuffEffect : BuffEffect
    {
        public override string EffectId => "RageBuff";
        public override Color PrimaryColor => new Color(255, 0, 0);
        public override Color SecondaryColor => new Color(255, 100, 0);
        public override int DustType => DustID.LifeDrain;
        public override SoundStyle? EffectSound => SoundID.Roar;
        
        public override void PlayOnPlayer(Player player, int intensity = 1)
        {
            base.PlayOnPlayer(player, intensity);
            // Extra fiery particles
            for (int i = 0; i < 10 * intensity; i++)
            {
                Vector2 velocity = new Vector2(0, Main.rand.NextFloat(-3f, -1f));
                Dust dust = Dust.NewDustPerfect(
                    player.Center + new Vector2(Main.rand.NextFloat(-player.width/2, player.width/2), player.height/2),
                    DustID.Torch,
                    velocity,
                    0,
                    PrimaryColor,
                    1.5f
                );
                dust.noGravity = true;
            }
        }
    }
    
    #endregion

    #region Debuff Effects
    
    /// <summary>
    /// Base debuff effect
    /// </summary>
    public class DebuffEffect : BaseSkillEffect
    {
        public override string EffectId => "Debuff";
        public override Color PrimaryColor => new Color(100, 0, 100);
        public override Color SecondaryColor => new Color(50, 50, 50);
        public override int DustType => DustID.Poisoned;
        public override SoundStyle? EffectSound => SoundID.Item17;
    }
    
    /// <summary>
    /// Poison debuff effect
    /// </summary>
    public class PoisonEffect : DebuffEffect
    {
        public override string EffectId => "Poison";
        public override Color PrimaryColor => new Color(100, 200, 0);
        public override Color SecondaryColor => new Color(50, 100, 0);
        public override int DustType => DustID.Poisoned;
    }
    
    /// <summary>
    /// Curse debuff effect
    /// </summary>
    public class CurseEffect : DebuffEffect
    {
        public override string EffectId => "Curse";
        public override Color PrimaryColor => new Color(50, 0, 80);
        public override Color SecondaryColor => new Color(100, 0, 150);
        public override int DustType => DustID.Shadowflame;
        public override SoundStyle? EffectSound => SoundID.Item104;
    }
    
    /// <summary>
    /// Slow/Freeze debuff effect
    /// </summary>
    public class SlowEffect : DebuffEffect
    {
        public override string EffectId => "Slow";
        public override Color PrimaryColor => new Color(150, 200, 255);
        public override Color SecondaryColor => Color.White;
        public override int DustType => DustID.IceTorch;
        public override SoundStyle? EffectSound => SoundID.Item30;
    }
    
    #endregion

    #region Movement Effects
    
    /// <summary>
    /// Teleport/Blink effect
    /// </summary>
    public class TeleportEffect : BaseSkillEffect
    {
        public override string EffectId => "Teleport";
        public override Color PrimaryColor => new Color(100, 100, 255);
        public override Color SecondaryColor => new Color(200, 200, 255);
        public override int DustType => DustID.MagicMirror;
        public override SoundStyle? EffectSound => SoundID.Item6;
        
        /// <summary>
        /// Play both departure and arrival effects
        /// </summary>
        public void PlayTeleport(Vector2 from, Vector2 to)
        {
            // Departure effect
            Play(from, 1);
            CreateCircularBurst(from, 12, 20f, 4f);
            
            // Arrival effect
            Play(to, 1);
            CreateCircularBurst(to, 12, 5f, 3f);
        }
    }
    
    /// <summary>
    /// Dash/Charge effect
    /// </summary>
    public class DashEffect : BaseSkillEffect
    {
        public override string EffectId => "Dash";
        public override Color PrimaryColor => new Color(255, 200, 100);
        public override Color SecondaryColor => new Color(255, 255, 200);
        public override int DustType => DustID.Torch;
        public override SoundStyle? EffectSound => SoundID.Item66;
        
        /// <summary>
        /// Create a trail effect between two points
        /// </summary>
        public void PlayTrail(Vector2 from, Vector2 to, int segments = 10)
        {
            PlaySound(from);
            
            for (int i = 0; i < segments; i++)
            {
                float t = i / (float)segments;
                Vector2 pos = Vector2.Lerp(from, to, t);
                
                for (int j = 0; j < 3; j++)
                {
                    Dust dust = Dust.NewDustPerfect(
                        pos + Main.rand.NextVector2Circular(5f, 5f),
                        DustType,
                        Vector2.Zero,
                        0,
                        Color.Lerp(PrimaryColor, SecondaryColor, t),
                        1.2f
                    );
                    dust.noGravity = true;
                    dust.fadeIn = 0.5f;
                }
            }
        }
    }
    
    /// <summary>
    /// Roll/Dodge effect
    /// </summary>
    public class DodgeEffect : BaseSkillEffect
    {
        public override string EffectId => "Dodge";
        public override Color PrimaryColor => new Color(200, 200, 200);
        public override Color SecondaryColor => new Color(150, 150, 150);
        public override int DustType => DustID.Smoke;
        public override SoundStyle? EffectSound => SoundID.Item7;
        
        protected override void SpawnParticles(Vector2 position, int intensity)
        {
            // Smoke puff
            int count = 8 * intensity;
            for (int i = 0; i < count; i++)
            {
                Vector2 velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-1f, 1f));
                Dust dust = Dust.NewDustPerfect(position, DustType, velocity, 150, default, 1.5f);
                dust.noGravity = false;
            }
        }
    }
    
    #endregion

    #region Summon Effects
    
    /// <summary>
    /// Summon creature effect
    /// </summary>
    public class SummonEffect : BaseSkillEffect
    {
        public override string EffectId => "Summon";
        public override Color PrimaryColor => new Color(100, 200, 255);
        public override Color SecondaryColor => new Color(200, 100, 255);
        public override int DustType => DustID.MagicMirror;
        public override SoundStyle? EffectSound => SoundID.Item44;
        
        public override void Play(Vector2 position, int intensity = 1)
        {
            PlaySound(position);
            // Rising spiral for summoning
            CreateSpiral(position, 25 * intensity, 60f);
            // Ground circle
            CreateCircularBurst(position, 16, 30f, 1f);
        }
    }
    
    /// <summary>
    /// Undead summon effect - darker
    /// </summary>
    public class UndeadSummonEffect : SummonEffect
    {
        public override string EffectId => "UndeadSummon";
        public override Color PrimaryColor => new Color(100, 50, 150);
        public override Color SecondaryColor => new Color(50, 150, 50);
        public override int DustType => DustID.Shadowflame;
        public override SoundStyle? EffectSound => SoundID.NPCDeath2;
    }
    
    #endregion

    #region Impact Effects
    
    /// <summary>
    /// Ground slam/impact effect
    /// </summary>
    public class GroundSlamEffect : BaseSkillEffect
    {
        public override string EffectId => "GroundSlam";
        public override Color PrimaryColor => new Color(139, 90, 43);
        public override Color SecondaryColor => new Color(100, 70, 30);
        public override int DustType => DustID.Dirt;
        public override SoundStyle? EffectSound => SoundID.Item14;
        
        public override void Play(Vector2 position, int intensity = 1)
        {
            PlaySound(position);
            CreateGroundImpact(position, 20 * intensity, 8f);
            // Shockwave
            CreateCircularBurst(position, 12, 10f, 5f);
        }
    }
    
    /// <summary>
    /// Shield bash/block effect
    /// </summary>
    public class ShieldEffect : BaseSkillEffect
    {
        public override string EffectId => "Shield";
        public override Color PrimaryColor => new Color(200, 200, 220);
        public override Color SecondaryColor => new Color(150, 150, 180);
        public override int DustType => DustID.Silver;
        public override SoundStyle? EffectSound => SoundID.Item37;
        
        public override void Play(Vector2 position, int intensity = 1)
        {
            PlaySound(position);
            // Metallic sparks
            for (int i = 0; i < 8 * intensity; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4f, 4f);
                Dust dust = Dust.NewDustPerfect(position, DustID.Silver, velocity, 0, PrimaryColor, 1.2f);
                dust.noGravity = false;
            }
        }
    }
    
    /// <summary>
    /// Explosion effect
    /// </summary>
    public class ExplosionEffect : BaseSkillEffect
    {
        public override string EffectId => "Explosion";
        public override Color PrimaryColor => new Color(255, 150, 0);
        public override Color SecondaryColor => new Color(255, 50, 0);
        public override int DustType => DustID.Torch;
        public override SoundStyle? EffectSound => SoundID.Item14;
        public override float ScaleMultiplier => 1.5f;
        
        public override void Play(Vector2 position, int intensity = 1)
        {
            PlaySound(position);
            
            // Core explosion
            for (int i = 0; i < 30 * intensity; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(8f, 8f);
                int dustType = Main.rand.NextBool(3) ? DustID.Smoke : DustID.Torch;
                Color color = Color.Lerp(PrimaryColor, SecondaryColor, Main.rand.NextFloat());
                
                Dust dust = Dust.NewDustPerfect(position, dustType, velocity, 0, color, 2f * ScaleMultiplier);
                dust.noGravity = true;
            }
            
            // Shockwave
            CreateCircularBurst(position, 16, 20f, 6f);
        }
    }
    
    #endregion
}
