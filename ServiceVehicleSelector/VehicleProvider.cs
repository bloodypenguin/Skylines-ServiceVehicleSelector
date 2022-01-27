using System;
using ColossalFramework;
using ColossalFramework.Math;

namespace ServiceVehicleSelector2
{
  public static class VehicleProvider
  {
    public static VehicleInfo GetVehicleInfo(ref Randomizer randomizer, ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, string prefabName)
    {
      return GetVehicleInfo(ref randomizer, service, subService, level, prefabName, VehicleInfo.VehicleType.None);
    }
    
    public static VehicleInfo GetVehicleInfo(ref Randomizer randomizer, ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, string prefabName, VehicleInfo.VehicleType vehicleType)
    {
        if(VehiclePrefabs.instance.isTwoVehicleTypes(service, subService, level))
        {
            var prefabData1 = VehiclePrefabs.instance.GetPrefabs(service, subService, level, vehicleType, 2).Find(item => item.PrefabName == prefabName);
            if(prefabData1 != null) return PrefabCollection<VehicleInfo>.GetPrefab((uint) prefabData1.PrefabDataIndex);
        }
        var prefabData = VehiclePrefabs.instance.GetPrefabs(service, subService, level, vehicleType, 1).Find(item => item.PrefabName == prefabName);
        if (prefabData != null) return PrefabCollection<VehicleInfo>.GetPrefab((uint) prefabData.PrefabDataIndex);
        Utils.LogWarning((object) ("Unknown prefab: " + prefabName));
        return Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref randomizer, service, subService, level);
    }
  }
}
