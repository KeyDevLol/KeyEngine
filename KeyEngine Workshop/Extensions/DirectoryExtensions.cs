namespace KeyEngine_Workshop.Extensions
{
    public static class DirectoryExtensions
    {
        extension (Directory)
        {
            public static void CloneDirectory(string root, string dest)
            {
                foreach (var directory in Directory.GetDirectories(root))
                {
                    var newDirectory = Path.Combine(dest, Path.GetFileName(directory));
                    Directory.CreateDirectory(newDirectory);
                    CloneDirectory(directory, newDirectory);
                }

                foreach (var file in Directory.GetFiles(root))
                {
                    File.Copy(file, Path.Combine(dest, Path.GetFileName(file)));
                }
            }
        }
    }
}
