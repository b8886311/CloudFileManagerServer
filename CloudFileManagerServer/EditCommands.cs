namespace CloudFileManagerServer
{
    public class CutCommand : ICommand
    {
        public string Description { get; private set; }
        public INode CutItem { get; private set; }
        private INode _rootNode { get; set; }
        private string _nodeName { get; set; }
        private DirectoryNode _originalParent { get; set; }

        public CutCommand(INode rootNode, string nodeName)
        {
            this._rootNode = rootNode;
            this._nodeName = nodeName.Trim();
            this.Description = $"剪下 {nodeName}";
        }

        public void Execute()
        {
            if (this.CutItem == null)
            {
                var item = Algorithm.FindUnknowTypeItem(_rootNode , _nodeName);
                if (item == null)
                {
                    throw new InvalidOperationException($"找不到 {_nodeName}");
                }

                this._originalParent = item.Parent as DirectoryNode;
                if (this._originalParent == null)
                {
                    throw new InvalidOperationException("無法剪下根目錄");
                }

                this.CutItem = item;
            }

            this._originalParent.RemoveChild(this.CutItem);
        }

        public void Undo()
        {
            if (this.CutItem != null && this.CutItem.Parent == null)
            {
                this._originalParent.AddChild(this.CutItem);
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
            var target = Algorithm.FindUnknowTypeItem(_rootNode, _nodeName);
            if (target == null)
            {
                throw new InvalidOperationException($"找不到 {_nodeName}");
            }

            this.CopyItem = Algorithm.CloneNode(target);
        }

        public void Undo()
        {
            // Do noting while undoing
        }
    }


    public class PasteCommand : ICommand
    {
        public string Description { get; private set; }
        public INode _pasteItem { get; private set; }
        private INode _rootNode { get; set; }
        private string _nodeName { get; set; }
        private bool _isMove { get; set; }
        private DirectoryNode _target { get; set; }
        private INode _pastedNode { get; set; }

        public PasteCommand(INode rootNode, string nodeName, INode _pasteItem, bool isMove = false)
        {
            this._rootNode = rootNode;
            this._nodeName = nodeName.Trim();
            this._pasteItem = _pasteItem;
            this._isMove = isMove;
            this.Description = $"貼上 {nodeName}";
        }

        public void Execute()
        {
            if (this._target == null)
            {
                this._target = this.FindTargetNode() as DirectoryNode;
                if (this._target == null)
                {
                    throw new InvalidOperationException("找不到目標位置");
                }
            }

            if (this._pastedNode == null)
            {
                // 剪下 => 搬移原物件（標籤、Id 都跟著走）
                // 複製 => 每次貼上都 clone 一份新的, 註冊新的記憶體位置
                this._pastedNode = this._isMove ? this._pasteItem : Algorithm.CloneNode(this._pasteItem);

                // 貼到自己或自己的子目錄底下會造成自我參照，
                // 之後任何遞迴都會 StackOverflow
                if (this.IsSelfOrDescendant(this._target, this._pastedNode))
                {
                    throw new InvalidOperationException("不能把資料夾貼到自己底下");
                }

                this.SetUniqueName(this._target, this._pastedNode);
            }

            this._target.AddChild(this._pastedNode);
        }

        public void Undo()
        {
            if (this._target != null && this._pastedNode != null)
            {
                this._target.RemoveChild(this._pastedNode);
            }
        }

        private void SetUniqueName(DirectoryNode target, INode node)
        {
            if (!target.Children.Exists(x => x.Name.Equals(node.Name, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            if (node is FileNode fileNode)
            {
                string newName = fileNode.File.FileName + "(2)" + fileNode.File.FileExtension;
                node.UpdateName(newName, newName);
            }
            else
            {
                node.UpdateName(node.Name + "(2)", node.Name_EN + "(2)");
            }
        }

        private bool IsSelfOrDescendant(INode candidate, INode ancestor)
        {
            INode current = candidate;
            while (current != null)
            {
                if (ReferenceEquals(current, ancestor))
                {
                    return true;
                }
                current = current.Parent;
            }
            return false;
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
                    return Algorithm.FindFolderNode(_rootNode, _nodeName);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
