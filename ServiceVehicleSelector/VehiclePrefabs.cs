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
        private PrefabData[] _shipPrefabData;
        private PrefabData[] _roadPrefabData;
        private PrefabData[] _roadSnowPrefabData;
        private PrefabData[] _cableCarPrefabData;
        private PrefabData[] _passengerPlanePrefabData;
        private PrefabData[] _passengerShipPrefabData;
        private PrefabData[] _passengerTrainPrefabData;
        
        private PrefabData[] _postVanPrefabData;
        private PrefabData[] _cargoPlanePrefabData;
        private PrefabData[] _postTruckPrefabData;
        
        private PrefabData[] _medicalHelicopterPrefabData;
        private PrefabData[] _policeHelicopterPrefabData;
        private PrefabData[] _fireHelicopterPrefabData;
        private PrefabData[] _disasterResponseHelicopterPrefabData;
        private PrefabData[] _disasterResponseTruckPrefabData;
        
        //TODO: pumping trucks, park trucks, evacuation buses
        

        public static void Init()
        {
            VehiclePrefabs.instance = new VehiclePrefabs();
            VehiclePrefabs.instance.FindAllPrefabs();
        }

        public static void Deinit()
        {
            VehiclePrefabs.instance = (VehiclePrefabs) null;
        }

        public PrefabData[] GetPrefab(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, VehicleInfo.VehicleType vehicleType)
        {
            if (subService == ItemClass.SubService.PublicTransportPost)
            {
                if (level == ItemClass.Level.Level2)
                {
                    return this._postVanPrefabData;
                } else if (level == ItemClass.Level.Level5)
                {
                    return this._postTruckPrefabData;
                }
            }
            
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

            if (subService == ItemClass.SubService.PublicTransportPlane)
            {
                if (level == ItemClass.Level.Level4)
                {
                    return this._cargoPlanePrefabData;
                }
                if (level == ItemClass.Level.Level1)
                {
                    return this._passengerPlanePrefabData;
                }
            }
            if (subService == ItemClass.SubService.PublicTransportShip)
            {
                if (level == ItemClass.Level.Level4)
                {
                    return this._shipPrefabData;
                }        
                if (level == ItemClass.Level.Level1)
                {
                    return this._passengerShipPrefabData;
                }
            }
            if (subService == ItemClass.SubService.PublicTransportPost)
            {
                if (level == ItemClass.Level.Level2)
                {
                    return this._postVanPrefabData;
                }        
                if (level == ItemClass.Level.Level5)
                {
                    return this._postTruckPrefabData;
                }
            }
            
            if (subService == ItemClass.SubService.PublicTransportTaxi)
                return this._taxiPrefabData;
            if (service == ItemClass.Service.FireDepartment)
            {
                if (vehicleType == VehicleInfo.VehicleType.Helicopter)
                {
                    return this._fireHelicopterPrefabData;  
                }
                else
                {
                    return this._fireDepartmentPrefabData;
                }
            }

            if (service == ItemClass.Service.Garbage)
                return this._garbagePrefabData;
            if (service == ItemClass.Service.HealthCare)
            {
                if (level == ItemClass.Level.Level1)
                {
                    return this._healthCarePrefabData;
                }
                else if (level == ItemClass.Level.Level2)
                {
                    return this._deathCarePrefabData;   
                }
                else if (level == ItemClass.Level.Level3)
                {
                    return this._medicalHelicopterPrefabData;   
                }
            }
            if (service == ItemClass.Service.PoliceDepartment)
            {
                if (level == ItemClass.Level.Level1)
                {
                    return this._policeDepartmentPrefabData;
                }

                if (level == ItemClass.Level.Level4)
                {
                    return this._prisonPrefabData;
                }
                if (level == ItemClass.Level.Level3)
                {
                    return this._policeHelicopterPrefabData;
                }
            }

            if (service == ItemClass.Service.Disaster)
            {
                if (level == ItemClass.Level.Level2)
                {
                    if (vehicleType == VehicleInfo.VehicleType.Helicopter)
                    {
                        return this._disasterResponseHelicopterPrefabData;
                    }
                    else
                    {
                        return this._disasterResponseTruckPrefabData;
                    }
                } 
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
            List<PrefabData> prefabDataList13 = new List<PrefabData>();           
            List<PrefabData> prefabDataList14 = new List<PrefabData>();
            List<PrefabData> prefabDataList15 = new List<PrefabData>();
            
            List<PrefabData> prefabDataList16 = new List<PrefabData>(); //Natural Disasters DLC
            List<PrefabData> prefabDataList17 = new List<PrefabData>();
            List<PrefabData> prefabDataList18 = new List<PrefabData>();
            List<PrefabData> prefabDataList19 = new List<PrefabData>();
            List<PrefabData> prefabDataList20 = new List<PrefabData>();
            List<PrefabData> prefabDataList21 = new List<PrefabData>(); //unused: pumping truck
            List<PrefabData> prefabDataList22 = new List<PrefabData>(); //unused: evac bus
            
            List<PrefabData> prefabDataList23 = new List<PrefabData>(); //unused: parklife truck
            
            List<PrefabData> prefabDataList24 = new List<PrefabData>(); //Industry DLC
            List<PrefabData> prefabDataList25 = new List<PrefabData>();
            List<PrefabData> prefabDataList26 = new List<PrefabData>();
            //TODO more Industry DLC + parklife truck
            
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
                        else if (prefab.m_class.m_subService == ItemClass.SubService.PublicTransportPlane)
                        {
                            if (prefab.m_class.m_level == ItemClass.Level.Level4)
                            {
                                prefabDataList24.Add(new PrefabData(prefab));
                            } else if (prefab.m_class.m_level == ItemClass.Level.Level1)
                            {
                                prefabDataList14.Add(new PrefabData(prefab));
                            }
                        } 
                        else if (prefab.m_class.m_subService == ItemClass.SubService.PublicTransportShip)
                        {
                            if (prefab.m_class.m_level == ItemClass.Level.Level4)
                            {
                                prefabDataList13.Add(new PrefabData(prefab));
                            } else if (prefab.m_class.m_level == ItemClass.Level.Level1)
                            {
                                prefabDataList15.Add(new PrefabData(prefab));
                            }
                        }
                        else if (prefab.m_class.m_subService == ItemClass.SubService.PublicTransportPost)
                        {
                            if (prefab.m_class.m_level == ItemClass.Level.Level2)
                            {
                                prefabDataList25.Add(new PrefabData(prefab));
                            } else if (prefab.m_class.m_level == ItemClass.Level.Level5)
                            {
                                prefabDataList26.Add(new PrefabData(prefab));
                            }
                        }
                    }
                    else if (prefab.m_class.m_service == ItemClass.Service.FireDepartment)
                    {
                        if (prefab.m_vehicleType == VehicleInfo.VehicleType.Helicopter)
                        {
                            prefabDataList18.Add(new PrefabData(prefab));     
                        }
                        else
                        {
                            prefabDataList5.Add(new PrefabData(prefab));                            
                        }
                    }
                    else if (prefab.m_class.m_service == ItemClass.Service.Garbage)
                        prefabDataList4.Add(new PrefabData(prefab));
                    else if (prefab.m_class.m_service == ItemClass.Service.HealthCare)
                    {
                        if (prefab.m_class.m_level == ItemClass.Level.Level1)
                            prefabDataList6.Add(new PrefabData(prefab));
                        else if (prefab.m_class.m_level == ItemClass.Level.Level2)
                            prefabDataList3.Add(new PrefabData(prefab));
                        else if (prefab.m_class.m_level == ItemClass.Level.Level3)
                            prefabDataList16.Add(new PrefabData(prefab));
                    }
                    else if (prefab.m_class.m_service == ItemClass.Service.PoliceDepartment)
                    {
                        if (prefab.m_class.m_level == ItemClass.Level.Level1)
                            prefabDataList7.Add(new PrefabData(prefab));
                        else if (prefab.m_class.m_level == ItemClass.Level.Level4)
                            prefabDataList8.Add(new PrefabData(prefab));
                        else if (prefab.m_class.m_level == ItemClass.Level.Level3)
                            prefabDataList17.Add(new PrefabData(prefab));
                    }
                    else if (prefab.m_class.m_service == ItemClass.Service.Road)
                    {
                        if (prefab.m_class.m_level == ItemClass.Level.Level2)
                            prefabDataList9.Add(new PrefabData(prefab));
                        else
                            prefabDataList10.Add(new PrefabData(prefab));
                    }
                    else if (prefab.m_class.m_service == ItemClass.Service.Disaster)
                    {
                        if (prefab.m_class.m_level == ItemClass.Level.Level2)
                        {
                            if (prefab.m_vehicleType == VehicleInfo.VehicleType.Helicopter)
                            {
                                prefabDataList19.Add(new PrefabData(prefab));
                            }
                            else
                            {
                                prefabDataList20.Add(new PrefabData(prefab));  
                            }
                        }
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
            this._shipPrefabData = prefabDataList13.ToArray();
            this._passengerPlanePrefabData = prefabDataList14.ToArray();
            this._passengerShipPrefabData = prefabDataList15.ToArray();
            this._medicalHelicopterPrefabData = prefabDataList16.ToArray();
            this._policeHelicopterPrefabData = prefabDataList17.ToArray();
            this._fireHelicopterPrefabData = prefabDataList18.ToArray();
            this._disasterResponseHelicopterPrefabData = prefabDataList19.ToArray();
            this._disasterResponseTruckPrefabData = prefabDataList20.ToArray();
            
            this._cargoPlanePrefabData = prefabDataList24.ToArray();
            this._postVanPrefabData = prefabDataList25.ToArray();
            this._postTruckPrefabData = prefabDataList26.ToArray();
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