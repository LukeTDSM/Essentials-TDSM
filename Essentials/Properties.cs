using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;

namespace Essentials
{
    public class Properties : PropertiesFile
    {
        private const bool DEFAULT_WARP_ENABLED = true;
        private const bool DEFAULT_WARP_REQUIRES_OP = true;

        private const string WARP_ENABLED = "warpenabled";
        private const string WARP_REQUIRES_OP = "warprequiresop";

        public Properties(String propertiesPath) : base(propertiesPath) { }

        public void pushData()
        {
            object temp = WarpEnabled;
            temp = WarpRequiresOp;
        }

        public bool WarpEnabled
        {
            get
            {
                return getValue(WARP_ENABLED, DEFAULT_WARP_ENABLED);
            }
        }

        public bool WarpRequiresOp
        {
            get
            {
                return getValue(WARP_REQUIRES_OP, DEFAULT_WARP_REQUIRES_OP);
            }
        }
    }
}
