using Godot;
using OpenVRCS.Utils.Nodes;

namespace OpenVRCS.Demo
{

    public class Main : Spatial
    {

        private OVRToaster _toaster;

        public override void _Ready()
        {
            base._Ready();
            _toaster = GetNode<OVRToaster>("OVRToaster");
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            if (_toaster != null && Input.IsActionJustPressed("ui_select"))
            {
                _toaster.Toast("Hello, World!");
            }
        }
    }

}
