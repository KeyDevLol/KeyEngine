using ImGuiNET;
using KeyEngine.Assets;
using KeyEngine.Editor.GUI.AssetEditor;
using KeyEngine.Graphics;
using KeyEngine.Rendering;
using System.Diagnostics;
using System.Numerics;

namespace KeyEngine.Editor.GUI.FileBrowser
{
    // TODO: Optimise directory/file tree
    public partial class FileBrowser : EditorWindow
    {
        private string currentFolder = "Assets";
        private string[] currentFolderPathElements = ["Assets"];
        private readonly string programFullPath = AppDomain.CurrentDomain.BaseDirectory;

        private readonly FileSystemWatcher fileWatcher;

        private static readonly Texture folderIcon = new Texture("Editor/ContentBrowser/FolderIcon.png");

        private string[] directories = [];
        private string[] files = [];

        private int iconsSize = 64;
        private float padding = 32f;

        public FileBrowser()
        {
            Title = "File Browser";

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
            float cellSize = iconsSize + padding;

            int columnCount = (int)(ImGui.GetContentRegionAvail().X / cellSize);
            if (columnCount < 1)
                columnCount = 1;

            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 5);

            if (ImGui.Button("<"))
            {
                if (currentFolder != "Assets")
                {
                    currentFolder = currentFolder[..currentFolder.LastIndexOf('\\')];
                    Refresh();
                }
            }

            string path = string.Empty;

            for (int i = 0; i < currentFolderPathElements.Length; i++)
            {
                string element = currentFolderPathElements[i] + "\\";
                path += element;

                ImGui.SameLine();
                ImGui.PushID(path);
                if (ImGui.Button(element))
                {
                    OpenFolder(path);
                }
                ImGui.PopID();
            }

            ImGui.PopStyleVar();
            //ImGui.Text(currentFolder);

            RenderHierarchyTree();

            ImGui.SameLine();
            ImGui.BeginChild("ColumnsPanel_FileBrowser", new Vector2(ImGui.GetWindowWidth() - 13, ImGui.GetWindowHeight() - 85));
            ImGui.Dummy(new Vector2(0, 5));
            ImGui.Columns(columnCount, $"{nameof(FileBrowser)}_Columns", false);
            
            ImGui.PushStyleColor(ImGuiCol.Button, Vector4.Zero);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, Vector4.Zero);

            bool folderChanged = false;

            DrawFolders(ref folderChanged);

            if (!folderChanged)
            {
                DrawFiles();
            }

            ImGui.PopStyleColor();
            ImGui.PopStyleColor();
            ImGui.EndChild();

