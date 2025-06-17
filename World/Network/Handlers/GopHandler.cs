using Enum.Main.MessageEnum;
using Enum.Main.OptionEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using World.Network.Interfaces;

namespace World.Network.Handlers
{
    public class GopHandler : IPacketGameHandler
    {
        public void RegisterPackets(IPacketHandler handler)
        {
            handler.Register("gop", HandleGop);
        }

        public static async Task HandleGop(ClientSession session, string[] parts)
        {
            var type = (Option)byte.Parse(parts[2]);
            var enabled = parts[3] == "1" ? false : true;

            switch (type)
            {
                case Option.DisplayHP:
                    session.Player.IsDisplayHealthBlocked = enabled;
                    await session.Player.SendMsgi(enabled ? MessageId.DISPLAY_HP_OFF : MessageId.DISPLAY_HP_ON);
                    break;

                case Option.DisplayCD:
                    session.Player.IsDisplayCdBlocked = enabled;
                    await session.Player.SendMsgi(enabled ? MessageId.DISPLAY_CD_OFF : MessageId.DISPLAY_CD_ON);
                    break;

                case Option.LockHud:
                    session.Player.IsBlockedHud = enabled;
                    await session.Player.SendMsgi(enabled ? MessageId.UI_BLOCKED : MessageId.UI_UNBLOCKED);
                    break;

                case Option.DisableHat:
                    session.Player.IsBlockedHat = enabled;
                    await session.Player.CurrentMap.Broadcast(await session.Player.Packets.GenerateEqPacket());
                    break;

                case Option.BuffBlocked:
                    session.Player.IsBlockedBuff = !enabled;
                    await session.Player.SendMsgi(enabled ? MessageId.BUFF_SHOW : MessageId.BUFF_HIDE);
                    break;

                case Option.HpBlocked:
                    session.Player.IsHealthBlocked = !enabled;
                    await session.Player.SendMsgi(!enabled ? MessageId.HP_INDICATOR_OFF : MessageId.HP_INDICATOR_ON);
                    break;

                case Option.EmoticonsBlocked:
                    session.Player.IsEmoticonBlocked = !enabled;
                    await session.Player.SendMsgi(!enabled ? MessageId.DISPLAY_EMOJIS_OFF : MessageId.DISPLAY_EMOJIS_ON);
                    break;

                case Option.QuickGetUp:
                    session.Player.IsQuickGetUpBlocked = !enabled;
                    await session.Player.SendMsgi(!enabled ? MessageId.GET_UP_OFF : MessageId.GET_UP_ON);
                    break;

                case Option.MinilandInviteBlocked:
                    session.Player.IsMinilandInviteBlocked = !enabled;
                    await session.Player.SendMsgi(!enabled ? MessageId.MINILAND_INVITATIONS_OFF : MessageId.MINILAND_INVITATIONS_ON);
                    break;

                case Option.HeroChatBlocked:
                    session.Player.IsHeroChatBlocked = !enabled;
                    await session.Player.SendMsgi(!enabled ? MessageId.SPEAKER_OFF : MessageId.SPEAKER_ON);
                    break;

                case Option.MouseAimLock:
                    session.Player.CursorAimLock = !enabled;
                    await session.Player.SendMsgi(!enabled ? MessageId.CURSOR_TRAP_OFF : MessageId.CURSOR_TRAP_ON);
                    break;

                case Option.ExchangeBlocked:
                    session.Player.IsExchangeBlocked = enabled;
                    await session.Player.SendMsgi(enabled ? MessageId.EXCHANGES_BLOCKED : MessageId.EXCHANGES_UNBLOCKED);
                    break;

                case Option.WhisperBlocked:
                    session.Player.IsWhispBlocked = enabled;
                    await session.Player.SendMsgi(enabled ? MessageId.WHISPER_UNBLOCKED : MessageId.WHISPER_BLOCKED);
                    break;

                case Option.FriendRequestBlocked:
                    session.Player.IsFriendRequestBlocked = enabled;
                    await session.Player.SendMsgi(enabled ? MessageId.FRIEND_UNBLOCKED : MessageId.FRIEND_BLOCKED);
                    break;

                case Option.FamilyRequestBlocked:
                    session.Player.IsFamilyRequestBlocked = enabled;
                    await session.Player.SendMsgi(enabled ? MessageId.FAMILY_BLOCKED : MessageId.FAMILY_UNBLOCKED);
                    break;

                case Option.GroupRequestBlocked:
                    session.Player.IsGroupRequestBlocked = enabled;
                    await session.Player.SendMsgi(enabled ? MessageId.GROUP_UNBLOCKED : MessageId.GROUP_BLOCKED);
                    break;

            }

            await session.SendPacket(session.Player.Packets.GenerateStat());
            await session.Player.UpdateCharacter();
        }
    }
}