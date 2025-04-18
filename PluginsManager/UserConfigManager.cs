using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PluginsManager
{
   public class UserConfigManager
    {   
        public string FolderPath { get; set; }
        public string ExceptionTabs = string.Empty;
        public string Post = Const.ConfigFile.DefaultPost;

        public UserConfigManager()
        {
            GetSettingsFromExistConfigFile();
        }

        private string GetConfigFilePath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string pluginConfigPath = Path.Combine(appDataPath, Const.ConfigFile.FolderName);
            if (!Directory.Exists(pluginConfigPath))
            {
                Directory.CreateDirectory(pluginConfigPath);
            }
            return Path.Combine(pluginConfigPath, Const.ConfigFile.Name);
        }
        private void GetSettingsFromExistConfigFile()
        {
            var configFilePath = GetConfigFilePath();
            if (!File.Exists(configFilePath))
            {
                SetPathToConfigFile();
            }
            else
            {
                XDocument xmlDoc = XDocument.Load(configFilePath);
                string folderPath = xmlDoc.Element(Const.ConfigFile.XmlSettings)?.Element(Const.ConfigFile.XmlFolderPath)?.Value ?? string.Empty;
                string exceptionTabs = xmlDoc.Element(Const.ConfigFile.XmlSettings)?.Element(Const.ConfigFile.XmlExceptionTabs)?.Value ?? string.Empty;
                string post = xmlDoc.Element(Const.ConfigFile.XmlSettings)?.Element(Const.ConfigFile.XmlPost)?.Value ?? string.Empty;
                FolderPath = folderPath;
                ExceptionTabs = exceptionTabs;
                Post = post;
            }
        }
        public bool SetPathToConfigFile()
        {
            var configFilePath = GetConfigFilePath();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (File.Exists(configFilePath))
                {
                    XDocument xmlDoc = XDocument.Load(configFilePath);
                    string tabConfig = xmlDoc.Element(Const.ConfigFile.XmlSettings)?.Element(Const.ConfigFile.XmlExceptionTabs)?.Value ?? string.Empty;
                    string post = xmlDoc.Element(Const.ConfigFile.XmlSettings)?.Element(Const.ConfigFile.XmlPost)?.Value ?? string.Empty;
                    ExceptionTabs = tabConfig;
                    Post = post;
                }

                openFileDialog.Title = "Выберете папку с файлами .dll";
                openFileDialog.CheckFileExists = false; 
                openFileDialog.CheckPathExists = true;  
                openFileDialog.DereferenceLinks = true;  
                openFileDialog.FileName = "Выбор папки"; 
                openFileDialog.Filter = "Все папки|*.*"; 
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string folderPath = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
                    FolderPath = folderPath;
                    XElement settings = new XElement(Const.ConfigFile.XmlSettings,
                        new XElement(Const.ConfigFile.XmlFolderPath, FolderPath),
                        new XElement(Const.ConfigFile.XmlExceptionTabs, ExceptionTabs),
                        new XElement(Const.ConfigFile.XmlPost, Post)
                    );
                    settings.Save(configFilePath);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
