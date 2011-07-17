using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server;
using Terraria_Server.Plugin;

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
                for (int i = 0; i < 20; i++)
                {
                    Item.NewItem((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height, 58, 1, false);
                }
            }
            else
            {
                try
                {
                    Player targetPlayer = Program.server.GetPlayerByName(Commands[1]);
                    for (int i = 0; i < 20; i++)
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
                        Main.npcs[i].StrikeNPC(npc.lifeMax, (float)90f, 0);
                        killCount++;
                    }
                }

                player.sendMessage("You butcher'd " + killCount.ToString() + " NPC's!", 255, 0f, 255f, 255f);
            }

        }

        public static void Kit(Player player, String[] Commands)
        {
            //Have to be op
            if (!player.Op)
            {
                player.sendMessage("Error: you must be Op to use /kit");
                return;
            }

            if (Commands.Length > 1)
            {
                //Admin KIT
                if (Commands[1].Equals("admin"))
                {
                    player.sendMessage("You have recieved the Admin kit.");
                    
                    Item.NewItem((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height, 58, 1, false);
                }

                //BUILDER KIT
                else if (Commands[1].Equals("builder"))
                {
                    player.sendMessage("You have recieved the Builder kit.");
                    
                    Item.NewItem((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height, 58, 1, false);
                }

                //Mod KIT
                else if (Commands[1].Equals("mod"))
                {
                    player.sendMessage("You have recieved the Mod kit.");

                }

                //Help ::: Shows what kits there are
                else if (Commands[1].Equals("help"))
                {
                    player.sendMessage("The kits are: admin, builder and mod!");
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
    }
}
