using System.Drawing;


namespace PluginsManager
{
    public class Command
    {
        public string Cmd_code {  get; set; }
        public string Cmd_name { get; set; }
        public string Cmd_tab { get; set; }
        public string  Cmd_description { get; set; }
        public Image Cmd_Image { get; set; }

        public Command(string tab, string code, string name, string description, Image img) 
        {
            Cmd_tab = tab;
            Cmd_code = code;
            Cmd_name = name;
            Cmd_description = description;
            Cmd_Image = img;
        }
    }
}
