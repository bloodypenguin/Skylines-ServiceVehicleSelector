// Decompiled with JetBrains decompiler
// Type: ServiceVehicleSelector.PrefabData
// Assembly: ServiceVehicleSelector, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D0EBD243-0D3C-4ED4-95A5-A73C88972683
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\519691655\ServiceVehicleSelector.dll

using System.Globalization;
using ColossalFramework.Globalization;
using System.Text.RegularExpressions;

namespace ServiceVehicleSelector2
{
  public class PrefabData
  {
    private VehicleInfo _info;

    public int PrefabDataIndex => this._info.m_prefabDataIndex;

    public string PrefabName => this._info.name;

    public string DisplayName
    {
      get
      {
        var m_displayName = Locale.GetUnchecked("VEHICLE_TITLE", this._info.name);
        if (m_displayName.StartsWith("VEHICLE_TITLE"))
        {
          m_displayName = this._info.name.Substring(this._info.name.IndexOf('.') + 1).Replace("_Data", "");
        }
        m_displayName = CleanName(m_displayName, !this._info.name.Contains("."));

        return m_displayName;
      }
    }
    
    public static string CleanName(string name, bool cleanNumbers = false)
    {
      name = Regex.Replace(name, @"^{{.*?}}\.", "");
      name = Regex.Replace(name, @"[_+\.]", " ");
      name = Regex.Replace(name, @"(\d[xX]\d)|([HL]\d)", "");
      if (cleanNumbers)
      {
        name = Regex.Replace(name, @"(\d+[\da-z])", "");
        name = Regex.Replace(name, @"\s\d+", " ");
      }
      name = Regex.Replace(name, @"\s+", " ").Trim();

      return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
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
