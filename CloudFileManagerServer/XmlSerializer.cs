using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace CloudFileManagerServer
{
    public class XmlSerializer
    {
        public string Serialize(INode root)
        {
            XDocument document = new XDocument(BuildElement(root));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.OmitXmlDeclaration = true;

            StringBuilder builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder, settings))
            {
                document.Save(writer);
            }
            return builder.ToString();
        }

        public void ExportToFile(INode root, string path)
        {
            File.WriteAllText(path, Serialize(root), Encoding.UTF8);
        }

        private XElement BuildElement(INode node)
        {
            XElement element = new XElement(ToElementName(node));

            if (node is DirectoryNode)
            {
                var directory = node as DirectoryNode;

                foreach (INode child in directory.Children)
                {
                    element.Add(BuildElement(child));
                }
            }
            else if (node is FileNode fileNode)
            {
                element.Value = fileNode.File.ToXmlString();
            }

            return element;
        }

        public string ToElementName(INode node)
        {
            if (node is DirectoryNode)
            {
                // 唯一例外
                if (node.Name == "2025備份")
                    return "Archive_2025";

                return $"{node.Name}_{node.Name_EN}";
            }

            //FileNode
            else
            {
                return node.Name.Replace(".", "_");
            }
        }
    }
}
