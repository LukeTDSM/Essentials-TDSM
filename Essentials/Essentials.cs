using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;


using Essentials;
using Terraria_Server.Plugin;
using Terraria_Server;
using Terraria_Server.Events;

namespace Essentials
{
    public class Essentials : Plugin
    {
        /*
         * @Developers
         * 
         * Plugins need to be in .NET 3.5
         * Otherwise TDSM will be unable to load it. 
         * 
         * As of June 16, 1:15 AM, TDSM should now load Plugins Dynamically.
         */

        // tConsole is used for when logging Output to the console & a log file.

        public Properties properties;
        public bool spawningAllowed = false;
        public bool tileBreakageAllowed = false;
        public bool isEnabled = false;
        public EssentialsWarp warp;

        public override void Load()
        {
            Name = "Essentials";
            Description = "Essential commands for TDSM.";
            Author = "Essentials";
            Version = "1";
            TDSMBuild = 15;

            string pluginFolder = Statics.getPluginPath + Statics.systemSeperator + "Essentials";
            //Create folder if it doesn't exist
            if (!Program.createDirectory(pluginFolder, true))
            {
                Program.tConsole.WriteLine("[Essentials] Failed to create crucial Folder");
                return;
            }

            //setup a new properties file
            properties = new Properties(pluginFolder + Statics.systemSeperator + "essentials.properties");
            properties.Load();
            properties.pushData(); //Creates default values if needed.
            properties.Save();

            //read properties data
            spawningAllowed = properties.isSpawningCancelled();
            tileBreakageAllowed = properties.getTileBreakage();

            //setup new warps.xml file
            warp = new EssentialsWarp(pluginFolder + Statics.systemSeperator + "warps.xml");
            //xml.SetupXml();

            isEnabled = true;
        }

        public override void Enable()
        {
            Program.tConsole.WriteLine(base.Name + " enabled.");
            //Register Hooks
            this.registerHook(Hooks.PLAYER_COMMAND);
            this.registerHook(Hooks.PLAYER_LOGIN);

            Main.stopSpawns = isEnabled;
            if (isEnabled)
            {
                Program.tConsole.WriteLine("Disabled NPC Spawning");
            }
        }

        public override void Disable()
        {
            Program.tConsole.WriteLine(base.Name + " disabled.");
            isEnabled = false;
        }

        public override void onPlayerCommand(PlayerCommandEvent Event)
        {
            if (isEnabled == false) { return; }
            string[] commands = Event.getMessage().ToLower().Split(' '); //Split into sections (to lower case to work with it better)
            if (commands.Length > 0)
            {
                if (commands[0] != null && commands[0].Trim().Length > 0) //If it is not nothing, and the string is actually something
                {
                    if (commands[0].Equals("/warp"))
                    {
                        Player sendingPlayer = Event.getPlayer();
                        if (commands.Length < 2)
                        {
                            Program.tConsole.WriteLine("[Warp] " + Event.getPlayer().name + " used /warp.");
                            sendingPlayer.sendMessage("Warp plugin, For Build: #" + ServerProtocol, 255, 255f, 255f, 255f);
                            sendingPlayer.sendMessage("For help, type /warp help", 255, 255f, 255f, 255f);
                        }
                        else if (commands[1].Equals("+"))
                        {
                            if (commands.Length < 3)
                                sendingPlayer.sendMessage("Add warp error: format must be /warp + <warpname>", 255, 255f, 255f, 255f);
                            else
                            {
                                warp.WriteWarp(sendingPlayer, commands[2]);
                            }
                        }
                        else if (commands[1].Equals("-"))
                        {
                            if (commands.Length < 3)
                                sendingPlayer.sendMessage("Remove warp error: format must be /warp - <warpname>", 255, 255f, 255f, 255f);
                            else
                            {
                                warp.DelWarp(sendingPlayer, commands[2]);
                            }
                        }
                        else if (commands[1].Equals("help"))
                        {

                        }
                        else if (commands[1].Equals("loc"))
                        {
                            sendingPlayer.sendMessage("Current position (x,y): (" + sendingPlayer.getLocation().X.ToString() + "," + sendingPlayer.getLocation().Y.ToString() + ").", 255, 255f, 255f, 255f);
                        }
                        else if (commands.Length < 3)
                        {
                            warp.Warp(sendingPlayer, commands[1]);
                        }
                    }
                }
            }
            Event.setCancelled(true);
        }

        public override void onPlayerJoin(PlayerLoginEvent Event)
        {
            
        }
    }
}
