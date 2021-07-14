#if TOOLS
using Godot;
using OpenVRCS.Utils.Scripts;

namespace OpenVRCS.Utils.Nodes
{

    [Tool]
    public class OVRToaster : Spatial
    {

        #region Backing Fields
        private float _maxAlpha = 0.7f;
        #endregion

        #region Other Fields
        private MeshInstance _toasterMesh;
        private DynamicFont _hind32;
        private DynamicFont _consola32;
        private Label _toasterText;
        private Tween _toasterTween;
        private Viewport _toasterViewport;
        private OVRToasterState _toasterState;
        #endregion

        #region Exported Properties
        [Export(PropertyHint.Range, "0.0f, 1.0f, 0.1f")]
        public float MaxAlpha
        {
            get => _maxAlpha;
            set
            {
                _maxAlpha = value;
                if (Engine.EditorHint && _toasterMesh != null)
                {
                    var material = _toasterMesh.GetSurfaceMaterial(0) as ShaderMaterial;
                    material.SetShaderParam("alpha", value);
                }
            }
        }
        #endregion

        #region Overwritten Base Methods
        public override void _EnterTree()
        {
            base._EnterTree();

            var quadMesh = new QuadMesh
            {
                Size = new Vector2(0.6f, 0.2f),
            };
            var toasterMaterial = GD.Load<ShaderMaterial>("res://addons/OpenVRCS/Utils/Materials/OVRToaster.material.tres");
            toasterMaterial.ResourceLocalToScene = true;

            _toasterMesh = new MeshInstance
            {
                Name = "ToasterMesh",
                Mesh = quadMesh,
                Transform = new Transform(new Quat(new Vector3(Mathf.Deg2Rad(-30.0f), 0.0f, 0.0f)), new Vector3(0.0f, -0.4f, -0.7f)),
            };
            _toasterMesh.SetSurfaceMaterial(0, toasterMaterial);

            _toasterTween = new Tween
            {
                Repeat = false,
                PlaybackProcessMode = Tween.TweenProcessMode.Idle,
                Name = "ToasterTween",
            };

            _toasterViewport = new Viewport
            {
                Name = "ToasterViewport",
                Size = new Vector2(600.0f, 200.0f),
                TransparentBg = true,
                Msaa = Viewport.MSAA.Disabled,
                Hdr = false,
                Disable3d = true,
                Usage = Viewport.UsageEnum.Usage2dNoSampling,
                RenderTargetVFlip = true,
                RenderTargetUpdateMode = Viewport.UpdateMode.Always,
                RenderTargetClearMode = Viewport.ClearMode.Always,
                GuiSnapControlsToPixels = true,
            };

            var control = new Control
            {
                AnchorTop = 0,
                AnchorRight = 0,
                AnchorBottom = 0,
                AnchorLeft = 0,
                MarginTop = 0,
                MarginRight = 600,
                MarginBottom = 200,
                MarginLeft = 0,
                Name = "OVRToasterDisplay",
            };

            var colorRect = new ColorRect
            {
                AnchorTop = 0,
                AnchorRight = 1,
                AnchorBottom = 1,
                AnchorLeft = 0,
                MarginTop = 0,
                MarginRight = 0,
                MarginBottom = 0,
                MarginLeft = 0,
                Name = "ColorRect",
                Material = GD.Load<ShaderMaterial>("res://addons/OpenVRCS/Utils/Materials/OVRToasterDisplay.material.tres"),
            };

            var margin1 = new MarginContainer
            {
                AnchorTop = 0,
                AnchorRight = 0,
                AnchorBottom = 1,
                AnchorLeft = 0,
                MarginTop = 12,
                MarginRight = 188,
                MarginBottom = -12,
                MarginLeft = 12,
                Name = "Margin 1",
            };

            var icon = new TextureRect
            {
                Texture = GD.Load<Texture>("res://addons/OpenVRCS/godot.png"),
                Expand = true,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                Name = "Icon",
            };

            _hind32 = ResourceLoader.Load<DynamicFont>("res://addons/OpenVRCS/Utils/Fonts/Hind-32.tres");
            _consola32 = ResourceLoader.Load<DynamicFont>("res://addons/OpenVRCS/Utils/Fonts/Consola-32.tres");
            _toasterText = new Label
            {
                AnchorTop = 0,
                AnchorRight = 0,
                AnchorBottom = 0,
                AnchorLeft = 0,
                MarginTop = 12,
                MarginRight = 588,
                MarginBottom = 188,
                MarginLeft = 200,
                Text = "This is a Toaster.\nA Toaster contains some useful text.",
                Align = Label.AlignEnum.Left,
                Valign = Label.VAlign.Center,
                Autowrap = true,
                ClipText = false,
                Uppercase = false,
                GrowHorizontal = Control.GrowDirection.End,
                GrowVertical = Control.GrowDirection.Both,
                SizeFlagsHorizontal = (int)Control.SizeFlags.ExpandFill,
                SizeFlagsVertical = (int)(Control.SizeFlags.Fill | Control.SizeFlags.ShrinkCenter),
                Name = "ToasterText",
            };
            _toasterText.AddFontOverride("font", _hind32);

            AddChild(_toasterMesh);
            _toasterMesh.AddChild(_toasterTween);
            _toasterMesh.AddChild(_toasterViewport);
            _toasterViewport.AddChild(control);
            control.AddChild(colorRect);
            colorRect.AddChild(margin1);
            margin1.AddChild(icon);
            colorRect.AddChild(_toasterText);
        }

