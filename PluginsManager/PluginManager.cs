using Autodesk.Revit.UI;
using Autodesk.Revit.DB;


namespace PluginsManager
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class PluginManager : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            FileManager file_manager = new FileManager();
            CommandManager command_manager = new CommandManager(uiApp, file_manager.FolderPath);
            WindowManager window_obj = new WindowManager(command_manager, file_manager, uiApp);
            return Result.Succeeded;
        }
    }
}
