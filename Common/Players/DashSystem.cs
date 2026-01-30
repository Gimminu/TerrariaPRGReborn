using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace Rpg.Common.Players
{
    /// <summary>
    /// Dash system - available to ALL players
    /// Uses stamina, short cooldown that improves with level
    /// Brief invulnerability window, short distance
    /// Core movement mechanic for the entire mod
    /// </summary>
    public class DashSystem : ModPlayer
    {
        // Dash constants
        private const int BASE_STAMINA_COST = 25; // Fixed cost
        private const float BASE_COOLDOWN = 2f; // 2 seconds base
        private const float COOLDOWN_PER_LEVEL = 0.02f; // -0.02s per level (max -2s at level 100)
        private const float MIN_COOLDOWN = 0.5f; // Minimum 0.5s cooldown
        
        private const float DASH_VELOCITY = 11f; // Shorter, tighter dash
        private const int DASH_DURATION = 6; // Frames of dash
        private const int INVULN_FRAMES = 12; // More noticeable invulnerability
        
        // Dash state
        private float dashCooldown = 0f;
        private int dashTimeRemaining = 0;
        private Vector2 dashDirection = Vector2.Zero;
        
        // Keybind
        public static ModKeybind DashKeybind { get; private set; }
        
        public override void Load()
        {
            // F key by default, customizable in Controls settings
            DashKeybind = KeybindLoader.RegisterKeybind(Mod, "Dash", "F");
        }
        
        public override void Unload()
        {
            DashKeybind = null;
        }
        
        public override void PostUpdate()
        {
            // Update cooldown
            if (dashCooldown > 0)
            {
                dashCooldown -= 1f / 60f; // Decrease by 1/60 per frame
                if (dashCooldown < 0)
                    dashCooldown = 0;
            }
            
            // Update dash state
            if (dashTimeRemaining > 0)
            {
                dashTimeRemaining--;
                
                // Apply dash velocity
                Player.velocity = dashDirection * DASH_VELOCITY;
                
                // Create dash trail
                CreateDashTrail();
            }
        }
        
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            var rpgPlayer = Player.GetModPlayer<RpgPlayer>();
            
            // Check dash keybind
            if (DashKeybind.JustPressed && CanDash(rpgPlayer))
            {
                PerformDash(rpgPlayer);
            }
        }
        
        private bool CanDash(RpgPlayer rpgPlayer)
        {
            // Check cooldown
            if (dashCooldown > 0)
            {
                ShowCooldownMessage();
                return false;
            }
            
            // Check stamina
            if (rpgPlayer.Stamina < BASE_STAMINA_COST)
            {
                ShowMessage("Not enough stamina!", Color.Orange);
                return false;
            }
            
            // Can't dash while CC'd or on mount
            if (Player.CCed || Player.mount.Active)
            {
                return false;
            }
            
            return true;
        }
        
        private void PerformDash(RpgPlayer rpgPlayer)
        {
            // Consume stamina
            rpgPlayer.ConsumeStamina(BASE_STAMINA_COST);
            
            // Determine dash direction
            dashDirection = GetDashDirection();
            
            if (dashDirection == Vector2.Zero)
            {
                // No input - dash in facing direction
                dashDirection = new Vector2(Player.direction, 0);
            }
            else
            {
                dashDirection.Normalize();
            }
            
            // Start dash
            dashTimeRemaining = DASH_DURATION;
            
            // Apply invulnerability
            Player.immune = true;
            Player.immuneTime = System.Math.Max(Player.immuneTime, INVULN_FRAMES);
            
            // Set cooldown (decreases with level)
            dashCooldown = CalculateCooldown(rpgPlayer.Level);
            
            // Effects
            PlayDashEffects();
            
            // Show message
            ShowMessage("Dash!", Color.Cyan);
        }
        
        private Vector2 GetDashDirection()
        {
            Vector2 direction = Vector2.Zero;
            
            // WASD/Arrow keys
            if (Player.controlLeft)
                direction.X -= 1;
            if (Player.controlRight)
                direction.X += 1;
            if (Player.controlUp)
                direction.Y -= 1;
            if (Player.controlDown)
                direction.Y += 1;
            
            return direction;
        }
        
        private float CalculateCooldown(int level)
        {
            float cooldown = BASE_COOLDOWN - (level * COOLDOWN_PER_LEVEL);
            return System.Math.Max(cooldown, MIN_COOLDOWN);
        }
        
        private void PlayDashEffects()
        {
            // Sound
            SoundEngine.PlaySound(SoundID.Item24, Player.position);
            
            // Initial burst
            for (int i = 0; i < 12; i++)
            {
                Vector2 velocity = -dashDirection * Main.rand.NextFloat(3f, 6f);
                velocity += new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
                
                Dust dust = Dust.NewDustDirect(
                    Player.position,
                    Player.width,
                    Player.height,
                    DustID.Cloud,
                    velocity.X,
                    velocity.Y,
                    100,
                    Color.White,
                    1.3f
                );
                dust.noGravity = true;
            }
        }
        
        private void CreateDashTrail()
        {
            // Constant trail during dash
            if (Main.GameUpdateCount % 2 == 0)
            {
                Vector2 dustPos = Player.Center + new Vector2(
                    Main.rand.Next(-Player.width / 2, Player.width / 2),
                    Main.rand.Next(-Player.height / 2, Player.height / 2)
                );
                
                Dust dust = Dust.NewDustDirect(
                    dustPos,
                    1,
                    1,
                    DustID.Smoke,
                    -dashDirection.X * 2f,
                    -dashDirection.Y * 2f,
                    100,
                    Color.LightGray,
                    1.0f
                );
                dust.noGravity = true;
                dust.fadeIn = 0.8f;
            }
        }
        
        private void ShowMessage(string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                CombatText.NewText(Player.Hitbox, color, text, false, false);
            }
        }
        
        private void ShowCooldownMessage()
        {
            if (Main.netMode != NetmodeID.Server && Main.GameUpdateCount % 30 == 0)
            {
                CombatText.NewText(Player.Hitbox, Color.Gray, $"Cooldown: {dashCooldown:F1}s", false, false);
            }
        }
        
        /// <summary>
        /// Get current dash cooldown for UI display
        /// </summary>
        public float GetDashCooldown() => dashCooldown;
        
        /// <summary>
        /// Get max dash cooldown for UI display
        /// </summary>
        public float GetMaxDashCooldown(int level) => CalculateCooldown(level);
        
        /// <summary>
        /// Check if currently dashing
        /// </summary>
        public bool IsDashing() => dashTimeRemaining > 0;
    }
}
