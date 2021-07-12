#if TOOLS
using Godot;

namespace OpenVRCS
{

    [Tool]
    public class OpenVRCSPlugin : EditorPlugin
    {

        public override void _EnterTree()
        {
            base._EnterTree();

            GD.Print("=================");
            GD.Print("OpenVRPlugin (C#)");
            GD.Print("=================");

            var ovrToasterNodeScript = GD.Load<Script>("res://addons/OpenVRCS/Utils/Nodes/OVRToaster.cs");
            var ovrToasterNodeIcon = GD.Load<Texture>("res://addons/OpenVRCS/Utils/Nodes/OVRToaster.png");
            AddCustomType("OVRToaster", "Spatial", ovrToasterNodeScript, ovrToasterNodeIcon);
        }

        public override void _ExitTree()
        {
            RemoveCustomType("OVRToaster");
        }

    }

}
#endif
