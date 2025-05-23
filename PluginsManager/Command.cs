using System.Drawing;

namespace PluginsManager
{
    public class Command
    {
        public string CmdCode { get; set; }
        public string CmdName { get; set; }
        public string CmdTab { get; set; }
        public string CmdDescription { get; set; }
        public Image CmdImage { get; set; }

        public Command(string code, string name, string description, Image img)
        {
            CmdCode = code;
            CmdName = name;
            CmdDescription = description;
            CmdImage = img;
        }
    }
}
