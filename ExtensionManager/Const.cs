namespace ExtensionManager
{
    public static class Const
    {
        public static class PropertyNames
        {
            public static string Application = "Application";
        }
        public static class DllFields
        {
            public static string Name = "IS_NAME";
            public static string TabName = "IS_TAB_NAME";
            public static string Description = "IS_DESCRIPTION";
            public static string Image = "IS_IMAGE";
        }
        public static class UserConfigFile
          {
            public static string FolderName = "ExtensionManager";
            public static string Name = "config.xml";
            public static string XmlSettings = "Settings";
            public static string XmlFolderPath = "FolderPath";
            public static string XmlExceptionTabs = "ExceptionTabs";
            public static string XmlPost = "Post";
            public static string DefaultPost = "user";
            public static string ManagerPost = "manager";
        }
        public static class AppProperties
        {
            public static string Guid = "7E402A4C-D611-45B3-995F-0CE2A4726A28";
            public static string PanelName = "Extension Manager";

            public static string ButtonNameExtensionManager = "Extension\nManager";
            public static string AssemblyNameExtensionManager = "ExtensionManager.ExtensionManager";
            public static string LargeImageExtensionManager = "ExtensionManager.Resources.robot32.png";
            public static string SmallImageExtensionManager = "ExtensionManager.Resources.robot16.png";

            public static string ButtonNameCommandinFolder = "Показать все\nкоманды в папке";
            public static string AssemblyNameCommandinFolder = "ExtensionManager.AllCommandsInFolder";
            public static string LargeImageCommandinFolder = "ExtensionManager.Resources.folderWheel32.png";
            public static string SmallImageCommandinFolder = "ExtensionManager.Resources.folderWheel16.png";
        }
        public static class CmdConfigFile
        {
            public static string ImageFolderName = "img";
            public static string Name = "config.xml";
            public static string XmlRoot = "Commands";
            public static string XmlCommand = "Command";
            public static string[] XmlCode = { "CmdCode",  "Имя проекта.Имя класса"};
            public static string[] XmlTab = { "CmdTab", "Название вкладки" };
            public static string[] XmlName = { "CmdName", "Имя для отображения" };
            public static string[] XmlDescription = { "CmdDescription", "Описание команды" };
            public static string[] XmlImage = { "CmdImage", "image.png" };

        }
    }
}
