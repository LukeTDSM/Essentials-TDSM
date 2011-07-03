using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server;

namespace Essentials
{
    public class Properties : PropertiesFile
    {
        public Properties(String pFile)
        {
            base.setFile(pFile);
        }

        public void pushData()
        {
            setWarpEnabled(isWarpEnabled());
            setRequiresOp(warpRequiresOp());
        }

        public bool isWarpEnabled()
        {
            string WarpEnabled = base.getValue("warpEnabled");
            if (WarpEnabled == null || WarpEnabled.Trim().Length < 0)
            {
                return true;
            }
            else
            {
                return Boolean.Parse(WarpEnabled);
            }
        }

        public void setWarpEnabled(bool WarpEnabled)
        {
            base.setValue("warpEnabled", WarpEnabled.ToString());
        }

        public bool warpRequiresOp()
        {
            string requiresOp = base.getValue("warpRequiresOp");
            if (requiresOp == null || requiresOp.Trim().Length < 0)
            {
                return true;
            }
            else
            {
                return Boolean.Parse(requiresOp);
            }
        }

        public void setRequiresOp(bool OpRequired)
        {
            base.setValue("warpRequiresOp", OpRequired.ToString());
        }
    }
}
