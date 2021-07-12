using Godot;

namespace OpenVRCS.Utils.Scripts
{

    public class OVRRenderModel : Reference
    {

        private Mesh _ovrRenderModel;

        public string[] ModelNames
        {
            get
            {
                var array = (Godot.Collections.Array)_ovrRenderModel.Call("model_names");
                var modelNames = new string[array.Count];
                for (var i=0; i<modelNames.Length; ++i) {
                    modelNames[i] = (string) array[i];
                }
                return modelNames;
            }
        }

        public OVRRenderModel()
        {
            if (Engine.EditorHint) return;

            _ovrRenderModel = ResourceLoader.Load<NativeScript>("res://addons/OpenVRCS/OpenVRRenderModel.gdns").New() as Mesh;
            if (_ovrRenderModel == null) throw new System.IO.FileNotFoundException("Native script file not found:", "res://addons/OpenVRCS/OpenVRRenderModel.gdns");
        }

        public Mesh LoadControllerMesh(string controllerName)
        {
            var isMeshLoaded = (bool)_ovrRenderModel.Call("load_model", controllerName.Substr(0, controllerName.Length - 2));
            if (isMeshLoaded)
                return _ovrRenderModel;

            isMeshLoaded = (bool)_ovrRenderModel.Call("load_model", "generic_controller");
            if (isMeshLoaded)
                return _ovrRenderModel;

            return null;
        }
    }

}
