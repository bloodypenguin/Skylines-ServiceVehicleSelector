using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace ServiceVehicleSelector2
{
    public class VehiclePrefabs
    {

        private static readonly List<PrefabData> EmptyList = new();
        
        public static VehiclePrefabs instance;
        private List<PrefabData> _deathCarePrefabData;
        private List<PrefabData> _fireDepartmentPrefabData;
        private List<PrefabData> _garbagePrefabData;
        private List<PrefabData> _healthCarePrefabData;
        private List<PrefabData> _policeDepartmentPrefabData;
        private List<PrefabData> _prisonPrefabData;
        private List<PrefabData> _taxiPrefabData;
        private List<PrefabData> _trainPrefabData;
        private List<PrefabData> _shipPrefabData;
        private List<PrefabData> _bargePrefabData;
        private List<PrefabData> _roadPrefabData;
        private List<PrefabData> _roadSnowPrefabData;
        private List<PrefabData> _cableCarPrefabData;
        private List<PrefabData> _passengerPlanePrefabData;
        private List<PrefabData> _passengerPlanePrefabDataLarge;
        private List<PrefabData> _passengerPlanePrefabDataSmall;
        private List<PrefabData> _passengerShipPrefabData;
        private List<PrefabData> _passengerTrainPrefabData;
        
        private List<PrefabData> _postVanPrefabData;
        private List<PrefabData> _cargoPlanePrefabData;
        private List<PrefabData> _postTruckPrefabData;
        
        private List<PrefabData> _medicalHelicopterPrefabData;
        private List<PrefabData> _policeHelicopterPrefabData;
        private List<PrefabData> _fireHelicopterPrefabData;
        private List<PrefabData> _disasterResponseHelicopterPrefabData;
        private List<PrefabData> _disasterResponseTruckPrefabData;

        private List<PrefabData> _intercityBusData;
        private List<PrefabData> _fishingBoatData;
        private List<PrefabData> _fishingBoat1Data;
        private List<PrefabData> _fishingBoat2Data;
        private List<PrefabData> _fishingBoat3Data;
        private List<PrefabData> _fishingBoat4Data;
        private List<PrefabData> _fishingBoat5Data;

        private List<PrefabData> _privatePlanesData;
        
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

        public List<PrefabData> GetPrefabs(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, VehicleInfo.VehicleType vehicleType)
        {
            if (service == ItemClass.Service.Monument && level == ItemClass.Level.Level5) //TODO: make a better check in the future
            {
                return _privatePlanesData;
            }
            
            if (service == ItemClass.Service.Fishing && vehicleType == VehicleInfo.VehicleType.Ship)
            {
                switch (level)
                {
                    case ItemClass.Level.None:
                        return this._fishingBoatData;
                    case ItemClass.Level.Level1:
                        return this._fishingBoat1Data;
                    case ItemClass.Level.Level2:
                        return this._fishingBoat2Data;
                    case ItemClass.Level.Level3:
                        return this._fishingBoat3Data;
                    case ItemClass.Level.Level4:
                        return this._fishingBoat4Data;
                    case ItemClass.Level.Level5:
                        return this._fishingBoat5Data;
                }
            }
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
            
            if (subService == ItemClass.SubService.PublicTransportBus)
            {
                if (level == ItemClass.Level.Level3)
                {
                    return this._intercityBusData;
                }
            }

            if (subService == ItemClass.SubService.PublicTransportPlane && vehicleType == VehicleInfo.VehicleType.Plane)
            {
                if (level == ItemClass.Level.Level1)
                {
                    return this._passengerPlanePrefabData;
                }
                if (level == ItemClass.Level.Level2)
                {
                    return this._passengerPlanePrefabDataLarge;
                }
                if (level == ItemClass.Level.Level3)
                {
                    return this._passengerPlanePrefabDataSmall;
                }
                if (level == ItemClass.Level.Level4)
                {
                    return this._cargoPlanePrefabData;
                }
            }
            if (subService == ItemClass.SubService.PublicTransportShip)
            {
                if (level == ItemClass.Level.Level4)
                {
                    return this._shipPrefabData;
                }      
                if (level == ItemClass.Level.Level5)
                {
                    return this._bargePrefabData;
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

            if (service == ItemClass.Service.Road)
            {
                if (level == ItemClass.Level.Level2)
                    return this._roadPrefabData;
                return this._roadSnowPrefabData;
            }
            
            return EmptyList;
        }

        private void FindAllPrefabs()
        {
            _taxiPrefabData = new List<PrefabData>();
            _trainPrefabData = new List<PrefabData>();
            _deathCarePrefabData = new List<PrefabData>();
            _garbagePrefabData = new List<PrefabData>();
            _fireDepartmentPrefabData = new List<PrefabData>();
            _healthCarePrefabData = new List<PrefabData>();
            _policeDepartmentPrefabData = new List<PrefabData>();
            _prisonPrefabData = new List<PrefabData>();
            _roadPrefabData = new List<PrefabData>();
            _roadSnowPrefabData = new List<PrefabData>();
            _cableCarPrefabData = new List<PrefabData>();
            _passengerTrainPrefabData = new List<PrefabData>();
            _shipPrefabData = new List<PrefabData>();   
            _bargePrefabData = new List<PrefabData>();
            _passengerPlanePrefabData = new List<PrefabData>();
            _passengerPlanePrefabDataLarge = new List<PrefabData>();
            _passengerPlanePrefabDataSmall = new List<PrefabData>();
            _passengerShipPrefabData = new List<PrefabData>();
            
            _medicalHelicopterPrefabData = new List<PrefabData>(); //Natural Disasters DLC
            _policeHelicopterPrefabData = new List<PrefabData>();
            _fireHelicopterPrefabData = new List<PrefabData>();
            _disasterResponseHelicopterPrefabData = new List<PrefabData>();
            _disasterResponseTruckPrefabData = new List<PrefabData>();
            //unused: pumping truck
            //unused: evac bus
            
            //unused: parklife truck
            
            _cargoPlanePrefabData = new List<PrefabData>(); //Industry DLC
            _postVanPrefabData = new List<PrefabData>();
            _postTruckPrefabData = new List<PrefabData>();
            
            _intercityBusData = new List<PrefabData>(); //Sunset Harbor DLC
            _fishingBoatData = new List<PrefabData>();
            _fishingBoat1Data = new List<PrefabData>();
            _fishingBoat2Data = new List<PrefabData>();
            _fishingBoat3Data = new List<PrefabData>();
            _fishingBoat4Data = new List<PrefabData>();
            _fishingBoat5Data = new List<PrefabData>();
            _privatePlanesData = new List<PrefabData>();
            
            //TODO more Industry DLC + parklife truck
            
            for (int index = 0; index < PrefabCollection<VehicleInfo>.PrefabCount(); ++index)
            {
                VehicleInfo prefab = PrefabCollection<VehicleInfo>.GetPrefab((uint) index);
                if ((Object) prefab != (Object) null && prefab.m_placementStyle != ItemClass.Placement.Procedural)
                {
                    if (prefab.m_vehicleAI is PrivatePlaneAI)
                    {
                        _privatePlanesData.Add(new PrefabData(prefab));
                    }
                    
                    if (prefab.m_class.m_service == ItemClass.Service.PublicTransport)
                    {
                        if (prefab.m_class.m_subService == ItemClass.SubService.PublicTransportTrain)
                        {
                            if (prefab.m_class.m_level == ItemClass.Level.Level4)
                            {
                                _trainPrefabData.Add(new PrefabData(prefab));
                            } else if (prefab.m_class.m_level == ItemClass.Level.Level1)
                            {
                                _passengerTrainPrefabData.Add(new PrefabData(prefab));
                            }
                        }
                        else if (prefab.m_class.m_subService == ItemClass.SubService.PublicTransportBus)
                        {
                            if (prefab.m_class.m_level == ItemClass.Level.Level3)
                            {
                                _intercityBusData.Add(new PrefabData(prefab));
                            }
                        }
                        else if (prefab.m_class.m_subService == ItemClass.SubService.PublicTransportTaxi)
                            _taxiPrefabData.Add(new PrefabData(prefab));
                        else if (prefab.m_class.m_subService == ItemClass.SubService.PublicTransportCableCar)
                            _cableCarPrefabData.Add(new PrefabData(prefab));
                        else if (prefab.m_class.m_subService == ItemClass.SubService.PublicTransportPlane && prefab.m_vehicleType == VehicleInfo.VehicleType.Plane)
                        {
                            if (prefab.m_class.m_level == ItemClass.Level.Level1)
                            {
                                _passengerPlanePrefabData.Add(new PrefabData(prefab));
                            } else if (prefab.m_class.m_level == ItemClass.Level.Level2)
                            {
                                _passengerPlanePrefabDataLarge.Add(new PrefabData(prefab));
                            } else if (prefab.m_class.m_level == ItemClass.Level.Level3)
                            {
                                _passengerPlanePrefabDataSmall.Add(new PrefabData(prefab));
                            } else  if (prefab.m_class.m_level == ItemClass.Level.Level4)
                            {
                                _cargoPlanePrefabData.Add(new PrefabData(prefab));
                            } 
                        } 
                        else if (prefab.m_class.m_subService == ItemClass.SubService.PublicTransportShip)
                        {
                            if (prefab.m_class.m_level == ItemClass.Level.Level4)
                            {
                                _shipPrefabData.Add(new PrefabData(prefab));
                            } else if (prefab.m_class.m_level == ItemClass.Level.Level1)
                            {
                                _passengerShipPrefabData.Add(new PrefabData(prefab));
                            } else if (prefab.m_class.m_level == ItemClass.Level.Level5)
                            {
                                _bargePrefabData.Add(new PrefabData(prefab));
                            }
                        }
                        else if (prefab.m_class.m_subService == ItemClass.SubService.PublicTransportPost)
                        {
                            if (prefab.m_class.m_level == ItemClass.Level.Level2)
                            {
                                _postVanPrefabData.Add(new PrefabData(prefab));
                            } else if (prefab.m_class.m_level == ItemClass.Level.Level5)
                            {
                                _postTruckPrefabData.Add(new PrefabData(prefab));
                            }
                        }
                    }
                    else if (prefab.m_class.m_service == ItemClass.Service.FireDepartment)
                    {
                        if (prefab.m_vehicleType == VehicleInfo.VehicleType.Helicopter)
                        {
                            _fireHelicopterPrefabData.Add(new PrefabData(prefab));     
                        }
                        else
                        {
                            _fireDepartmentPrefabData.Add(new PrefabData(prefab));                            
                        }
                    }
                    else if (prefab.m_class.m_service == ItemClass.Service.Garbage)
                    {
                        _garbagePrefabData.Add(new PrefabData(prefab));              
                    }
                    else if (prefab.m_class.m_service == ItemClass.Service.Fishing && prefab.m_vehicleType == VehicleInfo.VehicleType.Ship)
                    {
                        switch (prefab.m_class.m_level)
                        {
                            case ItemClass.Level.None:
                                _fishingBoatData.Add(new PrefabData(prefab));
                                break;
                            case ItemClass.Level.Level1:
                                _fishingBoat1Data.Add(new PrefabData(prefab));
                                break;
                            case ItemClass.Level.Level2:
                                _fishingBoat2Data.Add(new PrefabData(prefab));
                                break;
                            case ItemClass.Level.Level3:
                                _fishingBoat3Data.Add(new PrefabData(prefab));
                                break;
                            case ItemClass.Level.Level4:
                                _fishingBoat4Data.Add(new PrefabData(prefab));
                                break;
                            case ItemClass.Level.Level5:
                                _fishingBoat5Data.Add(new PrefabData(prefab));
                                break;
                        }           
                    }
                    else if (prefab.m_class.m_service == ItemClass.Service.HealthCare)
                    {
                        if (prefab.m_class.m_level == ItemClass.Level.Level1)
                            _healthCarePrefabData.Add(new PrefabData(prefab));
                        else if (prefab.m_class.m_level == ItemClass.Level.Level2)
                            _deathCarePrefabData.Add(new PrefabData(prefab));
                        else if (prefab.m_class.m_level == ItemClass.Level.Level3)
                            _medicalHelicopterPrefabData.Add(new PrefabData(prefab));
                    }
                    else if (prefab.m_class.m_service == ItemClass.Service.PoliceDepartment)
                    {
                        if (prefab.m_class.m_level == ItemClass.Level.Level1)
                            _policeDepartmentPrefabData.Add(new PrefabData(prefab));
                        else if (prefab.m_class.m_level == ItemClass.Level.Level4)
                            _prisonPrefabData.Add(new PrefabData(prefab));
                        else if (prefab.m_class.m_level == ItemClass.Level.Level3)
                            _policeHelicopterPrefabData.Add(new PrefabData(prefab));
                    }
                    else if (prefab.m_class.m_service == ItemClass.Service.Road)
                    {
                        if (prefab.m_class.m_level == ItemClass.Level.Level2)
                            _roadPrefabData.Add(new PrefabData(prefab));
                        else
                            _roadSnowPrefabData.Add(new PrefabData(prefab));
                    }
                    else if (prefab.m_class.m_service == ItemClass.Service.Disaster)
                    {
                        if (prefab.m_class.m_level == ItemClass.Level.Level2)
                        {
                            if (prefab.m_vehicleType == VehicleInfo.VehicleType.Helicopter)
                            {
                                _disasterResponseHelicopterPrefabData.Add(new PrefabData(prefab));
                            }
                            else
                            {
                                _disasterResponseTruckPrefabData.Add(new PrefabData(prefab));  
                            }
                        }
                    }
                }
            }
        }
    }
}