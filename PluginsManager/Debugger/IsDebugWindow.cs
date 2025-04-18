using System.Data;


namespace PluginsManager
{
    public static class IsDebugWindow
    {
        public static DataTable DtSheets { get; set; }
        static IsDebugWindow()
        {
            DtSheets = new DataTable();
            DtSheets.Columns.Add("данные");
        }
        public static void Show()
        {
            Debugger debugger = new Debugger();
            debugger.debugTable.DataSource = DtSheets;
            debugger.FormClosing += (s, e) => { DtSheets.Clear(); };
            debugger.ShowDialog();
        }
    }
}
