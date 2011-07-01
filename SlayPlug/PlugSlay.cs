using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using TDSMPlugin;
using Terraria_Server.Plugin;
using Terraria_Server;
using Terraria_Server.Events;

namespace TDSMSlayPlugin
{
    public class SlayPlugin : Plugin
    {
        public Properties properties;
        public bool spawningAllowed = false;
        public bool tileBreakageAllowed = false;
        public bool isEnabled = false;

        public override void Load()
        {
            Name = "SlayPlugin";
            Description = "A Slay Plugin for TDSM!";
            Author = "Luke";
            Version = "1";
            TDSMBuild = 9;

            string pluginFolder = Statics.getPluginPath + Statics.systemSeperator + "TDSM";
            //Create fodler if it doesn't exist
            if (!Program.createDirectory(pluginFolder, true))
            {
                Program.tConsole.WriteLine("[TSDM Plugin] Failed to create crucial Folder");
                return;
            }

            //setup a new properties file
            properties = new Properties(pluginFolder + Statics.systemSeperator + "tdsmplugin.properties");
            properties.Load();
            properties.pushData(); //Creates default values if needed.
            properties.Save();

            //read properties data
            spawningAllowed = properties.isSpawningCancelled();
            tileBreakageAllowed = properties.getTileBreakage();

            isEnabled = true;
        }

        public override void Disable()
        {
            Console.WriteLine(base.Name + " disabled.");
            isEnabled = false;
        }

        public override void Enable()
        {
            Console.WriteLine(base.Name + " enabled.");

            this.registerHook(Hooks.TILE_BREAK);
            this.registerHook(Hooks.PLAYER_COMMAND);
            this.registerHook(Hooks.PLAYER_CHAT);
            this.registerHook(Hooks.PLAYER_CHEST);
            this.registerHook(Hooks.PLAYER_HURT);
            this.registerHook(Hooks.PLAYER_LOGIN);
            this.registerHook(Hooks.PLAYER_LOGOUT);
            this.registerHook(Hooks.PLAYER_PARTYCHANGE);
            this.registerHook(Hooks.PLAYER_PRELOGIN);
            this.registerHook(Hooks.PLAYER_STATEUPDATE);
            this.registerHook(Hooks.CONSOLE_COMMAND);
            this.registerHook(Hooks.PLAYER_LOGIN);
            this.registerHook(Hooks.PLAYER_DEATH);
        }

        public override void onPlayerDeath(PlayerDeathEvent Event)
        {

        }

        public override void onPlayerCommand(PlayerCommandEvent Event)
        {
            if (isEnabled == false) { return; }
            string[] commands = Event.getMessage().ToLower().Split(' '); //Split into sections (to lower case to work with it better)
            if (commands.Length > 0)
            {
                if (commands[0] != null && commands[0].Trim().Length > 0) //If it is nothing, and the string is actually something
                {
                    if (commands[0].Equals("/slay"))
                    {	
                    	(Program.server.GetPlayerByName(Event.getSender().getName())).getDamage(10000);
                    	
                        Program.tConsole.WriteLine("[SlayPlug] Player used Slay Command: " + Event.getPlayer().name);

                        Player sendingPlayer = Event.getPlayer();
                        
                        sendingPlayer.sendMessage("Test." + ServerProtocol, 255, 255f, 255f, 255f);      

                        Event.setCancelled(true);
                    }
                }
            }
        }
        
        public override void onPlayerHurt(PlayerHurtEvent Event)
        {
            Event.setCancelled(false);
        }

        public override void onPlayerJoin(PlayerLoginEvent Event)
        {
            Event.setCancelled(false);
        }

        public override void onPlayerPreLogin(PlayerLoginEvent Event)
        {
            Event.setCancelled(false);
        }

        public override void onPlayerLogout(PlayerLogoutEvent Event)
        {
            Event.setCancelled(false);
        }

        public override void onPlayerPartyChange(PartyChangeEvent Event)
        {
            Event.setCancelled(false);
        }

        public override void onPlayerOpenChest(ChestOpenEvent Event)
        {
            Event.setCancelled(false);
        }

        public override void onPlayerStateUpdate(PlayerStateUpdateEvent Event)
        {
            Event.setCancelled(false);
        }

        public override void onTileBreak(TileBreakEvent Event)
        {
            if (isEnabled == false || tileBreakageAllowed == false) { return; }
            Event.setCancelled(true);
            Program.tConsole.WriteLine("[TSDM Plugin] Cancelled Tile change of Player: " + ((Player)Event.getSender()).name);
        }
        
    }
}
