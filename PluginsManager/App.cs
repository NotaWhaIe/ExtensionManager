using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Reflection;
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
            PushButtonData buttonDataFamilyCatalog = new PushButtonData("Plugins\nmanager", "Plugins\nmanager", assemblyPath, "IS_PluginsManager.PluginManager");
            var buttonFamilyCatalog = generalRibbonPanel.AddItem(buttonDataFamilyCatalog) as PushButton;
            buttonFamilyCatalog.LargeImage = new BitmapImage(new Uri(@"/IS_PluginsManager;component/Resources/robot32.png", UriKind.RelativeOrAbsolute));
            buttonFamilyCatalog.Image = new BitmapImage(new Uri(@"/IS_PluginsManager;component/Resources/robot16.png", UriKind.RelativeOrAbsolute));
            return Result.Succeeded;
        }
    }
}

