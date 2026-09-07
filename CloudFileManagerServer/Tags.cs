using System.Drawing;

namespace CloudFileManagerServer
{
    public enum TagLevel
    {
        Urgent = 0,
        Work = 1,
        Personal = 2
    }
    public class Tag
    {
        public string Name { get; set; }
        public Color color { get; set; }

        public override string ToString()
        {
            return $"{Name}({color.Name})";
        }
    }
}
