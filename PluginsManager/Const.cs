namespace PluginsManager
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
        public static class ConfigFile
        {
            public static string FolderName = "PluginsManager";
            public static string Name = "config.xml";
            public static string XmlSettings = "Settings";
            public static string XmlFolderPath = "FolderPath";
            public static string XmlExceptionTabs = "ExceptionTabs";
            public static string XmlPost = "Post";
            public static string DefaultPost = "user";
        }
        public static class AppProperties
        {
            public static string Guid = "4AB79F62-F346-4AC4-8D98-A1345DA39693";
            public static string PanelName = "Plugins Manager";

            public static string ButtonNamePluginsManager = "Plugins\nManager";
            public static string AssemblyNamePluginsManager = "PluginsManager.PluginManager";
            public static string LargeImagePluginsManager = "PluginsManager.Resources.robot32.png";
            public static string SmallImagePluginsManager = "PluginsManager.Resources.robot16.png";

            public static string ButtonNameCommandinFolder = "Показать все\nкоманды в папке";
            public static string AssemblyNameCommandinFolder = "PluginsManager.AllCommandsInFolder";
            public static string LargeImageCommandinFolder = "PluginsManager.Resources.folderWheel32.png";
            public static string SmallImageCommandinFolder = "PluginsManager.Resources.folderWheel16.png";

        }
    }
}
