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
    private  const float VerticalOffset = 40f; //TODO: needed due to the UI issue, revert if CO fixes the panel
    
    private bool _initialized;
    private ushort _cachedBuildingID;
    private ItemClass _cachedItemClass;
    private Building.Flags _cachedflags;
    private ItemClass _cachedItemVehicleClass;
    private CityServiceWorldInfoPanel _cityServiceWorldInfoPanel;
    private UIPanel _prefabPanel;
    private UIPanel _prefabPanel2;
    private UILabel _headerLabel;
    private UILabel _headerLabel2;
    private VehicleListBox _vehicleListBox;
    private VehicleListBox _vehicleListBox2;

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
        bool canSelectVehicle;
        ushort buildingId = Utils.GetPrivate<InstanceID>((object) this._cityServiceWorldInfoPanel, "m_InstanceID").Building;
        Building[] buffer = Singleton<BuildingManager>.instance.m_buildings.m_buffer;
        var building = buffer[(int) buildingId];
        var buildingInfo = building.Info;
        ItemClass itemClass = buildingInfo.m_class;
        
        if (itemClass.m_service == ItemClass.Service.PublicTransport &&
             ((itemClass.m_level == ItemClass.Level.Level4 && (itemClass.m_subService == ItemClass.SubService.PublicTransportTrain || itemClass.m_subService == ItemClass.SubService.PublicTransportPlane || itemClass.m_subService == ItemClass.SubService.PublicTransportShip)) ||
             itemClass.m_level == ItemClass.Level.Level5 && itemClass.m_subService == ItemClass.SubService.PublicTransportShip))
        {
          canSelectVehicle = true;
          if ((Object) this._cachedItemClass != (Object) itemClass)
          {
            _prefabPanel.relativePosition = new Vector3(_prefabPanel.parent.width + 1f, VerticalOffset);
            _headerLabel.text = "Vehicle types";
            this.PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, itemClass.m_level, VehicleInfo.VehicleType.None);
            this._cachedItemClass = itemClass;
          }
        }
        else if (buildingInfo.m_buildingAI is PrivateAirportAI ||
                 itemClass.m_service == ItemClass.Service.HealthCare && !(buildingInfo.m_buildingAI is SaunaAI) && !(buildingInfo.m_buildingAI.GetType().Name.Equals("NursingHomeAI")) ||
                 itemClass.m_service == ItemClass.Service.FireDepartment && !(buildingInfo.m_buildingAI is FirewatchTowerAI || buildingInfo.m_buildingAI is HelicopterDepotAI) ||
                 itemClass.m_service == ItemClass.Service.Garbage && !buildingInfo.m_isFloating ||
                 itemClass.m_service == ItemClass.Service.PoliceDepartment ||
                 itemClass.m_service == ItemClass.Service.Road && !(buildingInfo.m_buildingAI is TollBoothAI)|| 
                 itemClass.m_subService == ItemClass.SubService.PublicTransportTaxi && buildingInfo.m_buildingAI is DepotAI ||
                 itemClass.m_subService == ItemClass.SubService.PublicTransportPost ||
                 itemClass.m_subService == ItemClass.SubService.PublicTransportCableCar && buildingInfo.m_buildingAI is CableCarStationAI)
        {
            canSelectVehicle = true;
            if (this._cachedItemClass != itemClass)
            {
                _prefabPanel.relativePosition = new Vector3(_prefabPanel.parent.width + 1f, VerticalOffset);
                _headerLabel.text = "Vehicle types";
                this.PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, itemClass.m_level, VehicleInfo.VehicleType.None);
                this._cachedItemClass = itemClass;
            }
        }
        else if (itemClass.m_service == ItemClass.Service.PublicTransport &&
                 ((itemClass.m_level == ItemClass.Level.Level1 &&
                 (itemClass.m_subService == ItemClass.SubService.PublicTransportTrain ||
                  itemClass.m_subService == ItemClass.SubService.PublicTransportPlane ||
                  (itemClass.m_subService == ItemClass.SubService.PublicTransportShip && buildingInfo?.m_buildingAI is HarborAI harborAI && harborAI?.m_transportInfo?.m_netLayer == ItemClass.Layer.Default)))
                  || (itemClass.m_level == ItemClass.Level.Level3 && itemClass.m_subService == ItemClass.SubService.PublicTransportBus)))
        {
          canSelectVehicle = true;
          if ((Object) this._cachedItemClass != (Object) itemClass)
          {
            if (itemClass.m_subService == ItemClass.SubService.PublicTransportTrain)
            {          
              _prefabPanel.relativePosition = new Vector3(_prefabPanel.parent.width + 181f, VerticalOffset);
            }
            else
            {
              _prefabPanel.relativePosition = new Vector3(_prefabPanel.parent.width + 1f, VerticalOffset);
            }
            _headerLabel.text = "Intercity types";
            this.PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, itemClass.m_level, VehicleInfo.VehicleType.None);
            this._cachedItemClass = itemClass;
          }
        }       
        else if (itemClass.m_service == ItemClass.Service.FireDepartment && buildingInfo.m_buildingAI is HelicopterDepotAI || itemClass.m_service == ItemClass.Service.Disaster && buildingInfo.m_buildingAI is DisasterResponseBuildingAI)
        {
          canSelectVehicle = true;
          if ((Object) this._cachedItemClass != (Object) itemClass)
          {
            _prefabPanel.relativePosition = new Vector3(_prefabPanel.parent.width + 1f, VerticalOffset);
            _headerLabel.text = "Helicopter types";
            this.PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, itemClass.m_level, VehicleInfo.VehicleType.Helicopter);
            this._cachedItemClass = itemClass;
          }
        }
        else if (itemClass.m_service == ItemClass.Service.Fishing && buildingInfo.m_buildingAI is FishingHarborAI)
        {
          canSelectVehicle = true;
          var fishingHarborAi = ((FishingHarborAI)buildingInfo.m_buildingAI);
          var itemVehicleClass = fishingHarborAi.m_boatClass;
          if ((Object) this._cachedItemClass != (Object) itemClass || this._cachedItemVehicleClass != itemVehicleClass)
          {
            _prefabPanel.relativePosition = new Vector3(_prefabPanel.parent.width + 1f, VerticalOffset);
            _headerLabel.text = "Boat types";
            this.PopulateVehicleListBox(
              itemVehicleClass.m_service, 
              itemVehicleClass.m_subService, 
              itemVehicleClass.m_level, 
              VehicleInfo.VehicleType.Ship);
            this._cachedItemClass = itemClass;
            this._cachedItemVehicleClass = itemVehicleClass;
          }
        }
        else
        {
          canSelectVehicle = false;
        }     
       
        if (canSelectVehicle)
        {
          if ((int) this._cachedBuildingID != (int) buildingId)
          {
            HashSet<string> stringSet;
            if (ServiceVehicleSelectorMod.BuildingData.TryGetValue(buildingId, out stringSet) && stringSet.Count > 0)
              this._vehicleListBox.SelectedItems = stringSet;
            else
              this._vehicleListBox.SetSelectionStateToAll(false);
          }
          this._prefabPanel.Show();
        }
        else
          this._prefabPanel.Hide();
        this._cachedBuildingID = buildingId;
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
        UIPanel uiPanel, uiPanel2;
        uiPanel = this._cityServiceWorldInfoPanel.component.AddUIComponent<UIPanel>();
                
      
        //TODO: fix if CO fixes the issues
        var parentHeight = 285f; //uiPanel.parent.height; broken due to autoformat
      
        uiPanel.name = "SvsVehicleTypes";
        uiPanel.AlignTo(uiPanel.parent, UIAlignAnchor.TopRight);
        uiPanel.relativePosition = new Vector3(uiPanel.parent.width + 1f, VerticalOffset);
        uiPanel.width = 180f;
        uiPanel.height = parentHeight - 16f;
      
        uiPanel.backgroundSprite = "UnlockingPanel2";
        uiPanel.opacity = 0.95f;
        this._prefabPanel = uiPanel;
        UILabel uiLabel = uiPanel.AddUIComponent<UILabel>();
        uiLabel.text = "Select types";
        uiLabel.textAlignment = UIHorizontalAlignment.Center;
        uiLabel.font = UIHelper.Font;
        uiLabel.position = new Vector3((float) ((double) uiPanel.width / 2.0 - (double) uiLabel.width / 2.0), (float) ((double) uiLabel.height / 2.0 - 20.0));
        this._headerLabel = uiLabel;
        VehicleListBox vehicleListBox = VehicleListBox.Create((UIComponent) uiPanel);
        vehicleListBox.name = "VehicleListBox";
        vehicleListBox.AlignTo((UIComponent) uiPanel, UIAlignAnchor.TopLeft);
        vehicleListBox.relativePosition = new Vector3(3f, 40f);
        vehicleListBox.width = uiPanel.width - 6f;
        vehicleListBox.height = parentHeight - 61f;
        vehicleListBox.Font = UIHelper.Font;
        vehicleListBox.eventSelectedItemsChanged += new PropertyChangedEventHandler<HashSet<string>>(this.OnSelectedPrefabsChanged);
        this._vehicleListBox = vehicleListBox;

        ushort building_index = Utils.GetPrivate<InstanceID>((object) this._cityServiceWorldInfoPanel, "m_InstanceID").Building;
        Building building= BuildingManager.instance.m_buildings.m_buffer[building_index];
        BuildingInfo info = building.Info;
        if (info == null)
        {
            return;
        }
        if(info.GetAI() is PostOfficeAI || 
            ((info.GetAI() is HelicopterDepotAI || info.m_buildingAI.GetType().Name.Equals("NewPoliceStationAI")) && (building.m_flags & Building.Flags.Downgrading) != 0))
        {
            uiPanel2 = this._cityServiceWorldInfoPanel.component.AddUIComponent<UIPanel>();
            uiPanel2.name = "SvsVehicleTypes";
            uiPanel2.AlignTo(uiPanel, UIAlignAnchor.TopRight);
            uiPanel2.relativePosition = new Vector3(uiPanel.width + 1f, VerticalOffset);
            uiPanel2.width = 180f;
            uiPanel2.height = parentHeight - 16f;

            uiPanel2.backgroundSprite = "UnlockingPanel2";
            uiPanel2.opacity = 0.95f;
            this._prefabPanel = uiPanel2;
            UILabel uiLabel2 = uiPanel2.AddUIComponent<UILabel>();
            uiLabel2.text = "Select types";
            uiLabel2.textAlignment = UIHorizontalAlignment.Center;
            uiLabel2.font = UIHelper.Font;
            uiLabel2.position = new Vector3((float) ((double) uiPanel2.width / 2.0 - (double) uiLabel2.width / 2.0), (float) ((double) uiLabel2.height / 2.0 - 20.0));
            this._headerLabel2 = uiLabel2;
            VehicleListBox vehicleListBox2 = VehicleListBox.Create((UIComponent) uiPanel2);
            vehicleListBox2.name = "VehicleListBox";
            vehicleListBox2.AlignTo((UIComponent) uiPanel2, UIAlignAnchor.TopLeft);
            vehicleListBox2.relativePosition = new Vector3(3f, 40f);
            vehicleListBox2.width = uiPanel2.width - 6f;
            vehicleListBox2.height = parentHeight - 61f;
            vehicleListBox2.Font = UIHelper.Font;
            vehicleListBox2.eventSelectedItemsChanged += new PropertyChangedEventHandler<HashSet<string>>(this.OnSelectedPrefabsChanged);
            this._vehicleListBox2 = vehicleListBox2;
        }
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
      BuildingInfo info = BuildingManager.instance.m_buildings.m_buffer[building].Info;
      if (info == null)
      {
        return;
      }
      TransportStationAI transportStationAi = info.m_buildingAI as TransportStationAI;
      if (transportStationAi == null)
      {
        return;
      }
      ReleaseVehicles(transportStationAi, building, ref BuildingManager.instance.m_buildings.m_buffer[building]);
    }

    private static void ReleaseVehicles(TransportStationAI ai, ushort buildingID, ref Building data)
    {
      //TODO this should be replaced by the patch to call TransportStationAI.ReleaseVehicles
      Debug.Log("ReleaseVehicles");
    }

    private void PopulateVehicleListBox(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, VehicleInfo.VehicleType vehicleType, Building building = default(Building))
    {
        if(service == ItemClass.Service.PoliceDepartment)
        {
            if ((building.m_flags & Building.Flags.Downgrading) != 0)
            {
                this._vehicleListBox2.ClearItems();
                if(building.Info.GetAI() is HelicopterDepotAI) // heli depot with prison helis
                {

                }
                if (building.Info.m_buildingAI.GetType().Name.Equals("NewPoliceStationAI")) // police station with prison van fleet
                {

                }
            }
        }
        if(service == ItemClass.Service.PublicTransport && subService == ItemClass.SubService.PublicTransportPost && level == ItemClass.Level.Level2) // post offices
        {
            this._vehicleListBox2.ClearItems();
        }

        this._vehicleListBox.ClearItems(); 
        foreach (var prefabData in VehiclePrefabs.instance.GetPrefabs(service, subService, level, vehicleType, building))
        {
            this._vehicleListBox.AddItem(prefabData);
        }
    }
  }
}
