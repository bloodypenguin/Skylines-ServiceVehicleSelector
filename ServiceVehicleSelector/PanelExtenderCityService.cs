// Decompiled with JetBrains decompiler
// Type: ServiceVehicleSelector.PanelExtenderCityService
// Assembly: ServiceVehicleSelector, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D0EBD243-0D3C-4ED4-95A5-A73C88972683
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\519691655\ServiceVehicleSelector.dll

using ColossalFramework;
using ColossalFramework.UI;
using System.Collections.Generic;
using UnityEngine;

namespace ServiceVehicleSelector2
{
  public class PanelExtenderCityService : MonoBehaviour
  {
    private bool _initialized;
    private ushort _cachedBuildingID;
    private ItemClass _cachedItemClass;
    private bool _cachedIsCargoHub;
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
        bool canSelectVehicle = false;
        ushort building = Utils.GetPrivate<InstanceID>((object) this._cityServiceWorldInfoPanel, "m_InstanceID").Building;
        Building[] buffer = Singleton<BuildingManager>.instance.m_buildings.m_buffer;
        var buildingInfo = buffer[(int) building].Info;
        ItemClass itemClass = buildingInfo.m_class;
        
        if (itemClass.m_service == ItemClass.Service.PublicTransport && 
            itemClass.m_level == ItemClass.Level.Level4 &&
            (itemClass.m_subService == ItemClass.SubService.PublicTransportTrain || itemClass.m_subService == ItemClass.SubService.PublicTransportPlane || itemClass.m_subService == ItemClass.SubService.PublicTransportShip))
        {
          canSelectVehicle = true;
          var isCargoHub = IsCargoHub(buildingInfo);
          if ((Object) this._cachedItemClass != (Object) itemClass || this._cachedIsCargoHub != isCargoHub)
          {
            if (isCargoHub)
            {
              this.PopulateVehicleListBox(ItemClass.Service.PublicTransport, ItemClass.SubService.PublicTransportTrain, ItemClass.Level.Level4, VehicleInfo.VehicleType.None);  
            }
            else
            {
              this.PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, itemClass.m_level, VehicleInfo.VehicleType.None);
            }
            this._cachedItemClass = itemClass;
            this._cachedIsCargoHub = isCargoHub;
          }
        }
        else if (itemClass.m_service == ItemClass.Service.HealthCare && !(buildingInfo.m_buildingAI is SaunaAI) ||
                 itemClass.m_service == ItemClass.Service.FireDepartment && !(buildingInfo.m_buildingAI is FirewatchTowerAI || buildingInfo.m_buildingAI is HelicopterDepotAI) ||
                 itemClass.m_service == ItemClass.Service.Garbage && !buildingInfo.m_isFloating ||
                 itemClass.m_service == ItemClass.Service.PoliceDepartment ||
                 itemClass.m_service == ItemClass.Service.Road && !(buildingInfo.m_buildingAI is TollBoothAI)|| 
                 itemClass.m_subService == ItemClass.SubService.PublicTransportTaxi && buildingInfo.m_buildingAI is DepotAI ||
                 itemClass.m_subService == ItemClass.SubService.PublicTransportCableCar && buildingInfo.m_buildingAI is CableCarStationAI)
        {
          canSelectVehicle = true;
          if ((Object) this._cachedItemClass != (Object) itemClass)
          {
            this.PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, itemClass.m_level, VehicleInfo.VehicleType.None);
            this._cachedItemClass = itemClass;
          }
        }
        else if (itemClass.m_service == ItemClass.Service.FireDepartment && buildingInfo.m_buildingAI is HelicopterDepotAI || itemClass.m_service == ItemClass.Service.Disaster && buildingInfo.m_buildingAI is DisasterResponseBuildingAI)
        {
          canSelectVehicle = true;
          if ((Object) this._cachedItemClass != (Object) itemClass)
          {
            this.PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, itemClass.m_level, VehicleInfo.VehicleType.Helicopter);
            this._cachedItemClass = itemClass;
          }
        }
        
        
        if (canSelectVehicle)
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

    private static bool IsCargoHub(BuildingInfo buildingInfo)
    {
      var cargoStationAi = buildingInfo == null ? null : buildingInfo.m_buildingAI as CargoStationAI;
      if (cargoStationAi == null)
      {
        return false;
      }
      return cargoStationAi.m_transportInfo != null && cargoStationAi.m_transportInfo2 != null;
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

    private void PopulateVehicleListBox(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, VehicleInfo.VehicleType vehicleType)
    {
      this._vehicleListBox.ClearItems();
      PrefabData[] prefab = VehiclePrefabs.instance.GetPrefab(service, subService, level, vehicleType);
      int length = prefab.Length;
      for (int index = 0; index < length; ++index)
        this._vehicleListBox.AddItem(prefab[index]);
    }
  }
}
