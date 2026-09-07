using System.Text;
using System.Xml.Linq;

namespace CloudFileManagerServer
{
    public class TreeFormatter
    {
        public string FormatTree(INode rootNode)
        {
            string tags = rootNode.Tags.Count == 0 ? "" : "[" + string.Join(',', rootNode.Tags.Select(s => s.ToString())) + "]";
            return $"{rootNode.Name} ({rootNode.Name_EN}){tags}" + "\n" + FormatChildren(rootNode, "");
        }

        private string FormatChildren(INode node, string prefix)
        {
            if (node is not DirectoryNode)
                return "";

            var directory = node as DirectoryNode;
            
            if (directory.Children.Count == 0)
                return "";

            string result = "";
            for (int i = 0; i < directory.Children.Count; i++)
            {
                var child = directory.Children[i];
                bool isLast = i == directory.Children.Count - 1;
                string branch = isLast ? "└── " : "├── ";
                string nextPrefix = isLast ? prefix + "    " : prefix + "│   ";
                int layer = GetNodeLayer(child);
                result += prefix + branch + GetNodeInfo(child, layer) + "\n";
                result += FormatChildren(child, nextPrefix);
            }

            return result;
        }
        private int GetNodeLayer(INode node)
        {
            int layer = 0;

            while (node.Parent != null)
            {
                layer++;
                node = node.Parent;
            }
            return layer;
        }

        private string GetNodeInfo(INode node, int layer)
        {
            string tags = node.Tags.Count == 0 ? "" : "[" + string.Join(',', node.Tags.Select(s => s.ToString())) + "]";

            if (node is FileNode fileNode)
                return (node as FileNode).File.ToString() + tags;

            if (layer == 0)
                return $"{node.Name} ({node.Name_EN})" + tags;

            if (layer == 1)
                return $"{node.Name} ({node.Name_EN})[目錄]" + tags;

            else
                return $"{node.Name} ({node.Name_EN})[子目錄]" + tags;
        }
    }
}

