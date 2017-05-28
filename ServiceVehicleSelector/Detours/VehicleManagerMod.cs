using System;
using ColossalFramework;
using ColossalFramework.Math;

namespace ServiceVehicleSelector.Detours
{
  public static class VehicleManagerMod
  {
    public static VehicleInfo GetVehicleInfo(ref Randomizer randomizer, ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, ushort buildingID, string prefabName)
    {
      PrefabData prefabData = Array.Find<PrefabData>(VehiclePrefabs.instance.GetPrefab(service, subService, level), (Predicate<PrefabData>) (item => item.ObjectName == prefabName));
      if (prefabData != null)
        return PrefabCollection<VehicleInfo>.GetPrefab((uint) prefabData.PrefabDataIndex);
      Utils.LogWarning((object) ("Unknown prefab: " + prefabName));
      return Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref randomizer, service, subService, level);
    }
  }
}
