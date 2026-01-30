using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Rpg.Common.Effects
{
    /// <summary>
    /// Abstract base class for all skill effects
    /// Inherit from this to create custom visual/audio effects for skills
    /// </summary>
    public abstract class BaseSkillEffect
    {
        #region Properties
        
        /// <summary>
        /// Effect identifier for registration
        /// </summary>
        public abstract string EffectId { get; }
        
        /// <summary>
        /// Primary color of the effect
        /// </summary>
        public virtual Color PrimaryColor => Color.White;
        
        /// <summary>
        /// Secondary color for gradients/variations
        /// </summary>
        public virtual Color SecondaryColor => Color.Gray;
        
        /// <summary>
        /// Default dust type for this effect
        /// </summary>
        public virtual int DustType => DustID.MagicMirror;
        
        /// <summary>
        /// Sound to play when effect triggers
        /// </summary>
        public virtual SoundStyle? EffectSound => SoundID.Item4;
        
        /// <summary>
        /// Duration multiplier for the effect
        /// </summary>
        public virtual float DurationMultiplier => 1f;
        
        /// <summary>
        /// Scale multiplier for particles
        /// </summary>
        public virtual float ScaleMultiplier => 1f;
        
        #endregion

        #region Core Methods
        
        /// <summary>
        /// Play the effect at the specified position
        /// </summary>
        public virtual void Play(Vector2 position, int intensity = 1)
        {
            PlaySound(position);
            SpawnParticles(position, intensity);
        }
        
        /// <summary>
        /// Play the effect on a player
        /// </summary>
        public virtual void PlayOnPlayer(Player player, int intensity = 1)
        {
            Play(player.Center, intensity);
        }
        
        /// <summary>
        /// Play the effect on an NPC
        /// </summary>
        public virtual void PlayOnNPC(NPC npc, int intensity = 1)
        {
            Play(npc.Center, intensity);
        }
        
        #endregion

        #region Overridable Effect Methods
        
        /// <summary>
        /// Play the associated sound
        /// </summary>
        protected virtual void PlaySound(Vector2 position)
        {
            if (EffectSound.HasValue)
            {
                SoundEngine.PlaySound(EffectSound.Value, position);
            }
        }
        
        /// <summary>
        /// Spawn particles for the effect
        /// </summary>
        protected virtual void SpawnParticles(Vector2 position, int intensity)
        {
            int count = 10 * intensity;
            for (int i = 0; i < count; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4f, 4f);
                Dust dust = Dust.NewDustPerfect(
                    position + Main.rand.NextVector2Circular(20f, 20f),
                    DustType,
                    velocity,
                    0,
                    PrimaryColor,
                    1.5f * ScaleMultiplier
                );
                dust.noGravity = true;
            }
        }
        
        #endregion

        #region Utility Methods
        
        /// <summary>
        /// Create a circular burst of particles
        /// </summary>
        protected void CreateCircularBurst(Vector2 center, int count, float radius, float speed)
        {
            for (int i = 0; i < count; i++)
            {
                float angle = MathHelper.TwoPi * i / count;
                Vector2 direction = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle));
                Vector2 position = center + direction * radius;
                Vector2 velocity = direction * speed;
                
                Dust dust = Dust.NewDustPerfect(position, DustType, velocity, 0, PrimaryColor, 1.2f * ScaleMultiplier);
                dust.noGravity = true;
            }
        }
        
        /// <summary>
        /// Create an upward spiral effect
        /// </summary>
        protected void CreateSpiral(Vector2 center, int count, float height)
        {
            for (int i = 0; i < count; i++)
            {
                float progress = i / (float)count;
                float angle = progress * MathHelper.TwoPi * 3;
                float radius = 20f * (1f - progress);
                
                Vector2 offset = new Vector2(
                    (float)System.Math.Cos(angle) * radius,
                    -height * progress
                );
                
                Dust dust = Dust.NewDustPerfect(center + offset, DustType, Vector2.Zero, 0, 
                    Color.Lerp(PrimaryColor, SecondaryColor, progress), 1.5f * ScaleMultiplier);
                dust.noGravity = true;
            }
        }
        
        /// <summary>
        /// Create a ground impact effect (public version)
        /// </summary>
        public void CreateGroundImpact(Vector2 position, float radius)
        {
            int count = (int)(radius / 5f);
            CreateGroundImpact(position, count, radius / 10f);
        }
        
        /// <summary>
        /// Create a ground impact effect
        /// </summary>
        protected void CreateGroundImpact(Vector2 position, int count, float spread)
        {
            for (int i = 0; i < count; i++)
            {
                float angle = MathHelper.Pi + Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-spread, spread),
                    Main.rand.NextFloat(-3f, -1f)
                );
                
                Dust dust = Dust.NewDustPerfect(position, DustType, velocity, 0, PrimaryColor, 1.3f * ScaleMultiplier);
                dust.noGravity = false;
            }
        }
        
        #endregion
    }
}
