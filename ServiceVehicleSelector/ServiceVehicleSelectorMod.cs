using CitiesHarmony.API;
using ICities;

namespace ServiceVehicleSelector2
{
    public class ServiceVehicleSelectorMod : LoadingExtensionBase, IUserMod
    {
        private const string _version = "5.4.0";

        public string Name => "Service Vehicle Selector 2 (r" + _version + ")";

        public string Description => "Control the vehicle types a service building can spawn. ";
        
        public void OnEnabled() {
            HarmonyHelper.EnsureHarmonyInstalled();
        }
    }
}