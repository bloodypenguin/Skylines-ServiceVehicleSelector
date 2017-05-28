using System.Collections.Generic;
using UnityEngine;

namespace ServiceVehicleSelector2
{
    public class VehiclePrefabs
    {
        public static VehiclePrefabs instance;
        private PrefabData[] _deathCarePrefabData;
        private PrefabData[] _fireDepartmentPrefabData;
        private PrefabData[] _garbagePrefabData;
        private PrefabData[] _healthCarePrefabData;
        private PrefabData[] _policeDepartmentPrefabData;
        private PrefabData[] _prisonPrefabData;
        private PrefabData[] _taxiPrefabData;
        private PrefabData[] _trainPrefabData;
        private PrefabData[] _roadPrefabData;
        private PrefabData[] _roadSnowPrefabData;
        private PrefabData[] _cableCarPrefabData;
        private PrefabData[] _passengerPlanePrefabData;
        private PrefabData[] _passengerShipPrefabData;
        private PrefabData[] _passengerTrainPrefabData;

        public static void Init()
        {
            VehiclePrefabs.instance = new VehiclePrefabs();
            VehiclePrefabs.instance.FindAllPrefabs();
        }

        public static void Deinit()
        {
            VehiclePrefabs.instance = (VehiclePrefabs) null;
        }

        public PrefabData[] GetPrefab(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
        {
            if (subService == ItemClass.SubService.PublicTransportCableCar)
                return this._cableCarPrefabData;
            if (subService == ItemClass.SubService.PublicTransportTrain)
            {
                if (level == ItemClass.Level.Level1)
                {
                    return this._passengerTrainPrefabData;
                }
                else
                {
                    return this._trainPrefabData;
                }
            }
            if (subService == ItemClass.SubService.PublicTransportTaxi)
                return this._taxiPrefabData;
            if (service == ItemClass.Service.FireDepartment)
                return this._fireDepartmentPrefabData;
            if (service == ItemClass.Service.Garbage)
                return this._garbagePrefabData;
            if (service == ItemClass.Service.HealthCare)
            {
                if (level == ItemClass.Level.Level1)
                    return this._healthCarePrefabData;
                return this._deathCarePrefabData;
            }
            if (service == ItemClass.Service.PoliceDepartment)
            {
                if (level == ItemClass.Level.Level1)
                    return this._policeDepartmentPrefabData;
                return this._prisonPrefabData;
            }
            if (service != ItemClass.Service.Road)
                return (PrefabData[]) null;
            if (level == ItemClass.Level.Level2)
                return this._roadPrefabData;
            return this._roadSnowPrefabData;
        }

        private void FindAllPrefabs()
        {
            List<PrefabData> prefabDataList1 = new List<PrefabData>();
            List<PrefabData> prefabDataList2 = new List<PrefabData>();
            List<PrefabData> prefabDataList3 = new List<PrefabData>();
            List<PrefabData> prefabDataList4 = new List<PrefabData>();
            List<PrefabData> prefabDataList5 = new List<PrefabData>();
            List<PrefabData> prefabDataList6 = new List<PrefabData>();
            List<PrefabData> prefabDataList7 = new List<PrefabData>();
            List<PrefabData> prefabDataList8 = new List<PrefabData>();
            List<PrefabData> prefabDataList9 = new List<PrefabData>();
            List<PrefabData> prefabDataList10 = new List<PrefabData>();
            List<PrefabData> prefabDataList11 = new List<PrefabData>();
            List<PrefabData> prefabDataList12 = new List<PrefabData>();
            for (int index = 0; index < PrefabCollection<VehicleInfo>.PrefabCount(); ++index)
            {
                VehicleInfo prefab = PrefabCollection<VehicleInfo>.GetPrefab((uint) index);
                if ((Object) prefab != (Object) null && !VehiclePrefabs.IsTrailer(prefab))
                {
                    if (prefab.m_class.m_service == ItemClass.Service.PublicTransport)
                    {
                        if (prefab.m_class.m_subService == ItemClass.SubService.PublicTransportTrain)
                        {
                            if (prefab.m_class.m_level == ItemClass.Level.Level4)
                            {
                                prefabDataList2.Add(new PrefabData(prefab));
                            } else if (prefab.m_class.m_level == ItemClass.Level.Level1)
                            {
                                prefabDataList12.Add(new PrefabData(prefab));
                            }
                        }
                        else if (prefab.m_class.m_subService == ItemClass.SubService.PublicTransportTaxi)
                            prefabDataList1.Add(new PrefabData(prefab));
                        else if (prefab.m_class.m_subService == ItemClass.SubService.PublicTransportCableCar)
                            prefabDataList11.Add(new PrefabData(prefab));
                    }
                    else if (prefab.m_class.m_service == ItemClass.Service.FireDepartment)
                        prefabDataList5.Add(new PrefabData(prefab));
                    else if (prefab.m_class.m_service == ItemClass.Service.Garbage)
                        prefabDataList4.Add(new PrefabData(prefab));
                    else if (prefab.m_class.m_service == ItemClass.Service.HealthCare)
                    {
                        if (prefab.m_class.m_level == ItemClass.Level.Level1)
                            prefabDataList6.Add(new PrefabData(prefab));
                        else
                            prefabDataList3.Add(new PrefabData(prefab));
                    }
                    else if (prefab.m_class.m_service == ItemClass.Service.PoliceDepartment)
                    {
                        if (prefab.m_class.m_level == ItemClass.Level.Level1)
                            prefabDataList7.Add(new PrefabData(prefab));
                        else
                            prefabDataList8.Add(new PrefabData(prefab));
                    }
                    else if (prefab.m_class.m_service == ItemClass.Service.Road)
                    {
                        if (prefab.m_class.m_level == ItemClass.Level.Level2)
                            prefabDataList9.Add(new PrefabData(prefab));
                        else
                            prefabDataList10.Add(new PrefabData(prefab));
                    }
                }
            }
            this._taxiPrefabData = prefabDataList1.ToArray();
            this._trainPrefabData = prefabDataList2.ToArray();
            this._deathCarePrefabData = prefabDataList3.ToArray();
            this._fireDepartmentPrefabData = prefabDataList5.ToArray();
            this._garbagePrefabData = prefabDataList4.ToArray();
            this._healthCarePrefabData = prefabDataList6.ToArray();
            this._policeDepartmentPrefabData = prefabDataList7.ToArray();
            this._prisonPrefabData = prefabDataList8.ToArray();
            this._roadPrefabData = prefabDataList9.ToArray();
            this._roadSnowPrefabData = prefabDataList10.ToArray();
            this._cableCarPrefabData = prefabDataList11.ToArray();
            this._passengerTrainPrefabData = prefabDataList12.ToArray();
        }

        private static bool IsTrailer(VehicleInfo prefab)
        {
            string str = ColossalFramework.Globalization.Locale.GetUnchecked("VEHICLE_TITLE", prefab.name);
            if (!str.StartsWith("VEHICLE_TITLE"))
                return str.StartsWith("Trailer");
            return true;
        }
    }
}