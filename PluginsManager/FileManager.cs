using System;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PluginsManager
{
   public class FileManager
    {   
        public string FolderPath { get; set; }
        public string ExceptionTabs = "";
        public string Post = "user";
        public FileManager()
        {
            GetSettingsFromExistConfigFile();
        }
        private string GetConfigFilePath()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string pluginConfigPath = Path.Combine(appDataPath, "IS_Plugin");
            if (!Directory.Exists(pluginConfigPath))
            {
                Directory.CreateDirectory(pluginConfigPath);
            }
            return Path.Combine(pluginConfigPath, "config.xml");
        }
        private void GetSettingsFromExistConfigFile()
        {
            string configFilePath = GetConfigFilePath();
            if (!File.Exists(configFilePath))
            {
                SetPathToConfigFile();
            }
            else
            {
                XDocument xml_doc = XDocument.Load(configFilePath);
                string folderPath = xml_doc.Element("Settings")?.Element("FolderPath")?.Value ?? "";
                string exceptionTabs = xml_doc.Element("Settings")?.Element("ExceptionTabs")?.Value ?? "";
                string post = xml_doc.Element("Settings")?.Element("Post")?.Value ?? "";
                FolderPath = folderPath;
                ExceptionTabs = exceptionTabs;
                Post = post;
            }
        }
        public bool SetPathToConfigFile()
        {
            string configFilePath = GetConfigFilePath();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (File.Exists(configFilePath))
                {
                    XDocument xml_doc = XDocument.Load(configFilePath);
                    string tabConfig = xml_doc.Element("Settings")?.Element("ExceptionTabs")?.Value ?? "";
                    string post = xml_doc.Element("Settings")?.Element("Post")?.Value ?? "";
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
                    XElement settings = new XElement("Settings",
                        new XElement("FolderPath", FolderPath),
                        new XElement("ExceptionTabs", ExceptionTabs),
                        new XElement("Post", Post)
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
