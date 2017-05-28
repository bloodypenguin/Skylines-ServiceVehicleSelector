// Decompiled with JetBrains decompiler
// Type: ServiceVehicleSelector.PanelExtenderCityService
// Assembly: ServiceVehicleSelector, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D0EBD243-0D3C-4ED4-95A5-A73C88972683
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\519691655\ServiceVehicleSelector.dll

using ColossalFramework;
using ColossalFramework.UI;
using System.Collections.Generic;
using UnityEngine;

namespace ServiceVehicleSelector
{
  public class PanelExtenderCityService : MonoBehaviour
  {
    private bool _initialized;
    private ushort _cachedBuildingID;
    private ItemClass _cachedItemClass;
    private CityServiceWorldInfoPanel _cityServiceWorldInfoPanel;
    private UIPanel _prefabPanel;
    private VehicleListBox _vehicleListBox;

    private void Update()
    {
      if (!this._initialized)
      {
        this._cityServiceWorldInfoPanel = GameObject.Find("(Library) CityServiceWorldInfoPanel").GetComponent<CityServiceWorldInfoPanel>();
        if (!((Object) this._cityServiceWorldInfoPanel != (Object) null))
          return;
        this.CreatePrefabPanel();
        this._initialized = true;
      }
      else
      {
        if (!this._initialized || !this._cityServiceWorldInfoPanel.component.isVisible)
          return;
        bool flag = false;
        ushort building = Utils.GetPrivate<InstanceID>((object) this._cityServiceWorldInfoPanel, "m_InstanceID").Building;
        Building[] buffer = Singleton<BuildingManager>.instance.m_buildings.m_buffer;
        ItemClass itemClass = buffer[(int) building].Info.m_class;
        if (itemClass.m_service == ItemClass.Service.PublicTransport && itemClass.m_level == ItemClass.Level.Level4 && (itemClass.m_subService == ItemClass.SubService.PublicTransportTrain || buffer[(int) building].Info.name.Equals("Cargo Hub")))
        {
          flag = true;
          if ((Object) this._cachedItemClass != (Object) itemClass)
          {
            this.PopulateVehicleListBox(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrain, ItemClass.Level.Level4);
            this._cachedItemClass = itemClass;
          }
        }
        else if (itemClass.m_service == ItemClass.Service.HealthCare || itemClass.m_service == ItemClass.Service.FireDepartment || (itemClass.m_service == ItemClass.Service.Garbage || itemClass.m_service == ItemClass.Service.PoliceDepartment) || (itemClass.m_service == ItemClass.Service.Road || 
                    (itemClass.m_subService == ItemClass.SubService.PublicTransportTaxi && buffer[(int) building].Info.m_buildingAI is DepotAI) ||
                    (itemClass.m_subService == ItemClass.SubService.PublicTransportCableCar && buffer[(int)building].Info.m_buildingAI is CableCarStationAI)))
        {
          flag = true;
          if ((Object) this._cachedItemClass != (Object) itemClass)
          {
            this.PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, itemClass.m_level);
            this._cachedItemClass = itemClass;
          }
        }
        if (flag)
        {
          if ((int) this._cachedBuildingID != (int) building)
          {
            HashSet<string> stringSet;
            if (ServiceVehicleSelectorMod.BuildingData.TryGetValue(building, out stringSet) && stringSet.Count > 0)
              this._vehicleListBox.SelectedItems = stringSet;
            else
              this._vehicleListBox.SetSelectionStateToAll(false);
          }
          this._prefabPanel.Show();
        }
        else
          this._prefabPanel.Hide();
        this._cachedBuildingID = building;
      }
    }

    private void OnDestroy()
    {
      if (!((Object) this._prefabPanel != (Object) null))
        return;
      Object.Destroy((Object) this._prefabPanel.gameObject);
    }

    private void CreatePrefabPanel()
    {
      UIPanel uiPanel = this._cityServiceWorldInfoPanel.component.AddUIComponent<UIPanel>();
      uiPanel.name = "SvsVehicleTypes";
      uiPanel.AlignTo(uiPanel.parent, UIAlignAnchor.TopRight);
      uiPanel.relativePosition = new Vector3(uiPanel.parent.width + 1f, 0.0f);
      uiPanel.width = 180f;
      uiPanel.height = uiPanel.parent.height - 16f;
      uiPanel.backgroundSprite = "UnlockingPanel2";
      uiPanel.opacity = 0.95f;
      this._prefabPanel = uiPanel;
      UILabel uiLabel = uiPanel.AddUIComponent<UILabel>();
      uiLabel.text = "Select types";
      uiLabel.textAlignment = UIHorizontalAlignment.Center;
      uiLabel.font = UIHelper.Font;
      uiLabel.position = new Vector3((float) ((double) uiPanel.width / 2.0 - (double) uiLabel.width / 2.0), (float) ((double) uiLabel.height / 2.0 - 20.0));
      VehicleListBox vehicleListBox = VehicleListBox.Create((UIComponent) uiPanel);
      vehicleListBox.name = "VehicleListBox";
      vehicleListBox.AlignTo((UIComponent) uiPanel, UIAlignAnchor.TopLeft);
      vehicleListBox.relativePosition = new Vector3(3f, 40f);
      vehicleListBox.width = uiPanel.width - 6f;
      vehicleListBox.height = uiPanel.parent.height - 61f;
      vehicleListBox.Font = UIHelper.Font;
      vehicleListBox.eventSelectedItemsChanged += new PropertyChangedEventHandler<HashSet<string>>(this.OnSelectedPrefabsChanged);
      this._vehicleListBox = vehicleListBox;
    }

    private void OnSelectedPrefabsChanged(UIComponent component, HashSet<string> selectedItems)
    {
      ushort building = Utils.GetPrivate<InstanceID>((object) this._cityServiceWorldInfoPanel, "m_InstanceID").Building;
      if ((int) building == 0)
        return;
      HashSet<string> stringSet;
      if (!ServiceVehicleSelectorMod.BuildingData.TryGetValue(building, out stringSet))
        ServiceVehicleSelectorMod.BuildingData.Add(building, selectedItems);
      else
        ServiceVehicleSelectorMod.BuildingData[building] = selectedItems;
    }

    private void PopulateVehicleListBox(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
    {
      this._vehicleListBox.ClearItems();
      PrefabData[] prefab = VehiclePrefabs.instance.GetPrefab(service, subService, level);
      int length = prefab.Length;
      for (int index = 0; index < length; ++index)
        this._vehicleListBox.AddItem(prefab[index]);
    }
  }
}
