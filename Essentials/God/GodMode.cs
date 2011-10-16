using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Terraria_Server;

namespace Essentials.God
{
    public class GodMode : IDisposable
    {
        Essentials plugin;
        public Thread godThread;
        int secondRotation;
        public bool Running { get; set; }

        public GodMode(Essentials Plugin)
        {
            plugin = Plugin;
            secondRotation = Plugin.properties.GodModeRotation;

            Running = true;

            godThread = new Thread(this.Run);
            godThread.Start();
        }

        public void Run()
        {
            while (Running)
            {
                for (int i = 0; i < plugin.essentialsPlayerList.Count; i++)
                {
                    Player eplayer = Main.players[plugin.essentialsPlayerList.Keys.ElementAt(i)];
                    if (eplayer.statLife != eplayer.statLifeMax && !eplayer.dead)
                    {
                        if (plugin.essentialsPlayerList.Values.ElementAt(i))
                        {
                            Item.NewItem((int)eplayer.Position.X, (int)eplayer.Position.Y, eplayer.Width, eplayer.Height, 58, 1, false);
                        }
                    }
                }
                Thread.Sleep(secondRotation * 1000);
            }
        }

        public void Dispose() //Meh, May as well
        {
            if (godThread != null && godThread.IsAlive)
                godThread.Abort();

            godThread = null;

            plugin = null;
            secondRotation = default(Int32);
            Running = default(Boolean);
        }
    }
}
