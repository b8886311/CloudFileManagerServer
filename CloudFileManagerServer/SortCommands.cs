using System.Globalization;

namespace CloudFileManagerServer
{
    public class SortCommand : ICommand
    {
        private DirectoryNode _rootNode { get; set; }
        private IComparer<INode> _comparer { get; set; }
        private SortBy _sortBy { get; set; }
        private SortOrder _sortOrder { get; set; }

        private readonly Dictionary<DirectoryNode, List<INode>> _snapshots = new Dictionary<DirectoryNode, List<INode>>();

        public SortCommand(DirectoryNode rootNode, SortBy sortBy, SortOrder sortOrder)
        {
            _rootNode = rootNode;
            _sortBy = sortBy;
            _sortOrder = sortOrder;
            _comparer = NodeComparerFactory.Create(sortBy, sortOrder);
        }

        public string Description
        {
            get
            {
                string order = _sortOrder == SortOrder.Descending ? "降冪" : "升冪";
                return $"依 {_sortBy} {order} 排序 {_rootNode.Name}";
            }
        }

        public void Execute()
        {
            _snapshots.Clear(); 
            SortDirectory(_rootNode);
        }

        private void SortDirectory(DirectoryNode directory)
        {
            _snapshots[directory] = new List<INode>(directory.Children);
            directory.Children.Sort(_comparer);

            foreach (INode child in directory.Children)
            {
                if (child is DirectoryNode subDirectory)
                {
                    SortDirectory(subDirectory);
                }
            }
        }

        public void Undo()
        {
            foreach (KeyValuePair<DirectoryNode, List<INode>> entry in _snapshots)
            {
                entry.Key.Children.Clear();
                entry.Key.Children.AddRange(entry.Value);
            }
        }
    }


    public enum SortBy
    {
        Name,
        Size,
        Extension
    }

    public enum SortOrder
    {
        Ascending,
        Descending
    }

    public class NameComparer : IComparer<INode>
    {
        public int Compare(INode? a, INode? b)
        {
            return string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class SizeComparer : IComparer<INode>
    {
        public int Compare(INode? a, INode? b)
        {
            Algorithm algo = Algorithm.GetInstance();
            long sizeA = 0;
            sizeA = algo.FindNodeSizeDfs(a, sizeA);
            long sizeB = 0;
            sizeB = algo.FindNodeSizeDfs(b, sizeB);
            int result = sizeA.CompareTo(sizeB);
            if (result != 0)
            {
                return result;
            }
            return string.Compare(a!.Name, b!.Name, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class ExtensionComparer : IComparer<INode>
    {
        public int Compare(INode? a, INode? b)
        {
            int result = string.Compare(ExtensionOf(a), ExtensionOf(b),StringComparison.OrdinalIgnoreCase);
            if (result != 0)
            {
                return result;
            }
            return string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
        }

        private static string ExtensionOf(INode node)
        {
            if (node is FileNode fileNode)
            {
                return fileNode.File.FileExtension;
            }
            return "";
        }
    }

    public class ReverseComparer : IComparer<INode>
    {
        private readonly IComparer<INode> _inner;

        public ReverseComparer(IComparer<INode> inner)
        {
            _inner = inner;
        }

        public int Compare(INode? a, INode? b)
        {
            return -_inner.Compare(a, b);
        }
    }

    public class DirectoriesFirstComparer : IComparer<INode>
    {
        private readonly IComparer<INode> _inner;

        public DirectoriesFirstComparer(IComparer<INode> inner)
        {
            _inner = inner;
        }

        public int Compare(INode? a, INode? b)
        {
            bool aIsDirectory = a is DirectoryNode;
            bool bIsDirectory = b is DirectoryNode;

            if (aIsDirectory && !bIsDirectory)
            {
                return -1;
            }
            if (!aIsDirectory && bIsDirectory)
            {
                return 1;
            }
            return _inner.Compare(a, b);
        }
    }

    public static class NodeComparerFactory
    {
        public static IComparer<INode> Create(SortBy sortBy, SortOrder sortOrder, bool directoriesFirst = true)
        {
            IComparer<INode> comparer;

            if (sortBy == SortBy.Size)
            {
                comparer = new SizeComparer();
            }
            else if (sortBy == SortBy.Extension)
            {
                comparer = new ExtensionComparer();
            }
            else
            {
                comparer = new NameComparer();
            }

            if (sortOrder == SortOrder.Descending)
            {
                comparer = new ReverseComparer(comparer);
            }
            if (directoriesFirst)
            {
                comparer = new DirectoriesFirstComparer(comparer);
            }
            return comparer;
        }
    }
}

