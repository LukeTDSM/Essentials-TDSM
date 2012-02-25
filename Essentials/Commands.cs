using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server;
using Terraria_Server.Commands;
using Terraria_Server.Logging;

using Terraria_Server.Misc;
using Terraria_Server.WorldMod;
using Essentials.Kits;
using Essentials.Warps;

namespace Essentials
{
    public class Commands
    {
		public static void BloodMoon(ISender sender, ArgumentList args)
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
                            World.SetTime(53999, false, false);
					}
                    Essentials.Log("Triggered blood moon phase.");
				}
				else
				{
                    Server.notifyAll("Blood Moon disabled");
					Main.bloodMoon = false;
					Essentials.Log("Disabled blood moon phase.");
				}
				NetMessage.SendData((int)Packet.WORLD_DATA);
			}
			else
			{
				if (!Main.bloodMoon)
				{
					Main.bloodMoon = true;
					World.SetTime(0, false, false);
					NetMessage.SendData(25, -1, -1, "The Blood Moon is rising...", 255, 50f, 255f, 130f);
                    Essentials.Log("Triggered blood moon phase.");
				}
				else
				{
					Server.notifyAll("Blood Moon disabled");
					Main.bloodMoon = false;
                    Essentials.Log("Disabled blood moon phase.");
				}
				NetMessage.SendData((int)Packet.WORLD_DATA);
			}
		}

        public static void HealPlayer(ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                Player player = (Player)sender;
                if (args.Count < 1)
                {
                    player.sendMessage("You did not specify the player, so you were healed");

                    for (int i = 0; i < player.statLifeMax - player.statLife; i++)
                        Item.NewItem((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height, 58, 1, false);

					Essentials.Log(player.Name + " healed " + player.Name + ".");
                }
                else
                {
                    try
                    {
                        Player targetPlayer = Server.GetPlayerByName(args[0]);
                        for (int i = 0; i < targetPlayer.statLifeMax - targetPlayer.statLife; i++)
                            Item.NewItem((int)targetPlayer.Position.X, (int)targetPlayer.Position.Y, targetPlayer.Width, targetPlayer.Height, 58, 1, false);
                     
                        player.sendMessage("You have healed that player!");
						Essentials.Log(player.Name + " healed " + targetPlayer.Name + ".");
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
                    Essentials.Log("You cannot heal yourself as the console.");
                else
                {
                    try
                    {
                        Player targetPlayer = Server.GetPlayerByName(args[0]);
                        for (int i = 0; i < targetPlayer.statLifeMax - targetPlayer.statLife; i++)
                            Item.NewItem((int)targetPlayer.Position.X, (int)targetPlayer.Position.Y, targetPlayer.Width, targetPlayer.Height, 58, 1, false);
                        
                        Essentials.Log("Console healed " + targetPlayer.Name + ".");
                    }
                    catch (NullReferenceException)
                    {
                        Essentials.Log(ProgramLog.Error, "Player not online.");
                    }
                }
			}
        }

		public static void Invasion(ISender sender, ArgumentList args)
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
                            direction = 0;
						else if (args[i].ToLower().Equals("east"))
                            direction = Main.maxTilesX;
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
                    player.sendMessage("Setting invasion size, delay and direction to defaults.");

				Main.invasionX = direction;
				Main.invasionSize = size;
				Main.invasionDelay = delay;
				Main.invasionType = InvasionType.GOBLIN_ARMY;
				player.sendMessage("Set invasion to start, size " + Main.invasionSize.ToString() + ", type " + Main.invasionType.ToString() + ", delay " + Main.invasionDelay.ToString() + ".");
				NetMessage.SendData((int)Packet.WORLD_DATA);
			}
		}

        public static void ConnectionTest_Ping(ISender sender, ArgumentList args)
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
                            message += "pong ";
                        else if (args[i].ToLower().Equals("pong"))
                            message += "ping ";
                    }
                    message = message.Trim() + "!";
                    player.sendMessage(message);
                }
                else if (args.Count > 0)
                    player.sendMessage("This is ping pong! There ain't no room for " + args[0] + "!");
            }
        }

        public static void LastCommand(ISender sender, ArgumentList args)
        {
            Essentials Plugin = (Essentials)args.Plugin;
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
                            if (Plugin.lastEventByPlayer.Keys.Contains(sender.Name))
                                Plugin.lastEventByPlayer.Remove(sender.Name);

                            Plugin.lastEventByPlayer.Add(sender.Name, Command);
                            sender.sendMessage("Command registered!");
                        }
                        else
                            sender.sendMessage("Please specify a command");

                        return;
                    }
                }
                Player player = (Player)sender;
                String Message;
                Plugin.lastEventByPlayer.TryGetValue(player.Name, out Message);
                if (Message != null && Message.Length > 0)
                {
                    Essentials.Log("Executing last event: [" + Message + "]");

                    //This also calls to plugins
                    Program.commandParser.ParseAndProcess(player, Message);
                }
                else
                    player.sendMessage("Error: no previous command on file");
            }            
            //return false;
        }

        public static void Slay(ISender sender, ArgumentList args)
        {
            var player = args.GetOnlinePlayer(0);

            NetMessage.SendData(26, -1, -1, " of unknown causes...", player.whoAmi, 0, (float)9999, (float)0);
            sender.sendMessage("OMG! You killed " + player.Name + "!", 255, 0f, 255f, 255f);
            Essentials.Log("Player " + player + " used /slay on " + player.Name);          
        }

        public static void Suicide(ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                Player player = (Player)sender;
                if (!player.Op)
                    player.sendMessage("Error: you must be Op to use /suicide");
                else
                    NetMessage.SendData(26, -1, -1, " commited suicide!", player.whoAmi, 0, (float)player.statLifeMax, (float)0);
            }
        }

        public static void Butcher(ISender sender, ArgumentList args)
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
                            continue;

                        float direction = -1;
                        if (new Random().Next(-1, 0) == 0)
                            direction = 0;

                        NetMessage.SendData(28, -1, -1, "", npc.whoAmI, 9999, 10f, direction, 0);
                        if (Main.npcs[i].StrikeNPCInternal(npc.lifeMax, 9999, (int)direction, true) > 0.0)
                            killCount++;
                    }
                }

                player.sendMessage("You butchered " + killCount.ToString() + " NPC's!", 255, 0f, 255f, 255f);
            }            
        }

        public static void Kit(ISender sender, ArgumentList args)
        {
            Essentials Plugin = (Essentials)args.Plugin;

            Player player = args.GetOnlinePlayer(0);

            if (args.Count > 0)
            {
                if (KitManager.ContainsKit(args[0]))
                {
                    Kit kit = KitManager.GetKit(args[0]);
                    if (kit.ItemList != null && kit.ItemList.Count > 0)
                    {
                        foreach (KeyValuePair<Int32, Int32> ItemID in kit.ItemList)
                            Item.NewItem((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height, ItemID.Key, ItemID.Value, false);

                        player.sendMessage("Recived the '" + kit.Name + "' Kit.");
                    }
                    else
                        player.sendMessage("Issue with null kit/list");
                }

                //Help ::: Shows what kits there are
                else if (args[0].Equals("help"))
                {
                    String Kits = "";
                    foreach (Kit kit in KitManager.KitList)
                    {
                        if (kit.Name.Trim().Length > 0)
                            Kits = Kits + ", " + kit.Name;
                    }
                    if (Kits.StartsWith(","))
                        Kits = Kits.Remove(0, 1).Trim();

                    if (Kits.Length > 0)
                        player.sendMessage("Available Kits: " + Kits);

                }

                //If kit does not exist
                else
                    player.sendMessage("Error: specified kit " + args[0] + " does not exist. Please do /kit help");
            }
            //Error message
            else
                player.sendMessage("Error: You did not specify a kit! Do /kit help!");      
        }

        public static void GodMode(ISender sender, ArgumentList args)
        {
            Essentials Plugin = (Essentials)args.Plugin;

            Player player = args.GetOnlinePlayer(0);
            //if (!(sender is Player))
            //{
            //    if (!args.TryGetOnlinePlayer(1, out player))
            //    {
            //        sender.sendMessage("As a non player, Please specify one!");
            //        return;
            //    }
            //}

            if (player.HasClientMod)
            {
                //Tell the client to use God.
                bool On;
                if (Plugin.essentialsRPGPlayerList.TryGetValue(player.whoAmi, out On))
                {
                    NetMessage.SendData((int)Packets.CLIENT_MOD_GOD, player.whoAmi, -1, "", 0);
                    if (!Server.AllowTDCMRPG)
                        Plugin.essentialsRPGPlayerList.Remove(player.whoAmi);
                }
                else
                {
                    NetMessage.SendData((int)Packets.CLIENT_MOD_GOD, player.whoAmi, -1, "", 1);
                    if (!Server.AllowTDCMRPG)
                        Plugin.essentialsRPGPlayerList.Add(player.whoAmi, true);
                }

                return;
            }

            bool found = false;
            bool godModeStatus = false;
            for (int i = 0; i < Plugin.essentialsPlayerList.Count; i++)
            {
                int PlayerID = Plugin.essentialsPlayerList.Keys.ElementAt(i);
                Player eplayer = Main.players[PlayerID];
                if (eplayer.Name.Equals(player.Name))
                {
                    bool GodMode = !Plugin.essentialsPlayerList.Values.ElementAt(i);
                    Plugin.essentialsPlayerList.Remove(PlayerID);
                    Plugin.essentialsPlayerList.Add(PlayerID, GodMode);
                    godModeStatus = GodMode;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                godModeStatus = true;
                Plugin.essentialsPlayerList.Add(player.whoAmi, godModeStatus);
            }
            
            player.sendMessage("God Mode Status: " + godModeStatus.ToString());            
        }

        public static void Spawn(ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                Player player = (Player)sender;
                player.Teleport(Main.spawnTileX * 16f, Main.spawnTileY * 16f);
                player.sendMessage("You have been Teleported to Spawn");
            }
        }

        public static void Info(ISender sender, ArgumentList args)
        {
            sender.sendMessage("Essentials Plugin for TDSM b" + Statics.BUILD, 255, 160f, 32f, 240f);
            String platform = Platform.Type.ToString();
            switch (Platform.Type)
            {
                case Platform.PlatformType.LINUX:
					platform = "Linux";
                    break;
                case Platform.PlatformType.MAC:
					platform = "Mac";
                    break;
                case Platform.PlatformType.WINDOWS:
					platform = "Windows";
                    break;
            }
            sender.sendMessage("The current OS running this sever is: " + platform, 255, 160f, 32f, 240f);        
        }

        public static void SetSpawn(ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                var player = sender as Player;
                var saveWorld = args.TryPop("-save");
                
                Main.spawnTileX = (int)(player.Position.X / 16);
                Main.spawnTileY = (int)(player.Position.Y / 16);

                if (saveWorld)
                    WorldIO.SaveWorld(World.SavePath);

                Server.notifyOps(String.Format("{0} set Spawn to {1}, {2}.", sender.Name, Main.spawnTileX, Main.spawnTileY));
            }
        }

        public static void MessagePlayer(ISender sender, ArgumentList args)
        {
            var Player = args.GetOnlinePlayer(0);
            var Message = args.GetString(1);

            Player.sendMessage(String.Format("[{0}] PM: {1}", sender.Name, Message), ChatColor.DarkGray);
            Server.notifyOps(String.Format("PM {0} => {1}: {2}", sender.Name, Player.Name, Message));
        }

        public static void Warp(ISender sender, ArgumentList args)
        {
            if (sender is Player)
            {
                string warpName = args.GetString(0);
                Player player = sender as Player;

                Warp warp = WarpManager.GetWarp(warpName);

                if (warp == default(Warp) || !warp.IsUserAccessible(player))
                    throw new CommandError("Warp not found!");

                if (warp.Location == default(Vector2))
                    throw new CommandError("Warp Location is invalid!");

                if (!player.Teleport(warp.Location))
                    throw new CommandError("There was an error in the Teleport.");
            }
        }

        public static void SetWarp(ISender sender, ArgumentList args)
        {
            Vector2 Location = default(Vector2);

            if (!(sender is Player))
            {
                int x, y;
                if (!args.TryParseTwo<Int32, Int32>("", out x, "", out y))
                    throw new CommandError("Non Player Senders need to provide -x & -y tile indices.");
            }
            else
                Location = (sender as Player).Position / 16;

            string name = args.GetString(0);

            Warp warp = new Warp()
            {
                Name = name,
                Location = Location,
                Type = WarpType.PUBLIC,
                Users = new List<String>()
            };

            WarpManager.WarpList.Add(warp);
            WarpManager.Save((args.Plugin as Essentials).WarpFile, "warp");

            sender.Message("Warp {0} has been created.", warp.Name);
        }
        
        /* Following few are untested */
        public static void ManageWarp(ISender sender, ArgumentList args)
        {
            //remove/add players, delete warp
            bool delete = args.TryPop("delete");
            bool remove = args.TryPop("removeplayer");
            bool add    = args.TryPop("addplayer");
            string username = args.GetString(0);

            int warpIndex;
            if (!WarpManager.ContainsWarp(username, out warpIndex))
                throw new CommandError("No Warp by that name exists.");

            if (delete)
            {
                WarpManager.WarpList.RemoveAt(warpIndex);
                sender.sendMessage("Warp successfully removed.");
            }
            else if (remove)
            {
                Warp warp = WarpManager.WarpList.ElementAt(warpIndex);

                int usrIndex;
                if (warp.ContainsUser(username, out usrIndex))
                {
                    warp.Users.RemoveAt(usrIndex);
                    WarpManager.UpdateWarp(warp);
                    sender.Message("{0} has been removed from {1}", username, warp.Name);
                }
                else
                    throw new CommandError("No user exists.");
            }
            else if (add)
            {

                Warp warp = WarpManager.WarpList.ElementAt(warpIndex);

                int usrIndex;
                if (!warp.ContainsUser(username, out usrIndex))
                {
                    warp.Users.Add(WarpManager.CleanUserName(username));
                    WarpManager.UpdateWarp(warp);
                    sender.Message("{0} has been added to {1}", username, warp.Name);
                }
                else
                    throw new CommandError("User exists already.");
            }
            else
                throw new CommandError("No Manage Action defined.");
        }

        public static void ListWarp(ISender sender, ArgumentList args)
        {
            if (WarpManager.WarpList.Count < 1)
                throw new CommandError("There are no warps.");

            int maxPages = WarpManager.WarpList.Count / 5;
            int page = args.GetInt(0);

            if (maxPages > 0 && page > maxPages - 1 || page < 1)
            {
                throw new CommandError("Pages: 1 => {0}", maxPages);
            }

            page *= 5;

            if (page >= WarpManager.WarpList.Count)
                page = WarpManager.WarpList.Count;

            int start = page - 5;
            if (page < 4)
                start = 0;

            for (int i = start; i < page; i++)
            {
                Warp warp = WarpManager.WarpList.ToArray()[i];

                if (warp.IsUserAccessible(sender.Name) || !(sender is Player) || sender.Op) //!player's = OP ??
                {
                    sender.Message("Warp: {0}", warp.Name);
                }
                else
                {
                    sender.Message("<PRIVATE>");
                }
            }
        }
    }
}
