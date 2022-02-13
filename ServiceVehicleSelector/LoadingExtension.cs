using CitiesHarmony.API;
using ColossalFramework.UI;
using ICities;
using ServiceVehicleSelector2.HarmonyPatches;
using ServiceVehicleSelector2.HarmonyPatches.PlayerBuildingAIPatch;
using ServiceVehicleSelector2.HarmonyPatches.ServiceBuildingVehicleSelectorPatch;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ServiceVehicleSelector2
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private LoadMode _loadMode;
        private GameObject _gameObject;
        
        public override void OnLevelLoaded(LoadMode mode)
        {
            if (!HarmonyHelper.IsHarmonyInstalled)
            {
                return;
            }
            _loadMode = mode;
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame && _loadMode != LoadMode.NewGameFromScenario)
                return;

            var objectOfType = Object.FindObjectOfType<UIView>();
            if (objectOfType != null)
            {
                _gameObject = new GameObject("PanelExtenderCityService")
                {
                    transform =
                    {
                        parent = objectOfType.transform
                    }
                };
                _gameObject.AddComponent<PanelExtenderCityService>();
            }

            VehiclePrefabs.Init();

            PanelExtenderCityServicePatch.Apply(); //needed for reverse redirect
            ServiceBuildingAIPatch.Apply();
            CargoTruckAIChangeVehicleTypePatch.Apply();
            GetVehicleInfoPatch.Apply();
            GetSelectedVehiclePatch.Apply();

            SimulationManager.instance.AddAction(SerializableDataExtension.ValidateBuildingData);
        }

        public override void OnLevelUnloading()
        {
            if (!HarmonyHelper.IsHarmonyInstalled)
            {
                return;
            }
            if (_loadMode != LoadMode.LoadGame && _loadMode != LoadMode.NewGame &&
                _loadMode != LoadMode.NewGameFromScenario)
                return;

            ServiceBuildingAIPatch.Undo();
            CargoTruckAIChangeVehicleTypePatch.Undo();
            PanelExtenderCityServicePatch.Undo();
            GetVehicleInfoPatch.Undo();
            GetSelectedVehiclePatch.Undo();
            VehiclePrefabs.Deinit();
 
            if (!(_gameObject != null))
                return;
            Object.Destroy(_gameObject);
        }
    }
}