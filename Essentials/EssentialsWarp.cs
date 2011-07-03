using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

using Terraria_Server;

namespace Essentials
{
    public class EssentialsWarp
    {
        public bool enabled;
        public bool requiresOp;
        public string xmlFile;
        public XmlDocument warpFile;
        //private string warpProps;
        private XmlReader reader;
        ////private bool firstrun = false;
        private XPathNavigator navi;
        //private XmlReaderSettings rSettings;
        //private XmlTextWriter twriter;
        private XmlWriter writer;
        private XmlWriterSettings wSettings;
        private Dictionary<string, Vector2> warplist;

        public void SetupWarps()
        {
            warpFile = new XmlDocument();
            wSettings = new XmlWriterSettings();
            wSettings.OmitXmlDeclaration = true;
            wSettings.Indent = true;
            if (!(File.Exists(xmlFile)))
            {
                FileStream file = File.Create(xmlFile);
                file.Close();
                warpFile.LoadXml("<warps></warps>");
                navi = warpFile.CreateNavigator();
                WriteXML();
            }
            else
            {
                reader = XmlTextReader.Create(xmlFile);
                warpFile.Load(reader);
                navi = warpFile.CreateNavigator();
                Vector2 loc = new Vector2();
                string name;
                foreach (XmlElement e in warpFile.SelectNodes("/warps/warp"))
                {
                    name = e.ChildNodes[0].InnerText;
                    loc.X = float.Parse(e.ChildNodes[1].InnerText);
                    loc.Y = float.Parse(e.ChildNodes[2].InnerText);
                    warplist.Add(name, loc);
                }
                reader.Close();
            }
        }

        public EssentialsWarp(string file)
        {
            xmlFile = file;
            warplist = new Dictionary<string, Vector2>();
            SetupWarps();
        }

        public void DelWarp(Player player, string warpName)
        {
            if(warplist.ContainsKey(warpName))
            {
                XmlNodeList node = warpFile.SelectNodes("/warps");
                for(int i = 0; i < node.Count; i++)
                {
                    for (int j = 0; j < node[i].ChildNodes.Count; j++)
                    {
                        if (node[i].ChildNodes[j].FirstChild.InnerText == warpName)
                        {
                            node[i].RemoveChild(node[i].ChildNodes[j]);
                            break;
                        }
                    }
                }
                WriteXML();
                warplist.Remove(warpName);
                player.sendMessage("Warp " + warpName + " removed.", 255, 0f, 255f, 255f);
                Program.tConsole.WriteLine(player.getName() + " removed warp " + warpName);
            }
            else
            {
                player.sendMessage("Error: Warp " + warpName + " does not exist.");
            }
        }

        public void List(Player player, int page = 1)
        {
            string[] list = new string[warplist.Keys.Count];
            int startingIndex = (page - 1) * 5;
            warplist.Keys.CopyTo(list, 0);
            int maxIndex = list.Length;
            //if((page * 5) > list.Length)
            //{
            //    if(list.Length > (page - 1) * 5)
            //    {
            //        startingIndex = (page * 5);
            //    }
            //    else
            //    {
            //        startingIndex = 0;
            //    }
            //}
            player.sendMessage("Warp List page " + page, 255, 200f, 255f, 255f);
            page--;
            for (int i = 0 + (page * 5); i < ((page + 1) * 5 > list.Length ? list.Length : (page * 5) + 5); i++)
            {
                player.sendMessage(list[i], 255, 0f, 255f, 255f);
            }
        }

        public void Warp(Player player, string warpName)
        {
            if(warplist.ContainsKey(warpName))
            {
                Vector2 warpLoc = new Vector2();
                warplist.TryGetValue(warpName, out warpLoc);
                player.teleportTo(warpLoc.X, warpLoc.Y);
                player.sendMessage("Warped to " + warpName + ".", 255, 0f, 255f, 255f);
                Program.tConsole.WriteLine(player.getName() + " used /warp " + warpName);
            }
            else
            {
                player.sendMessage("Error: warp " + warpName + " does not exist.");
            }
        }
        
        public void WriteWarp(Player player, string warpName)
        {
            if (!warplist.ContainsKey(warpName))
            {
                navi.MoveToRoot();
                navi.MoveToFirstChild();
                navi.AppendChild("<warp><name>" + warpName + "</name><x>" + player.getLocation().X + "</x><y>" + player.getLocation().Y + "</y></warp>");
                WriteXML();
                warplist.Add(warpName, player.getLocation());
                player.sendMessage("Warp " + warpName + " created.", 255, 0f, 255f, 255f);
                Program.tConsole.WriteLine(player.getName() + " created warp " + warpName + " at " + player.getLocation().X + "," + player.getLocation().Y);
            }
            else
            {
                player.sendMessage("Error: Warp " + warpName + " already exists.");
            }
        }

        private void WriteXML()
        {
            writer = XmlWriter.Create(xmlFile, wSettings);
            warpFile.WriteTo(writer);
            writer.Flush();
            writer.Close();
        }

    }
}
