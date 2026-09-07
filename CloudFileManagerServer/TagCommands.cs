namespace CloudFileManagerServer
{
    public class TagCommand : ICommand
    {
        private string _nodeName { get; set; }
        private string _tagLevel { get; set; }
        private INode _rootNode { get; set; }
        public string Description { get; private set; }

        public TagCommand(INode rootNode, string nodeName, string tagLevel)
        {
            this._nodeName = nodeName.Trim();
            this._tagLevel = tagLevel;
            this._rootNode = rootNode;
            this.Description = $"節點 {nodeName} 添加/刪除 {tagLevel} 標籤";
        }

        public void Execute()
        {
            AppendTag();
        }

        public void Undo()
        {
            AppendTag();
        }

        private void AppendTag()
        {
            try
            {
                if (!Enum.TryParse(_tagLevel, true, out TagLevel level))
                {
                    throw new ArgumentException($"Invalid tag level: {_tagLevel}");
                }

                if (_nodeName.Contains(".txt") || _nodeName.Contains(".docx") ||
                    _nodeName.Contains(".png") || _nodeName.Contains(".jpg"))
                {

                    var node = Algorithm.GetInstance().FindFileDfs(_rootNode, _nodeName);
                    if (node != null)
                    {
                        node.AppendTag(level);
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid node name: {_nodeName}. It is a file, not a folder.");
                    }
                }
                else
                {
                    var node = Algorithm.GetInstance().FindFolderNode(_rootNode, _nodeName);
                    if (node != null)
                    {
                        node.AppendTag(level);
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid node name: {_nodeName}. It is not found.");
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
