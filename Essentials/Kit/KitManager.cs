using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Essentials.Kit
{
    public class KitManager
    {
        public String kitFileLocation { get; set; }
        public List<Kit> KitList { get; set; }

        public KitManager(String KitFile)
        {
            kitFileLocation = KitFile;
        }

        public void LoadKits() {
            KitList = new List<Kit>();
            XmlDocument xmlReader = new XmlDocument();

            xmlReader.Load(kitFileLocation);

            foreach (XmlElement element in xmlReader.DocumentElement.ChildNodes)
            {
                Kit kit = new Kit();
                kit.ItemList = new List<int>();
                foreach (XmlNode nodeList in element.ChildNodes)
                {
                    switch (nodeList.Name.Trim().ToLower())
                    {
                        case "name":
                            {
                                kit.Name = nodeList.InnerText;
                                break;
                            }
                        case "description":
                            {
                                kit.Description = nodeList.InnerText;
                                break;
                            }
                        case "item":
                            {
                                try
                                {
                                    kit.ItemList.Add(Convert.ToInt32(nodeList.Attributes[0].Value));
                                } catch 
                                {
                                }
                                break;
                            }
                    }
                }
                if (kit.ItemList.Count > 0) //Kits need data
                {
                    KitList.Add(kit);
                }
            }
            
        }

        public void CreateTemplate()
        {
            XmlTextWriter xmlWriter = new XmlTextWriter(kitFileLocation, null);
            xmlWriter.WriteStartDocument();
            //Add a template kit
            xmlWriter.WriteStartElement("kits"); //Parent
            xmlWriter.WriteStartElement("kit"); //Actual kit data

                xmlWriter.WriteStartElement("name");
                xmlWriter.WriteString("admins");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("description");
                xmlWriter.WriteString("Kit for Admins");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("item");
                xmlWriter.WriteAttributeString("id", "122");
                xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("kit");

                xmlWriter.WriteStartElement("name");
                xmlWriter.WriteString("builder");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("description");
                xmlWriter.WriteString("Kit for Builders");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("item");
                xmlWriter.WriteAttributeString("id", "58");
                xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("kit");

                xmlWriter.WriteStartElement("name");
                xmlWriter.WriteString("mod");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("description");
                xmlWriter.WriteString("Kit for Mods");
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("item");
                xmlWriter.WriteAttributeString("id", "58");
                xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            xmlWriter.Close();
        }

        public bool ContainsKit(String KitName)
        {
            foreach (Kit kit in KitList)
            {
                if(kit.Name.Trim().ToLower().Equals(KitName.Trim().ToLower())) {
                    return true;
                }
            }
            return false;
        }

        public Kit getKit(String KitName)
        {
            foreach (Kit kit in KitList)
            {
                if (kit.Name.Trim().ToLower().Equals(KitName.Trim().ToLower()))
                {
                    return kit;
                }
            }
            return null;
        }
    }
}
