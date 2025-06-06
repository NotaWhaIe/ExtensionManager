using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionManager
{
    public static class AssemblyLoader
    {
        // Имя папки кеша формируется динамически: <ИмяВашейСборки> + "Temp"
        private static readonly string CacheFolderName = Assembly.GetExecutingAssembly().GetName().Name + "Temp";
        private static readonly string RootTempDirectory = Path.Combine(Path.GetTempPath(), CacheFolderName);

        /// <summary>
        /// Загружает указанную сборку (dllFile) вместе со всеми зависимостями.
        /// При загрузке создаётся уникальная временная папка внутри %TEMP%\<CacheFolderName>\,
        /// куда копируются сама DLL и её зависимости.
        /// </summary>
        /// <param name="dllFile">Полный путь к файлу .dll, который необходимо загрузить.</param>
        /// <returns>Инстанс System.Reflection.Assembly, загруженный из временного пути.</returns>
        public static Assembly LoadAssemblyWithDependencies(string dllFile)
        {
            if (string.IsNullOrWhiteSpace(dllFile))
                throw new ArgumentException("Путь к DLL не может быть пустым.", nameof(dllFile));

            if (!File.Exists(dllFile))
                throw new FileNotFoundException($"Не найдена сборка по указанному пути: {dllFile}", dllFile);

            // 1. Создаём уникальную временную директорию для этой конкретной загрузки
            var tempDirectory = CreateUniqueTempDirectory();

            // 2. Копируем основную сборку в tempDirectory
            var tempAssemblyPath = CopyAssemblyToTemp(dllFile, tempDirectory);

            // 3. Подписываемся на событие AssemblyResolve, чтобы ловить запросы зависимостей
            SubscribeToAssemblyResolve(dllFile, tempDirectory);

            // 4. Загружаем саму сборку из временной папки и возвращаем
            return Assembly.LoadFile(tempAssemblyPath);
        }

        /// <summary>
        /// Создаёт новую уникальную папку внутри RootTempDirectory и возвращает её путь.
        /// </summary>
        private static string CreateUniqueTempDirectory()
        {
            // Убедимся, что корневая папка существует
            if (!Directory.Exists(RootTempDirectory))
            {
                Directory.CreateDirectory(RootTempDirectory);
            }

            // Сгенерируем подпапку по GUID и создадим её
            var tempDir = Path.Combine(RootTempDirectory, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            return tempDir;
        }

        /// <summary>
        /// Копирует файл сборки из sourcePath в targetDirectory и возвращает путь к скопированной DLL.
        /// </summary>
        private static string CopyAssemblyToTemp(string sourcePath, string targetDirectory)
        {
            var fileName = Path.GetFileName(sourcePath);
            var targetPath = Path.Combine(targetDirectory, fileName);
            File.Copy(sourcePath, targetPath, overwrite: true);
            return targetPath;
        }

        /// <summary>
        /// Подписывается на AppDomain.CurrentDomain.AssemblyResolve, чтобы в момент запроса зависимостей:
        /// 1) Сначала искать их в tempDirectory.
        /// 2) Если там нет — копировать из оригинальной папки (где лежит dllFile) и загружать.
        /// </summary>
        private static void SubscribeToAssemblyResolve(string originalPath, string tempDirectory)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                // Имя библиотеки без версии, культуры и т.п. + ".dll"
                var assemblyName = new AssemblyName(args.Name).Name + ".dll";

                // 1) Смотрим в tempDirectory
                var tempCandidate = Path.Combine(tempDirectory, assemblyName);
                if (File.Exists(tempCandidate))
                {
                    return Assembly.LoadFile(tempCandidate);
                }

                // 2) Если нет — пробуем найти рядом с оригинальным файлом
                var originalDir = Path.GetDirectoryName(originalPath);
                var originalCandidate = Path.Combine(originalDir, assemblyName);
                if (File.Exists(originalCandidate))
                {
                    // Копируем зависимость в tempDirectory и загружаем
                    var copiedPath = Path.Combine(tempDirectory, assemblyName);
                    File.Copy(originalCandidate, copiedPath, overwrite: true);
                    return Assembly.LoadFile(copiedPath);
                }

                // Если зависимость нигде не найдена — возвращаем null, и загрузка упадёт исключением
                return null;
            };
        }
    }
}
