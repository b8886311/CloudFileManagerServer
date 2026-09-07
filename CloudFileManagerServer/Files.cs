namespace CloudFileManagerServer
{
    public interface IFile
    {
        public string FileName { get; }
        public string FileExtension { get; }
        public string FileFullName { get; }
        public long Size { get; }
        public string SizeStr { get; }
        public DateTime CreationTime { get; }
        public string ToXmlString();
        public IFile Clone();
        public void UpdateName(string newName);
    }
    public class BaseFile : IFile
    {
        public string FileName { get; private set; }

        public string FileExtension { get; private set; }

        public string FileFullName { get; private set; }

        public long Size { get; private set; }

        public string SizeStr { get; private set; }

        public DateTime CreationTime { get; private set; }

        public virtual string ToXmlString()
        {
            return $"{this.FileFullName} (大小: {this.SizeStr})";
        }

        public virtual IFile Clone()
        {
            return new BaseFile(this.FileFullName, this.SizeStr);
        }

        public void UpdateName(string newName)
        {
            this.FileFullName = newName;
            int dot = FileFullName.LastIndexOf('.');
            if (dot < 0)
            {
                this.FileName = newName;
                this.FileExtension = "";
            }
            else
            {
                this.FileName = newName.Substring(0, dot);
                this.FileExtension = newName.Substring(dot);
            }
        }

        public BaseFile(string fileFullName, string sizeStr)
        {
            this.FileFullName = fileFullName;
            int dot = FileFullName.LastIndexOf('.');
            if (dot < 0)
            {
                this.FileName = fileFullName;
                this.FileExtension = "";
            }
            else
            {
                this.FileName = fileFullName.Substring(0, dot);
                this.FileExtension = fileFullName.Substring(dot);
            }
            this.Size = SizeFormatter.Parse(sizeStr); ;
            this.SizeStr = sizeStr;
            this.CreationTime = DateTime.Now;
        }
    }
    public class WordFile : BaseFile
    {
        public int PageCount { get; private set; }
        public WordFile(string fileName, string size, int pageCount): base(fileName, size)
        {
            this.PageCount = pageCount;
        }
        public override string ToString()
        {
            return $"{this.FileFullName} [Word 檔案] (頁數: {this.PageCount}, 大小: {this.SizeStr})";
        }
        public override string ToXmlString()
        {
            return $"頁數: {this.PageCount}, 大小: {this.SizeStr}";
        }

        public override IFile Clone()
        {
            return new WordFile(this.FileFullName, this.SizeStr, this.PageCount);
        }
    }

    public class ImageFile : BaseFile
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public string Resolution { get; private set; }
        public ImageFile(string fileName, string size, int width, int height): base(fileName, size)
        {
            this.Width = width;
            this.Height = height;
            this.Resolution = width + "x" + height;
        }
        public override string ToString()
        {
            return $"{this.FileFullName} [圖片] (解析度: {this.Resolution}, 大小: {this.SizeStr})";
        }

        public override string ToXmlString()
        {
            return $"解析度: {this.Resolution} ,大小: {this.SizeStr}";
        }
        public override IFile Clone()
        {
            return new ImageFile(this.FileFullName, this.SizeStr, this.Width, this.Height);
        }
    }

    public class TextFile : BaseFile
    {
        public string Encoding { get; private set; }
        public TextFile(string fileName, string size, string encoding): base(fileName, size)
        {
            this.Encoding = encoding;
        }
        public override string ToString()
        {
            return $"{this.FileFullName} [純文字檔] (編碼: {this.Encoding}, 大小: {this.SizeStr})";
        }
        public override string ToXmlString()
        {
            return $"編碼: {this.Encoding}, 大小: {this.SizeStr}";
        }
        public override IFile Clone()
        {
            return new TextFile(this.FileFullName, this.SizeStr, this.Encoding);
        }
    }

    public static class SizeFormatter
    {
        public static string Format(long bytes)
        {
            if (bytes < 1024)
            {
                return bytes + "B";
            }
            if (bytes < 1024 * 1024)
            {
                return (bytes / 1024.0).ToString("F2") + "KB";
            }
            if (bytes < 1024 * 1024 * 1024)
            {
                return (bytes / (1024.0 * 1024)).ToString("F2") + "MB"; 
            }
            return (bytes / (1024.0 * 1024 * 1024)).ToString("F2") + "GB";
        }

        public static long Parse(string sizeStr)
        {
            if (string.IsNullOrWhiteSpace(sizeStr))
                return 0;

            sizeStr = sizeStr.Trim().ToLower();

            string numberPart = new string(sizeStr.TakeWhile(c => char.IsDigit(c) || c == '.').ToArray());

            if (!double.TryParse(numberPart, out double size)) 
                throw new FormatException($"Invalid size format: {sizeStr}");

            if (sizeStr.Contains("gb"))
                return (long)(size * 1024 * 1024 * 1024);
            if (sizeStr.Contains("mb"))
                return (long)(size * 1024 * 1024);
            if (sizeStr.Contains("kb"))
                return (long)(size * 1024);

            return (long)size;
        }
    }
}
