using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;


using Essentials;
using Terraria_Server.Plugin;
using Terraria_Server;
using Terraria_Server.Commands;
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
            Version = "0.2";
            TDSMBuild = 19;

            string pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + "Essentials";
            //Create folder if it doesn't exist
            CreateDirectory(pluginFolder);

            //setup a new properties file
            properties = new Properties(pluginFolder + Path.DirectorySeparatorChar + "essentials.properties");
            properties.Load();
            properties.pushData(); //Creates default values if needed.
            properties.Save();
            
            //setup new Warp
            warp = new EssentialsWarp(pluginFolder + Path.DirectorySeparatorChar + "warps.xml");

            //read properties data
            warp.enabled = properties.WarpEnabled;
            warp.requiresOp = properties.WarpRequiresOp;

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
            string[] commands = Event.Message.ToLower().Split(' '); //Split into sections (to lower case to work with it better)
            if (commands.Length > 0)
            {
                if (commands[0] != null && commands[0].Trim().Length > 0) //If it is not nothing, and the string is actually something
                {
                    if (commands[0].Equals("/warp"))
                    {
                        Player sendingPlayer = Event.Player;
                        if (warp.requiresOp && !(sendingPlayer.Op))
                        {
                            sendingPlayer.sendMessage("Error: /warp requires Op status");
                            Event.Cancelled = true;
                            return;
                        }
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
                            else if (commands[1].Equals("list"))
                            {
                                if ((commands.Length > 2 && commands[2].Equals("1")) || commands.Length < 3)
                                {
                                    warp.List(sendingPlayer, 1);
                                }
                                else if (commands.Length > 2)
                                {
                                    try
                                    {
                                        warp.List(sendingPlayer, Int32.Parse(commands[2]));
                                    }
                                    catch (FormatException)
                                    {
                                        sendingPlayer.sendMessage("Error: /warp list must be given a number");
                                    }
                                }
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
                        Event.Cancelled = true;
                    }
                    else if (commands[0].Equals("/slay"))
                    {
                        if (!Event.Player.Op)
                        {
                            Event.Player.sendMessage("Error: you must be Op to use /slay");
                            return;
                        }
                        if (commands.Length < 2)
                        {
                            Event.Player.sendMessage("Error: you must specify a player to slay");
                        }
                        else
                        {
                            try
                            {
                                Player targetPlayer = Program.server.GetPlayerByName(commands[1]);
                                NetMessage.SendData(26, -1, -1, " of unknown causes...", targetPlayer.whoAmi, 0, (float)9999, (float)0);
                                Event.Player.sendMessage("OMG!  You killed " + commands[1] + "!", 255, 0f, 255f, 255f);
                                Program.tConsole.WriteLine("Player " + Event.Player + " used /slay on " + targetPlayer.getName());
                            }
                            catch (NullReferenceException)
                            {
                                Event.Player.sendMessage("Error: Player not online.");
                            }
                        }
                        Event.Cancelled = true;
                    }
                }
            }
        }

        private static void CreateDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }
    }
}
