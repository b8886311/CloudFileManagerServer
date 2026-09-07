using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using System.Xml;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace CloudFileManagerServer
{
    public class FileManager
    {
        public static FileManager _instance { get; set; }
        public static FileManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new FileManager();
            }
            return _instance;
        }
        public DirectoryNode Root { get; private set; }

        private CommandManager _commandManager { get; set; }

        public FileManager()
        {
            Root = new DirectoryNode("根目錄", "Root");
            var Docs = new DirectoryNode("專案文件", "Project_Docs");
            var Notes = new DirectoryNode("個人筆記", "Personal_Notes");
            Root.AddChild(Docs);
            Root.AddChild(Notes);
            Docs.AddChild(new FileNode(new WordFile("需求規格書.docx", "500KB", 15)));
            Docs.AddChild(new FileNode(new ImageFile("系統架構圖.png", "2MB", 1920, 1080)));
            Notes.AddChild(new FileNode(new TextFile("待辦清單.txt", "1KB", "UTF-8")));
            var Archive = new DirectoryNode("2025備份", "Archive_2025");
            Notes.AddChild(Archive);
            Archive.AddChild(new FileNode(new WordFile("舊會議記錄.docx", "200KB", 5)));
            Root.AddChild(new FileNode(new TextFile("README.txt", "500B", "ASCII")));

            _commandManager = new CommandManager();

        }

        #region FindFile
        public ApiResponse<string> FindFileDfs(string fileName)
        {
            var node = Algorithm.FindFileDfs(Root, fileName);

            ApiResponse<string> response = new ApiResponse<string>()
            {
                Result = node == null ? "" : node.GetFullPath(),
                TraverseLog = Algorithm.GetTraverseLog()
            };
            return response;
        }
        
        public ApiResponse<string> FindFileBfs(string fileName)
        {
            var node = Algorithm.FindFileBfs(Root, fileName);

            ApiResponse<string> response = new ApiResponse<string>()
            {
                Result = node == null ? "" : node.GetFullPath(),
                TraverseLog = Algorithm.GetTraverseLog()
            };
            return response;
        }
        #endregion

        #region FindExtension

        public ApiResponse<string[]> FindExtensionDfs(string extension)
        {
            var files = Algorithm.FindExtensionDfs(Root, extension);

            ApiResponse<string[]> response = new ApiResponse<string[]>()
            {
                Result = files.Select(f => f.GetFullPath()).ToArray(),  
                TraverseLog = Algorithm.GetTraverseLog()
            };
            return response;
        }
      
        public ApiResponse<string[]> FindExtensionBfs(string extension)
        {
            var files = Algorithm.FindExtensionBfs(Root, extension);

            ApiResponse<string[]> response = new ApiResponse<string[]>()
            {
                Result = files.Select(f => f.GetFullPath()).ToArray(),
                TraverseLog = Algorithm.GetTraverseLog()
            };

            return response;
        }
        #endregion

        #region FindFolderSize

        public ApiResponse<string> FindFolderSize(string folderName)
        {
            var size = Algorithm.FindFolderSize(Root, folderName);
            
            ApiResponse<string> response = new ApiResponse<string>()
            {
                Result = SizeFormatter.Format(size),
                TraverseLog = Algorithm.GetTraverseLog()
            };
            return response;
        }
      
        #endregion

        #region ParseToPlainText
        public string ParseToPlainText()
        {
            TreeFormatter formatter = new TreeFormatter();
            return formatter.FormatTree(Root);

        }
        #endregion

        #region ParseToXml
        public string ParseToXml()
        {
            var serializer = new XmlSerializer();
            return serializer.Serialize(Root);
        }
        #endregion

        public string AppendTag(string nodeName, string tagLevel)
        {
            var message = "";

            try
            {
                TagCommand tc = new TagCommand(Root, nodeName, tagLevel);
                message = _commandManager.Execute(tc);
            }
            catch (Exception ex)
            {
                message = $"Error: {ex.Message}";
            }

            return message;
        }

        public string SortNode(string sortBy, bool ascending)
        {
            var message = "";

            try
            {
                if (!Enum.TryParse(sortBy, true, out SortBy sortType))
                {
                    throw new ArgumentException($"Invalid sort type: {sortBy}");
                }

                SortOrder sortOrder = ascending ? SortOrder.Ascending : SortOrder.Descending;
          

                SortCommand sc = new SortCommand(Root, sortType, sortOrder);
                message = _commandManager.Execute(sc);
            }
            catch (Exception ex)
            {
                message = $"Error: {ex.Message}";
            }

            return message;
        }

        public string CutNode(string nodeName)
        {
            var message = "";

            try
            {
                CutCommand cc = new CutCommand(Root, nodeName);
                message = _commandManager.Execute(cc);
            }
            catch (Exception ex)
            {
                message = $"Error: {ex.Message}";
            }

            return message;
        }

        public string CopyNode(string nodeName)
        {
            var message = "";
            try
            {
                CopyCommand cc = new CopyCommand(Root, nodeName);
                message = _commandManager.Execute(cc);
            }
            catch (Exception ex)
            {
                message = $"Error: {ex.Message}";
            }
            return message;
        }

        public string PasteNode(string nodeName)
        {
            var message = "";

            try
            {
                if (!_commandManager.CanUndo)
                {
                    throw new InvalidOperationException("No operation to paste");
                }

                var command = _commandManager.GetLastCutOrCopyItem();
                if (command == null)
                {
                    throw new InvalidOperationException("No item to paste");
                }

                var item = command is CutCommand cutCommand ? (command as CutCommand).CutItem : (command as CopyCommand).CopyItem;

                PasteCommand pc = new PasteCommand(Root, nodeName, item);
                message = _commandManager.Execute(pc);
            }
            catch (Exception ex)
            {
                message = $"Error: {ex.Message}";
            }

            return message;
        }

        public string Undo()
        {
            var message = "";

            try
            {
                message = _commandManager.Undo();
            }
            catch (Exception ex)
            {
                message = $"Error: {ex.Message}";
            }

            return message;
        }

        public string Redo()
        {
            var message = "";

            try
            {
                message = _commandManager.Redo();
            }
            catch (Exception ex)
            {
                message = $"Error: {ex.Message}";
            }

            return message;
        }
    }
}
