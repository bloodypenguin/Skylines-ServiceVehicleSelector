// Decompiled with JetBrains decompiler
// Type: ServiceVehicleSelector.VehicleManagerMod
// Assembly: ServiceVehicleSelector, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D0EBD243-0D3C-4ED4-95A5-A73C88972683
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\519691655\ServiceVehicleSelector.dll

using ColossalFramework;
using ColossalFramework.Math;
using System;

namespace ServiceVehicleSelector
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
