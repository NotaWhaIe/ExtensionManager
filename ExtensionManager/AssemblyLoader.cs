using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace ExtensionManager
{
    public class AssemblyLoader
    {
        public string FolderDllPath { get; set; }

        /// <summary>
        /// Загружает указанную сборку (dllFile) вместе со всеми зависимостями,
        /// предполагая, что все нужные файлы уже лежат в одной папке.
        /// </summary>
        /// <param name="folderDllPath">Папка с зависимостями сборок.</param>
        /// <param name="dllFile">Полный путь к файлу .dll для загрузки.</param>
        /// <returns>Экземпляр System.Reflection.Assembly, загруженный из указанного пути.</returns>
        public Assembly LoadAssemblyWithDependencies(string folderDllPath, string dllFile)
        {
            FolderDllPath = folderDllPath;

            if (string.IsNullOrWhiteSpace(dllFile))
                throw new ArgumentException("Путь к DLL не может быть пустым.", nameof(dllFile));

            if (!File.Exists(dllFile))
                throw new FileNotFoundException($"Не найдена сборка по указанному пути: {dllFile}", dllFile);

            // Папка, где лежит исходная сборка и её зависимости
            var assemblyDirectory = Path.GetDirectoryName(dllFile)
                                   ?? throw new InvalidOperationException("Не удалось определить директорию сборки.");

            // Проверяем, не загружена ли уже такая же сборка (по имени и MVID)
            var existing = FindLoadedByMvid(dllFile);
            if (existing != null)
                return existing; // Возвращаем уже загруженную сборку

            // Подписываемся на событие загрузки зависимостей
            SubscribeToAssemblyResolve(assemblyDirectory);

            // Загружаем саму сборку
            return Assembly.LoadFile(dllFile);
        }

        /// <summary>
        /// Ищет в домене приложение сборку с тем же простым именем и MVID.
        /// Если найдена сборка с тем же именем, но другим MVID — выводит уведомление.
        /// </summary>
        private Assembly FindLoadedByMvid(string dllFile)
        {
            string simpleName = Path.GetFileNameWithoutExtension(dllFile);
            Guid newMvid = GetModuleMvid(dllFile);

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.GetName().Name.Equals(simpleName, StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(asm.Location))
                    {
                        Guid loadedMvid = GetModuleMvid(asm.Location);
                        if (loadedMvid == newMvid)
                            return asm; // Совпадение — возвращаем существующую сборку.
                                        // Новую не загружаем
                        //else
                        //{
                        //    // Выводим уведомление о различии MVID
                        //    TaskDialog.Show(
                        //        "Уведомление",
                        //        $"Обнаружена сборка с тем же именем, но другим MVID.\n" +
                        //        $"Файл: {Path.GetFileName(asm.Location)} (MVID={loadedMvid})\n" +
                        //        $"Файл: {Path.GetFileName(dllFile)} (MVID={newMvid})"
                        //    );
                        //}
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Считывает MVID модуля из PE-файла без загрузки сборки.
        /// </summary>
        private static Guid GetModuleMvid(string assemblyPath)
        {
            using var stream = File.OpenRead(assemblyPath);
            using var peReader = new PEReader(stream);
            var metadata = peReader.GetMetadataReader();
            var moduleDef = metadata.GetModuleDefinition();
            return metadata.GetGuid(moduleDef.Mvid);
        }

        /// <summary>
        /// Подписывается на AppDomain.CurrentDomain.AssemblyResolve, чтобы
        /// при запросе зависимостей искать их в указанной директории.
        /// </summary>
        private void SubscribeToAssemblyResolve(string tempDirectory)
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                var name = new AssemblyName(args.Name).Name + ".dll";

                // 1) temp
                var tempPath = Path.Combine(tempDirectory, name);
                if (File.Exists(tempPath))
                    return Assembly.LoadFile(tempPath);

                // 2) original
                var origPath = Path.Combine(FolderDllPath, name);
                if (File.Exists(origPath))
                {
                    // копируем зависимость в temp
                    var copyTo = Path.Combine(tempDirectory, name);
                    File.Copy(origPath, copyTo, overwrite: true);
                    return Assembly.LoadFile(copyTo);
                }

                // не найдено
                return null;
            };
        }
    }
}
