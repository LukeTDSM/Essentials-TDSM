using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Terraria_Server;

namespace Essentials
{
    public class EssentialsWarp
    {
        public bool enabled;
        //public XmlDocument warpFile;
        //private string warpProps;
        /////private XmlReader reader;
        ////private bool firstrun = false;
        //private XPathNavigator navi;
        ////private XmlReaderSettings rSettings;
        //private XmlTextWriter twriter;
        //private XmlWriter writer;
        //private XmlWriterSettings wSettings;
        private Dictionary<string, Vector2> warplist;

        public void SetupWarps()
        {
            //warpFile = new XmlDocument();
            //navi = warpFile.CreateNavigator();
            //wSettings = new XmlWriterSettings();
            //wSettings.OmitXmlDeclaration = true;
            //wSettings.Indent = true;
            //if(!(File.Exists(xmlFile)))
            //{
                //FileStream file = File.Create(xmlFile);
               // TextWriter t = new StreamWriter(xmlFile);
               // t.WriteLine("<players>\n\n</players>");
                //t.Close();
                //writer = XmlWriter.Create(xmlFile);
                //writer.WriteStartElement("players");
                //writer.WriteEndElement();
                //navi.AppendChild("players");
                //warpFile.WriteTo(writer);
                //writer.Flush();
            //}
            //warpFile.Load(xmlFile);
            //reader = XmlTextReader.Create(xmlFile);
            //warpFile.Load(reader);
            //foreach (XmlElement e in warpFile.SelectNodes("/user"))
            //{

            //}
            //reader.Close();
            //writer = XmlWriter.Create(xmlFile, wSettings);
            //warpFile.WriteTo(writer);
            //writer.Flush();          
            //Program.tConsole.WriteLine("Loading XML file.");
        }
        public EssentialsWarp(string file)
        {
            //warpProps = file;
            warplist = new Dictionary<string, Vector2>();
            //warps = new WarpProps(file);
            //SetupWarps();
        }

        public void DelWarp(Player player, string warpName)
        {
            if(warplist.ContainsKey(warpName))
            {
                warplist.Remove(warpName);
                player.sendMessage("Warp " + warpName + " removed.", 255, 255f, 255f, 255f);
            }
            else
            {
                player.sendMessage("Error: Warp " + warpName + " does not exist.", 255, 255f, 255f, 255f);
            }
        }

        public void Warp(Player player, string warpName)
        {
            if(warplist.ContainsKey(warpName))
            {
                Vector2 warpLoc = new Vector2();
                warplist.TryGetValue(warpName, out warpLoc);
                player.teleportTo(warpLoc.X, warpLoc.Y);
                player.sendMessage("Warped to " + warpName + ".", 255, 255f, 255f, 255f);
            }
            else
            {
                player.sendMessage("Error: warp " + warpName + "does not exist.", 255, 255f, 255f, 255f);
            }
        }
        
        public void WriteWarp(Player player, string warpName)
        {
           if (!warplist.ContainsKey(warpName))
            {
                warplist.Add(warpName, player.getLocation());
                player.sendMessage("Warp " + warpName + " created.", 255, 255f, 255f, 255f);
            }
            else
            {
                player.sendMessage("Error: Warp " + warpName + " already exists.", 255, 255f, 255f, 255f);
            }
        }

        private void WriteWarpXML(Player player, string warpName)
        {

        }

        public void CloseXml()
        {
            //writer.Close();
        }

    }
}
