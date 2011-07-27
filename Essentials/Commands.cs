using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server;
using Terraria_Server.Commands;
using Terraria_Server.Logging;
using Terraria_Server.Plugin;

using Essentials.Kit;

namespace Essentials
{
    public class Commands
    {
		public static void BloodMoon(Server server, ISender sender, ArgumentList args)
		{
			if (sender is Player)
			{
				Player player = sender as Player;
				if (!Main.bloodMoon)
				{
					Main.bloodMoon = true;
					if (args.Count > 0)
					{
						if (!args[0].ToLower().Equals("time:false"))
						{
							server.World.setTime(53999, false, false);
						}
					}
					ProgramLog.Admin.Log("[" + Essentials.pluginName + "]: Triggered blood moon phase.");
				}
				else
				{
					server.notifyAll("Blood Moon disabled");
					Main.bloodMoon = false;
					ProgramLog.Admin.Log("[" + Essentials.pluginName + "]: Disabled blood moon phase.");
				}
				NetMessage.SendData((int)Packet.WORLD_DATA);
			}
			else
			{
				if (!Main.bloodMoon)
				{
					Main.bloodMoon = true;
					server.World.setTime(0, false, false);
					NetMessage.SendData(25, -1, -1, "The Blood Moon is rising...", 255, 50f, 255f, 130f);
					ProgramLog.Admin.Log("[" + Essentials.pluginName + "]: Triggered blood moon phase.");
				}
				else
				{
					server.notifyAll("Blood Moon disabled");
					Main.bloodMoon = false;
					ProgramLog.Admin.Log("[" + Essentials.pluginName + "]: Disabled blood moon phase.");
				}
				NetMessage.SendData((int)Packet.WORLD_DATA);
			}
		}

        public static void HealPlayer(Server server, ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                Player player = (Player)sender;
                if (args.Count < 1)
                {
                    player.sendMessage("You did not specify the player, so you were healed");
                    for (int i = 0; i < player.statLifeMax - player.statLife; i++)
                    {
                        Item.NewItem((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height, 58, 1, false);
                    }
					ProgramLog.Admin.Log("[" + Essentials.pluginName + "]: " + player.Name + " healed " + player.Name + ".");
                }
                else
                {
                    try
                    {
                        Player targetPlayer = Program.server.GetPlayerByName(args[0]);
                        for (int i = 0; i < targetPlayer.statLifeMax - targetPlayer.statLife; i++)
                        {
                            Item.NewItem((int)targetPlayer.Position.X, (int)targetPlayer.Position.Y, targetPlayer.Width, targetPlayer.Height, 58, 1, false);
                        }
                        player.sendMessage("You have healed that player!");
						ProgramLog.Admin.Log("[" + Essentials.pluginName + "]: " + player.Name + " healed " + targetPlayer.Name + ".");
                    }
                    catch (NullReferenceException)
                    {
                        player.sendMessage("Error: Player not online.");
                    }
                }
            }
           	else
			{
				if (args.Count < 1)
                {
                    ProgramLog.Admin.Log("[" + Essentials.pluginName + "]: You cannot heal yourself as the console.");
                }
                else
                {
                    try
                    {
                        Player targetPlayer = Program.server.GetPlayerByName(args[0]);
                        for (int i = 0; i < targetPlayer.statLifeMax - targetPlayer.statLife; i++)
                        {
                            Item.NewItem((int)targetPlayer.Position.X, (int)targetPlayer.Position.Y, targetPlayer.Width, targetPlayer.Height, 58, 1, false);
                        }
						ProgramLog.Admin.Log("[" + Essentials.pluginName + "]: console healed " + targetPlayer.Name + ".");
                    }
                    catch (NullReferenceException)
                    {
                        ProgramLog.Error.Log("[" + Essentials.pluginName + "]: Error: Player not online.");
                    }
                }
			}
        }

