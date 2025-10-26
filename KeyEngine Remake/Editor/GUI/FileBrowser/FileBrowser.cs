using ImGuiNET;
using KeyEngine.Graphics;
using System.Diagnostics;
using System.Numerics;

namespace KeyEngine.Editor.GUI
{
    public class FileBrowser : EditorWindow
    {
        private string currentFolder = "Assets";
        private readonly FileSystemWatcher fileWatcher;

        private static readonly Texture folderIcon = new Texture("Editor/ContentBrowser/FolderIcon.png");

        private string[] directories = [];
        private string[] files = [];

        private int iconsSize = 64;

        public FileBrowser()
        {
            title = "File Browser";

            fileWatcher = new FileSystemWatcher(currentFolder);

            fileWatcher.Changed += FileChanged;
            fileWatcher.Deleted += FileChanged;
            fileWatcher.Created += FileChanged;
            fileWatcher.Renamed += FileChanged;

            fileWatcher.EnableRaisingEvents = true;

            Refresh();
        }

        public override void Render()
        {
            float padding = 32f;
            float cellSize = iconsSize + padding;

            int columnCount = (int)(ImGui.GetContentRegionAvail().X / cellSize);
            if (columnCount < 1)
                columnCount = 1;

            if (ImGui.SmallButton("<"))
            {
                if (currentFolder != "Assets")
                {
                    currentFolder = currentFolder[..currentFolder.LastIndexOf('\\')];
                    Refresh();
                }
            }

            ImGui.SameLine();
            ImGui.Text(currentFolder);
            ImGui.BeginChild("ColumnsPanel_FileBrowser", new Vector2(ImGui.GetWindowWidth() - 13, ImGui.GetWindowHeight() - 85));
            ImGui.Dummy(new Vector2(0, 5));
            ImGui.Columns(columnCount, $"{nameof(FileBrowser)}_Columns", false);

            ImGui.PushStyleColor(ImGuiCol.Button, Vector4.Zero);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Vector4.Zero);
            //ImGui.PushStyleColor(ImGuiCol.ButtonActive, Vector4.Zero);

            bool folderChanged = false;

            foreach (string directory in directories)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);

                ImGui.ImageButton(directory, folderIcon.Handle, new Vector2(iconsSize, iconsSize), new Vector2(0, 1), new Vector2(1, 0), Vector4.Zero, new Vector4(1, 0.737f, 0.847f, 1));

                if (ImGui.IsItemHovered())
                {
                    if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                    {
                        currentFolder = Path.Combine(currentFolder, directoryInfo.Name);
                        Refresh();
                        folderChanged = true;
                    }
                }

                ImGui.TextWrapped(directoryInfo.Name);
                ImGui.NextColumn();
            }

            if (!folderChanged)
            {
                foreach (string file in files)
                {
                    FileInfo fileInfo = new FileInfo(file);

                    string fileName = Path.GetFileName(file);

                    ImGui.ImageButton(currentFolder, FileIconHelper.GetFileIcon(fileInfo.Extension).Handle, new System.Numerics.Vector2(iconsSize, iconsSize), new System.Numerics.Vector2(0, 1), new System.Numerics.Vector2(1, 0));

                    Log.Print(ImGui.IsItemFocused());

                    if (ImGui.IsItemHovered())
                    {
                        if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                        {
                            ProcessStartInfo info = new ProcessStartInfo(fileInfo.FullName)
                            {
                                UseShellExecute = true
                            };

                            Process.Start(info);
                        }
                    }

                    ImGui.TextWrapped(fileName);

                    ImGui.NextColumn();
                }
            }

            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            //ImGui.PopStyleColor();
            ImGui.EndChild();

            ImGui.SetCursorPosX(ImGui.GetWindowWidth() - 128);
            ImGui.PushItemWidth(128);
            ImGui.SliderInt("##IconsScale_FileBrowser", ref iconsSize, 16, 64);
        }

        private void FileChanged(object sender, FileSystemEventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            directories = Directory.GetDirectories(currentFolder);
            files = Directory.GetFiles(currentFolder);

            fileWatcher.Path = currentFolder;
        }

        private static class FileIconHelper
        {
            private static readonly Texture imageFileIcon = new Texture("Editor/ContentBrowser/FileImageIcon.png");
            private static readonly Texture audioFileIcon = new Texture("Editor/ContentBrowser/FileAudioIcon.png");
            private static readonly Texture shaderFileIcon = new Texture("Editor/ContentBrowser/FileShaderIcon.png");
            private static readonly Texture unidentifiedFileIcon = new Texture("Editor/ContentBrowser/FileIcon.png");

            private static Dictionary<string, Texture> icons = new Dictionary<string, Texture>()
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
}