            DrawIconScaleSlider();
        }

        private void RenderHierarchyTree()
        {
            if (ImGui.BeginChild("Folders_Hierarchy_FileBrowser", new Vector2(100, ImGui.GetWindowHeight() - 85), ImGuiChildFlags.ResizeX))
            {
                Vector2 windowSize = ImGui.GetWindowSize();
                Vector2 windowPos = ImGui.GetWindowPos();
                Vector2 topRight = new Vector2(windowPos.X + windowSize.X, windowPos.Y);
                Vector2 bottomRight = new Vector2(windowPos.X + windowSize.X, windowPos.Y + windowSize.Y);
                Vector2 sizeOffset = new Vector2(2, 0);

                ImDrawListPtr drawList = ImGui.GetWindowDrawList();
                drawList.AddQuadFilled(topRight, bottomRight, bottomRight - sizeOffset, topRight - sizeOffset, ImGui.GetColorU32(ImGuiCol.Separator));

                ImGui.Dummy(new Vector2(0, 10));

                foreach (string directory in Directory.GetDirectories("Assets"))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(directory);

                    DrawHierarchyTreeFolder(directoryInfo);
                }

            }

            ImGui.EndChild(); // Folders Hierarchy
        }


        private void DrawHierarchyTreeFolder(DirectoryInfo directory)
        {
            Vector2 cursorPos = ImGui.GetCursorPos();

            ImGui.SetCursorPosX(cursorPos.X + 10);
            ImGui.Image(folderIcon.Handle, new Vector2(12), new Vector2(0, 1), new Vector2(1, 0));
            ImGui.SameLine();
            ImGui.SetCursorPosX(cursorPos.X + 25);
            bool treeIsOpened = ImGui.TreeNodeEx($"{directory.Name}", ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.NoAutoOpenOnLog);

            Vector2 rectMax = ImGui.GetItemRectMax();
            Vector2 rectMin = ImGui.GetItemRectMin();
            Vector2 rectSize = ImGui.GetItemRectSize();

            if (treeIsOpened)
            {
                FileInfo[] files = directory.GetFiles();
                DirectoryInfo[] directories = directory.GetDirectories();

                foreach (DirectoryInfo nestedDirectory in directories)
                {
                    DrawHierarchyTreeFolder(nestedDirectory);
                }

                foreach (FileInfo file in files)
                {
                    if (file.Extension == ".assetdata")
                        continue;

                    cursorPos = ImGui.GetCursorPos();

                    ImGui.SetCursorPosX(cursorPos.X + 9);
                    ImGui.Image(FileIconHelper.GetFileIcon(file.Extension).Handle, new Vector2(12), new Vector2(0, 1), new Vector2(1, 0));
                    ImGui.SameLine();
                    ImGui.PushID(file.FullName);
                    ImGui.Selectable(file.Name);
                    ImGui.PopID();
                    if (ImGui.IsItemDoubleClicked(ImGuiMouseButton.Left))
                        OpenFileInExternalProgram(file.FullName);
                }

                ImGui.TreePop();
            }

            if (ImGui.IsMouseHoveringRect(new Vector2(rectMin.X + 20, rectMin.Y), new Vector2(rectMin.X + ImGui.GetWindowSize().X, rectMax.Y)) && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                OpenFolder(GetProgramRelativePath(directory.FullName));
        }

        private void DrawIconScaleSlider()
        {
            ImGui.SetCursorPosX(ImGui.GetWindowWidth() - 128);
            ImGui.PushItemWidth(128);
            ImGui.SliderInt("##IconsScale_FileBrowser", ref iconsSize, 16, 64);
        }

        private void DrawFolders(ref bool folderChanged)
        {
            foreach (string directory in directories)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);

                ImGui.ImageButton(directory, folderIcon.Handle, new Vector2(iconsSize, iconsSize), new Vector2(0, 1), new Vector2(1, 0), Vector4.Zero, new Vector4(1, 0.737f, 0.847f, 1));

                if (ImGui.IsItemDoubleClicked(ImGuiMouseButton.Left))
                {
                    OpenFolder(GetProgramRelativePath(directory));
                    folderChanged = true;
                }

                ImGui.TextWrapped(directoryInfo.Name);
                ImGui.NextColumn();
            }
        }

        private void DrawFiles()
        {
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);

                if (fileInfo.Extension == ".assetdata")
                    continue;

                string fileName = Path.GetFileName(file);

                ImGui.PushID($"{currentFolder}_{fileName}");
                ImGui.ImageButton(currentFolder, FileIconHelper.GetFileIcon(fileInfo.Extension).Handle, new Vector2(iconsSize, iconsSize), new Vector2(0, 1), new Vector2(1, 0));
                ImGui.PopID();

                if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
                {
                    Log.Print(file);
                    AssetEditorWindow.Singleton.CurrentAsset = AssetsManager.GetAssetInfo(file);
                }

                if (ImGui.IsItemDoubleClicked(ImGuiMouseButton.Left))
                {
                    OpenFileInExternalProgram(file);
                }

                ImGui.TextWrapped(fileName);

                ImGui.NextColumn();
            }
        }


        //public class HierarchyTree
        //{
        //    public List<HierarchyDirectory> Directories = [];

        //    public void Refresh(string relativeFodlerPath)
        //    {
        //        DirectoryInfo directoryInfo = new DirectoryInfo(relativeFodlerPath);

        //        FileInfo[] files = directoryInfo.GetFiles();
        //        DirectoryInfo[] directories = directoryInfo.GetDirectories();
        //        Directories.Add(new(files, directories));

        //        Directories.Clear();

        //        foreach (DirectoryInfo directory in directories)
        //        {
        //            HierarchyDirectory hierarchyDirectory = new HierarchyDirectory();
        //        }
        //    }
        //}
        //public class HierarchyDirectory
        //{
        //    public FileInfo[] Files;
        //    public DirectoryInfo[] Directories;

        //    public HierarchyDirectory(FileInfo[] files, DirectoryInfo[] directories)
        //    {
        //        Files = files;
        //        Directories = directories;
        //    }
        //}


        private void OpenFolder(string relativePath)
        {
            if (relativePath.EndsWith("\\"))
                relativePath = relativePath[..^1];

            currentFolder = relativePath;
            Refresh();
        }
        
        private void OpenFileInExternalProgram(string fullPath)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(fullPath)
            {
                UseShellExecute = true
            };

            Process.Start(processStartInfo);
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
            currentFolderPathElements = currentFolder.Split('\\');
        }

        private string GetProgramRelativePath(string fullPath) => Path.GetRelativePath(programFullPath, fullPath);
    }
}
