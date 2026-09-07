namespace CloudFileManagerServer
{
    public class Algorithm
    {

        public TraverseLogger TraverseLogger { get; private set; }

        public Algorithm()
        {
            this.TraverseLogger = new TraverseLogger();
        }

        public static Algorithm Instance { get; set; }

        public static Algorithm GetInstance()
        {
            if (Instance == null)
            {
                Instance = new Algorithm();
            }
            return Instance;
        }

        #region FindFile
        public INode FindFileDfs(INode rootNode, string fileName)
        {
            TraverseLogger.Initialize();
            TraverseLogger.Start();

            fileName = fileName.Trim();
            var node = FindFileDfsCore(rootNode, fileName);

            TraverseLogger.Stop();
            return node;
        }
        public INode FindFileDfsCore(INode current, string fileName)
        {
            if (current is FileNode)
            {
                var file = current as FileNode;
                if (file.File.FileFullName.Trim().Equals(fileName.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    TraverseLogger.Log($"[符合] {file.Name}");
                    return file;
                }
                else
                {
                    TraverseLogger.Log($"掃描檔案: {file.Name}");
                }
            }
            if (current is DirectoryNode)
            {
                var directory = current as DirectoryNode;
                TraverseLogger.Log($"搜尋目錄: {directory.Name}");

                foreach (var child in directory.Children)
                {
                    var result = FindFileDfsCore(child, fileName);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }
        public INode FindFileBfs(INode rootNode, string fileName)
        {
            TraverseLogger.Initialize();
            TraverseLogger.Start();

            fileName = fileName.Trim();

            var queue = new Queue<INode>();
            queue.Enqueue(rootNode);
            INode target = null;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current is FileNode)
                {
                    var file = current as FileNode;
                    if (file.File.FileFullName.Trim().Equals(fileName.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        TraverseLogger.Log($"[符合] {file.Name}");
                        target = current;
                        break;
                    }
                    else
                    {
                        TraverseLogger.Log($"掃描檔案: {file.Name}");
                    }
                }

                if (current is DirectoryNode)
                {
                    var directory = current as DirectoryNode;
                    TraverseLogger.Log($"搜尋目錄: {directory.Name}");

                    foreach (var child in directory.Children)
                    {
                        queue.Enqueue(child);
                    }
                }
            }
            TraverseLogger.Stop();
            return target;
        }
        #endregion

        #region FindExtension

        public List<INode> FindExtensionDfs(INode rootNode, string extension)
        {
            TraverseLogger.Initialize();
            TraverseLogger.Start();

            extension = extension.Trim();
            List<INode> files = new List<INode>();
            files = FindExtensionDfsCore(rootNode, extension, files);

            TraverseLogger.Stop();
            return files;
        }
        public List<INode> FindExtensionDfsCore(INode current, string extension, List<INode> nodes)
        {
            if (current is FileNode)
            {
                var file = current as FileNode;
                if (file.File.FileExtension.Trim().Equals(extension.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    nodes.Add(file);
                    TraverseLogger.Log($"[符合] {file.Name}");
                }
                else
                {
                    TraverseLogger.Log($"掃描檔案: {file.Name}");
                }
            }
            if (current is DirectoryNode)
            {
                var directory = current as DirectoryNode;
                TraverseLogger.Log($"搜尋目錄: {directory.Name}");
                foreach (var child in directory.Children)
                {
                    var result = FindExtensionDfsCore(child, extension, nodes);
                }
            }
            return nodes;
        }
        public List<INode> FindExtensionBfs(INode rootNode, string extension)
        {
            TraverseLogger.Initialize();
            TraverseLogger.Start();

            extension = extension.Trim();
            List<INode> files = new List<INode>();

            var queue = new Queue<INode>();
            queue.Enqueue(rootNode);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current is FileNode)
                {
                    var file = current as FileNode;
                    if (file.File.FileExtension.Trim().Equals(extension.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        files.Add(file);
                        TraverseLogger.Log($"[符合] {file.Name}");
                    }
                    else
                    {
                        TraverseLogger.Log($"掃描檔案: {file.Name}");
                    }
                }

                if (current is DirectoryNode)
                {
                    var directory = current as DirectoryNode;
                    TraverseLogger.Log($"搜尋目錄: {directory.Name}");
                    foreach (var child in directory.Children)
                    {
                        queue.Enqueue(child);
                    }
                }
            }

            TraverseLogger.Stop();
            return files;
        }
        #endregion

        #region FindFolderSize

        public long FindFolderSize(INode rootNode, string folderName)
        {
            TraverseLogger.Initialize();
            TraverseLogger.Start();

            folderName = folderName.Trim();
            long size = 0;
            var node = FindFolderNode(rootNode, folderName);
            if (node != null)
            {
                size = FindNodeSizeDfs(node, size);
            }

            TraverseLogger.Log($"總容量: {SizeFormatter.Format(size)}");
            TraverseLogger.Stop();
            return size;
        }

        public INode FindFolderNode(INode rootNode, string folderName)
        {
            var queue = new Queue<INode>();
            queue.Enqueue(rootNode);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node is DirectoryNode)
                {
                    if (node.Name.Equals(folderName, StringComparison.OrdinalIgnoreCase))
                    {
                        TraverseLogger.Log($"[符合] {node.Name}");
                        return node;
                    }
                    else
                    {
                        var directory = node as DirectoryNode;
                        TraverseLogger.Log($"搜尋目錄: {directory.Name}");
                        foreach (var child in directory.Children)
                        {
                            queue.Enqueue(child);
                        }
                    }
                }
            }
            return null;
        }

        public long FindNodeSizeDfs(INode current, long folderSize)
        {
            if (current is FileNode)
            {
                var file = current as FileNode;
                TraverseLogger.Log($"[符合] {file.Name} 大小: {file.File.SizeStr}");
                return file.File.Size;
            }
            if (current is DirectoryNode)
            {
                var directory = current as DirectoryNode;
                TraverseLogger.Log($"搜尋目錄: {directory.Name}");

                foreach (var child in directory.Children)
                {
                    var fileSize = FindNodeSizeDfs(child, folderSize);
                    folderSize += fileSize;
                }
            }

            return folderSize;
        }
        #endregion

        public INode CloneNode(INode node)
        {
            if (node is FileNode fileNode)
            {
                if (fileNode.File is WordFile wordFile)
                {
                    return new FileNode(new WordFile(wordFile.FileFullName, wordFile.SizeStr, wordFile.PageCount));
                }
                if (fileNode.File is ImageFile imageFile)
                {
                    return new FileNode(new ImageFile(imageFile.FileFullName, imageFile.SizeStr, imageFile.Width, imageFile.Height));
                }
                if (fileNode.File is TextFile textFile)
                {
                    return new FileNode(new TextFile(textFile.FileFullName, textFile.SizeStr, textFile.Encoding));
                }

                return null;
            }
            else if (node is DirectoryNode directoryNode)
            {
                var newDirectory = new DirectoryNode(directoryNode.Name, directoryNode.Name_EN);
                foreach (var child in directoryNode.Children)
                {
                    var clonedChild = CloneNode(child);
                    newDirectory.AddChild(clonedChild);
                }
                return newDirectory;
            }
            else
            {
                throw new InvalidOperationException("Unknown node type");
            }
        }
    }
}
