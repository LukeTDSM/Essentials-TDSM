using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;

namespace Essentials
{
    public class Properties : PropertiesFile
    {
        private const bool DEFAULT_KITS_ENABLED = true;
        private const bool DEFAULT_KITS_REQUIRES_OP = true;
        private const int DEFAULT_GOD_ROTATION = 1;
        private const bool DEFAULT_REMEMBER_PLAYER_POSITIONS = true;

        private const string KITS_ENABLED = "kitsenabled";
        private const string KITS_REQUIRES_OP = "kitsrequiresop";
        private const string GOD_ROTATION = "godmode-rotation";
		private const string REMEMBER_PLAYER_POSITIONS = "remember-player-positions";

        public Properties(String propertiesPath) : base(propertiesPath) { }

        public void pushData()
        {
            object temp = KitsEnabled;
            temp = KitsRequiresOp;
            temp = GodModeRotation;
			temp = RememberPlayerPositions;
        }

        public bool KitsEnabled
        {
            get
            {
                return getValue(KITS_ENABLED, DEFAULT_KITS_ENABLED);
            }
        }

        public bool KitsRequiresOp
        {
            get
            {
                return getValue(KITS_REQUIRES_OP, DEFAULT_KITS_REQUIRES_OP);
            }
        }

		public int GodModeRotation
		{
			get
			{
				return getValue(GOD_ROTATION, DEFAULT_GOD_ROTATION);
			}
		}

		public bool RememberPlayerPositions
		{
			get
			{
				return getValue(REMEMBER_PLAYER_POSITIONS, DEFAULT_REMEMBER_PLAYER_POSITIONS);
			}
		}
    }
}
