using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Plugins;
using Terraria_Server;
using Terraria_Server.Logging;

namespace Essentials
{
    public enum Packets
    {
        CLIENT_MOD_GOD = 253
    }

    public partial class Net
    {
        public static void OnUnkownPacketSend(ref HookContext ctx, ref HookArgs.UnkownSendPacket args)
        {
            switch (args.PacketId)
            {
                case (int)Packets.CLIENT_MOD_GOD:
                    {
                        NetMessageExtension msg = new NetMessageExtension();

                        if (args.RemoteClient != -1)
                        {
                            var player = Main.players[args.RemoteClient];

                            if (player.HasClientMod)
                            {
                                if (Server.AllowTDCMRPG)
                                {
                                    Server.notifyOps(
										String.Format("Failed attempt to {0} God Mode on an RPG Server.", true, (args.Number == 1) ? "give" : "remove"));
                                    return;
                                }

                                Server.notifyOps(
									String.Format("{0} {1} God Mode.", true, player.Name, (args.Number == 1) ? "has" : "doesn't have"));

                                msg.GodTDCMClient(args.Number == 1);
                                args.Message = msg;
                                ctx.SetResult(HookResult.IGNORE); //Let TDSM know it's to ignore returning.
                            }
                        }
                        break;
                    }

            }
        }
    }

    public partial class NetMessageExtension : NetMessage
    {
        public void GodTDCMClient(bool God)
        {
            Header((int)Packets.CLIENT_MOD_GOD, 4);

            Int((God) ? 1 : 0);
            
            End();
        }
    }
}
