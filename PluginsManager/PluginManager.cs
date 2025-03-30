using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

namespace PluginsManager
{
    [Transaction(TransactionMode.Manual)]
    public class PluginManager : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            FileManager fileManager = new FileManager();
            CommandManager commandManager = new CommandManager(uiApp, fileManager.FolderPath);
            WindowManager windowManager = new WindowManager(commandManager, fileManager, uiApp);

            return Result.Succeeded;
        }
    }
}
