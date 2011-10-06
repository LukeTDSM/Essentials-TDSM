using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using Terraria_Server;
using Terraria_Server.Commands;
using Terraria_Server.Logging;

using Essentials;
using Essentials.Kit;
using Terraria_Server.Plugins;
using Essentials.God;

namespace Essentials
{
    public class Essentials : BasePlugin
    {
        public Properties properties;
        public Dictionary<int, bool> essentialsPlayerList; //int - player ID, bool - god mode
        public Dictionary<int, bool> essentialsRPGPlayerList; //''
        public KitManager kitManager { get; set; }
        public Dictionary<String, String> lastEventByPlayer;

        public GodMode God { get; set; }

        protected override void Initialized(object state)
        {
            Name = "Essentials";
            Description = "Essential commands for TDSM.";
            Author = "Luke";
            Version = "0.6";
            TDSMBuild = 29;

            string pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + "Essentials";
            string kitsFile = pluginFolder + Path.DirectorySeparatorChar + "kits.xml";
            string propertiesFile = pluginFolder + Path.DirectorySeparatorChar + "essentials.properties";
            
            lastEventByPlayer = new Dictionary<String, String>();
            essentialsPlayerList = new Dictionary<Int32, Boolean>();
            essentialsRPGPlayerList = new Dictionary<Int32, Boolean>();

            if (!Directory.Exists(pluginFolder))
                CreateDirectory(pluginFolder); //Touch Directory, We need this.

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
            catch (Exception)
            {
                Console.Write("Create a parsable file? [Y/n]: ");
                if (Console.ReadLine().ToLower() == "y")
                {
                    kitManager.CreateTemplate();
                    goto LOADKITS; //Go back to reload data ;)...I'm lazy I KNOW
                }
            }
            Log("Complete, Loaded " + kitManager.KitList.Count + " Kit(s)");
        }

        protected override void Enabled()
        {
            ProgramLog.Log(base.Name + " enabled.");

            //Prepare & Start the God Mode Thread.
            God = new GodMode(this);

            //Add Commands
            AddCommand("!")
                .WithAccessLevel(AccessLevel.PLAYER)
                .Calls(Commands.LastCommand);

			AddCommand("bloodmoon")
				.WithAccessLevel(AccessLevel.OP)
				.Calls(Commands.BloodMoon);

            AddCommand("slay")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(Commands.Slay);

            AddCommand("heal")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(Commands.HealPlayer);

			AddCommand("invasion")
				.WithAccessLevel(AccessLevel.OP)
				.Calls(Commands.Invasion);

            AddCommand("ping")
                .WithAccessLevel(AccessLevel.PLAYER)
                .Calls(Commands.ConnectionTest_Ping); //Need to make a single static function

            AddCommand("pong")
                .WithAccessLevel(AccessLevel.PLAYER)
                .Calls(Commands.ConnectionTest_Ping); //^^

            AddCommand("suicide")
                .WithAccessLevel(AccessLevel.PLAYER)
                .Calls(Commands.Suicide);

            AddCommand("butcher")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Kill all NPC's within a radius")
                .WithHelpText("Usage:    butcher <radius>")
                .WithHelpText("          butcher <radius> -g[uide]")
                .Calls(Commands.Butcher);

            AddCommand("kit")
                .WithAccessLevel(AccessLevel.PLAYER)
                .Calls(Commands.Kit);

            AddCommand("god")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(Commands.GodMode);

            AddCommand("spawn")
                .WithAccessLevel(AccessLevel.PLAYER)
                .Calls(Commands.Spawn);

            AddCommand("info")
                .WithAccessLevel(AccessLevel.PLAYER)
                .Calls(Commands.Info);

            AddCommand("setspawn")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(Commands.SetSpawn);

            AddCommand("pm")
                .WithAccessLevel(AccessLevel.PLAYER)
                .Calls(Commands.MessagePlayer);

            Hook(HookPoints.PlayerEnteredGame, OnPlayerEnterGame);
            Hook(HookPoints.UnkownSendPacket, Net.OnUnkownPacketSend);
        }

        protected override void Disabled()
        {
            God.Running = false;
            ProgramLog.Plugin.Log(base.Name + " disabled.");
        }

        public static void Log(LogChannel Level, string message)
        {
            Level.Log("[Essentials] " + message);
        }

        public static void Log(string message)
        {
            Log(ProgramLog.Plugin, message);
        }

        void OnPlayerEnterGame(ref HookContext ctx, ref HookArgs.PlayerEnteredGame args)
        {
            if (essentialsPlayerList.ContainsKey(ctx.Player.Connection.SlotIndex))
            {
                essentialsPlayerList.Remove(ctx.Player.Connection.SlotIndex);
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
