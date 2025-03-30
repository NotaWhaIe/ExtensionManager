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
        public List<Type> AllTypes = new List<Type>();
        public List<Command> AllCommands = new List<Command>();
        public Dictionary<string, List<Command>> CommandsDictionary = new Dictionary<string, List<Command>>();
        public string FolderPath { get; set; }
        public ExternalEvent ExternalEvent { get; set; }

        public CommandManager(UIApplication uiApp, string folderPath)
        {
            UiApp = uiApp;
            FolderPath = folderPath;
            GetExternalCommandsFromAssembly(FolderPath);
            Handler eventHandler = new Handler(this);
            ExternalEvent externalEvent = ExternalEvent.Create(eventHandler);
            ExternalEvent = externalEvent;
        }

        public void Refresh(string folderPath)
        {
            AllTypes.Clear();
            AllCommands.Clear();
            CommandsDictionary.Clear();
            FolderPath = folderPath;
            GetExternalCommandsFromAssembly(FolderPath);
            Handler eventHandler = new Handler(this);
            ExternalEvent externalEvent = ExternalEvent.Create(eventHandler);
            ExternalEvent = externalEvent;
        }

        public void RunCommand(string commandName)
        {
            var commandType = AllTypes.FirstOrDefault(x => x.FullName == commandName);
            IExternalCommand commandInstance = (IExternalCommand)Activator.CreateInstance(commandType);
            ExternalCommandData commandData = Create(UiApp);
            string message = string.Empty;
            ElementSet elements = null;
            Result result = commandInstance.Execute(commandData, ref message, elements);
        }

        public ExternalCommandData Create(UIApplication uiApplication)
        {
            // Находим тип ExternalCommandData
            Type externalCommandDataType = typeof(ExternalCommandData);

            // Находим внутренний конструктор
            ConstructorInfo constructor = externalCommandDataType
                .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault();

            if (constructor is null)
            {
                throw new InvalidOperationException("Не удалось найти конструктор ExternalCommandData.");
            }

            // Создаем экземпляр через рефлексию
            ExternalCommandData data = (ExternalCommandData)constructor.Invoke(null);

            // Устанавливаем свойство Application через рефлексию
            PropertyInfo applicationProperty = externalCommandDataType.GetProperty(
                Constants.PropertyNames.Application,
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

        private void GetExternalCommandsFromAssembly(string folderPath)
        {
            try
            {
                var dllFiles = Directory.GetFiles(folderPath, "*.dll");
                foreach (var dllFile in dllFiles)
                {
                    var assemblyBytes = File.ReadAllBytes(dllFile);
                    var assembly = Assembly.Load(assemblyBytes);

                    IEnumerable<Type> externalCommands = assembly.GetTypes()
                        .Where(type => typeof(IExternalCommand).IsAssignableFrom(type) && !type.IsAbstract);

                    AllTypes.AddRange(externalCommands);

                    foreach (var type in externalCommands)
                    {
                        FillCommandsDictionaryAndList(type, assembly);
                    }
                }

                SortCommandsDictionary();
            }
            catch(Exception)
            {
                throw;
            }
        }

        private void FillCommandsDictionaryAndList(Type type, Assembly assembly)
        {
            // TODO: move literals to a separate static class
            string commandName = type.GetProperty("IS_NAME", BindingFlags.Public | BindingFlags.Static)
                ?.GetValue(null)
                ?.ToString();
            string tabName = type.GetProperty("IS_TAB_NAME", BindingFlags.Public | BindingFlags.Static)
                ?.GetValue(null)
                ?.ToString();
            string commandDescription = type.GetProperty("IS_DESCRIPTION", BindingFlags.Public | BindingFlags.Static)
                ?.GetValue(null)
                ?.ToString();
            string commandImage = type.GetProperty("IS_IMAGE", BindingFlags.Public | BindingFlags.Static)
                ?.GetValue(null)
                ?.ToString();
            if (tabName != null)
            {
                Image image = null;
                if (!string.IsNullOrEmpty(commandImage))
                {
                    using (Stream stream = assembly.GetManifestResourceStream(commandImage))
                    {
                        if (stream != null)
                        {
                            image = Image.FromStream(stream);
                        }
                    }
                }

                var command = new Command(tabName, type.FullName, commandName, commandDescription, image);
                AllCommands.Add(command);
                if (!CommandsDictionary.ContainsKey(tabName))
                { 
                    CommandsDictionary.Add(tabName, new List<Command> { command });
                }
                else
                {
                    CommandsDictionary[tabName].Add(command);
                } 
            }
        }

        private void SortCommandsDictionary()
        {
            CommandsDictionary = CommandsDictionary
                    .OrderBy(pair => pair.Key)
                    .ToDictionary(pair => pair.Key, pair => pair.Value);

            foreach (var key in CommandsDictionary.Keys.ToList())
            {
                CommandsDictionary[key] = CommandsDictionary[key].OrderBy(val => val.CmdName).ToList();
            }
        }
    }
}
