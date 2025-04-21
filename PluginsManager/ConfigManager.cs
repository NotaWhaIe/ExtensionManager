using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using static PluginsManager.Const;

namespace PluginsManager
{
   public class ConfigManager
    {   
        public string FolderDllPath { get; set; }
        public string ExceptionTabs = string.Empty;
        public string Post = UserConfigFile.DefaultPost;
        public Dictionary<string, Dictionary<string, string>> CommamdConfigDictionary { get; set; }
        public string TempDllFolderPath { get; set; }
        public string UserConfigPath { get; set; }
        public TempFiles TempFiles { get; set; }
        public ConfigManager(TempFiles tempFiles)
        {
            TempFiles = tempFiles;
            GetConfigsPath();
            GetUserSettings();
            GetDllSettings(TempDllFolderPath);
        }

        private void GetConfigsPath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string pluginConfigPath = Path.Combine(appDataPath, UserConfigFile.FolderName);
            if (!Directory.Exists(pluginConfigPath))
            {
                Directory.CreateDirectory(pluginConfigPath);
            }
            UserConfigPath =  Path.Combine(pluginConfigPath, UserConfigFile.Name);
            TempDllFolderPath = Path.Combine(appDataPath, Const.UserConfigFile.FolderName, "temp");
        }

        private void GetUserSettings()
        {
            if (!File.Exists(UserConfigPath))
            {
                SetPathToUserConfigFileDialog();
            }
            else
            {
                XDocument xmlDoc = XDocument.Load(UserConfigPath);
                FolderDllPath = xmlDoc.Element(Const.UserConfigFile.XmlSettings)?.Element(Const.UserConfigFile.XmlFolderPath)?.Value ?? string.Empty;
                ExceptionTabs = xmlDoc.Element(Const.UserConfigFile.XmlSettings)?.Element(Const.UserConfigFile.XmlExceptionTabs)?.Value ?? string.Empty;
                Post = xmlDoc.Element(Const.UserConfigFile.XmlSettings)?.Element(Const.UserConfigFile.XmlPost)?.Value ?? string.Empty;
            }
        }
        public bool SetPathToUserConfigFileDialog()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (File.Exists(UserConfigPath))
                {
                    XDocument xmlDoc = XDocument.Load(UserConfigPath);
                    ExceptionTabs = xmlDoc.Element(Const.UserConfigFile.XmlSettings)?.Element(Const.UserConfigFile.XmlExceptionTabs)?.Value ?? string.Empty;
                    Post = xmlDoc.Element(Const.UserConfigFile.XmlSettings)?.Element(Const.UserConfigFile.XmlPost)?.Value ?? string.Empty;
                }

                openFileDialog.Title = "Выберете папку с файлами .dll";
                openFileDialog.CheckFileExists = false; 
                openFileDialog.CheckPathExists = true;  
                openFileDialog.DereferenceLinks = true;  
                openFileDialog.FileName = "Выбор папки"; 
                openFileDialog.Filter = "Все папки|*.*"; 
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = Path.GetDirectoryName(openFileDialog.FileName);
                    
                    CreateConfigFile(folderPath);
                    TempFiles.CreateTemp(folderPath);

                    FolderDllPath = folderPath;
                    XElement settings = new XElement(Const.UserConfigFile.XmlSettings,
                        new XElement(Const.UserConfigFile.XmlFolderPath, FolderDllPath),
                        new XElement(Const.UserConfigFile.XmlExceptionTabs, ExceptionTabs),
                        new XElement(Const.UserConfigFile.XmlPost, Post)
                    );
                    settings.Save(UserConfigPath);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private static string GetConfigFilePath(string dllFolderPath)
        {
            if (!string.IsNullOrEmpty(dllFolderPath))
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

        public void GetDllSettings(string dllFolderPath)
        {
            if (CommamdConfigDictionary != null)
            {
                CommamdConfigDictionary.Clear();
            }

            Dictionary<string, Dictionary<string, string>> commandConfigDictionary = new Dictionary<string, Dictionary<string, string>>();
            var configFilePath = GetConfigFilePath(dllFolderPath);

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
            CommamdConfigDictionary = commandConfigDictionary;
        }

        public static void CreateConfigFile(string dllFolderPath)
        {
            var configFilePath = GetConfigFilePath(dllFolderPath);
            if (!File.Exists(configFilePath))
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
        }
    }
}
