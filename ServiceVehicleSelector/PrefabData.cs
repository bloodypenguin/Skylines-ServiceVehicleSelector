// Decompiled with JetBrains decompiler
// Type: ServiceVehicleSelector.PrefabData
// Assembly: ServiceVehicleSelector, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D0EBD243-0D3C-4ED4-95A5-A73C88972683
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\519691655\ServiceVehicleSelector.dll

namespace ServiceVehicleSelector
{
  public class PrefabData
  {
    private VehicleInfo _info;

    public int PrefabDataIndex
    {
      get
      {
        return this._info.m_prefabDataIndex;
      }
    }

    public string ObjectName
    {
      get
      {
        return this._info.name;
      }
    }

    public string Title
    {
      get
      {
        return ColossalFramework.Globalization.Locale.Get("VEHICLE_TITLE", PrefabCollection<VehicleInfo>.PrefabName((uint) this.PrefabDataIndex));
      }
    }

    public ItemClass.SubService SubService
    {
      get
      {
        return this._info.m_class.m_subService;
      }
    }

    public PrefabData(VehicleInfo prefab)
    {
      this._info = prefab;
    }
  }
}
