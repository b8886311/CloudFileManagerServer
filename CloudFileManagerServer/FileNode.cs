using System.Drawing;
using System.Linq.Expressions;

namespace CloudFileManagerServer
{
    public interface INode
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Name_EN { get; }
        public string GetFullPath();
        public INode? Parent { get; }
        public List<Tag> Tags { get; }
        public void AppendTag(TagLevel level);
        public void UpdateName(string newName, string newName_EN);
    }

    public class BaseNode : INode
    {
        public Guid Id { get; private set; }

        public INode? Parent { get; private set; }

        public string Name { get; private set; }
        public string Name_EN { get; private set; }

        public List<Tag> Tags { get; private set; }

        public string GetFullPath()
        {
            if (Parent == null)
                return Name;
            return Path.Combine(Parent.GetFullPath(), Name);
        }
        public void AppendTag(TagLevel level)
        {
            Tag tag = new Tag();

            switch (level)
            {
                case TagLevel.Urgent:
                    tag = new Tag { Name = "Urgent", color = Color.Red };
                    break;
                case TagLevel.Work:
                    tag = new Tag { Name = "Work", color = Color.Blue };
                    break;
                case TagLevel.Personal:
                    tag = new Tag { Name = "Personal", color = Color.Green };
                    break;
            }

            var item = this.Tags.Where(t => t.Name == tag.Name && t.color == tag.color).FirstOrDefault();
            if (item == null)
            {
                this.Tags.Add(tag);
            }
            else
            {
                this.Tags.Remove(item);
            }
        }

        public virtual void UpdateName(string newName, string newName_EN)
        {
            this.Name = newName;
            this.Name_EN = newName_EN;
        }

        public BaseNode(string name, string name_en)
        {
            this.Tags = new List<Tag>();
            this.Id = Guid.NewGuid();
            this.Name = name;
            this.Name_EN = name_en;
        }

        internal void SetParent(INode? parent)
        {
            this.Parent = parent;
        }
    }

    public class DirectoryNode : BaseNode
    {
        public List<INode> Children { get; private set; }
        public DirectoryNode(string name, string name_en) : base(name, name_en)
        {
            Children = new List<INode>();
        }

        public void AddChild(INode child)
        {
            if (Children.Exists(x => x.Name.Equals(child.Name)))
            {
                throw new Exception($"Child with ch name {child.Name} already exists in directory {this.Name}");
            }
            if (child.Name_EN != "")
            {
                if (Children.Exists(x => x.Name_EN.Equals(child.Name_EN, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new Exception($"Child with EN name {child.Name_EN} already exists in directory {this.Name}");
                }
            }

            if (child is not BaseNode childNode)
            {
                throw new ArgumentException("Child 必須是 BaseNode。", nameof(child));
            }

            childNode.SetParent(this);
            Children.Add(child);
        }
        public void RemoveChild(INode child)
        {
            if (!Children.Exists(x => x.Name.Equals(child.Name)))
            {
                throw new Exception($"Child with CH name {child.Name} does not exist in directory {this.Name}");
            }
            if (child.Name_EN != "")
            {
                if (!Children.Exists(x => x.Name_EN.Equals(child.Name_EN, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new Exception($"Child with EN name {child.Name_EN} does not exist in directory {this.Name}");
                }
            }

            if (child is BaseNode childNode)
            {
                childNode.SetParent(null);
            }

            Children.Remove(child);
        }
    }

    public class FileNode : BaseNode
    {
        public IFile File { get; private set; }

        public FileNode(IFile file) : base(file.FileFullName, "")
        {
            this.File = file;
        }
        public override void UpdateName(string newName, string newName_EN)
        {
            this.File.UpdateName(newName);
        }
    }
}