		public static void Invasion(Server server, ISender sender, ArgumentList args)
		{
			int direction = 0;
			int size = 100;
			int delay = 0;
			if (sender is Player)
			{
				Player player = sender as Player;
				if (args.Count > 0)
				{
					for (int i = 0; i < args.Count; i++)
					{
						if (args[i].ToLower().Equals("end"))
						{
							Main.invasionSize = 0;
							player.sendMessage("Invasion ended.");
							NetMessage.SendData((int)Packet.WORLD_DATA);
							return;
						}
						if (args[i].ToLower().Equals("west"))
						{
							direction = 0;
						}
						else if (args[i].ToLower().Equals("east"))
						{
							direction = Main.maxTilesX;
						}
						else if (args[i].ToLower().Contains("size:"))
						{
							try
							{
								size = Int32.Parse(args[i].Remove(0, 5));
							}
							catch
							{
								player.sendMessage("Error parsing invasion size; setting to default (100).");
							}
						}
						else if (args[i].ToLower().Contains("delay:"))
						{
							try
							{
								delay = Int32.Parse(args[i].Remove(0, 6));
							}
							catch
							{
								player.sendMessage("Error parsing invasion delay; setting to default (0).");
							}
						}
					}
				}
				else
				{
					player.sendMessage("Setting invasion size, delay and direction to defaults.");
				}
				Main.invasionX = direction;
				Main.invasionSize = size;
				Main.invasionDelay = delay;
				Main.invasionType = 1;
				player.sendMessage("Set invasion to start, size " + Main.invasionSize.ToString() + ", type " + Main.invasionType.ToString() + ", delay " + Main.invasionDelay.ToString() + ".");
				NetMessage.SendData((int)Packet.WORLD_DATA);
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

        public static void LastCommand(Server server, ISender sender, ArgumentList args)
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
            Player player = (Player)sender;
            if (args.Count < 1)
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

                Boolean KillGuide = (args.Count > 1 && args[1].Trim().Length > 0 && args[1].Trim().ToLower().Equals("-g")); //Burr

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
                    int NPC_X = (int)npc.Position.X / 16;
                    int NPC_Y = (int)npc.Position.Y / 16;
                    int Player_X = (int)player.Position.X / 16;
                    int Player_Y = (int)player.Position.Y / 16;

                    if ((Math.Max(Player_X, NPC_X) - Math.Min(Player_X, NPC_X)) <= Radius &&
                        (Math.Max(Player_Y, NPC_Y) - Math.Min(Player_Y, NPC_Y)) <= Radius)
                    {
                        if (npc.Name.ToLower().Equals("guide") && !KillGuide)
                        {
                            continue;
                        } 
                        float direction = -1;
                        if (new Random().Next(-1, 0) == 0)
                        {
                            direction = 0;
                        }
                        NetMessage.SendData(28, -1, -1, "", npc.whoAmI, 9999, 10f, direction, 0);
                        if (Main.npcs[i].StrikeNPC(npc.lifeMax, 9999, (int)direction) > 0.0) //Bahah fly little ****ers
                        {
                            killCount++;
                        }
                    }
                }

                player.sendMessage("You butchered " + killCount.ToString() + " NPC's!", 255, 0f, 255f, 255f);
            }            
        }

        public static void Kit(Server server, ISender sender, ArgumentList args)
        {
            Player player = (Player)sender;
            if (!(sender is Player))
            {
                if (!args.TryGetOnlinePlayer(1, out player))
                {
                    sender.sendMessage("As a non player, Please specify one!");
                    return;
                }
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

        public static void GodMode(Server server, ISender sender, ArgumentList args)
        {
            Player player = (Player)sender;
            if (!(sender is Player))
            {
                if (!args.TryGetOnlinePlayer(1, out player))
                {
                    sender.sendMessage("As a non player, Please specify one!");
                    return;
                }
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
            sender.sendMessage("Essentials Plugin for TDSM b" + Statics.BUILD, 255, 160f, 32f, 240f);
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
            sender.sendMessage("The current OS running this sever is: " + Platform, 255, 160f, 32f, 240f);        
        }

    }
}
