using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace RpgMod.Common.Base
{
    /// <summary>
    /// Reusable particle system base class
    /// Inherit from this for custom particle effects (level up, skill effects, etc)
    /// </summary>
    public abstract class BaseParticle
    {
        #region Properties

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Color Color { get; set; }
        public float Scale { get; set; }
        public float Rotation { get; set; }
        public int TimeLeft { get; set; }
        public int MaxTime { get; set; }
        public bool Active { get; set; }

        #endregion

        #region Abstract Members (Must Override)

        /// <summary>Texture path for this particle</summary>
        public abstract string TexturePath { get; }

        /// <summary>Particle type identifier</summary>
        public abstract ParticleType ParticleType { get; }

        #endregion

        #region Virtual Members (Can Override)

        /// <summary>
        /// Initialize particle with default values
        /// </summary>
        public virtual void Initialize(Vector2 position, Vector2 velocity, Color color, float scale, int lifetime)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Scale = scale;
            TimeLeft = lifetime;
            MaxTime = lifetime;
            Active = true;
            Rotation = 0f;
        }

        /// <summary>
        /// Update particle physics and state
        /// </summary>
        public virtual void Update()
        {
            if (!Active) return;

            // Update position
            Position += Velocity;

            // Update velocity (gravity, drag, etc)
            UpdateVelocity();

            // Update visual properties
            UpdateVisuals();

            // Decrement lifetime
            TimeLeft--;
            if (TimeLeft <= 0)
            {
                Active = false;
            }
        }

        /// <summary>
        /// Update velocity (gravity, drag, etc)
        /// </summary>
        protected virtual void UpdateVelocity()
        {
            // Apply gravity
            Vector2 velocity = Velocity;
            velocity.Y += 0.15f;
            Velocity = velocity;

            // Apply drag
            Velocity *= 0.98f;
        }

        /// <summary>
        /// Update visual properties (fade, scale, rotation)
        /// </summary>
        protected virtual void UpdateVisuals()
        {
            // Fade out near end of life
            float lifeRatio = (float)TimeLeft / MaxTime;
            if (lifeRatio < 0.3f)
            {
                Color *= 0.95f; // Fade out
            }

            // Rotate
            Rotation += 0.05f;

            // Scale changes
            if (lifeRatio > 0.8f)
            {
                Scale *= 1.05f; // Grow at start
            }
            else if (lifeRatio < 0.2f)
            {
                Scale *= 0.95f; // Shrink at end
            }
        }

        /// <summary>
        /// Draw the particle
        /// </summary>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!Active) return;

            Texture2D texture = ModContent.Request<Texture2D>(TexturePath).Value;
            
            Vector2 drawPosition = Position - Main.screenPosition;
            Rectangle sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

            spriteBatch.Draw(
                texture,
                drawPosition,
                sourceRect,
                Color,
                Rotation,
                origin,
                Scale,
                SpriteEffects.None,
                0f
            );
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Check if particle should collide with tiles
        /// </summary>
        protected virtual bool ShouldCollide()
        {
            return false; // Default: no collision
        }

        /// <summary>
        /// Called when particle collides with tile
        /// </summary>
        protected virtual void OnTileCollision()
        {
            // Bounce or die
            Vector2 velocity = Velocity;
            velocity.Y *= -0.5f;
            Velocity = velocity;
        }

        /// <summary>
        /// Create a simple sparkle particle
        /// </summary>
        public static BaseParticle CreateSparkle(Vector2 position, Vector2 velocity, Color color)
        {
            var particle = new SparkleParticle();
            particle.Initialize(position, velocity, color, 0.9f, 45);
            return particle;
        }

        #endregion
    }

    internal class SparkleParticle : BaseParticle
    {
        public override string TexturePath => "Terraria/Images/UI/Star";

        public override ParticleType ParticleType => ParticleType.Sparkle;

        protected override void UpdateVisuals()
        {
            // Gentle fade and slight rotation for a soft sparkle
            float lifeRatio = (float)TimeLeft / MaxTime;
            Color *= 0.94f;
            Rotation += 0.12f;
            Scale *= lifeRatio > 0.5f ? 1.01f : 0.98f;
        }
    }
}
