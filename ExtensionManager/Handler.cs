using Autodesk.Revit.UI;
using System;

namespace ExtensionManager
{
    public class Handler : IExternalEventHandler
    {
        private CommandManager _command_manager;

        public Handler(CommandManager command_manager)
        {
            _command_manager = command_manager;
        }
        public void Execute(UIApplication app)
        {
            try
            {
                _command_manager.RunCommand(GlobComandName.Name);
                
            }
            catch (Exception ex)
            {
                TaskDialog td = new TaskDialog("Ошибка");
                td.MainContent = $"{ex.Message}\n\n[Подробности]\n{ex.GetBaseException()}";
                td.Show();
            }
            finally
            {
                _command_manager.ExternalEvent?.Dispose();
                _command_manager.ExternalEvent = null;
            }
        }

        public string GetName()
        {
            return "My Dynamic External Event Handler";
        }
    }
}
