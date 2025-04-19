using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using static PluginsManager.Const;


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
                string pluginConfigPathImg = Path.Combine(dllFolderPath, CmdConfigFile.ImageFolderName);
                if (!Directory.Exists(pluginConfigPath))
                {
                    Directory.CreateDirectory(pluginConfigPath);
                }
                if (!Directory.Exists(pluginConfigPathImg))
                {
                    Directory.CreateDirectory(pluginConfigPathImg);
                }
                return Path.Combine(pluginConfigPath, CmdConfigFile.Name);
            }
            else
            {
                return string.Empty;
            }
        }

        public void CreateConfigFile(string configFilePath)
        {
            XElement root = new XElement(CmdConfigFile.XmlRoot);
            XElement cmdCommand = new XElement(CmdConfigFile.XmlCommand,
                new XElement(CmdConfigFile.XmlCode[0], CmdConfigFile.XmlCode[1]),
                new XElement(CmdConfigFile.XmlTab[0], CmdConfigFile.XmlTab[1]),
                new XElement(CmdConfigFile.XmlName[0], CmdConfigFile.XmlName[1]),
                new XElement(CmdConfigFile.XmlDescription[0], CmdConfigFile.XmlDescription[1]),
                new XElement(CmdConfigFile.XmlImage[0], CmdConfigFile.XmlImage[1])
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
                    var commands = xDoc.Descendants(CmdConfigFile.XmlCommand);
                    foreach (var command in commands)
                    {
                        string cmdCode = command.Element(CmdConfigFile.XmlCode[0])?.Value;
                        string cmdTab = command.Element(CmdConfigFile.XmlTab[0])?.Value;
                        string cmdName = command.Element(CmdConfigFile.XmlName[0])?.Value;
                        string cmdDescription = command.Element(CmdConfigFile.XmlDescription[0])?.Value;
                        string cmdImage = command.Element(CmdConfigFile.XmlImage[0])?.Value;
                        if (!commandConfigDictionary.ContainsKey(cmdCode))
                        {
                            commandConfigDictionary[cmdCode] = new Dictionary<string, string>()
                        {
                            {CmdConfigFile.XmlTab[0], cmdTab },
                            {CmdConfigFile.XmlName[0], cmdName },
                            {CmdConfigFile.XmlDescription[0], cmdDescription },
                            {CmdConfigFile.XmlImage[0], cmdImage },
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
