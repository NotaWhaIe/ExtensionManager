using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace ExtensionManager
{
    internal class App : IExternalApplication
    {
        private static AddInId addinId = new AddInId(new Guid(Const.AppProperties.Guid));

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            RibbonPanel panel = application.CreateRibbonPanel(Const.AppProperties.PanelName);
            
            var assemblyPath = Assembly.GetExecutingAssembly().Location;

            var btnDataPluginManager = new PushButtonData(Const.AppProperties.ButtonNameExtensionManager, Const.AppProperties.ButtonNameExtensionManager, assemblyPath, Const.AppProperties.AssemblyNameExtensionManager);
            var btnFamilyCatalog = panel.AddItem(btnDataPluginManager) as PushButton;
            btnFamilyCatalog.LargeImage = GetImageFromResources(Const.AppProperties.LargeImageExtensionManager);
            btnFamilyCatalog.Image = GetImageFromResources(Const.AppProperties.SmallImageExtensionManager);

            var btnDataAllCommandsInFolder = new PushButtonData(Const.AppProperties.ButtonNameCommandinFolder, Const.AppProperties.ButtonNameCommandinFolder, assemblyPath, Const.AppProperties.AssemblyNameCommandinFolder);
            var btnAllCommandsInFolder = panel.AddItem(btnDataAllCommandsInFolder) as PushButton;
            btnAllCommandsInFolder.LargeImage = GetImageFromResources(Const.AppProperties.LargeImageCommandinFolder);
            btnAllCommandsInFolder.Image = GetImageFromResources(Const.AppProperties.SmallImageCommandinFolder);

            return Result.Succeeded;
        }
        public BitmapImage GetImageFromResources(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream($"{resourceName}"))
            {
                if (stream is null)
                {
                    throw new ArgumentException($"Ресурс '{resourceName}' не найден.");
                }

                var bitmap = new BitmapImage();

                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();

                return bitmap;
            }
        }
    }
}

