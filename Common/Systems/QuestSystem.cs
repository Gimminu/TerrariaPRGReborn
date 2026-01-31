using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using RpgMod.Common;

namespace RpgMod.Common.Systems
{
    /// <summary>
    /// Basic quest system for RPG feel
    /// </summary>
    public class QuestSystem : ModSystem
    {
        public readonly struct QuestEntry
        {
            public QuestEntry(Quest quest, int ownerIndex)
            {
                Quest = quest;
                OwnerIndex = ownerIndex;
            }

            public Quest Quest { get; }
            public int OwnerIndex { get; }
        }

        public static List<QuestEntry> ActiveQuests { get; private set; } = new();

        public override void Load()
        {
            ActiveQuests.Clear();
        }

        public override void Unload()
        {
            ActiveQuests.Clear();
        }

        public static void AddQuest(Quest quest, int ownerIndex = -1, bool silent = false)
        {
            if (quest == null)
                return;

            quest.OwnerIndex = ownerIndex;

            bool alreadyExists = ActiveQuests.Exists(entry => ReferenceEquals(entry.Quest, quest));
            if (!alreadyExists)
            {
                ActiveQuests.Add(new QuestEntry(quest, ownerIndex));
                if (!silent)
                {
                    string ownerText = ownerIndex >= 0 ? $" (Player {ownerIndex + 1})" : string.Empty;
                    Main.NewText($"New Quest: {quest.Title}{ownerText}", new Color(100, 255, 100));
                }
            }
        }

        public static void CompleteQuest(Quest quest)
        {
            int index = ActiveQuests.FindIndex(entry => ReferenceEquals(entry.Quest, quest));
            if (index < 0)
                return;

            var entry = ActiveQuests[index];
            ActiveQuests.RemoveAt(index);

            AwardQuestRewards(entry);
            AnnounceCompletion(entry);
        }

        public override void SaveWorldData(TagCompound tag)
        {
            var questTags = new List<TagCompound>();
            foreach (var quest in ActiveQuests)
            {
                var entryTag = new TagCompound
                {
                    ["OwnerIndex"] = quest.OwnerIndex,
                    ["Quest"] = quest.Quest.Save()
                };
                questTags.Add(entryTag);
            }
            tag["ActiveQuests"] = questTags;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            ActiveQuests.Clear();
            if (tag.ContainsKey("ActiveQuests"))
            {
                var questTags = tag.GetList<TagCompound>("ActiveQuests");
                foreach (var entryTag in questTags)
                {
                    if (!entryTag.ContainsKey("Quest"))
                        continue;

                    var quest = Quest.Load(entryTag.GetCompound("Quest"));
                    if (quest != null)
                    {
                        int ownerIndex = entryTag.GetInt("OwnerIndex");
                        AddQuest(quest, ownerIndex, silent: true);
                    }
                }
            }
        }

        private static void AwardQuestRewards(QuestEntry entry)
        {
            foreach (var player in GetRecipients(entry.OwnerIndex))
            {
                var rpgPlayer = player.GetModPlayer<Players.RpgPlayer>();
                var playerLevel = player.GetModPlayer<Players.PlayerLevel>();

                playerLevel.GainExperience(entry.Quest.XPReward, XPSource.Quest);
                rpgPlayer.StatPoints += entry.Quest.StatPointReward;
                rpgPlayer.SkillPoints += entry.Quest.SkillPointReward;
            }
        }

