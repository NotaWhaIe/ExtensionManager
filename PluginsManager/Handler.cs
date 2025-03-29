using Autodesk.Revit.UI;
using System;


namespace PluginsManager
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
                TaskDialog.Show("Ошибка", $"Команда завершилась с ошибкой:\n\"{ex.Message}\"");
            }
            finally
            {
                _command_manager.External_event?.Dispose();
                _command_manager.External_event = null;
            }
        }
        public string GetName()
        {
            return "My Dynamic External Event Handler";
        }
    }
}
