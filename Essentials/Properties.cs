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
            setSpawningCancelled(isWarpEnabled());
            setTileBreakage(getTileBreakage());
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

        public void setSpawningCancelled(bool WarpEnabled)
        {
            base.setValue("warpEnabled", WarpEnabled.ToString());
        }

        public bool getTileBreakage()
        {
            string TileBreakage = base.getValue("tilebreakage");
            if (TileBreakage == null || TileBreakage.Trim().Length < 0)
            {
                return true;
            }
            else
            {
                return Boolean.Parse(TileBreakage);
            }
        }

        public void setTileBreakage(bool TileBreakage)
        {
            base.setValue("tilebreakage", TileBreakage.ToString());
        }
    }
}
