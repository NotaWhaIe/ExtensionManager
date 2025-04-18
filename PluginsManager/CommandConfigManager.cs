using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;


namespace PluginsManager
{
    public class CommandConfigManager
    {
        public Dictionary<string, Dictionary<string, string>> CommamdConfigDictionary { get; set; }
        public CommandConfigManager(UserConfigManager userConfigManager)
        {
            string dllFolderPath = userConfigManager.FolderPath;
            GetSettings(dllFolderPath);
        }
        private string GetConfigFilePath(string dllFolderPath)
        {
            if (dllFolderPath != string.Empty)
            {
                string pluginConfigPath = Path.Combine(dllFolderPath);
                string pluginConfigPathImg = Path.Combine(dllFolderPath, "img");
                if (!Directory.Exists(pluginConfigPath))
                {
                    Directory.CreateDirectory(pluginConfigPath);
                }
                if (!Directory.Exists(pluginConfigPathImg))
                {
                    Directory.CreateDirectory(pluginConfigPathImg);
                }
                return Path.Combine(pluginConfigPath, "config.xml");
            }
            else
            {
                return string.Empty;
            }
        }

        public void CreateConfigFile(string configFilePath)
        {
            XElement root = new XElement("Commands");
            XElement cmdCommand = new XElement("Command",
                new XElement("CmdCode", "Code"),
                new XElement("CmdTab", "MyTab"),
                new XElement("CmdName", "MyCommand"),
                new XElement("CmdDescription", "This is a sample command description."),
                new XElement("CmdImage", "image.png")
                );
            root.Add(cmdCommand);
            XDocument xmlDoc = new XDocument(new XDeclaration("1.0", "UTF-8", null), root);
            xmlDoc.Save(configFilePath);
        }
        static void AddXmlElement(XmlDocument xmlDoc, XmlElement parentElement, string elementName, string elementValue)
        {
            XmlElement element = xmlDoc.CreateElement(elementName);
            element.InnerText = elementValue;
            parentElement.AppendChild(element);
        }

        public void GetSettings(string dllFolderPath)
        {
            if (CommamdConfigDictionary != null)
            {
                CommamdConfigDictionary.Clear();
            }
            
            Dictionary<string, Dictionary<string, string>> commandConfigDictionary = new Dictionary<string, Dictionary<string, string>>();
            var configFilePath = GetConfigFilePath(dllFolderPath);
            if (!File.Exists(configFilePath))
            {
                CreateConfigFile(configFilePath);
            }
            else
            {
                try
                {
                    XDocument xDoc = XDocument.Load(configFilePath);
                    var commands = xDoc.Descendants("Command");
                    foreach (var command in commands)
                    {
                        string cmdCode = command.Element("CmdCode")?.Value;
                        string cmdTab = command.Element("CmdTab")?.Value;
                        string cmdName = command.Element("CmdName")?.Value;
                        string cmdDescription = command.Element("CmdDescription")?.Value;
                        string cmdImage = command.Element("CmdImage")?.Value;
                        if (!commandConfigDictionary.ContainsKey(cmdCode))
                        {
                            commandConfigDictionary[cmdCode] = new Dictionary<string, string>()
                        {
                            {"CmdTab", cmdTab },
                            {"CmdName", cmdName },
                            {"CmdDescription", cmdDescription },
                            {"CmdImage", cmdImage },
                        };
                        }
                    }
                }
                catch { }
                
            }
            CommamdConfigDictionary = commandConfigDictionary;
        }
    }
}
