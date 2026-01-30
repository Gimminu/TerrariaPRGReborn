using Terraria.ModLoader;

namespace Rpg.Common.Skills
{
    /// <summary>
    /// Mod system to initialize skill database
    /// </summary>
    public class SkillDatabaseSystem : ModSystem
    {
        public override void Load()
        {
            SkillDatabase.Initialize();
        }
        
        public override void Unload()
        {
            // Cleanup if needed
        }
    }
}
