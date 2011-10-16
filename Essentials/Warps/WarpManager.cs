using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Terraria_Server.Misc;
using Terraria_Server;

namespace Essentials.Warps
{
    public class WarpManager
    {
        public static List<Warp> WarpList { get; set; }

        public static void LoadData(string WarpRecords)
        {
            WarpList = new List<Warp>();
            
            XmlDocument xmlReader = new XmlDocument();

            xmlReader.Load(WarpRecords);

            int atrIndex = 0;

            foreach (XmlElement element in xmlReader.DocumentElement.ChildNodes)
            {
                Warp warp = new Warp()
                {
                    Name = "<LOADING>",
                    Type = WarpType.PUBLIC,
                    Location = default(Vector2),
                    Users = new List<String>()
                };

                foreach (XmlNode nodeList in element.ChildNodes)
                {
                    switch (nodeList.Name.Trim().ToLower())
                    {
                        case "name":
                            {
                                warp.Name = nodeList.InnerText;
                                break;
                            }
                        case "type":
                            {
                                int Type;
                                if (Int32.TryParse(nodeList.InnerText, out Type))
                                    warp.Type = (WarpType)Type;

                                break;
                            }
                        case "location":
                            {
                                float X, Y;
                                if (float.TryParse(nodeList.Attributes[0].Value, out X) && float.TryParse(nodeList.Attributes[0].Value, out Y))
                                    warp.Location = new Vector2(X, Y);
                                else
                                    Essentials.Log("Error loading {0} Location is not a float", warp.Name);
                                break;
                            }
                        case "users":
                            {
                                warp.Users.Add(
                                    CleanUserName(
                                        nodeList.Attributes[atrIndex++].Value
                                    )
                                );
                                break;
                            }
                    }
                }

                WarpList.Add(warp);
            }

        }

        public static void Save(string WarpRecords, Action<XmlWriter> SaveMethod, string Identifier)
        {
            XmlTextWriter xmlWriter = new XmlTextWriter(WarpRecords, null);
            xmlWriter.WriteStartDocument();

            //Add a template warp
            xmlWriter.WriteStartElement(String.Format("{0}s", Identifier));

            SaveMethod.Invoke(xmlWriter);

            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            xmlWriter.Close();
        }

        public static void Save(string WarpRecords, string Indentifier)
        {
            Save(WarpRecords,
                delegate(XmlWriter xmlWriter)
                {
                    foreach (Warp warp in WarpList)
                    {
                        WriteWarpElement(xmlWriter, warp);
                    }
                }
                , Indentifier);
        }

        public static void CreateTemplate(string WarpRecords, string Indentifier)
        {
            Save(WarpRecords,
                delegate(XmlWriter xmlWriter)
                {
                    WriteWarpElement(xmlWriter, new Warp()
                    {
                        Name = "Spawn",
                        Location = new Vector2(Main.spawnTileX, Main.spawnTileY),
                        Type = WarpType.PUBLIC
                    });
                }
                , Indentifier);
        }

        public static void WriteWarpElement(XmlWriter xmlWriter, Warp Warp)
        {
            xmlWriter.WriteStartElement("Warp");

                xmlWriter.WriteStartElement("Name");
                xmlWriter.WriteString(Warp.Name);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Location");
                xmlWriter.WriteAttributeString("X", Warp.Location.X.ToString());
                xmlWriter.WriteAttributeString("Y", Warp.Location.Y.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Type");
                xmlWriter.WriteString(Warp.Type.ToString());
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("Users");
                    foreach(string usr in Warp.Users)
                        xmlWriter.WriteString(usr);

                xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }

        public static bool ContainsWarp(string WarpName)
        {
            int Index;
            return ContainsWarp(WarpName, out Index);
        }

        public static bool ContainsWarp(string WarpName, out int Index)
        {
            Index = -1;
            for (int i = 0; i < WarpList.Count - 1; i++)
            {
                Warp warp = WarpList.ToArray()[i];
                if (warp.Name.Trim().ToLower() == WarpName.Trim().ToLower())
                {
                    Index = i;
                    return true;
                }
            }
            return false;
        }

        public static bool RemoveWarp(string WarpName)
        {
            int warpIndex = -1;
            for (int i = 0; i < WarpList.Count - 1; i++)
            {
                if (WarpList.ToArray()[i].Name.Trim().ToLower() == WarpName.Trim().ToLower())
                {
                    warpIndex = i;
                    break;
                }
            }

            if (warpIndex > -1)
                WarpList.RemoveAt(warpIndex);

            return warpIndex > -1;
        }

        public static Warp GetWarp(string WarpName, out int Index)
        {
            Index = -1;
            for (int i = 0; i < WarpList.Count - 1; i++)
            {
                Warp warp = WarpList.ToArray()[i];
                if (warp.Name.Trim().ToLower() == WarpName.Trim().ToLower())
                {
                    Index = i;
                    return warp;
                }
            }
            return default(Warp);
        }

        public static Warp GetWarp(string WarpName)
        {
            int index;
            return GetWarp(WarpName, out index);
        }

        public static string CleanUserName(string user)
        {
            return user.Replace(" ", "").Trim().ToLower();
        }

        public static void UpdateWarp(Warp warp)
        {
            int Index;
            if (ContainsWarp(warp.Name, out Index))
                WarpList.ToArray()[Index] = warp;
        }
    }
}
