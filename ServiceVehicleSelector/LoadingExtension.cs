using System;
using System.Collections.Generic;
using System.Linq;
using CitiesHarmony.API;
using ColossalFramework;
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
            
            SimulationManager.instance.AddAction(ValidateBuildingData);
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

        private static void ValidateBuildingData()
        {
            Utils.Log("SVS2 - Validating building data.");
            try
            {
                var keysToRemove = new HashSet<ushort>();
                var valuesToRemove = new Dictionary<ushort, HashSet<string>>();
                foreach (var keyValuePair in SerializableDataExtension.BuildingData)
                {
                    if (!IsStationValid(keyValuePair.Key))
                    {
                        keysToRemove.Add(keyValuePair.Key);
                        continue;
                    }

                    foreach (var prefabName in keyValuePair.Value.Where(prefabName =>
                                 PrefabCollection<VehicleInfo>.FindLoaded(prefabName) == null))
                    {
                        if (!valuesToRemove.ContainsKey(keyValuePair.Key))
                        {
                            valuesToRemove.Add(keyValuePair.Key, new HashSet<string>());
                        }

                        valuesToRemove[keyValuePair.Key].Add(prefabName);
                    }
                }

                foreach (var buildingId in keysToRemove)
                {
                    SerializableDataExtension.BuildingData.Remove(buildingId);
                    Utils.LogWarning($"SVS2 - Removed building {buildingId} from config as it's not valid anymore");
                }

                foreach (var keyValuePair in valuesToRemove)
                {
                    foreach (var prefabName in keyValuePair.Value)
                    {
                        SerializableDataExtension.BuildingData[keyValuePair.Key].Remove(prefabName);
                        Utils.LogWarning(
                            $"SVS2 - Removed prefab {prefabName} from building {keyValuePair.Key} config as it's not found");
                    }
                }
                
                Utils.Log("SVS2 - Building data successfully validated.");
            }
            catch (Exception ex)
            {
                Utils.LogError($"SVS2 - An error happened while validating building data.\n{ex.Message}\n{ex.StackTrace}");
            }
        }
        
        private static bool IsStationValid(ushort buildingID)
        {
            var building = Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID];
            return !(building.Info == null) &&
                   (building.m_flags & Building.Flags.Created) != Building.Flags.None;
        }
    }
}