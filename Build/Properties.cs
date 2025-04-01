using System;

namespace PluginsManager
{
    public static class Properties
    {
        public static string Version = "1.0.0";
        public static string AddinPath = @"..\PluginsManager\PluginsManager.addin";
        public static string DllFolder = @"..\PluginsManager\bin\Build\";
        public static string SubfolderName = "PluginsManager";
        public static string ProjectName = "Plugins Manager";
        public static string InstallDir24 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Autodesk\Revit\Addins\2024";
        public static string InstallDir23 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Autodesk\Revit\Addins\2023";
        public static string InstallDir22 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Autodesk\Revit\Addins\2022";
        public static string InstallDir21 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Autodesk\Revit\Addins\2021";
        public static string Guid = "8BD7D784-31D5-4536-9FE2-416A2DC958B8";
        public static string OutputDir = "..\\";
    }
}
