using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Essentials.Kits
{
    public class KitManager
    {
        public static List<Kit> KitList { get; set; }

        public static List<Kit> LoadData(string KitsLocation)
        {
            KitList = new List<Kit>();
            XmlDocument xmlReader = new XmlDocument();

            xmlReader.Load(KitsLocation);

            foreach (XmlElement element in xmlReader.DocumentElement.ChildNodes)
            {
                Kit kit = new Kit()
                {
                    ItemList = new Dictionary<Int32, Int32>()
                };

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
                                    int ID = Convert.ToInt32(nodeList.Attributes["id"].Value);
                                    int Stack = Convert.ToInt32(nodeList.Attributes["amount"].Value);
                                    kit.ItemList.Add(ID, Stack);
                                }
                                catch
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
            return KitList;
        }

        public static void CreateTemplate(string Records, string Indentifier)
        {
            XmlTextWriter xmlWriter = new XmlTextWriter(Records, null);
            xmlWriter.WriteStartDocument();
            //Add a template kit
            xmlWriter.WriteStartElement("kits");

                WriteKitElement(xmlWriter,
                    new Kit()
                    {
                        Name = "admins",
                        Description = "Kit for Admins",
                        ItemList = new Dictionary<Int32, Int32>()
                        {
                            {122, 1}
                        }
                    }
                );

                WriteKitElement(xmlWriter,
                    new Kit()
                    {
                        Name = "builder",
                        Description = "Kit for Builders",
                        ItemList = new Dictionary<Int32, Int32>()
                        {
                            {58, 1}
                        }
                    }
                );

                WriteKitElement(xmlWriter,
                    new Kit()
                    {
                        Name = "mod",
                        Description = "Kit for Mods",
                        ItemList = new Dictionary<Int32, Int32>()
                        {
                            {58, 1}
                        }
                    }
                );

            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            xmlWriter.Close();
        }

        public static void WriteKitElement(XmlWriter xmlWriter, Kit Kit) 
        {
            xmlWriter.WriteStartElement("kit");

                xmlWriter.WriteStartElement("name");
                xmlWriter.WriteString(Kit.Name);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("description");
                xmlWriter.WriteString(Kit.Description);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("item");

                foreach (KeyValuePair<Int32, Int32> Item in Kit.ItemList)
                {
                    xmlWriter.WriteAttributeString("id", Item.Key.ToString());
                    xmlWriter.WriteAttributeString("amount", Item.Value.ToString());
                }

                xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
        }

        public static bool ContainsKit(string KitName)
        {
            foreach (Kit kit in KitList)
            {
                if(kit.Name.Trim().ToLower().Equals(KitName.Trim().ToLower())) {
                    return true;
                }
            }
            return false;
        }

        public static Kit GetKit(string KitName)
        {
            foreach (Kit kit in KitList)
            {
                if (kit.Name.Trim().ToLower().Equals(KitName.Trim().ToLower()))
                {
                    return kit;
                }
            }
            return default(Kit);
        }
    }
}
