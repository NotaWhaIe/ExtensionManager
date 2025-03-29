using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PluginsManager
{
    public class CommandManager
    {
        public UIApplication UiApp {  get; set; }
        public List<Type> Commands  = new List<Type>();
        public List<Command> All_commands = new List<Command>();
        public Dictionary<string, List<Command>> Commands_dict = new Dictionary<string, List<Command>>();
        public string Folder_path { get; set; }
        public ExternalEvent External_event { get; set; }
        public CommandManager(UIApplication uiApp, string folder_path)
        {
            UiApp = uiApp;
            Folder_path = folder_path;
            GetExternalCommandsFromAssembly(Folder_path);
            Handler eventHandler = new Handler(this);
            ExternalEvent external_event = ExternalEvent.Create(eventHandler);
            External_event = external_event;
        }
        public void Refrash(string folder_path)
        {
            Commands.Clear();
            All_commands.Clear();
            Commands_dict.Clear();
            Folder_path = folder_path;
            GetExternalCommandsFromAssembly(Folder_path);
            Handler eventHandler = new Handler(this);
            ExternalEvent external_event = ExternalEvent.Create(eventHandler);
            External_event = external_event;
        }

        public void RunCommand(string command_name)
        {
            var commandType = Commands.FirstOrDefault(x => x.FullName == command_name);
            IExternalCommand commandInstance = (IExternalCommand)Activator.CreateInstance(commandType);
            ExternalCommandData commandData = Create(UiApp);
            string message = string.Empty;
            ElementSet elements = null;
            Result result = commandInstance.Execute(commandData, ref message, elements);
        }

        public static ExternalCommandData Create(UIApplication uiApplication)
        {
            // Находим тип ExternalCommandData
            Type externalCommandDataType = typeof(ExternalCommandData);

            // Находим внутренний конструктор
            ConstructorInfo constructor = externalCommandDataType
                .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault();

            if (constructor == null)
            {
                throw new InvalidOperationException("Не удалось найти конструктор ExternalCommandData.");
            }

            // Создаем экземпляр через рефлексию
            ExternalCommandData data = (ExternalCommandData)constructor.Invoke(null);

            // Устанавливаем свойство Application через рефлексию
            PropertyInfo applicationProperty = externalCommandDataType.GetProperty(
                "Application",
                BindingFlags.Public | BindingFlags.Instance
            );

            if (applicationProperty != null && applicationProperty.CanWrite)
            {
                applicationProperty.SetValue(data, uiApplication);
            }
            else
            {
                throw new InvalidOperationException("Не удалось установить свойство Application.");
            }
            return data;
        }

        public void GetExternalCommandsFromAssembly(string folderPath)
        {
            try
            {
                string[] dllFiles = Directory.GetFiles(folderPath, "*.dll");
                foreach (string dllFile in dllFiles)
                {
                    byte[] assemblyBytes = File.ReadAllBytes(dllFile);
                    Assembly assembly = Assembly.Load(assemblyBytes);
                    IEnumerable<Type> externalCommands = assembly.GetTypes()
                        .Where(type => typeof(IExternalCommand)
                        .IsAssignableFrom(type) && !type.IsAbstract);
                    Commands.AddRange(externalCommands);
                    foreach (Type type in externalCommands)
                    {
                        FillCommandsDictAndList(type, assembly);
                    }
                }
                Commands_dict = Commands_dict.OrderBy(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
                foreach (var key in Commands_dict.Keys.ToList())
                {
                    Commands_dict[key] = Commands_dict[key].OrderBy(val => val.Cmd_name).ToList();
                }
            }
            catch { }
        }

        public void FillCommandsDictAndList(Type type, Assembly assembly)
        {
            string commandName = type.GetProperty("IS_NAME", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)?.GetValue(null)?.ToString();
            string tabName = type.GetProperty("IS_TAB_NAME", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)?.GetValue(null)?.ToString();
            string commandDescription = type.GetProperty("IS_DESCRIPTION", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)?.GetValue(null)?.ToString();
            string commandImage = type.GetProperty("IS_IMAGE", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)?.GetValue(null)?.ToString();
            if (tabName != null)
            {
                Image img = null;
                if (commandImage != null & commandImage != "")
                {
                    using (Stream stream = assembly.GetManifestResourceStream(commandImage))
                    {
                        if (stream != null)
                        {
                            img = Image.FromStream(stream);
                        }
                    }
                }
                Command cmd = new Command(tabName, type.FullName, commandName, commandDescription, img);
                All_commands.Add(cmd);
                if (!Commands_dict.ContainsKey(tabName))
                { 
                    Commands_dict.Add(tabName, new List<Command>() { cmd });
                }
                else
                {
                    Commands_dict[tabName].Add(cmd);
                } 
            }
        }
    }
}
