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

        private const string KITS_ENABLED = "kitsenabled";
        private const string KITS_REQUIRES_OP = "kitsrequiresop";

        public Properties(String propertiesPath) : base(propertiesPath) { }

        public void pushData()
        {
            object temp = KitsEnabled;
            temp = KitsRequiresOp;
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
    }
}
