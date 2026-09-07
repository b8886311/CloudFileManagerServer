namespace CloudFileManagerServer
{
    public class CutCommand : ICommand
    {
        public string Description { get; private set; }
        public INode CutItem { get; private set; }
        private string _parent { get; set; }
        private INode _rootNode { get; set; }
        private string _nodeName { get; set; }

        public CutCommand(INode rootNode, string nodeName)
        {
            this._rootNode = rootNode;
            this._nodeName = nodeName.Trim();
            this.Description = $"剪下 {nodeName}";
        }

        public void Execute()
        {
            var item = this.FindCutItem(_nodeName);
            this.CutItem = Algorithm.GetInstance().CloneNode(item);
            (item.Parent as DirectoryNode)?.RemoveChild(item);
        }

        public void Undo()
        {
            var item = this.FindCutItem(this.CutItem.Parent.Name);
            (item as DirectoryNode).AddChild(this.CutItem);
        }

        private INode FindCutItem(string nodeName)
        {
            try
            {
                if (nodeName == "根目錄")
                    throw new InvalidOperationException("無法操作根目錄");

                if (nodeName.Contains(".txt") || nodeName.Contains(".docx") ||
                   nodeName.Contains(".png") || nodeName.Contains(".jpg"))
                {
                    return Algorithm.GetInstance().FindFileDfs(_rootNode , nodeName);
                }
                else
                {
                    return Algorithm.GetInstance().FindFolderNode(_rootNode, nodeName);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }


    public class CopyCommand : ICommand
    {
        public string Description { get; private set; }
        public INode CopyItem { get; private set; }
        private INode _rootNode { get; set; }
        private string _nodeName { get; set; }

        public CopyCommand(INode rootNode, string nodeName)
        {
            this._rootNode = rootNode;
            this._nodeName = nodeName.Trim();
            this.Description = $"複製 {nodeName}";
        }

        public void Execute()
        {
            var target  = this.FindTargetItem();
            this.CopyItem = Algorithm.GetInstance().CloneNode(this.CopyItem);
        }

        public void Undo()
        {
            var target = this.FindTargetItem();
            this.CopyItem = Algorithm.GetInstance().CloneNode(this.CopyItem);
        }

        private INode FindTargetItem()
        {
            try
            {
                if (_nodeName == "根目錄")
                    throw new InvalidOperationException("無法操作根目錄");

                if (_nodeName.Contains(".txt") || _nodeName.Contains(".docx") ||
                   _nodeName.Contains(".png") || _nodeName.Contains(".jpg"))
                {
                    return Algorithm.GetInstance().FindFileDfs(_rootNode, _nodeName);
                }
                else
                {
                    return Algorithm.GetInstance().FindFolderNode(_rootNode, _nodeName);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }


    public class PasteCommand : ICommand
    {
        public string Description { get; private set; }
        public INode _pasteItem { get; private set; }
        private INode _rootNode { get; set; }
        private string _nodeName { get; set; }

        public PasteCommand(INode rootNode, string nodeName, INode _pasteItem)
        {
            this._rootNode = rootNode;
            this._nodeName = nodeName.Trim();
            this._pasteItem = _pasteItem;
            this.Description = $"貼上 {nodeName}";
        }

        public void Execute()
        {
            INode target = this.FindTargetNode();

            if (target == null)
            {
                throw new InvalidOperationException("找不到目標位置");
            }

            if (target.Name.Equals(this._pasteItem.Parent.Name))
            {
                var directory = target as DirectoryNode;
                foreach (var child in directory.Children)
                {
                    if (child.Name.Equals(this._pasteItem.Name))
                    {
                        if (this._pasteItem is FileNode fileNode)
                        {
                            string newFileName = fileNode.File.FileName + "(2)" + fileNode.File.FileExtension;
                            this._pasteItem.UpdateName(newFileName, newFileName);
                        }
                        if (this._pasteItem is DirectoryNode directoryNode)
                        {
                            string newFileName = directoryNode.Name + "(2)";
                            string newFileEnName = directoryNode.Name_EN + "(2)";
                            this._pasteItem.UpdateName(newFileName, newFileEnName);
                        }
                    }
                }
            }

            (target as DirectoryNode).AddChild(this._pasteItem);
        }

        public void Undo()
        {
            INode target = this.FindTargetNode();
            (target as DirectoryNode).RemoveChild(this._pasteItem);
        }

        private INode FindTargetNode()
        {
            try
            {
                if (_nodeName.Contains(".txt") || _nodeName.Contains(".docx") ||
                   _nodeName.Contains(".png") || _nodeName.Contains(".jpg"))
                {
                    throw new InvalidOperationException("該路徑非資料夾無法貼上");
                }
                else
                {
                    return Algorithm.GetInstance().FindFolderNode(_rootNode, _nodeName);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