        public override void _Ready()
        {
            base._Ready();
            var material = _toasterMesh.GetSurfaceMaterial(0) as ShaderMaterial;
            if (material == null || !material.Shader.HasParam("alpha") || !material.Shader.HasParam("albedo"))
                return;

            if (!Engine.EditorHint)
            {
                _toasterMesh.Visible = false;
                material.SetShaderParam("alpha", 0.0f);
            }

            MaxAlpha = MaxAlpha;

            material.SetShaderParam("albedo", _toasterViewport.GetTexture());
            _toasterViewport.RenderTargetUpdateMode = Viewport.UpdateMode.Always;
            _toasterState = OVRToasterState.Hidden;
        }

        public override void _ExitTree()
        {
            if (_toasterMesh != null)
            {
                if (_toasterMesh.IsInsideTree())
                {
                    RemoveChild(_toasterMesh);
                    _toasterMesh.QueueFree();
                }
                else
                    _toasterMesh.Free();
                _toasterMesh = null;
            }
            base._ExitTree();
        }
        #endregion

        #region Public Node specific Methods
        public void Toast(string text, float viewDuration = 3.0f, bool useMonospaceFont = false)
        {
            var toasterMaterial = _toasterMesh.GetSurfaceMaterial(0) as ShaderMaterial;

            if (toasterMaterial == null || !toasterMaterial.Shader.HasParam("alpha"))
                return;

            var currentAlpha = Mathf.Clamp((float)toasterMaterial.GetShaderParam("alpha"), 0.0f, 1.0f);
            _toasterViewport.RenderTargetUpdateMode = Viewport.UpdateMode.Always;
            _toasterMesh.Visible = true;

            if (useMonospaceFont)
                _toasterText.AddFontOverride("font", _consola32);
            else
                _toasterText.AddFontOverride("font", _hind32);


            if (_toasterState != OVRToasterState.Visible)
            {
                _toasterTween.StopAll();
                _toasterTween.InterpolateProperty(
                    toasterMaterial,
                    "shader_param/alpha",
                    currentAlpha,
                    1.0f,
                    _maxAlpha - currentAlpha,
                    Tween.TransitionType.Quad,
                    Tween.EaseType.InOut,
                    0.0f
                );
                _toasterTween.Start();
            }
        }
        #endregion
    }

}
#endif
