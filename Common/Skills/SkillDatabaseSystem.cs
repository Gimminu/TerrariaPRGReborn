using Terraria.ModLoader;

namespace RpgMod.Common.Skills
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
