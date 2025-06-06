using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

namespace ExtensionManager
{
    [Transaction(TransactionMode.Manual)]
    public class AllCommandsInFolder : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            TempFiles tempFiles = new TempFiles();
            ConfigManager configManager = new ConfigManager(tempFiles);
            CommandManager commandManager = new CommandManager(uiApp, tempFiles.TempDirectory, configManager);
            foreach (var type in commandManager.AllTypes)
            {
                IsDebugWindow.AddRow(type.FullName);
            }
            IsDebugWindow.Show();


            return Result.Succeeded;
        }
    }
}
