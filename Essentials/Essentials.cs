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
using Essentials.Kits;
using Terraria_Server.Plugins;
using Essentials.God;
using Essentials.Warps;
using System.Threading;

namespace Essentials
{
    public class Essentials : BasePlugin
    {
        public Properties properties;
        public Dictionary<Int32, Boolean> essentialsPlayerList; //int - player ID, bool - god mode
        public Dictionary<Int32, Boolean> essentialsRPGPlayerList; //''
        public Dictionary<String, String> lastEventByPlayer;

        public string KitFile { get; set; }
        public string WarpFile { get; set; }
        public GodMode God { get; set; }

        protected override void Initialized(object state)
        {
            Name = "Essentials";
            Description = "Essential commands for TDSM.";
            Author = "Luke";
            Version = "0.6";
            TDSMBuild = 36;

            string pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + "Essentials";
            string propertiesFile = pluginFolder + Path.DirectorySeparatorChar + "essentials.properties";

            KitFile = pluginFolder + Path.DirectorySeparatorChar + "kits.xml";
            WarpFile = pluginFolder + Path.DirectorySeparatorChar + "warps.xml";
            
            lastEventByPlayer = new Dictionary<String, String>();
            essentialsPlayerList = new Dictionary<Int32, Boolean>();
            essentialsRPGPlayerList = new Dictionary<Int32, Boolean>();

            if (!Directory.Exists(pluginFolder))
                CreateDirectory(pluginFolder); //Touch Directory, We need this.

            //We do not want to delete records!
            if (!File.Exists(KitFile))
                File.Create(KitFile).Close();
            if (!File.Exists(WarpFile))
                File.Create(WarpFile).Close();

            if (!File.Exists(propertiesFile))
                File.Create(propertiesFile).Close();

            properties = new Properties(pluginFolder + Path.DirectorySeparatorChar + "essentials.properties");
            properties.Load();
            properties.pushData();
            properties.Save();
        }

        public void LoadData<T>(string RecordsFile, string Identifier, 
                                Func<String, List<T>> LoadMethod, Action<String, String> CreateMethod)
        {
            Log("Loading {0}s...", Identifier);

        LOAD:
            List<T> Items = null;
            try
            {
                Items = LoadMethod.Invoke(RecordsFile);
            }
            catch (Exception)
            {
                Console.Write("Create a parsable file? [Y/n]: ");
                if (Console.ReadLine().ToLower() == "y")
                {
                    CreateMethod.Invoke(RecordsFile, Identifier);
                    goto LOAD;
                }
            }

            Log("Complete, Loaded " + ((Items != null) ? Items.Count : 0) + " {0}(s)", Identifier);
        }

        protected override void Enabled()
        {
            //Prepare & Start the God Mode Thread.
            God = new GodMode(this);

            //Add Commands
            AddCommand("!")
                .WithAccessLevel(AccessLevel.PLAYER)
                .Calls(Commands.LastCommand)
                .WithPermissionNode("essentials.!");

			AddCommand("bloodmoon")
				.WithAccessLevel(AccessLevel.OP)
                .Calls(Commands.BloodMoon)
                .WithPermissionNode("essentials.bloodmoon");

            AddCommand("slay")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(Commands.Slay)
                .WithPermissionNode("essentials.slay");

            AddCommand("heal")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(Commands.HealPlayer)
                .WithPermissionNode("essentials.heal");

			AddCommand("invasion")
				.WithAccessLevel(AccessLevel.OP)
				.Calls(Commands.Invasion)
                .WithPermissionNode("essentials.invasion");

            AddCommand("ping")
                .WithAccessLevel(AccessLevel.PLAYER)
                .Calls(Commands.ConnectionTest_Ping) //Need to make a single static function
                .WithPermissionNode("essentials.ping");

            AddCommand("pong")
                .WithAccessLevel(AccessLevel.PLAYER)
                .Calls(Commands.ConnectionTest_Ping) //^^
                .WithPermissionNode("essentials.pong");

            AddCommand("suicide")
                .WithAccessLevel(AccessLevel.PLAYER)
                .Calls(Commands.Suicide)
                .WithPermissionNode("essentials.suicide");

            AddCommand("butcher")
                .WithAccessLevel(AccessLevel.OP)
                .WithDescription("Kill all NPC's within a radius")
                .WithHelpText("Usage:    butcher <radius>")
                .WithHelpText("          butcher <radius> -g[uide]")
                .Calls(Commands.Butcher)
                .WithPermissionNode("essentials.butcher");

            AddCommand("kit")
                .WithAccessLevel(AccessLevel.PLAYER)
                .Calls(Commands.Kit)
                .WithPermissionNode("essentials.kit");

            AddCommand("god")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(Commands.GodMode)
                .WithPermissionNode("essentials.god");

            AddCommand("spawn")
                .WithAccessLevel(AccessLevel.PLAYER)
                .Calls(Commands.Spawn)
                .WithPermissionNode("essentials.spawn");

            AddCommand("info")
                .WithAccessLevel(AccessLevel.PLAYER)
                .Calls(Commands.Info)
                .WithPermissionNode("essentials.info");

            AddCommand("setspawn")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(Commands.SetSpawn)
                .WithPermissionNode("essentials.setspawn");

            AddCommand("pm")
                .WithAccessLevel(AccessLevel.PLAYER)
                .Calls(Commands.MessagePlayer)
                .WithPermissionNode("essentials.pm");

            AddCommand("warp")
                .WithAccessLevel(AccessLevel.PLAYER)
                .Calls(Commands.Warp)
                .WithPermissionNode("essentials.warp");

            AddCommand("setwarp")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(Commands.SetWarp)
                .WithPermissionNode("essentials.setwarp");

            AddCommand("mwarp")
                .WithAccessLevel(AccessLevel.OP)
                .Calls(Commands.ManageWarp)
                .WithPermissionNode("essentials.mwarp");

            AddCommand("warplist")
                .WithAccessLevel(AccessLevel.PLAYER)
                .Calls(Commands.ListWarp)
                .WithPermissionNode("essentials.warplist");

            Hook(HookPoints.PlayerEnteredGame, OnPlayerEnterGame);
            Hook(HookPoints.UnkownSendPacket, Net.OnUnkownPacketSend);
            Hook(HookPoints.WorldLoaded, OnWorldLoaded);

            ProgramLog.Log(base.Name + " enabled.");
        }

        protected override void Disabled()
        {
            God.Running = false;

            while (God.Running)
                Thread.Sleep(100);

            God.Dispose();

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

        public static void Log(string message, params object[] args)
        {
            Log(String.Format(message, args));
        }

#region Hooks

        void OnWorldLoaded(ref HookContext ctx, ref HookArgs.WorldLoaded args)
        {
            /* For the template Warp, it uses  spawn axis, so they need to be loaded. */
            LoadData(KitFile, typeof(Kit).Name, KitManager.LoadData, KitManager.CreateTemplate);
            LoadData(WarpFile, typeof(Warp).Name, WarpManager.LoadData, WarpManager.CreateTemplate);
        }

        void OnPlayerEnterGame(ref HookContext ctx, ref HookArgs.PlayerEnteredGame args)
        {
            if (essentialsPlayerList.ContainsKey(ctx.Player.Connection.SlotIndex))
            {
                essentialsPlayerList.Remove(ctx.Player.Connection.SlotIndex);
            }
        }

#endregion
       
        private static void CreateDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }
    }
}
