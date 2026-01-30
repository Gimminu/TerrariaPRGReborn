using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Rpg.Common.NPCs;
using Rpg.Common.Systems;

namespace Rpg.Common.Projectiles
{
    /// <summary>
    /// Boosts hostile projectile damage in line with world level so enemies keep up with NPC scaling.
    /// Inspired by AnotherRPG's progressive projectile damage multipliers.
    /// </summary>
    public class WorldLevelProjectileScaling : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        private bool scaled = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            base.OnSpawn(projectile, source);

            if (scaled || !projectile.hostile)
                return;

            int scaleLevel = 0;
            if (source is EntitySource_Parent parentSource && parentSource.Entity is NPC npc)
            {
                var rpgNpc = npc.GetGlobalNPC<RpgGlobalNPC>();
                if (rpgNpc != null)
                    scaleLevel = rpgNpc.MonsterLevel;
            }

            if (scaleLevel <= 0)
                scaleLevel = RpgWorld.GetEffectiveWorldLevel();

            float multiplier = 1f + (scaleLevel * RpgConstants.MONSTER_PROJECTILE_SCALE_PER_LEVEL);
            multiplier = Math.Min(multiplier, RpgConstants.MONSTER_PROJECTILE_MAX_MULTIPLIER);
            projectile.damage = Math.Max(1, (int)(projectile.damage * multiplier));
            scaled = true;
        }
    }
}