        private static IEnumerable<Player> GetRecipients(int ownerIndex)
        {
            // Owner bound quest: award only to owner if valid, otherwise fall back to all active players
            if (ownerIndex >= 0 && ownerIndex < Main.maxPlayers)
            {
                Player owner = Main.player[ownerIndex];
                if (owner != null && owner.active)
                {
                    yield return owner;
                    yield break;
                }
            }

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player != null && player.active)
                    yield return player;
            }
        }

        private static void AnnounceCompletion(QuestEntry entry)
        {
            string ownerName = entry.OwnerIndex >= 0 && entry.OwnerIndex < Main.maxPlayers
                ? Main.player[entry.OwnerIndex]?.name
                : null;

            string header = ownerName != null
                ? $"Quest Complete ({ownerName})"
                : "Quest Complete";

            Main.NewText($"{header}: {entry.Quest.Title}", new Color(255, 255, 100));
            Main.NewText($"Rewards: {entry.Quest.XPReward} XP, {entry.Quest.StatPointReward} Stat Points, {entry.Quest.SkillPointReward} Skill Points", new Color(255, 200, 100));
        }
    }

    public abstract class Quest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int XPReward { get; set; }
        public int StatPointReward { get; set; }
        public int SkillPointReward { get; set; }
        public bool IsCompleted { get; set; }
        public int OwnerIndex { get; set; } = -1; // -1 = world/anyone

        public abstract bool CheckCompletion(Player player);
        public abstract string GetProgressText();

        public virtual TagCompound Save()
        {
            return new TagCompound
            {
                ["Title"] = Title,
                ["Description"] = Description,
                ["XPReward"] = XPReward,
                ["StatPointReward"] = StatPointReward,
                ["SkillPointReward"] = SkillPointReward,
                ["IsCompleted"] = IsCompleted,
                ["OwnerIndex"] = OwnerIndex
            };
        }

        public static Quest Load(TagCompound tag)
        {
            string title = tag.GetString("Title");
            string questType = tag.GetString("QuestType");

            Quest quest = questType switch
            {
                "Kill" => KillQuest.Load(tag),
                _ => null
            };

            if (quest != null)
            {
                quest.Title = title;
                quest.Description = tag.GetString("Description");
                quest.XPReward = tag.GetInt("XPReward");
                quest.StatPointReward = tag.GetInt("StatPointReward");
                quest.SkillPointReward = tag.GetInt("SkillPointReward");
                quest.IsCompleted = tag.GetBool("IsCompleted");
                quest.OwnerIndex = tag.GetInt("OwnerIndex");
            }

            return quest;
        }
    }

    public class KillQuest : Quest
    {
        public string TargetNPC { get; set; }
        public int RequiredKills { get; set; }
        public int CurrentKills { get; set; }

        public KillQuest(string title, string description, string targetNPC, int requiredKills, int xpReward, int statReward = 1, int skillReward = 0)
        {
            Title = title;
            Description = description;
            TargetNPC = targetNPC;
            RequiredKills = requiredKills;
            XPReward = xpReward;
            StatPointReward = statReward;
            SkillPointReward = skillReward;
            CurrentKills = 0;
            IsCompleted = false;
        }

        public override bool CheckCompletion(Player player)
        {
            return CurrentKills >= RequiredKills;
        }

        public override string GetProgressText()
        {
            return $"{CurrentKills}/{RequiredKills} {TargetNPC} slain";
        }

        public void OnNPCKilled(NPC npc)
        {
            bool isTarget = false;
            
            // Special handling for zombies (include all zombie variants)
            if (TargetNPC == "Zombie")
            {
                isTarget = NPCID.Sets.Zombies[npc.type];
            }
            else
            {
                // Default check for other targets
                isTarget = npc.FullName.Contains(TargetNPC) || npc.TypeName.Contains(TargetNPC);
            }
            
            if (isTarget)
            {
                CurrentKills++;
                if (CheckCompletion(null))
                {
                    QuestSystem.CompleteQuest(this);
                }
            }
        }

        public override TagCompound Save()
        {
            var tag = base.Save();
            tag["TargetNPC"] = TargetNPC;
            tag["RequiredKills"] = RequiredKills;
            tag["CurrentKills"] = CurrentKills;
            tag["QuestType"] = "Kill";
            return tag;
        }

        public new static Quest Load(TagCompound tag)
        {
            if (tag.GetString("QuestType") == "Kill")
            {
                var quest = new KillQuest(
                    tag.GetString("Title"),
                    tag.GetString("Description"),
                    tag.GetString("TargetNPC"),
                    tag.GetInt("RequiredKills"),
                    tag.GetInt("XPReward"),
                    tag.GetInt("StatPointReward"),
                    tag.GetInt("SkillPointReward")
                );
                quest.CurrentKills = tag.GetInt("CurrentKills");
                quest.IsCompleted = tag.GetBool("IsCompleted");
                return quest;
            }
            return null;
        }
    }
}
