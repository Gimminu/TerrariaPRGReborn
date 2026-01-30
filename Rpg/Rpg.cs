using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common;
using Rpg.Common.Players;

namespace Rpg
{
    /// <summary>
    /// 테라리아 RPG 리본 메인 모드 클래스
    /// 메이플스토리 스타일 전직 시스템 기반 RPG 모드
    /// </summary>
    public class Rpg : Mod
    {
        public override void Load()
        {
            // 모드 로드 시 초기화
            base.Load();
        }

        public override void Unload()
        {
            // 모드 언로드 시 정리
            base.Unload();
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            RpgMessageType messageType = (RpgMessageType)reader.ReadByte();
            switch (messageType)
            {
                case RpgMessageType.SyncPlayerProgress:
                {
                    if (Main.netMode != NetmodeID.Server)
                        return;

                    byte playerId = reader.ReadByte();
                    int level = reader.ReadInt32();
                    long currentXP = reader.ReadInt64();
                    JobType job = (JobType)reader.ReadInt32();

                    if (playerId != whoAmI || playerId >= Main.maxPlayers)
                        return;

                    Player player = Main.player[playerId];
                    if (player == null || !player.active)
                        return;

                    var rpgPlayer = player.GetModPlayer<RpgPlayer>();
                    rpgPlayer.ApplyProgressSync(level, currentXP, job);
                    break;
                }
                case RpgMessageType.SyncPlayerJob:
                {
                    if (Main.netMode != NetmodeID.Server)
                        return;

                    byte playerId = reader.ReadByte();
                    JobType job = (JobType)reader.ReadInt32();

                    if (playerId != whoAmI || playerId >= Main.maxPlayers)
                        return;

                    Player player = Main.player[playerId];
                    if (player == null || !player.active)
                        return;

                    player.GetModPlayer<RpgPlayer>().ApplyJobSync(job);
                    break;
                }
                case RpgMessageType.AwardXP:
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        return;

                    byte playerId = reader.ReadByte();
                    long baseXP = reader.ReadInt64();
                    XPSource source = (XPSource)reader.ReadByte();
                    int monsterLevel = reader.ReadInt32();

                    if (playerId != Main.myPlayer || playerId >= Main.maxPlayers)
                        return;

                    Player player = Main.player[playerId];
                    if (player == null || !player.active)
                        return;

                    player.GetModPlayer<PlayerLevel>().GainExperience(baseXP, source, monsterLevel, true);
                    break;
                }
            }
        }
    }

    public enum RpgMessageType : byte
    {
        SyncPlayerProgress,
        SyncPlayerJob,
        AwardXP
    }
}
