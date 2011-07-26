using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server;
using Terraria_Server.Plugin;
using Essentials.Kit;
using Terraria_Server.Commands;
using Terraria_Server.Logging;

namespace Essentials
{
    public class Commands
    {
		public static void BloodMoon(Server server, ISender sender, ArgumentList args)
		{
			if (sender is Player)
			{
				Player player = sender as Player;
				if (!player.Op)
				{
					player.sendMessage("Error: you must be an Op to use /bloodmoon");
				}
				else if (!Main.bloodMoon)
				{
					Main.bloodMoon = true;
					server.World.setTime(0, false, false);
					NetMessage.SendData(25, -1, -1, "The Blood Moon is rising...", 255, 50f, 255f, 130f);
					ProgramLog.Admin.Log("Triggered blood moon phase.");
				}
				else
				{
					server.notifyAll("Blood Moon disabled");
					Main.bloodMoon = false;
					ProgramLog.Admin.Log("Disabled blood moon phase.");
				}
				NetMessage.SendData((int)Packet.WORLD_DATA);
			}
		}

        public static void HealPlayer(Server server, ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                Player player = (Player)sender;
                if (!player.Op)
                {
                    player.sendMessage("Error: you must be an Op to use /heal");
                }
                else if (args.Count < 1)
                {
                    player.sendMessage("You did not specify the player, so you were healed");
                    for (int i = 0; i < player.statLifeMax - player.statLife; i++)
                    {
                        Item.NewItem((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height, 58, 1, false);
                    }
                }
                else
                {
                    try
                    {
                        Player targetPlayer = Program.server.GetPlayerByName(args[0]);
                        for (int i = 0; i < targetPlayer.statLifeMax - targetPlayer.statLife; i++)
                        {
                            Item.NewItem((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height, 58, 1, false);
                        }
                        player.sendMessage("You have healed that player!");
                    }
                    catch (NullReferenceException)
                    {
                        player.sendMessage("Error: Player not online.");
                    }
                }
            }            
        }

        public static void ConnectionTest_Ping(Server server, ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                Player player = (Player)sender;
				if ((args.Count > 0 && (args[0].ToLower().Equals("ping") ||
                    args[0].ToLower().Equals("pong"))) || args.Count < 1)
                {
                    //args[0] = args[0].Remove(0, 1);
                    string message = "";
                    for (int i = 0; i < args.Count; i++)
                    {
                        if (args[i].ToLower().Equals("ping"))
                        {
                            message += "pong ";
                        }
                        else if (args[i].ToLower().Equals("pong"))
                        {
                            message += "ping ";
                        }
                    }
                    message = message.Trim() + "!";
                    player.sendMessage(message);
                }
                else if (args.Count > 0)
                {
                    player.sendMessage("This is ping pong! There ain't no room for " + args[0] + "!");
                }
            }
        }

        public static void LastCommand(Server server, ISender sender, ArgumentList args) //Dictionary<String, PlayerCommandEvent> lastEventByPlayer, Player player)
        {
            if (sender is Player)
            {
                if (args.Count > 1)
                {
                    if (args[1].Trim().ToLower().Equals("register"))
                    {
                        String Command = string.Join(" ", args);
                        Command = Command.Remove(0, Command.IndexOf(args[1]) + args[1].Length).Trim();
                        if (Command.Length > 0)
                        {
                            if (Essentials.lastEventByPlayer.Keys.Contains(sender.Name))
                            {
                                Essentials.lastEventByPlayer.Remove(sender.Name);
                            }
                            Essentials.lastEventByPlayer.Add(sender.Name, Command);
                            sender.sendMessage("Command registered!");
                        }
                        else
                        {
                            sender.sendMessage("Please specify a command");
                        }
                        return;
                    }
                }
                Player player = (Player)sender;
                String Message;
                Essentials.lastEventByPlayer.TryGetValue(player.Name, out Message);
                if (Message != null && Message.Length > 0)
                {
                    Essentials.Log("Executing last event: [" + Message + "]", "Essentials");

                    //This also calls to plugins
                    Program.commandParser.ParseAndProcess(player, Message);
                }
                else
                {
                    player.sendMessage("Error: no previous command on file");
                }
            }            
            //return false;
        }

