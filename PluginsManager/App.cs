using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace PluginsManager
{
    internal class App : IExternalApplication
    {
        static AddInId addinId = new AddInId(new Guid("4AB79F62-F346-4AC4-8D98-A1345DA39693"));
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string tabName = "IS";
            application.CreateRibbonTab(tabName);
            Autodesk.Revit.UI.RibbonPanel generalRibbonPanel = application.CreateRibbonPanel(tabName, "Plugins manager");
            PushButtonData buttonDataFamilyCatalog = new PushButtonData("Plugins\nmanager", "Plugins\nmanager", assemblyPath, "PluginsManager.PluginManager");
            var buttonFamilyCatalog = generalRibbonPanel.AddItem(buttonDataFamilyCatalog) as PushButton;
            //buttonFamilyCatalog.LargeImage = new BitmapImage(new Uri(@"/PluginsManager;component/Resources/robot32.png", UriKind.RelativeOrAbsolute));
            //buttonFamilyCatalog.Image = new BitmapImage(new Uri(@"/PluginsManager;component/Resources/robot16.png", UriKind.RelativeOrAbsolute));//

            string imageName32 = "PluginsManager.Resources.robot32.png";
            string imageName16 = "PluginsManager.Resources.robot16.png";
            buttonFamilyCatalog.LargeImage = GetImageFromResources(imageName32);
            buttonFamilyCatalog.Image = GetImageFromResources(imageName16);

            //Assembly assembly = Assembly.GetExecutingAssembly();
            //string[] resourceNames = assembly.GetManifestResourceNames();

            //foreach (string name in resourceNames)
            //{
            //    Console.WriteLine(name); // Проверьте, есть ли ваш ресурс в этом списке
            //    TaskDialog.Show("a", name);
            //}
            return Result.Succeeded;
        }
        public BitmapImage GetImageFromResources(string resourceName)
        {
            // Загружаем текущую сборку
            Assembly assembly = Assembly.GetExecutingAssembly();
            

            // Получаем поток ресурса
            using (Stream stream = assembly.GetManifestResourceStream($"{resourceName}"))
            {
                if (stream == null)
                {
                    throw new ArgumentException($"Ресурс '{resourceName}' не найден.");
                }

                // Создаем BitmapImage из потока
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // Обязательно для освобождения потока
                bitmap.EndInit();
                bitmap.Freeze(); // Делает объект потокобезопасным
                return bitmap;
            }
        }
    }
}

