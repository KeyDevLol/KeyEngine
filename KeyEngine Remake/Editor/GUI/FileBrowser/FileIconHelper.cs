using KeyEngine.Graphics;

namespace KeyEngine.Editor.GUI.FileBrowser
{
    public static class FileIconHelper
    {
        private static readonly Texture imageFileIcon = new Texture("Editor/ContentBrowser/FileImageIcon.png");
        private static readonly Texture audioFileIcon = new Texture("Editor/ContentBrowser/FileAudioIcon.png");
        private static readonly Texture shaderFileIcon = new Texture("Editor/ContentBrowser/FileShaderIcon.png");
        private static readonly Texture unidentifiedFileIcon = new Texture("Editor/ContentBrowser/FileIcon.png");

        private static readonly Dictionary<string, Texture> icons = new Dictionary<string, Texture>()
        {
            // Image files
            { ".png", imageFileIcon },
            { ".jpg", imageFileIcon },
            { ".jpeg", imageFileIcon },
            { ".bmp", imageFileIcon },
            { ".tga", imageFileIcon },
            { ".psd", imageFileIcon },

            // Audio files
            { ".wav", audioFileIcon },
            { ".ogg", audioFileIcon },

            // Shader files
            { ".frag", shaderFileIcon },
            { ".vert", shaderFileIcon },
        };

        public static Texture GetFileIcon(string extension)
        {
            ArgumentNullException.ThrowIfNull(extension);

            if (icons.TryGetValue(extension, out Texture? result))
            {
                return result;
            }

            return unidentifiedFileIcon;
        }
    }
}
