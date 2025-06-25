using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine.Editor.GUI.Windows
{
    public class HelloWindow : EditorWindow
    {
        public HelloWindow()
        {

        }

        public override void Render()
        {
            ImGui.Button("Lol");
        }
    }
}
