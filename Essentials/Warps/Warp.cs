using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using Terraria_Server;

namespace Essentials.Warps
{
    public struct Warp
    {
        public string       Name        { get; set; }
        public Vector2      Location    { get; set; }
        public WarpType     Type        { get; set; }
        public List<String> Users       { get; set; }

        public bool IsUserAccessible(string PlayerName)
        {
            return Users.Contains(WarpManager.CleanUserName(PlayerName)) || Type == WarpType.PUBLIC;
        }

        public bool IsUserAccessible(ISender player)
        {
            return IsUserAccessible(player.Name) || player.Op;
        }

        public bool ContainsUser(string Name, out int Index)
        {
            Index = -1;
            for (int i = 0; i < Users.Count; i++)
            {
                if (WarpManager.CleanUserName(Name) == Users.ToArray()[i])
                {
                    Index = i;
                    return true;
                }
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is Warp)
            {
                Warp warp = (Warp)obj;
                return warp == this;
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return  Name.GetHashCode() ^ 
                    Location.GetHashCode() ^ 
                    Type.GetHashCode() ^
                    Users.GetHashCode();
        }

        public static bool operator ==(Warp warp1, Warp warp2)
        {
            return warp1.Name == warp2.Name &&
                warp1.Location == warp2.Location &&
                warp1.Type == warp2.Type &&
                warp1.Users == warp2.Users;
        }

        public static bool operator !=(Warp warp1, Warp warp2)
        {
            return warp1.Name != warp2.Name ||
                warp1.Location != warp2.Location ||
                warp1.Type != warp2.Type ||
                warp1.Users != warp2.Users;
        }
    }

    public enum WarpType
    {
        PRIVATE = 0,
        PUBLIC = 1
    }
}
