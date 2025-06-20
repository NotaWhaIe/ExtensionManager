using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using static ExtensionManager.Const;

namespace ExtensionManager
{
    public class ConfigManager
    {
        public string FolderDllPath { get; set; }
        public string ExceptionTabs = string.Empty;
        public string Post = UserConfigFile.DefaultPost;
        public Dictionary<string, Dictionary<string, string>> CommamdConfigDictionary { get; private set; }
        public string TempDllFolderPath { get; private set; }
        public string UserConfigPath { get; private set; }
        public TempFiles TempFiles { get; }

        public ConfigManager(TempFiles tempFiles)
        {
            TempFiles = tempFiles;
            GetConfigsPath();
            GetUserSettings();

            // Копируем DLL и создаём командный конфиг в temp
            try
            {
                TempFiles.CreateTemp(FolderDllPath);
                CreateConfigFile(TempDllFolderPath);
                CleanOldTempFolders();
            }
            catch (Exception ex)
            {
                // При ошибках копирования или очистки — логируем и продолжаем
                Console.WriteLine($"Warning: error during temp setup: {ex.Message}");
            }

            // Загружаем настройки команд
            GetDllSettings(TempDllFolderPath);
        }

        private void GetConfigsPath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string root = Path.Combine(appData, UserConfigFile.FolderName);
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);

            string imgFolder = Path.Combine(root, CmdConfigFile.ImageFolderName);
            if (!Directory.Exists(imgFolder))
                Directory.CreateDirectory(imgFolder);

            UserConfigPath = Path.Combine(root, UserConfigFile.Name);
            TempDllFolderPath = Path.Combine(root, "temp");
        }

        private void GetUserSettings()
        {
            if (!File.Exists(UserConfigPath))
            {
                SetPathToUserConfigFileDialog();
                return;
            }

            var xml = XDocument.Load(UserConfigPath);
            FolderDllPath = xml.Element(UserConfigFile.XmlSettings)?
                                .Element(UserConfigFile.XmlFolderPath)?.Value
                              ?? string.Empty;
            ExceptionTabs = xml.Element(UserConfigFile.XmlSettings)?
                                .Element(UserConfigFile.XmlExceptionTabs)?.Value
                              ?? string.Empty;
            Post = xml.Element(UserConfigFile.XmlSettings)?
                        .Element(UserConfigFile.XmlPost)?.Value
                   ?? string.Empty;
        }

        public bool SetPathToUserConfigFileDialog()
        {
            using var dlg = new OpenFileDialog
            {
                Title = "Выберите папку с файлами .dll",
                CheckFileExists = false,
                CheckPathExists = true,
                DereferenceLinks = true,
                FileName = "Выбор папки",
                Filter = "Все папки|*.*"
            };
            if (dlg.ShowDialog() != DialogResult.OK)
                return false;

            FolderDllPath = Path.GetDirectoryName(dlg.FileName);

            try
            {
                TempFiles.CreateTemp(FolderDllPath);
                CreateConfigFile(TempDllFolderPath);
                CleanOldTempFolders();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: error during temp setup: {ex.Message}");
            }

            var settings = new XElement(UserConfigFile.XmlSettings,
                new XElement(UserConfigFile.XmlFolderPath, FolderDllPath),
                new XElement(UserConfigFile.XmlExceptionTabs, ExceptionTabs),
                new XElement(UserConfigFile.XmlPost, Post)
            );
            new XDocument(new XDeclaration("1.0", "utf-8", "yes"), settings)
                .Save(UserConfigPath);

            GetDllSettings(TempDllFolderPath);
            return true;
        }

        public void RefreshPlugins()
        {
            try
            {
                if (Directory.Exists(TempDllFolderPath))
                    Directory.Delete(TempDllFolderPath, true);

                TempFiles.CreateTemp(FolderDllPath);
                CreateConfigFile(TempDllFolderPath);
                CleanOldTempFolders();
                GetDllSettings(TempDllFolderPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: error during refreshing plugins: {ex.Message}");
            }
        }

        public void GetDllSettings(string dllFolderPath)
        {
            CommamdConfigDictionary = new Dictionary<string, Dictionary<string, string>>();
            string configPath = Path.Combine(dllFolderPath, CmdConfigFile.Name);
            if (!File.Exists(configPath))
                return;

            try
            {
                var xdoc = XDocument.Load(configPath);
                foreach (var cmd in xdoc.Descendants(CmdConfigFile.XmlCommand))
                {
                    string code = cmd.Element(CmdConfigFile.XmlCode[0])?.Value;
                    if (string.IsNullOrEmpty(code))
                        continue;

                    CommamdConfigDictionary[code] = new Dictionary<string, string>
                    {
                        { CmdConfigFile.XmlTab[0],         cmd.Element(CmdConfigFile.XmlTab[0])?.Value },
                        { CmdConfigFile.XmlName[0],        cmd.Element(CmdConfigFile.XmlName[0])?.Value },
                        { CmdConfigFile.XmlDescription[0], cmd.Element(CmdConfigFile.XmlDescription[0])?.Value },
                        { CmdConfigFile.XmlImage[0],       cmd.Element(CmdConfigFile.XmlImage[0])?.Value }
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: error loading config XML: {ex.Message}");
            }
        }

        private static void CreateConfigFile(string dllFolderPath)
        {
            if (!Directory.Exists(dllFolderPath))
                Directory.CreateDirectory(dllFolderPath);

            string cfg = Path.Combine(dllFolderPath, CmdConfigFile.Name);
            if (File.Exists(cfg))
                return;

            var root = new XElement(CmdConfigFile.XmlRoot);
            var cmd = new XElement(CmdConfigFile.XmlCommand,
                new XElement(CmdConfigFile.XmlCode[0], CmdConfigFile.XmlCode[1]),
                new XElement(CmdConfigFile.XmlTab[0], CmdConfigFile.XmlTab[1]),
                new XElement(CmdConfigFile.XmlName[0], CmdConfigFile.XmlName[1]),
                new XElement(CmdConfigFile.XmlDescription[0], CmdConfigFile.XmlDescription[1]),
                new XElement(CmdConfigFile.XmlImage[0], CmdConfigFile.XmlImage[1])
            );
            root.Add(cmd);
            new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root).Save(cfg);
        }

        private void CleanOldTempFolders()
        {
            if (!Directory.Exists(TempDllFolderPath))
                return;

            // Удаляем старые файлы, кроме config.xml
            foreach (var file in Directory.GetFiles(TempDllFolderPath))
            {
                if (Path.GetFileName(file).Equals(CmdConfigFile.Name, StringComparison.OrdinalIgnoreCase))
                    continue;

                TryDeleteFile(file);
            }

            // Очищаем старые подпапки, оставляя только самую новую
            var subdirs = Directory.GetDirectories(TempDllFolderPath);
            var newest = subdirs.OrderByDescending(d => Directory.GetLastWriteTime(d)).FirstOrDefault();
            foreach (var dir in subdirs)
            {
                if (dir.Equals(newest, StringComparison.OrdinalIgnoreCase))
                    continue;

                TryDeleteDirectory(dir);
            }
        }

        private void TryDeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (IOException) { /* файл занят — пропускаем */ }
            catch (UnauthorizedAccessException) { /* нет прав — пропускаем */ }
        }

        private void TryDeleteDirectory(string path)
        {
            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException) { /* занято — пропускаем */ }
            catch (UnauthorizedAccessException) { /* нет прав — пропускаем */ }
        }
    }
}
