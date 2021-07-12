using Godot;

namespace OpenVRCS.Utils.Scripts
{

    public class OVRConfig : Reference
    {

        private Reference _openVRConfig;

        public OVRApplicationType ApplicationType
        {
            get => (OVRApplicationType)_openVRConfig.Call("get_application_type");
            set => _openVRConfig.Call("set_application_type", value);
        }

        public OVRTrackingUniverse TrackingUniverse
        {
            get => (OVRTrackingUniverse)_openVRConfig.Call("get_tracking_universe");
            set => _openVRConfig.Call("set_tracking_universe", value);
        }

        public string DefaultActionSet
        {
            get => (string)_openVRConfig.Call("get_default_action_set");
            set => _openVRConfig.Call("set_default_action_set", value);
        }

        public bool PlayAreaAvailable
        {
            get => (bool)_openVRConfig.Call("play_area_available");
        }

        public OVRConfig()
        {
            if (Engine.EditorHint) return;

            _openVRConfig = ResourceLoader.Load<NativeScript>("res://addons/OpenVRCS/OpenVRConfig.gdns").New() as Reference;
            if (_openVRConfig == null) throw new System.IO.FileNotFoundException("Native script file not found:", "res://addons/OpenVRCS/OpenVRConfig.gdns");
        }

        public void RegisterActionSet(string actionSet)
        {
            _openVRConfig.Call("register_action_set", actionSet);
        }

        public void SetActiveActionSet(string actionSet)
        {
            _openVRConfig.Call("set_active_action_set", actionSet);
        }

        public void ToggleActionSetActive(string actionSet, bool isActive)
        {
            _openVRConfig.Call("toggle_action_set_active", actionSet, isActive);
        }

        public bool IsActionSetActive(string actionSet)
        {
            return (bool)_openVRConfig.Call("is_action_set_active", actionSet);
        }

        public Vector3[] GetPlayArea()
        {
            return (Vector3[])_openVRConfig.Call("get_play_area");
        }

        // float get_device_battery_percentage(vr::TrackedDeviceIndex_t p_tracked_device_index);
        // bool is_device_charging(vr::TrackedDeviceIndex_t p_tracked_device_index);
    }

}
