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

        public bool isEnabled = false;
        public int i;
        private Dictionary<string, PlayerCommandEvent> lastEventByPlayer;

        public override void Load()
        {
            Name = "Essentials";
            Description = "Essential commands for TDSM.";
            Author = "Essentials";
            Version = "0.2";
            TDSMBuild = 24;

            string pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + "Essentials";
            lastEventByPlayer = new Dictionary<string, PlayerCommandEvent>();
            //Create folder if it doesn't exist
            //CreateDirectory(pluginFolder);

            //setup a new properties file
            //properties = new Properties(pluginFolder + Path.DirectorySeparatorChar + "essentials.properties");
            //properties.Load();
            ////properties.pushData(); //Creates default values if needed.
            //properties.Save();
            
            //delete unnecessary warps.xml
            if(File.Exists(pluginFolder + Path.DirectorySeparatorChar + "warps.xml"))
                File.Delete(pluginFolder + Path.DirectorySeparatorChar + "warps.xml");

            //delete unnecessary properties file
            if (File.Exists(pluginFolder + Path.DirectorySeparatorChar + "essentials.properties"))
                File.Delete(pluginFolder + Path.DirectorySeparatorChar + "essentials.properties");

            //then delete unnecessary plugin directory
            if (Directory.Exists(pluginFolder))
                Directory.Delete(pluginFolder);

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

        public void Log(string message)
        {
            Program.tConsole.WriteLine("[" + base.Name + "] " + message);
        }

        public override void onPlayerCommand(PlayerCommandEvent Event)
        {
            if (isEnabled == false) { return; }
            string[] commands = Event.Message.ToLower().Split(' '); //Split into sections (to lower case to work with it better)
            if (commands.Length > 0)
            {
                if (commands[0] != null && commands[0].Trim().Length > 0) //If it is not nothing, and the string is actually something
                {
                 //Last command COMMAND!
                    if (commands[0].Equals("/!"))
                    {
                        PlayerCommandEvent lastEvent = null;
                        lastEventByPlayer.TryGetValue(Event.Player.Name, out lastEvent);
                        if (lastEvent != null)
                        {
                            Event.Cancelled = true;
                            Log("Executing last event: [" + lastEvent.Message + "]");
                            // send it to the other plugins in case it's a plugin command
                            Program.server.getPluginManager().processHook(Hooks.PLAYER_COMMAND, lastEvent);
                            if (lastEvent.Cancelled)
                            {
                                return;
                            }
                            else
                            {
                                Program.commandParser.parsePlayerCommand(Event.Player, lastEvent.Message);
                            }
                        }
                        else
                        {
                            Event.Player.sendMessage("Error: no previous command on file");
                        }
                    }
                    else if (commands[0].Substring(0, 1).Equals("/"))
                    {
                        lastEventByPlayer[Event.Player.Name] = Event;
                    }
                    
                    //Slay COMMAND
                    if (commands[0].Equals("/slay"))
                    {
                        if (!Event.Player.Op)
                        {
                            Event.Player.sendMessage("Error: you must be Op to use /slay");
                        }
                        else if (commands.Length < 2)
                        {
                            Event.Player.sendMessage("Error: you must specify a player to slay");
                        }
                        else
                        {
                            try
                            {
                                Player targetPlayer = Program.server.GetPlayerByName(commands[1]);
                                NetMessage.SendData(26, -1, -1, " of unknown causes...", targetPlayer.whoAmi, 0, (float)9999, (float)0);
                                Event.Player.sendMessage("OMG! You killed " + commands[1] + "!", 255, 0f, 255f, 255f);
                                Log("Player " + Event.Player + " used /slay on " + targetPlayer.getName());
                            }
                            catch (NullReferenceException)
                            {
                                Event.Player.sendMessage("Error: Player not online.");
                            }
                        }
                    }
                        
                    //GOD COMMAND!
                    if (commands[0].Equals("/god"))
                    {
                     if (!Event.Player.Op)
                     {
                     Event.Player.sendMessage("Error: you must be an Op to use /god");
                     }
                     else
                     {
                         Event.Player.setGodMode(true);
                         Event.Player.sendMessage("You are a GOD!");
                     }
                         Event.Cancelled = true;
                        }
                    }
                
                 //HEAL COMMAND!
                 if (commands[0].Equals("/heal"))
                 {
                 if (!Event.Player.Op)
                 {
                 Event.Player.sendMessage("Error: you must be an Op to use /heal");
                 }
                 else if (commands.Length < 2)
                 {
                
                 Player player = Event.Player;
                
                 Event.Player.sendMessage("You did not specify the player, so you were healed");
                
                
                 for(i = 0; i < 20; i++)
                 {
                 Item.NewItem((int)player.Position.X, (int)player.Position.Y, player.width, player.height, 58, 1, false);
                 }
                 }
                 else
                 {
                 try
                 {
                 Player player = Program.server.GetPlayerByName(commands[1]);;
                
                 for(i = 0; i < 20; i++)
                 {
                 Item.NewItem((int)player.Position.X, (int)player.Position.Y, player.width, player.height, 58, 1, false);
                 }
                
                 Event.Player.sendMessage("You have healed that player!");
                 }
                 catch (NullReferenceException)
                            {
                                Event.Player.sendMessage("Error: Player not online.");
                            }
                 }
                
                 Event.Cancelled = true;
                 }
                
                 //Ping! Command!
                    if (commands[0].Equals("/ping") || commands[0].Equals("/pong"))
                    {
                        if ((commands.Length > 1 && (commands[1].ToLower().Equals("ping") || commands[1].ToLower().Equals("pong"))) || commands.Length < 2)
                        {
                            commands[0] = commands[0].Remove(0, 1);
                            string message = "";
                            for (int i = 0; i < commands.Length; i++)
                            {
                                if (commands[i].ToLower().Equals("ping"))
                                {
                                    message += "pong ";
                                }
                                else if (commands[i].ToLower().Equals("pong"))
                                {
                                    message += "ping ";
                                }
                            }
                            message = message.Trim() + "!";
                            Event.Player.sendMessage(message);
                        }
                        else if (commands.Length > 1)
                        {
                            Event.Player.sendMessage("This is ping pong! There ain't no room for " + commands[1] + "!");
                        }
                        Event.Cancelled = true;
                    }
                
                 //SUICIDE COMMAND!
                 if (commands[0].Equals("/suicide"))
                 {
                 Player Suicide = Event.Player;
                 if (!Event.Player.Op)
                 {
                 Event.Player.sendMessage("Error: you must be Op to use /suicide");
                 }
                 else
                 {
                 NetMessage.SendData(26, -1, -1, " commited suicide!", Suicide.whoAmi, 0, (float)9999, (float)0);
                 }
                 }
                
                 //Kits!
                 if (commands[0].Equals("/kit"))
                 {
                        Event.Cancelled = true;
                 //Have to be op
                 if (!Event.Player.Op)
                 {
                 Event.Player.sendMessage("Error: you must be Op to use /kit");
                            return;
                 }
                
if (commands.Length > 1)
{
                 //Admin KIT
                 if (commands[1].Equals("admin"))
                 {
                 Event.Player.sendMessage("You have recieved the Admin kit.");
                 }
                 //BUILDER KIT
                 else if (commands[1].Equals("builder"))
                 {
                 Event.Player.sendMessage("You have recieved the Builder kit.");
                 }
                 //Mod KIT
                 else if (commands[1].Equals("mod"))
                 {
                 Event.Player.sendMessage("You have recieved the Mod kit.");
                 }
                 //Help ::: Shows what kits there are
                            else if (commands[1].Equals("help"))
                            {
                                Event.Player.sendMessage("The kits are: admin, builder and mod!");
                            }
                            else
                            {
                                Event.Player.sendMessage("Error: specified kit " + commands[1] + " does not exist.");
                            }
}
                 //Error message
                 else
                        {
                            Event.Player.sendMessage("Error: You did not specify a kit!");
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