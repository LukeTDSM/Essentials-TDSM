using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server;
using Terraria_Server.Plugin;
using Essentials.Kit;

namespace Essentials
{
    public class Commands
    {
        public static void HealPlayer(Player player, String[] Commands)
        {
            if (!player.Op)
            {
                player.sendMessage("Error: you must be an Op to use /heal");
            }
            else if (Commands.Length < 2)
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
                    Player targetPlayer = Program.server.GetPlayerByName(Commands[1]);
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

        public static void ConnectionTest(Player player, String[] Commands)
        {
            if ((Commands.Length > 1 && (Commands[1].ToLower().Equals("ping") || Commands[1].ToLower().Equals("pong"))) || Commands.Length < 2)
            {
                Commands[0] = Commands[0].Remove(0, 1);
                string message = "";
                for (int i = 0; i < Commands.Length; i++)
                {
                    if (Commands[i].ToLower().Equals("ping"))
                    {
                        message += "pong ";
                    }
                    else if (Commands[i].ToLower().Equals("pong"))
                    {
                        message += "ping ";
                    }
                }
                message = message.Trim() + "!";
                player.sendMessage(message);
            }
            else if (Commands.Length > 1)
            {
                player.sendMessage("This is ping pong! There ain't no room for " + Commands[1] + "!");
            }

        }

        public static bool LastCommand(Dictionary<String, PlayerCommandEvent> lastEventByPlayer, Player player)
        {
            PlayerCommandEvent lastEvent = null;
            lastEventByPlayer.TryGetValue(player.Name, out lastEvent);
            if (lastEvent != null)
            {
                Essentials.Log("Executing last event: [" + lastEvent.Message + "]", "Essentials");
                // send it to the other plugins in case it's a plugin command
                Program.server.PluginManager.processHook(Hooks.PLAYER_COMMAND, lastEvent);
                if (lastEvent.Cancelled)
                {
                    return true;
                }
                else
                {
                    Program.commandParser.parsePlayerCommand(player, lastEvent.Message);
                }
                return true;
            }
            else
            {
                player.sendMessage("Error: no previous command on file");
            }
            return false;
        }
        
        public static void Slay(Player player, String[] Commands)
        {
            if (!player.Op)
            {
                player.sendMessage("Error: you must be Op to use /slay");
            }
            else if (Commands.Length < 2)
            {
                player.sendMessage("Error: you must specify a player to slay");
            }
            else
            {
                try
                {
                    Player targetPlayer = Program.server.GetPlayerByName(Commands[1]);
                    NetMessage.SendData(26, -1, -1, " of unknown causes...", targetPlayer.whoAmi, 0, (float)9999, (float)0);
                    player.sendMessage("OMG! You killed " + Commands[1] + "!", 255, 0f, 255f, 255f);
                    Essentials.Log("Player " + player + " used /slay on " + targetPlayer.Name, "Essentials");
                }
                catch (NullReferenceException)
                {
                    player.sendMessage("Error: Player not online.");
                }
            }
        }

        public static void Suicide(Player player)
        {
            if (!player.Op)
            {
                player.sendMessage("Error: you must be Op to use /suicide");
            }
            else
            {
                NetMessage.SendData(26, -1, -1, " commited suicide!", player.whoAmi, 0, (float)player.statLifeMax, (float)0);
            }
        }

        public static void Butcher(Player player, String[] Commands)
        {
            // if play not op!
            if (!player.Op)
            {
                player.sendMessage("Error: you must be Op to use /butcher");
            }
            else
            {
                int Radius = 7;
                if (Commands.Length > 1 && Commands[1] != null && Commands[1].Trim().Length > 0)
                {
                    try
                    {
                        Radius = Convert.ToInt32(Commands[1]);
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

        public static void Kit(Player player, String[] Commands, KitManager kitManager)
        {
            //Have to be op
            if (!player.Op)
            {
                player.sendMessage("Error: you must be Op to use /kit");
                return;
            }

            if (Commands.Length > 1)
            {
                if (kitManager.ContainsKit(Commands[1]))
                {
                    Kit.Kit kit = kitManager.getKit(Commands[1]);
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
                else if (Commands[1].Equals("help"))
                {
                    String Kits = "";
                    foreach (Kit.Kit kit in kitManager.KitList)
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
                    player.sendMessage("Error: specified kit " + Commands[1] + " does not exist. Please do /kit help");
                }
            }
            //Error message
            else
            {
                player.sendMessage("Error: You did not specify a kit! Do /kit help!");
            }
        }

        public static void GodMode(Player player, Dictionary<Int32, Boolean> playerList)
        {
            if (!player.Op)
            {
                player.sendMessage("Error: you must be Op to use God Mode");
                return;
            }
            
            bool found = false;
            bool godModeStatus = false;
            for (int i = 0; i < playerList.Count; i++ )
            {
                int PlayerID = playerList.Keys.ElementAt(i);
                Player eplayer = Main.players[PlayerID];
                if (eplayer.Name.Equals(player.Name))
                {
                    bool GodMode = !playerList.Values.ElementAt(i);
                    playerList.Remove(PlayerID);
                    playerList.Add(PlayerID, GodMode);
                    godModeStatus = GodMode;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                godModeStatus = true;
                playerList.Add(player.whoAmi, godModeStatus);
            }
            
            player.sendMessage("God Mode Status: " + godModeStatus.ToString());
        }

        public static void Spawn(Player player)
        {
            player.teleportTo(Main.spawnTileX, Main.spawnTileY);
            player.sendMessage("Ypu have been Teleported to Spawn");
        }
    
    }
}
