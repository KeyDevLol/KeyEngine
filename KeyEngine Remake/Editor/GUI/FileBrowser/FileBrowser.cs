using ImGuiNET;
using KeyEngine.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine.Editor.GUI
{
    public class FileBrowser : EditorWindow
    {
        private string currentFolder = "Assets";
        private readonly FileSystemWatcher fileWatcher;

        private static readonly Texture folderIcon = new Texture("Editor/ContentBrowser/FolderIcon.png");

        private string[] directories = [];
        private string[] files = [];

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
            float padding = 16f;
            float thumbnailSize = 64f;
            float cellSize = thumbnailSize + padding;

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

            ImGui.Columns(columnCount, $"{nameof(FileBrowser)}_Columns", false);

            ImGui.PushStyleColor(ImGuiCol.Button, new System.Numerics.Vector4(0, 0, 0, 0));
            bool folderChanged = false;

            foreach (string directory in directories)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);

                ImGui.ImageButton(directory, folderIcon.Handle, new System.Numerics.Vector2(thumbnailSize, thumbnailSize), new System.Numerics.Vector2(0, 1), new System.Numerics.Vector2(1, 0));

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

                    ImGui.ImageButton(currentFolder, FileIconHelper.GetFileIcon(fileInfo.Extension).Handle, new System.Numerics.Vector2(thumbnailSize, thumbnailSize), new System.Numerics.Vector2(0, 1), new System.Numerics.Vector2(1, 0));

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
