// Decompiled with JetBrains decompiler
// Type: ServiceVehicleSelector.ServiceVehicleSelectorMod
// Assembly: ServiceVehicleSelector, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D0EBD243-0D3C-4ED4-95A5-A73C88972683
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\519691655\ServiceVehicleSelector.dll

using ColossalFramework;
using ColossalFramework.UI;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harmony;
using ServiceVehicleSelector2.HarmonyPatches;
using UnityEngine;

namespace ServiceVehicleSelector2
{
    public class ServiceVehicleSelectorMod : IUserMod, ILoadingExtension
    {
        private static readonly string _version = "4.0.3";
        private static readonly string _dataID = "CTS_BuildingData";
        private static readonly string _dataVersion = "v001";
        public static Dictionary<ushort, HashSet<string>> BuildingData;
        private LoadMode _loadMode;
        private GameObject _gameObject;
        private static bool _isImprovedPublicTransportPresent;
        private HarmonyInstance HarmonyInstance;

        public string Name => "Service Vehicle Selector 2 (r" + ServiceVehicleSelectorMod._version + ")";

        public string Description => "Control the vehicle types a service building can spawn. ";

        public void OnCreated(ILoading loading)
        {
            HarmonyInstance = HarmonyInstance.Create("github.com/bloodypenguin/Skylines-ServiceVehicleSelector");
        }

        public void OnLevelLoaded(LoadMode mode)
        {
            this._loadMode = mode;
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame && this._loadMode != LoadMode.NewGameFromScenario)
                return;
            try
            {
                _isImprovedPublicTransportPresent = Utils.IsModActive("Improved Public Transport");
            }
            catch
            {
                _isImprovedPublicTransportPresent = false;
            }

            UIView objectOfType = UnityEngine.Object.FindObjectOfType<UIView>();
            if ((UnityEngine.Object) objectOfType != (UnityEngine.Object) null)
            {
                this._gameObject = new GameObject("PanelExtenderCityService");
                this._gameObject.transform.parent = objectOfType.transform;
                this._gameObject.AddComponent<PanelExtenderCityService>();
            }

            VehiclePrefabs.Init();
            if (!ServiceVehicleSelectorMod.TryLoadData(out ServiceVehicleSelectorMod.BuildingData))
                Utils.Log((object) "Loading default building data.");
            Transpile(typeof(CargoTruckAI), nameof(CargoTruckAI.ChangeVehicleType),
                CargoTruckAIPatch.GetTranspiler(), true);
            Transpile(typeof(DepotAI), nameof(DepotAI.StartTransfer),
                ServiceBuildingAIPatch.GetTranspiler());
            Transpile(typeof(TransportStationAI), "CreateOutgoingVehicle",
                ServiceBuildingAIPatch.GetTranspiler());
            Transpile(typeof(TransportStationAI), "CreateIncomingVehicle",
                ServiceBuildingAIPatch.GetTranspiler());
            Transpile(typeof(PrivateAirportAI), "CheckVehicles",
                ServiceBuildingAIPatch.GetTranspiler());
            Transpile(typeof(PanelExtenderCityService), "OnSelectedPrefabsChanged",
                PanelExtenderCityServicePatch.GetTranspiler()); //needed for reverse redirect
            Transpile(typeof(PostOfficeAI), nameof(PostOfficeAI.StartTransfer),
                ServiceBuildingAIPatch.GetTranspiler());
            Transpile(typeof(CableCarStationAI), "CreateVehicle",
                ServiceBuildingAIPatch.GetTranspiler());
            Transpile(typeof(LandfillSiteAI), nameof(LandfillSiteAI.StartTransfer),
                ServiceBuildingAIPatch.GetTranspiler());
            Transpile(typeof(CemeteryAI), nameof(CemeteryAI.StartTransfer), ServiceBuildingAIPatch.GetTranspiler());
            Transpile(typeof(PoliceStationAI), nameof(PoliceStationAI.StartTransfer),
                ServiceBuildingAIPatch.GetTranspiler());
            Transpile(typeof(HospitalAI), nameof(HospitalAI.StartTransfer), ServiceBuildingAIPatch.GetTranspiler());
            Transpile(typeof(SnowDumpAI), nameof(SnowDumpAI.StartTransfer), ServiceBuildingAIPatch.GetTranspiler());
            Transpile(typeof(MaintenanceDepotAI), nameof(MaintenanceDepotAI.StartTransfer),
                ServiceBuildingAIPatch.GetTranspiler());
            Transpile(typeof(FireStationAI), nameof(FireStationAI.StartTransfer),
                ServiceBuildingAIPatch.GetTranspiler());
            Transpile(typeof(HelicopterDepotAI), nameof(HelicopterDepotAI.StartTransfer),
                ServiceBuildingAIPatch.GetTranspiler());
            Transpile(typeof(DisasterResponseBuildingAI), nameof(DisasterResponseBuildingAI.StartTransfer),
                ServiceBuildingAIPatch.GetTranspiler());
            Transpile(typeof(FishingHarborAI), nameof(FishingHarborAI.TrySpawnBoat),
                ServiceBuildingAIPatch.GetTranspiler());

