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
            Author = "Essentials";
            Version = "4.0";
            TDSMBuild = 26;

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
                if (commands[0] != null && commands[0].Trim().Length > 0) //If it is not nothing, and the string is actually something
                {
                 //Last command COMMAND!
                    if (commands[0].Equals("/!"))
                    {
                        Commands.LastCommand(lastEventByPlayer, Event.Player);
                    }
                    
                    //Slay COMMAND
                    else if (commands[0].Equals("/slay"))
                    {
                        Commands.Slay(Event.Player, commands);
                        Event.Cancelled = true;
                    }

                   //HEAL COMMAND!
                    else if (commands[0].Equals("/heal"))
                    {
                        Commands.HealPlayer(Event.Player, commands);
                        Event.Cancelled = true;
                    }

                    //Ping! Command!
                    else if (commands[0].Equals("/ping") || commands[0].Equals("/pong"))
                    {
                        Commands.ConnectionTest(Event.Player, commands);
                        Event.Cancelled = true;
                    }

                    //SUICIDE COMMAND!
                    else if (commands[0].Equals("/suicide"))
                    {
                        Commands.Suicide(Event.Player);
                        Event.Cancelled = true;
                    }

                    //Butcher
                    else if (commands[0].Equals("/butcher"))
                    {
                        Commands.Butcher(Event.Player, commands);
                        Event.Cancelled = true;
                    }

                    //Kits!
                    else if (commands[0].Equals("/kit"))
                    {
                        if (properties.KitsEnabled)
                        {
                            Commands.Kit(Event.Player, commands, kitManager);
                            Event.Cancelled = true;
                        }
                    }

                    //God Mode!
                    else if (commands[0].Equals("/god"))
                    {
                        if (properties.KitsEnabled)
                        {
                            Commands.GodMode(Event.Player, essentialsPlayerList);
                            Event.Cancelled = true;
                        }
                    }

                    if (commands[0].Substring(0, 1).Equals("/"))
                    {
                        lastEventByPlayer[Event.Player.Name] = Event;
                    }
                        
                    //GOD COMMAND!
                    /*else if (commands[0].Equals("/god"))
                    {
                        Needs a thread first.
                        if (!Event.Player.Op)
                        {
                            Event.Player.sendMessage("Error: you must be an Op to use /god");
                        }
                        
                        if (commands[1].Equals("off"))
                        {
                            Event.Player.sendMessage("You are a GOD!");
                            
                            while(true) 
                            {
                                Player player = Event.Player;
                                Item.NewItem((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height, 58, 1, false);
                            }
                        }

                        Event.Cancelled = true;
                    }*/
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
