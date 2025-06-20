using System;
using System.IO;
using System.Windows.Forms;


//namespace ExtensionManager
//{
//    public class TempFiles
//    {
//        public string TempDirectory;
//        public TempFiles()
//        {
//            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); 
//            TempDirectory = Path.Combine(appDataPath, Const.UserConfigFile.FolderName, "temp");
//        }
//        public void CreateTemp(string sourceDirectory)
//        {
//            try
//            {
//                double totalSize = GetDirectorySize(sourceDirectory);
//                double totalSizeMb = Math.Round(totalSize / 1024.0 / 1024.0, 2);
//                if (totalSizeMb > 50)
//                {
//                    DialogResult result = MessageBox.Show(
//                        $"Размер папки {totalSizeMb}, хотите ее загрузить?",
//                        "Подтверждение",
//                        MessageBoxButtons.YesNo,
//                        MessageBoxIcon.Question);

//                    if (result == DialogResult.Yes)
//                    {
//                        if (Directory.Exists(TempDirectory))
//                        {
//                            Directory.Delete(TempDirectory, recursive: true);
//                        }
//                        CopyDirectory(sourceDirectory, TempDirectory);
//                    }
//                }
//                else
//                {
//                    if (Directory.Exists(TempDirectory))
//                    {
//                        Directory.Delete(TempDirectory, recursive: true);
//                    }
//                    CopyDirectory(sourceDirectory, TempDirectory);
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Произошла ошибка: {ex.Message}");
//            }
//        }

//        private static void CopyDirectory(string sourceDir, string destinationDir)
//        {
//            Directory.CreateDirectory(destinationDir);

//            foreach (string file in Directory.GetFiles(sourceDir))
//            {
//                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));

//                File.Copy(file, destFile, overwrite: true);
//            }

//            foreach (string dir in Directory.GetDirectories(sourceDir))
//            {
//                string destSubDir = Path.Combine(destinationDir, Path.GetFileName(dir));

//                CopyDirectory(dir, destSubDir);
//            }
//        }

//        private static long GetDirectorySize(string path)
//        {
//            long size = 0;

//            foreach (string file in Directory.GetFiles(path))
//            {
//                size += new FileInfo(file).Length;
//            }

//            foreach (string dir in Directory.GetDirectories(path))
//            {
//                size += GetDirectorySize(dir);
//            }

//            return size;
//        }
//    }
//}


namespace ExtensionManager
{
    public class TempFiles
    {
        /// <summary>
        /// Корневая папка для всех «временных» копий плагинов:
        /// %AppData%\ExtensionManager\temp
        /// </summary>
        public string RootTempDirectory { get; }

        /// <summary>
        /// Фактическая папка с конкретным плагином, 
        /// например %AppData%\ExtensionManager\temp\Test2022_a25bab5f-ace7-45be-bca4-36a7490a5497
        /// </summary>
        public string TempDirectory { get; private set; }


        public TempFiles()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string basePath = Path.Combine(appDataPath, Const.UserConfigFile.FolderName);
            RootTempDirectory = Path.Combine(basePath, "temp");

            if (!Directory.Exists(RootTempDirectory))
                Directory.CreateDirectory(RootTempDirectory);
        }

        public void CreateTemp(string sourceDirectory)
        {
            try
            {
                // Считаем размер исходной папки
                double totalBytes = GetDirectorySize(sourceDirectory);

                double totalMb = Math.Round(totalBytes / 1024.0 / 1024.0, 2);
                bool doCopy = true;

                if (totalMb > 50)
                {
                    var result = MessageBox.Show(
                        $"Размер папки {totalMb} MB. Загружать?",
                        "Подтверждение",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );
                    doCopy = result == DialogResult.Yes;
                }

                if (!doCopy)
                    return;

                // Генерируем новую подпапку с GUID
                string plugName = new DirectoryInfo(sourceDirectory).Name;
                string newFolder = $"{plugName}_{Guid.NewGuid()}";
                string targetDir = Path.Combine(RootTempDirectory, newFolder);

                if (Directory.Exists(targetDir))
                    Directory.Delete(targetDir, recursive: true);

                CopyDirectory(sourceDirectory, targetDir);

                TempDirectory = targetDir;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка создания временной папки: {ex.Message}");
            }
        }

        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, destFile, overwrite: true);
            }
            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                string destSub = Path.Combine(destinationDir, Path.GetFileName(dir));
                CopyDirectory(dir, destSub);
            }
        }

        private static long GetDirectorySize(string path)
        {
            long size = 0;
            foreach (var file in Directory.GetFiles(path))
                size += new FileInfo(file).Length;
            foreach (var dir in Directory.GetDirectories(path))
                size += GetDirectorySize(dir);
            return size;
        }
    }
}
