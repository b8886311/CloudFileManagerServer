namespace CloudFileManagerServer
{
    //public class FileEditor : ICommand
    //{
    //    public static void RenameFile(string filePath, string newFileName)
    //    {
    //        if (File.Exists(filePath))
    //        {
    //            string directory = Path.GetDirectoryName(filePath);
    //            string newFilePath = Path.Combine(directory, newFileName);
    //            File.Move(filePath, newFilePath);
    //        }
    //    }
    //    public static void MoveFile(string sourceFilePath, string destinationDirectory)
    //    {
    //        if (File.Exists(sourceFilePath) && Directory.Exists(destinationDirectory))
    //        {
    //            string fileName = Path.GetFileName(sourceFilePath);
    //            string destinationFilePath = Path.Combine(destinationDirectory, fileName);
    //            File.Move(sourceFilePath, destinationFilePath);
    //        }
    //    }
    //    public static void DeleteFile(string filePath)
    //    {
    //        if (File.Exists(filePath))
    //        {
    //            File.Delete(filePath);
    //        }
    //    }

    //    public void Undo()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public void Redo()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
