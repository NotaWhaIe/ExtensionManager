using System;
using System.IO;
using System.Windows.Forms;


namespace ExtensionManager
{
    public class TempFiles
    {
        public string TempDirectory;
        public TempFiles()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); 
            TempDirectory = Path.Combine(appDataPath, Const.UserConfigFile.FolderName, "temp");
        }
        public void CreateTemp(string sourceDirectory)
        {
            try
            {
                double totalSize = GetDirectorySize(sourceDirectory);
                double totalSizeMb = Math.Round(totalSize / 1024.0 / 1024.0, 2);
                if (totalSizeMb > 50)
                {
                    DialogResult result = MessageBox.Show(
                        $"Размер папки {totalSizeMb}, хотите ее загрузить?",
                        "Подтверждение",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        if (Directory.Exists(TempDirectory))
                        {
                            Directory.Delete(TempDirectory, recursive: true);
                        }
                        CopyDirectory(sourceDirectory, TempDirectory);
                    }
                }
                else
                {
                    if (Directory.Exists(TempDirectory))
                    {
                        Directory.Delete(TempDirectory, recursive: true);
                    }
                    CopyDirectory(sourceDirectory, TempDirectory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));

                File.Copy(file, destFile, overwrite: true);
            }

            foreach (string dir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destinationDir, Path.GetFileName(dir));

                CopyDirectory(dir, destSubDir);
            }
        }

        private static long GetDirectorySize(string path)
        {
            long size = 0;

            foreach (string file in Directory.GetFiles(path))
            {
                size += new FileInfo(file).Length;
            }

            foreach (string dir in Directory.GetDirectories(path))
            {
                size += GetDirectorySize(dir);
            }

            return size;
        }
    }
}
