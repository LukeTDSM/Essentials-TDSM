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
        public bool tileBreakageAllowed = false;
        public bool isEnabled = false;
        public EssentialsWarp warp;

        public override void Load()
        {
            Name = "Essentials";
            Description = "Essential commands for TDSM.";
            Author = "Essentials";
            Version = "1";
            TDSMBuild = 16;

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
            
            //setup new Warp
            warp = new EssentialsWarp(pluginFolder + Statics.systemSeperator + "warps.xml");

            //read properties data
            warp.enabled = properties.isWarpEnabled();

            
            //xml.SetupXml();

            isEnabled = true;
        }

        public override void Enable()
        {
            Program.tConsole.WriteLine(base.Name + " enabled.");
            //Register Hooks
            this.registerHook(Hooks.PLAYER_COMMAND);
            //this.registerHook(Hooks.PLAYER_LOGIN);
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
                        if(warp.enabled)
                        {
                            if (commands.Length < 2)
                            {
                                sendingPlayer.sendMessage("For help, type /warp help", 255, 0f, 255f, 255f);
                            }
                            else if (commands[1].Equals("+"))
                            {
                                if (commands.Length < 3)
                                    sendingPlayer.sendMessage("Add warp error: format must be /warp + <warpname>");
                                else
                                {
                                    warp.WriteWarp(sendingPlayer, commands[2]);
                                }
                            }
                            else if (commands[1].Equals("-"))
                            {
                                if (commands.Length < 3)
                                    sendingPlayer.sendMessage("Remove warp error: format must be /warp - <warpname>");
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
                        else
                        {
                            sendingPlayer.sendMessage("Error: Warp not enabled");
                        }
                        Event.setCancelled(true);
                    }
                    else if (commands[0].Equals("/slay"))
                    {
                        Player sendingPlayer = Event.getPlayer();
                        if (!sendingPlayer.isOp())
                        {
                            sendingPlayer.sendMessage("Error: you must be Op to use /slay");
                        }
                        else if (commands.Length < 2)
                        {
                            sendingPlayer.sendMessage("Error: you must specify a player to slay");
                        }
                        else
                        {
                            try
                            {
                                Player killedPlayer = Program.server.GetPlayerByName(commands[1]);//.Hurt(200, 0, false, false, " died of unknown causes");
                                killedPlayer.KillMe(10000, 0, false, " died of unknown causes...");
                                sendingPlayer.sendMessage("OMG!  You killed" + commands[1], 255, 0f, 255f, 255f);
                                Program.tConsole.WriteLine("[Essentials] " + sendingPlayer.getName() + " used Slay on: " + killedPlayer.getName());
                            }
                            catch (NullReferenceException)
                            {
                                sendingPlayer.sendMessage("Error: Player not online.");
                            }
                        }
                        Event.setCancelled(true);
                    }
                }
            }
        }

        public override void onPlayerJoin(PlayerLoginEvent Event)
        {
            
        }
    }
}