        public static void Slay(Server server, ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                Player player = (Player)sender;
                if (!player.Op)
                {
                    player.sendMessage("Error: you must be Op to use /slay");
                }
                else if (args.Count < 1)
                {
                    player.sendMessage("Error: you must specify a player to slay");
                }
                else
                {
                    try
                    {
                        Player targetPlayer = Program.server.GetPlayerByName(args[0]);
                        NetMessage.SendData(26, -1, -1, " of unknown causes...", targetPlayer.whoAmi, 0, (float)9999, (float)0);
                        player.sendMessage("OMG! You killed " + args[0] + "!", 255, 0f, 255f, 255f);
                        Essentials.Log("Player " + player + " used /slay on " + targetPlayer.Name, "Essentials");
                    }
                    catch (NullReferenceException)
                    {
                        player.sendMessage("Error: Player not online.");
                    }
                }
            }            
        }

        public static void Suicide(Server server, ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                Player player = (Player)sender;
                if (!player.Op)
                {
                    player.sendMessage("Error: you must be Op to use /suicide");
                }
                else
                {
                    NetMessage.SendData(26, -1, -1, " commited suicide!", player.whoAmi, 0, (float)player.statLifeMax, (float)0);
                }
            }
        }

        public static void Butcher(Server server, ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                Player player = (Player)sender;
                // if play not op!
                if (!player.Op)
                {
                    player.sendMessage("Error: you must be Op to use /butcher");
                }
                else
                {
                    int Radius = 7;
                    if (args.Count > 0 && args[0] != null && args[0].Trim().Length > 0)
                    {
                        try
                        {
                            Radius = Convert.ToInt32(args[0]);
                        }
                        catch
                        {
                            //Not a value, Keep at default radius
                        }
                    }

                    //Start code!
                    int killCount = 0;
                    for (int i = 0; i < Main.npcs.Length - 1; i++)
                    {
                        NPC npc = Main.npcs[i];
                        if (((npc.Position.X - player.Position.X) / 16 <= Radius) ||
                            (npc.Position.Y - player.Position.Y) / 16 <= Radius)
                        {
                            if (Main.npcs[i].StrikeNPC(npc.lifeMax, (float)90f, 0) > 0.0)
                            {
                                killCount++;
                            }
                        }
                    }

                    player.sendMessage("You butcher'd " + killCount.ToString() + " NPC's!", 255, 0f, 255f, 255f);
                }
            }            
        }

        public static void Kit(Server server, ISender sender, ArgumentList args) //, KitManager kitManager)
        {
            if (sender is Player)
            {
                Player player = (Player)sender;
                //Have to be op
                if (!player.Op)
                {
                    player.sendMessage("Error: you must be Op to use /kit");
                    return;
                }

                if (args.Count > 0)
                {
                    if (Essentials.kitManager.ContainsKit(args[0]))
                    {
                        Kit.Kit kit = Essentials.kitManager.getKit(args[0]);
                        if (kit != null && kit.ItemList != null)
                        {
                            foreach (int ItemID in kit.ItemList)
                            {
                                Item.NewItem((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height, ItemID, 1, false);
                            }

                            player.sendMessage("Recived the '" + kit.Name + "' Kit.");
                        }
                        else
                        {
                            player.sendMessage("Issue with null kit/list");
                        }
                    }

                    //Help ::: Shows what kits there are
                    else if (args[0].Equals("help"))
                    {
                        String Kits = "";
                        foreach (Kit.Kit kit in Essentials.kitManager.KitList)
                        {
                            if (kit.Name.Trim().Length > 0)
                            {
                                Kits = Kits + ", " + kit.Name;
                            }
                        }
                        if (Kits.StartsWith(","))
                        {
                            Kits = Kits.Remove(0, 1).Trim();
                        }
                        if (Kits.Length > 0)
                        {
                            player.sendMessage("Available Kits: " + Kits);
                        }
                    }

                    //If kit does not exist
                    else
                    {
                        player.sendMessage("Error: specified kit " + args[0] + " does not exist. Please do /kit help");
                    }
                }
                //Error message
                else
                {
                    player.sendMessage("Error: You did not specify a kit! Do /kit help!");
                }
            }            
        }

        public static void GodMode(Server server, ISender sender, ArgumentList args) //Player player, Dictionary<Int32, Boolean> playerList)
        {
            if (sender is Player)
            {
                Player player = (Player)sender;
                if (!player.Op)
                {
                    player.sendMessage("Error: you must be Op to use God Mode");
                    return;
                }
            
                bool found = false;
                bool godModeStatus = false;
                for (int i = 0; i < Essentials.essentialsPlayerList.Count; i++ )
                {
                    int PlayerID = Essentials.essentialsPlayerList.Keys.ElementAt(i);
                    Player eplayer = Main.players[PlayerID];
                    if (eplayer.Name.Equals(player.Name))
                    {
                        bool GodMode = !Essentials.essentialsPlayerList.Values.ElementAt(i);
                        Essentials.essentialsPlayerList.Remove(PlayerID);
                        Essentials.essentialsPlayerList.Add(PlayerID, GodMode);
                        godModeStatus = GodMode;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    godModeStatus = true;
                    Essentials.essentialsPlayerList.Add(player.whoAmi, godModeStatus);
                }
            
                player.sendMessage("God Mode Status: " + godModeStatus.ToString());
            }
            
        }

        public static void Spawn(Server server, ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                Player player = (Player)sender;
                player.teleportTo(Main.spawnTileX, Main.spawnTileY);
                player.sendMessage("You have been Teleported to Spawn");
            }
        }

        public static void Info(Server server, ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                Player player = (Player)sender;
                player.sendMessage("Essentials Plugin for TDSM b" + Statics.BUILD, 255, 160f, 32f, 240f);
                String Platform = Terraria_Server.Definitions.Platform.Type.ToString();
                switch (Terraria_Server.Definitions.Platform.Type)
                {
                    case Terraria_Server.Definitions.Platform.PlatformType.LINUX:
                        Platform = "Linux";
                        break;
                    case Terraria_Server.Definitions.Platform.PlatformType.MAC:
                        Platform = "Mac";
                        break;
                    case Terraria_Server.Definitions.Platform.PlatformType.WINDOWS:
                        Platform = "Windows";
                        break;
                }
                player.sendMessage("The current OS running this sever is: " + Platform, 255, 160f, 32f, 240f);
            }            
        }

        public static void Plugins(Server server, ISender sender, ArgumentList args)
        {
            /*
             * Commands:
             *      list    - shows all loaded plugins
             *      info    - shows a plugin's author & description etc
             *      disable - disables a plugin
             *      enable  - enables a plugin
             */
            if (args.Count > 0 && args[0] != null && args[0].Trim().Length > 0 && sender is Player)
            {
                Player player = (Player)sender;
                String command = args[0].Trim();
                switch (command)
                {
                    case "list":
                        {
                            String plugins = "None."; //If no plugins
                            if (Program.server.PluginManager.PluginList.Count > 0)
                            {
                                plugins = "";

                                foreach (Plugin plugin in Program.server.PluginManager.PluginList.Values)
                                {
                                    if (plugin.Name.Trim().Length > 0)
                                    {
                                        plugins = ", " + plugin.Name.Trim() + " " + ((!plugin.Enabled) ? "[DISABLED]" : ""); //, Plugin1, Plugin2
                                    }
                                }
                                if (plugins.StartsWith(","))
                                {
                                    plugins = plugins.Remove(0, 1).Trim(); //Plugin1, Plugin2 {Remove the ', ' from the start and trim the ends}
                                }
                            }

                            player.sendMessage("Loaded Plugins: " + plugins + ".");
                            break;
                        }
                    case "info":
                        {
                            if (!(args.Count > 0 && args[1] != null && args[0].Trim().Length > 0))
                            {
                                player.sendMessage("Please review your argument count.");
                            }

                            //Get plugin Name
                            String pluginName = string.Join(" ", args);
                            pluginName = pluginName.Remove(0, pluginName.IndexOf(args[1])).Trim();

                            if (Program.server.PluginManager.PluginList.Count > 0)
                            {
                                Plugin fplugin = Program.server.PluginManager.getPlugin(pluginName);
                                if (fplugin != null)
                                {
                                    player.sendMessage("Plugin Name: " + fplugin.Name);
                                    player.sendMessage("Plugin Author: " + fplugin.Author);
                                    player.sendMessage("Plugin Description: " + fplugin.Description);
                                    player.sendMessage("Plugin Enabled: " + fplugin.Enabled.ToString());
                                }
                                else
                                {
                                    player.sendMessage("Sorry, That Plugin was not found. (" + args[1] + ")");
                                }
                            }
                            else
                            {
                                player.sendMessage("Sorry, There are no Plugins Loaded.");
                            }
                            break;
                        }
                    case "disable":
                        {
                            if (!player.Op)
                            {
                                player.sendMessage("Error: you must be Op to use feature.");
                                return;
                            }
                            if (!(args.Count > 0 && args[1] != null && args[1].Trim().Length > 0))
                            {
                                player.sendMessage("Please review your argument count.");
                            }

                            //Get plugin Name
                            String pluginName = string.Join(" ", args);
                            pluginName = pluginName.Remove(0, pluginName.IndexOf(args[1])).Trim();

                            if (Program.server.PluginManager.PluginList.Count > 0)
                            {
                                Plugin fplugin = Program.server.PluginManager.getPlugin(pluginName);
                                if (fplugin != null)
                                {
                                    if (fplugin.Enabled)
                                    {
                                        if (Program.server.PluginManager.DisablePlugin(fplugin.Name))
                                        {
                                            player.sendMessage(args[1] + " was Disabled!");
                                        }
                                        else
                                        {
                                            player.sendMessage("Sorry, here was an issue Disabling that plugin. (" + args[1] + ")");
                                        }
                                    }
                                    else
                                    {
                                        player.sendMessage("Sorry, That Plugin is already Disabled. (" + args[1] + ")");
                                    }
                                }
                                else
                                {
                                    player.sendMessage("Sorry, That Plugin was not found. (" + args[1] + ")");
                                }
                            }
                            else
                            {
                                player.sendMessage("Sorry, There are no Plugins Loaded.");
                            }
                            break;
                        }
                    case "enable":
                        {
                            if (!player.Op)
                            {
                                player.sendMessage("Error: you must be Op to use this feature.");
                                return;
                            }
                            if (!(args.Count > 0 && args[1] != null && args[0].Trim().Length > 0))
                            {
                                player.sendMessage("Please review your argument count.");
                            }

                            //Get plugin Name
                            String pluginName = string.Join(" ", args);
                            pluginName = pluginName.Remove(0, pluginName.IndexOf(args[1])).Trim();

                            if (Program.server.PluginManager.PluginList.Count > 0)
                            {
                                Plugin fplugin = Program.server.PluginManager.getPlugin(pluginName);
                                if (fplugin != null)
                                {
                                    if (!fplugin.Enabled)
                                    {
                                        if (Program.server.PluginManager.EnablePlugin(fplugin.Name))
                                        {
                                            player.sendMessage(args[1] + " was Enabled!");
                                        }
                                        else
                                        {
                                            player.sendMessage("Sorry, here was an issue Enabling that plugin. (" + args[1] + ")");
                                        }
                                    }
                                    else
                                    {
                                        player.sendMessage("Sorry, That Plugin is already Enabled. (" + args[1] + ")");
                                    }
                                }
                                else
                                {
                                    player.sendMessage("Sorry, That Plugin was not found. (" + args[1] + ")");
                                }
                            }
                            else
                            {
                                player.sendMessage("Sorry, There are no Plugins Loaded.");
                            }
                            break;
                        }
                    default:
                        {
                            player.sendMessage("Please review the usage of this function");
                            break;
                        }
                }
            }
        }
    
    }
}
