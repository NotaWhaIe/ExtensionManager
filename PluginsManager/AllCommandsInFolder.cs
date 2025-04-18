using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;

namespace PluginsManager
{
    [Transaction(TransactionMode.Manual)]
    public class AllCommandsInFolder : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;

            UserConfigManager fileManager = new UserConfigManager();
            CommandConfigManager commandConfigManager = new CommandConfigManager(fileManager);
            CommandManager commandManager = new CommandManager(uiApp, fileManager.FolderPath, commandConfigManager);
            foreach (var type in commandManager.AllTypes)
            {
                IsDebugWindow.DtSheets.Rows.Add(type.FullName);
            }
            IsDebugWindow.Show();


            return Result.Succeeded;
        }
    }
}
