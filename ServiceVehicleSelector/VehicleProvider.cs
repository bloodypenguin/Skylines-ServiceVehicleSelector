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
      var prefabData = Array.Find(VehiclePrefabs.instance.GetPrefab(service, subService, level, vehicleType), item => item.ObjectName == prefabName);
      if (prefabData != null)
        return PrefabCollection<VehicleInfo>.GetPrefab((uint) prefabData.PrefabDataIndex);
      Utils.LogWarning((object) ("Unknown prefab: " + prefabName));
      return Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref randomizer, service, subService, level);
    }
  }
}
