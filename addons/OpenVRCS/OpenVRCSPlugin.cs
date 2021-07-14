#if TOOLS
using System.Collections.Generic;
using Godot;
using OpenVRCS.Utils.Nodes;

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

            Connect("scene_changed", this, nameof(OnSceneChanged));
        }

        public override void _ExitTree()
        {
            Disconnect("scene_changed", this, nameof(OnSceneChanged));
            RemoveCustomType("OVRToaster");
        }

        private void OnSceneChanged(Node sceneRoot)
        {
            if (!Engine.EditorHint || sceneRoot == null)
                return;
            foreach(var toasterNode in FindToasterNodes(sceneRoot))
            {
                var viewport = toasterNode.GetNode<Viewport>("ToasterMesh/ToasterViewport");
                if (viewport == null)
                    continue;
                var toasterMesh = toasterNode.GetNode<MeshInstance>("ToasterMesh");
                if (toasterMesh == null)
                    continue;
                var material = toasterMesh.GetSurfaceMaterial(0) as ShaderMaterial;
                if (material == null || !material.Shader.HasParam("albedo"))
                    continue;
                material.SetShaderParam("albedo", viewport.GetTexture());
                viewport.RenderTargetUpdateMode = Viewport.UpdateMode.Once;
            }
        }

        private IEnumerable<OVRToaster> FindToasterNodes(Node parent)
        {
            foreach(var possibleNode in parent.GetChildren()) {
                var toasterNode = possibleNode as OVRToaster;
                if (toasterNode != null)
                    yield return toasterNode;
                var node = possibleNode as Node;
                if (node != null)
                    foreach (var toasterSubNode in FindToasterNodes(node))
                        yield return toasterSubNode;
            }
        }
    }

}
#endif
