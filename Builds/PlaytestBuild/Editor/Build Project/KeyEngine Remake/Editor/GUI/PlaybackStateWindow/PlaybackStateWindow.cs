using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine.Editor.GUI
{
    public class PlaybackStateWindow : EditorWindow
    {
        public PlaybackStateWindow()
        {
            title = "Playback";
        }

        public override void Render()
        {
            if (ImGui.Button("Play"))
            {

            }
            
            if (ImGui.Button("Stop"))
            {

            }
        }
    }
}
