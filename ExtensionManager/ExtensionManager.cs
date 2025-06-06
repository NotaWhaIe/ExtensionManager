using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

namespace ExtensionManager
{
    [Transaction(TransactionMode.Manual)]
    public class ExtensionManager : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            TempFiles tempFiles = new TempFiles();
            ConfigManager configManager = new ConfigManager(tempFiles);
            CommandManager commandManager = new CommandManager(uiApp, tempFiles.TempDirectory, configManager);
            WindowManager windowManager = new WindowManager(commandManager, configManager, configManager, tempFiles, uiApp);

            return Result.Succeeded;
        }
    }
}
