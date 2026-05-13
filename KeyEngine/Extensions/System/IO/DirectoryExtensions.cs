namespace KeyEngine.Extensions.System.IO
{
    public static class DirectoryExtensions
    {
        extension (Directory)
        {
            public static void Copy(string sourcePath, string targetPath)
            {
                foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
                }

                foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
                }
            }
        }
    }
}
