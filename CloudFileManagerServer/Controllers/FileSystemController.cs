using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace CloudFileManagerServer.Controllers
{
    [ApiController]
    [Route("api/file-system")]
    public class FileSystemController: ControllerBase
    {
        private readonly ILogger<FileSystemController> _logger;

        public FileSystemController(ILogger<FileSystemController> logger)
        {
            _logger = logger;
        }

        [HttpGet("find-file", Name = "FindFile")]
        public IActionResult FindFile(string fileName, string way = "dfs")
        {
            ApiResponse<string> response;
            if (way == "dfs")
                response = FileManager.GetInstance().FindFileDfs(fileName);

            else
                response = FileManager.GetInstance().FindFileBfs(fileName);

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(response, options);
            return Ok($"{json}");

        }

        [HttpGet("find-extension", Name = "FindExtension")]
        public IActionResult FindExtension(string extension, string way = "dfs")
        {
            ApiResponse<string[]> response;
            if (way == "dfs")
                response = FileManager.GetInstance().FindExtensionDfs(extension);
            else
                response = FileManager.GetInstance().FindExtensionBfs(extension);

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(response, options);
            return Ok($"{json}");
        }

        [HttpGet("find-folder-size", Name = "FindFolderSize")]
        public IActionResult FindFolderSize(string folderName)
        {
            ApiResponse<string> response = FileManager.GetInstance().FindFolderSize(folderName);
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(response, options);
            return Ok($"{json}");
        }

        [HttpGet("parse-to-plain-text", Name = "ParseToPlainText")]
        public IActionResult ParseToPlainText()
        {
            var result = FileManager.GetInstance().ParseToPlainText();
            return Ok(result);
        }

        [HttpGet("parse-to-xml", Name = "ParseToXml")]
        public IActionResult ParseToXml()
        {
            var result = FileManager.GetInstance().ParseToXml();
            return Ok(result);
        }

        [HttpGet("append-tag", Name = "AppendTag")]
        public IActionResult AppendTag(string nodeName, string tagLevel)
        {
            var message = FileManager.GetInstance().AppendTag(nodeName, tagLevel);

            var result = FileManager.GetInstance().ParseToPlainText();

            var text = message + Environment.NewLine + Environment.NewLine + result;
            return Ok(text);
        }

        [HttpGet("sort-node", Name = "SortNode")]
        public IActionResult SortNode(string sortBy, bool ascending)
        {
            var message = FileManager.GetInstance().SortNode(sortBy, ascending);

            var result = FileManager.GetInstance().ParseToPlainText();

            var text = message + Environment.NewLine + Environment.NewLine + result;
            return Ok(text);
        }

        [HttpGet("cut-node", Name = "CutNode")]
        public IActionResult CutNode(string nodeName)
        {
            var message = FileManager.GetInstance().CutNode(nodeName);

            var result = FileManager.GetInstance().ParseToPlainText();

            var text = message + Environment.NewLine + Environment.NewLine + result;
            return Ok(text);
        }

        [HttpGet("copy-node", Name = "CopyNode")]
        public IActionResult CopyNode(string nodeName)
        {
            var message = FileManager.GetInstance().CopyNode(nodeName);

            var result = FileManager.GetInstance().ParseToPlainText();

            var text = message + Environment.NewLine + Environment.NewLine + result;
            return Ok(text);
        }

        [HttpGet("paste-node", Name = "PasteNode")]
        public IActionResult PasteNode(string nodeName)
        {
            var message = FileManager.GetInstance().PasteNode(nodeName);

            var result = FileManager.GetInstance().ParseToPlainText();

            var text = message + Environment.NewLine + Environment.NewLine + result;
            return Ok(text);
        }

        [HttpGet("undo", Name = "Undo")]
        public IActionResult Undo()
        {
            var message = FileManager.GetInstance().Undo();

            var result = FileManager.GetInstance().ParseToPlainText();

            var text = message + Environment.NewLine + Environment.NewLine + result;
            return Ok(text);
        }

        [HttpGet("redo", Name = "Redo")]
        public IActionResult Redo()
        {
            var message = FileManager.GetInstance().Redo();

            var result = FileManager.GetInstance().ParseToPlainText();

            var text = message + Environment.NewLine + Environment.NewLine + result;
            return Ok(text);
        }
    }
}