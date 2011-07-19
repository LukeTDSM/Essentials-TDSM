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
using Essentials.Kit;

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
        private Dictionary<String, PlayerCommandEvent> lastEventByPlayer;
        public Properties properties;
        public KitManager kitManager { get; set; }
        public Dictionary<int, bool> essentialsPlayerList; //int - player ID, bool - god mode

        public override void Load()
        {
            Name = "Essentials";
            Description = "Essential commands for TDSM.";
            Author = "Luke";
            Version = "0.5";
            TDSMBuild = 27;

            string pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + "Essentials";
            string kitsFile = pluginFolder + Path.DirectorySeparatorChar + "kits.xml";
            string propertiesFile = pluginFolder + Path.DirectorySeparatorChar + "essentials.properties";
            
            lastEventByPlayer = new Dictionary<String, PlayerCommandEvent>();
            essentialsPlayerList = new Dictionary<int, bool>();

            if (!Directory.Exists(pluginFolder))
                CreateDirectory(pluginFolder); //Touch Directory, Wee need this.

            //We do not want to delete records!
            if (!File.Exists(kitsFile))
                File.Create(kitsFile).Close();

            if (!File.Exists(propertiesFile))
                File.Create(propertiesFile).Close();

            properties = new Properties(pluginFolder + Path.DirectorySeparatorChar + "essentials.properties");
            properties.Load();
            properties.pushData();
            properties.Save();

            Log("Loading Kits...");
            kitManager = new KitManager(kitsFile);

        LOADKITS:
            try
            {
                    kitManager.LoadKits();
            }
            catch (Exception e)
            {
                Log(e.Message);
                Console.Write("Create a parsable file? [Y/n]: ");
                if (Console.ReadLine().ToLower() == "y")
                {
                    kitManager.CreateTemplate();
                    goto LOADKITS; //Go back to reload data ;)...Im lazy I KNOW
                }
            }
            Log("Complete, Loaded " + kitManager.KitList.Count + " Kit(s)");
            
            isEnabled = true;
        }

        public override void Enable()
        {
            Program.tConsole.WriteLine(base.Name + " enabled.");
            //Register Hooks
            new God.GodMode(this);
            this.registerHook(Hooks.PLAYER_COMMAND);
        }

        public override void Disable()
        {
            Program.tConsole.WriteLine(base.Name + " disabled.");
            isEnabled = false;
        }

        public static void Log(String message, String pluginName)
        {
            Program.tConsole.WriteLine("[" + pluginName + "] " + message);
        }

        public void Log(String message)
        {
           Log(message, base.Name);
        }
        
        public override void onPlayerCommand(PlayerCommandEvent Event)
        {
            if (isEnabled == false) { return; }
            String[] commands = Event.Message.ToLower().Split(' '); //Split into sections (to lower case to work with it better)
            if (commands.Length > 0)
            {
                if (commands[0] != null && commands[0].Trim().Length > 0 && commands[0].Substring(0, 1).Equals("/")) //If it is not nothing, and the string is actually something
                {
                    switch (commands[0].Remove(0, 1)) //Remove '/' from command
                    {
                        case "!": //Last Command
                            {
                                Commands.LastCommand(lastEventByPlayer, Event.Player);
                                break;
                            }
                        case "slay":
                            {
                                Commands.Slay(Event.Player, commands);
                                Event.Cancelled = true;
                                break;
                            }
                        case "heal":
                            {
                                Commands.HealPlayer(Event.Player, commands);
                                Event.Cancelled = true;
                                break;
                            }
                        case "ping":
                            {
                                Commands.ConnectionTest(Event.Player, commands);
                                Event.Cancelled = true;
                                break;
                            }
                        case "pong":
                            {
                                goto case "ping";
                            }
                        case "suicide":
                            {
                                Commands.Suicide(Event.Player);
                                Event.Cancelled = true;
                                break;
                            }
                        case "butcher":
                            {
                                Commands.Butcher(Event.Player, commands);
                                Event.Cancelled = true;
                                break;
                            }
                        case "kit":
                            {
                                if (properties.KitsEnabled)
                                {
                                    Commands.Kit(Event.Player, commands, kitManager);
                                    Event.Cancelled = true;
                                }
                                break;
                            }
                        case "god":
                            {
                                Commands.GodMode(Event.Player, essentialsPlayerList);
                                Event.Cancelled = true;
                                break;
                            }
                        case "spawn":
                            {
                                Commands.Spawn(Event.Player);
                                Event.Cancelled = true;
                                break;
                            }
                        case "info":
                            {
                                Commands.Info(Event.Player);
                                Event.Cancelled = true;
                                break;
                            }
                        case "plugins":
                            {
                                Commands.Plugins(Event.Player, commands);
                                Event.Cancelled = true;
                                break;
                            }
                        case "plugin":
                            {
                                goto case "plugins";
                            }
                    }
                    
                    lastEventByPlayer[Event.Player.Name] = Event; //Register last command
                }
            }
        }
        private static void CreateDirectory(String dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }
    }
}
