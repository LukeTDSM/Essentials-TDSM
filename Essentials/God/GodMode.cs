using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Terraria_Server;

namespace Essentials.God
{
    public class GodMode
    {
        Essentials plugin;
        public Thread godThread;
        int secondRotation;
        public bool Running { get; set; }

        public GodMode(Essentials Plugin)
        {
            plugin = Plugin;
            secondRotation = Essentials.properties.GodModeRotation;

            Running = true;

            godThread = new Thread(this.Run);
            godThread.Start();
        }

        public void Run()
        {
            while (Running)
            {
                for (int i = 0; i < Essentials.essentialsPlayerList.Count; i++)
                {
                    Player eplayer = Main.players[Essentials.essentialsPlayerList.Keys.ElementAt(i)];
                    if (eplayer.statLife != eplayer.statLifeMax && !eplayer.dead)
                    {
                        if (Essentials.essentialsPlayerList.Values.ElementAt(i))
                        {
                            Item.NewItem((int)eplayer.Position.X, (int)eplayer.Position.Y, eplayer.Width, eplayer.Height, 58, 1, false);
                        }
                    }
                }
                Thread.Sleep(secondRotation * 1000);
            }
        }
    }
}
