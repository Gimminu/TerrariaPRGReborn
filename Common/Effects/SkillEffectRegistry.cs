using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Effects.Types;
using Rpg.Common;

namespace Rpg.Common.Effects
{
    /// <summary>
    /// Central registry for all skill effects
    /// Register custom effects here and retrieve them by ID or type
    /// </summary>
    public class SkillEffectRegistry : ModSystem
    {
        private static Dictionary<string, BaseSkillEffect> _effectsById;
        private static Dictionary<Type, BaseSkillEffect> _effectsByType;
        
        // Singleton instances for common effects
        public static HealEffect Heal { get; private set; }
        public static DivineHealEffect DivineHeal { get; private set; }
        public static MassHealEffect MassHeal { get; private set; }
        
        public static PhysicalDamageEffect PhysicalDamage { get; private set; }
        public static CriticalHitEffect CriticalHit { get; private set; }
        public static FireDamageEffect FireDamage { get; private set; }
        public static IceDamageEffect IceDamage { get; private set; }
        public static LightningDamageEffect LightningDamage { get; private set; }
        public static ShadowDamageEffect ShadowDamage { get; private set; }
        
        public static BuffEffect Buff { get; private set; }
        public static PowerBuffEffect PowerBuff { get; private set; }
        public static DefenseBuffEffect DefenseBuff { get; private set; }
        public static SpeedBuffEffect SpeedBuff { get; private set; }
        public static RageBuffEffect RageBuff { get; private set; }
        
        public static DebuffEffect Debuff { get; private set; }
        public static PoisonEffect Poison { get; private set; }
        public static CurseEffect Curse { get; private set; }
        public static SlowEffect Slow { get; private set; }
        
        public static TeleportEffect Teleport { get; private set; }
        public static DashEffect Dash { get; private set; }
        public static DodgeEffect Dodge { get; private set; }
        
        public static SummonEffect Summon { get; private set; }
        public static UndeadSummonEffect UndeadSummon { get; private set; }
        
        public static GroundSlamEffect GroundSlam { get; private set; }
        public static ShieldEffect Shield { get; private set; }
        public static ExplosionEffect Explosion { get; private set; }
        
        public override void Load()
        {
            _effectsById = new Dictionary<string, BaseSkillEffect>(StringComparer.OrdinalIgnoreCase);
            _effectsByType = new Dictionary<Type, BaseSkillEffect>();
            
            // Register all built-in effects
            RegisterBuiltInEffects();
        }
        
        public override void Unload()
        {
            _effectsById?.Clear();
            _effectsByType?.Clear();
            _effectsById = null;
            _effectsByType = null;
            
            // Clear singleton references
            Heal = null;
            DivineHeal = null;
            MassHeal = null;
            PhysicalDamage = null;
            CriticalHit = null;
            FireDamage = null;
            IceDamage = null;
            LightningDamage = null;
            ShadowDamage = null;
            Buff = null;
            PowerBuff = null;
            DefenseBuff = null;
            SpeedBuff = null;
            RageBuff = null;
            Debuff = null;
            Poison = null;
            Curse = null;
            Slow = null;
            Teleport = null;
            Dash = null;
            Dodge = null;
            Summon = null;
            UndeadSummon = null;
            GroundSlam = null;
            Shield = null;
            Explosion = null;
        }
        
        private void RegisterBuiltInEffects()
        {
            // Healing
            Heal = RegisterEffect(new HealEffect());
            DivineHeal = RegisterEffect(new DivineHealEffect());
            MassHeal = RegisterEffect(new MassHealEffect());
            
            // Damage
            PhysicalDamage = RegisterEffect(new PhysicalDamageEffect());
            CriticalHit = RegisterEffect(new CriticalHitEffect());
            FireDamage = RegisterEffect(new FireDamageEffect());
            IceDamage = RegisterEffect(new IceDamageEffect());
            LightningDamage = RegisterEffect(new LightningDamageEffect());
            ShadowDamage = RegisterEffect(new ShadowDamageEffect());
            
            // Buffs
            Buff = RegisterEffect(new BuffEffect());
            PowerBuff = RegisterEffect(new PowerBuffEffect());
            DefenseBuff = RegisterEffect(new DefenseBuffEffect());
            SpeedBuff = RegisterEffect(new SpeedBuffEffect());
            RageBuff = RegisterEffect(new RageBuffEffect());
            
            // Debuffs
            Debuff = RegisterEffect(new DebuffEffect());
            Poison = RegisterEffect(new PoisonEffect());
            Curse = RegisterEffect(new CurseEffect());
            Slow = RegisterEffect(new SlowEffect());
            
            // Movement
            Teleport = RegisterEffect(new TeleportEffect());
            Dash = RegisterEffect(new DashEffect());
            Dodge = RegisterEffect(new DodgeEffect());
            
            // Summon
            Summon = RegisterEffect(new SummonEffect());
            UndeadSummon = RegisterEffect(new UndeadSummonEffect());
            
            // Impact
            GroundSlam = RegisterEffect(new GroundSlamEffect());
            Shield = RegisterEffect(new ShieldEffect());
            Explosion = RegisterEffect(new ExplosionEffect());
        }
        
        #region Public API
        
        /// <summary>
        /// Register a custom effect
        /// </summary>
        public static T RegisterEffect<T>(T effect) where T : BaseSkillEffect
        {
            if (_effectsById == null || _effectsByType == null)
                return effect;
                
            _effectsById[effect.EffectId] = effect;
            _effectsByType[effect.GetType()] = effect;
            return effect;
        }
        
        /// <summary>
        /// Get effect by ID
        /// </summary>
        public static BaseSkillEffect GetEffect(string effectId)
        {
            if (_effectsById != null && _effectsById.TryGetValue(effectId, out var effect))
                return effect;
            return null;
        }
        
        /// <summary>
        /// Get effect by type
        /// </summary>
        public static T GetEffect<T>() where T : BaseSkillEffect
        {
            if (_effectsByType != null && _effectsByType.TryGetValue(typeof(T), out var effect))
                return effect as T;
            return null;
        }
        
        /// <summary>
        /// Play an effect by ID
        /// </summary>
        public static void PlayEffect(string effectId, Vector2 position, int intensity = 1)
        {
            GetEffect(effectId)?.Play(position, intensity);
        }
        
        /// <summary>
        /// Play an effect on a player
        /// </summary>
        public static void PlayEffectOnPlayer(string effectId, Player player, int intensity = 1)
        {
            GetEffect(effectId)?.PlayOnPlayer(player, intensity);
        }
        
        /// <summary>
        /// Play an effect on an NPC
        /// </summary>
        public static void PlayEffectOnNPC(string effectId, NPC npc, int intensity = 1)
        {
            GetEffect(effectId)?.PlayOnNPC(npc, intensity);
        }
        
        /// <summary>
        /// Get effect for a skill based on its type
        /// </summary>
        public static BaseSkillEffect GetEffectForSkillType(SkillType skillType)
        {
            return skillType switch
            {
                SkillType.Active => PhysicalDamage,
                SkillType.Passive => Buff,
                SkillType.Buff => Buff,
                SkillType.Debuff => Debuff,
                SkillType.Movement => Dash,
                SkillType.Utility => Buff,
                _ => PhysicalDamage
            };
        }
        
        #endregion
    }
}