            SerializableDataExtension.instance.EventSaveData +=
                new SerializableDataExtension.SaveDataEventHandler(ServiceVehicleSelectorMod.OnSaveData);
            SerializableDataExtension.instance.Loaded = true;
        }

        public void OnLevelUnloading()
        {
            _isImprovedPublicTransportPresent = false;
            if (this._loadMode != LoadMode.LoadGame && this._loadMode != LoadMode.NewGame &&
                this._loadMode != LoadMode.NewGameFromScenario)
                return;
            ServiceVehicleSelectorMod.BuildingData.Clear();
            ServiceVehicleSelectorMod.BuildingData = (Dictionary<ushort, HashSet<string>>) null;
            HarmonyInstance?.UnpatchAll();
            VehiclePrefabs.Deinit();
            SerializableDataExtension.instance.EventSaveData -=
                new SerializableDataExtension.SaveDataEventHandler(ServiceVehicleSelectorMod.OnSaveData);
            SerializableDataExtension.instance.Loaded = false;
            if (!((UnityEngine.Object) this._gameObject != (UnityEngine.Object) null))
                return;
            UnityEngine.Object.Destroy((UnityEngine.Object) this._gameObject);
        }

        private void Transpile(Type type, string methodName, MethodInfo transpiler, bool staticMethod = false)
        {
            try
            {
                var bindingFlags = BindingFlags.NonPublic | BindingFlags.Public;
                if (staticMethod)
                {
                    bindingFlags |= BindingFlags.Static;
                }
                else
                {
                    bindingFlags |= BindingFlags.Instance;
                }

                HarmonyInstance.Patch(type.GetMethod(methodName,
                        bindingFlags),
                    transpiler: new HarmonyMethod(transpiler));
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Service Vehicle Selector 2: Failed to transpile method " + methodName);
                UnityEngine.Debug.LogException(e);
            }
        }


        public void OnReleased()
        {
        }

        private static bool TryLoadData(out Dictionary<ushort, HashSet<string>> data)
        {
            data = new Dictionary<ushort, HashSet<string>>();
            byte[] data1 =
                SerializableDataExtension.instance.SerializableData.LoadData(ServiceVehicleSelectorMod._dataID);
            if (data1 == null)
                return false;
            int index1 = 0;
            string empty = string.Empty;
            try
            {
                Utils.Log((object) "Try to load building data.");
                string str = SerializableDataExtension.ReadString(data1, ref index1);
                if (string.IsNullOrEmpty(str) || str.Length != 4)
                {
                    Utils.LogWarning((object) "Unknown data found.");
                    return false;
                }

                Utils.Log((object) ("Found building data version: " + str));
                while (index1 < data1.Length)
                {
                    HashSet<string> stringSet = new HashSet<string>();
                    ushort key = SerializableDataExtension.ReadUInt16(data1, ref index1);
                    int num = SerializableDataExtension.ReadInt32(data1, ref index1);
                    for (int index2 = 0; index2 < num; ++index2)
                    {
                        string name = SerializableDataExtension.ReadString(data1, ref index1);
                        if ((UnityEngine.Object) PrefabCollection<VehicleInfo>.FindLoaded(name) !=
                            (UnityEngine.Object) null)
                            stringSet.Add(name);
                    }

                    data.Add(key, stringSet);
                }

                return true;
            }
            catch (Exception ex)
            {
                Utils.LogWarning((object) ("Could not load building data. " + ex.Message));
                data = new Dictionary<ushort, HashSet<string>>();
                return false;
            }
        }

        private static void OnSaveData()
        {
            FastList<byte> data = new FastList<byte>();
            HashSet<ushort> ushortSet = new HashSet<ushort>();
            try
            {
                SerializableDataExtension.WriteString(ServiceVehicleSelectorMod._dataVersion, data);
                foreach (KeyValuePair<ushort, HashSet<string>> keyValuePair in ServiceVehicleSelectorMod.BuildingData)
                {
                    if (!ServiceVehicleSelectorMod.IsStationValid(keyValuePair.Key) || keyValuePair.Value.Count == 0)
                    {
                        ushortSet.Add(keyValuePair.Key);
                    }
                    else
                    {
                        SerializableDataExtension.WriteUInt16(keyValuePair.Key, data);
                        SerializableDataExtension.WriteStringArray(keyValuePair.Value.ToArray<string>(), data);
                    }
                }

                SerializableDataExtension.instance.SerializableData.SaveData(ServiceVehicleSelectorMod._dataID,
                    data.ToArray());
            }
            catch (Exception ex)
            {
                string msg = "Error while saving building data! " + ex.Message + " " + (object) ex.InnerException;
                Utils.LogError((object) msg);
                CODebugBase<LogChannel>.Log(LogChannel.Modding, msg, ErrorLevel.Error);
                return;
            }

            foreach (ushort key in ushortSet)
                ServiceVehicleSelectorMod.BuildingData.Remove(key);
        }

        private static bool IsStationValid(ushort buildingID)
        {
            Building building = Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) buildingID];
            return !((UnityEngine.Object) building.Info == (UnityEngine.Object) null) &&
                   (building.m_flags & Building.Flags.Created) != Building.Flags.None;
        }
    }
}