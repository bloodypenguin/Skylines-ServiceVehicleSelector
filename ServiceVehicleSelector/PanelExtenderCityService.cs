using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace ServiceVehicleSelector2
{
  public class PanelExtenderCityService : MonoBehaviour
  {
    private  const float VerticalOffset = 40f; //TODO: needed due to the UI issue, revert if CO fixes the panel
    
    private bool _initialized;
    private ushort _cachedBuildingID;
    private ItemClass _cachedItemClass;
    private ItemClass _cachedItemVehicleClass;
    private CityServiceWorldInfoPanel _cityServiceWorldInfoPanel;
    private UIPanel _prefabPanel;
    private UILabel _headerLabel;
    private VehicleListBox _vehicleListBox;

    private void Update()
    {
      if (!_initialized)
      {
        _cityServiceWorldInfoPanel = GameObject.Find("(Library) CityServiceWorldInfoPanel").GetComponent<CityServiceWorldInfoPanel>();
        if (!(_cityServiceWorldInfoPanel != null))
          return;
        CreatePrefabPanel();
        _initialized = true;
      }
      else
      {
        if (!_initialized || !_cityServiceWorldInfoPanel.component.isVisible)
          return;
        bool canSelectVehicle;
        var buildingId = Utils.GetPrivate<InstanceID>(_cityServiceWorldInfoPanel, "m_InstanceID").Building;
        var buffer = Singleton<BuildingManager>.instance.m_buildings.m_buffer;
        var building = buffer[buildingId];
        var buildingInfo = building.Info;
        var itemClass = buildingInfo.m_class;
        
        if (itemClass.m_service == ItemClass.Service.PublicTransport &&
             (itemClass.m_level == ItemClass.Level.Level4 && itemClass.m_subService is ItemClass.SubService.PublicTransportTrain or ItemClass.SubService.PublicTransportPlane or ItemClass.SubService.PublicTransportShip ||
             itemClass.m_level == ItemClass.Level.Level5 && itemClass.m_subService == ItemClass.SubService.PublicTransportShip)) //the last condition is for barges
        {
          canSelectVehicle = true;
          if (_cachedItemClass != itemClass)
          {
            _prefabPanel.relativePosition = new Vector3(_prefabPanel.parent.width + 1f, VerticalOffset);
            _headerLabel.text = "Vehicle types";
            PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, itemClass.m_level, VehicleInfo.VehicleType.None);
            _cachedItemClass = itemClass;
          }
        }
        else if (buildingInfo.m_buildingAI is PrivateAirportAI ||
                 itemClass.m_service == ItemClass.Service.HealthCare && buildingInfo.m_buildingAI is HospitalAI or HelicopterDepotAI or CemeteryAI ||
                 itemClass.m_service == ItemClass.Service.FireDepartment && buildingInfo.m_buildingAI is not (FirewatchTowerAI or HelicopterDepotAI) ||
                 itemClass.m_service == ItemClass.Service.Garbage && !buildingInfo.m_isFloating ||
                 itemClass.m_service == ItemClass.Service.PoliceDepartment ||
                 itemClass.m_service == ItemClass.Service.Road && buildingInfo.m_buildingAI is not TollBoothAI|| 
                 itemClass.m_subService == ItemClass.SubService.PublicTransportTaxi && buildingInfo.m_buildingAI is DepotAI ||
                 itemClass.m_subService == ItemClass.SubService.PublicTransportPost ||
                 itemClass.m_subService == ItemClass.SubService.PublicTransportCableCar && buildingInfo.m_buildingAI is CableCarStationAI)
        {
          canSelectVehicle = true;
          if (_cachedItemClass != itemClass)
          {
            _prefabPanel.relativePosition = new Vector3(_prefabPanel.parent.width + 1f, VerticalOffset);
            _headerLabel.text = "Vehicle types";
            PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, itemClass.m_level, VehicleInfo.VehicleType.None);
            _cachedItemClass = itemClass;
          }
        }
        else if (itemClass.m_service == ItemClass.Service.PublicTransport &&
                 (itemClass.m_level == ItemClass.Level.Level1 &&
                  (itemClass.m_subService == ItemClass.SubService.PublicTransportTrain ||
                   itemClass.m_subService == ItemClass.SubService.PublicTransportShip && buildingInfo?.m_buildingAI is HarborAI harborAI && harborAI?.m_transportInfo?.m_netLayer == ItemClass.Layer.Default)
                  || itemClass.m_level == ItemClass.Level.Level3 && itemClass.m_subService == ItemClass.SubService.PublicTransportBus 
                  || itemClass.m_subService == ItemClass.SubService.PublicTransportPlane && itemClass.m_level is ItemClass.Level.Level1 or ItemClass.Level.Level2 or ItemClass.Level.Level3 && buildingInfo.m_buildingAI is DepotAI depotAi && depotAi.m_transportInfo?.m_vehicleType == VehicleInfo.VehicleType.Plane))
        {
          canSelectVehicle = true;
          if (_cachedItemClass != itemClass)
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

            var buildingAi = buildingInfo.m_buildingAI as TransportStationAI;
            PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, itemClass.m_level, buildingAi.m_transportInfo.m_vehicleType);
            _cachedItemClass = itemClass;
          }
        }       
        else if (itemClass.m_service == ItemClass.Service.FireDepartment && buildingInfo.m_buildingAI is HelicopterDepotAI || itemClass.m_service == ItemClass.Service.Disaster && buildingInfo.m_buildingAI is DisasterResponseBuildingAI)
        {
          canSelectVehicle = true;
          if (_cachedItemClass != itemClass)
          {
            _prefabPanel.relativePosition = new Vector3(_prefabPanel.parent.width + 1f, VerticalOffset);
            _headerLabel.text = "Helicopter types";
            PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, itemClass.m_level, VehicleInfo.VehicleType.Helicopter);
            _cachedItemClass = itemClass;
          }
        }
        else if (itemClass.m_service == ItemClass.Service.Fishing && buildingInfo.m_buildingAI is FishingHarborAI)
        {
          canSelectVehicle = true;
          var fishingHarborAi = ((FishingHarborAI)buildingInfo.m_buildingAI);
          var itemVehicleClass = fishingHarborAi.m_boatClass;
          if (_cachedItemClass != itemClass || _cachedItemVehicleClass != itemVehicleClass)
          {
            _prefabPanel.relativePosition = new Vector3(_prefabPanel.parent.width + 1f, VerticalOffset);
            _headerLabel.text = "Boat types";
            PopulateVehicleListBox(
              itemVehicleClass.m_service, 
              itemVehicleClass.m_subService, 
              itemVehicleClass.m_level, 
              VehicleInfo.VehicleType.Ship);
            _cachedItemClass = itemClass;
            _cachedItemVehicleClass = itemVehicleClass;
          }
        }
        else
        {
          canSelectVehicle = false;
        }     
       
        if (canSelectVehicle)
        {
          if (_cachedBuildingID != buildingId)
          {
            HashSet<string> stringSet;
            if (ServiceVehicleSelectorMod.BuildingData.TryGetValue(buildingId, out stringSet) && stringSet.Count > 0)
              _vehicleListBox.SelectedItems = stringSet;
            else
              _vehicleListBox.SetSelectionStateToAll(false);
          }
          _prefabPanel.Show();
        }
        else
          _prefabPanel.Hide();
        _cachedBuildingID = buildingId;
      }
    }

    private void OnDestroy()
    {
      if (!(_prefabPanel != null))
        return;
      Destroy(_prefabPanel.gameObject);
    }

    private void CreatePrefabPanel()
    {
      UIPanel uiPanel = _cityServiceWorldInfoPanel.component.AddUIComponent<UIPanel>();
      
      //TODO: fix if CO fixes the issues
      var parentHeight = 285f; //uiPanel.parent.height; broken due to autoformat
      
      uiPanel.name = "SvsVehicleTypes";
      uiPanel.AlignTo(uiPanel.parent, UIAlignAnchor.TopRight);
      uiPanel.relativePosition = new Vector3(uiPanel.parent.width + 1f, VerticalOffset);
      uiPanel.width = 180f;
      uiPanel.height = parentHeight - 16f;
      
      uiPanel.backgroundSprite = "UnlockingPanel2";
      uiPanel.opacity = 0.95f;
      _prefabPanel = uiPanel;
      var uiLabel = uiPanel.AddUIComponent<UILabel>();
      uiLabel.text = "Select types";
      uiLabel.textAlignment = UIHorizontalAlignment.Center;
      uiLabel.font = UIHelper.Font;
      uiLabel.position = new Vector3((float) (uiPanel.width / 2.0 - uiLabel.width / 2.0), (float) (uiLabel.height / 2.0 - 20.0));
      _headerLabel = uiLabel;
      var vehicleListBox = VehicleListBox.Create(uiPanel);
      vehicleListBox.name = "VehicleListBox";
      vehicleListBox.AlignTo(uiPanel, UIAlignAnchor.TopLeft);
      vehicleListBox.relativePosition = new Vector3(3f, 40f);
      vehicleListBox.width = uiPanel.width - 6f;
      vehicleListBox.height = parentHeight - 61f;
      vehicleListBox.Font = UIHelper.Font;
      vehicleListBox.eventSelectedItemsChanged += OnSelectedPrefabsChanged;
      _vehicleListBox = vehicleListBox;
    }

    private void OnSelectedPrefabsChanged(UIComponent component, HashSet<string> selectedItems)
    {
      var building = Utils.GetPrivate<InstanceID>(_cityServiceWorldInfoPanel, "m_InstanceID").Building;
      if (building == 0)
        return;
      HashSet<string> stringSet;
      if (!ServiceVehicleSelectorMod.BuildingData.TryGetValue(building, out stringSet))
        ServiceVehicleSelectorMod.BuildingData.Add(building, selectedItems);
      else
        ServiceVehicleSelectorMod.BuildingData[building] = selectedItems;
      var info = BuildingManager.instance.m_buildings.m_buffer[building].Info;
      if (info == null)
      {
        return;
      }
      var transportStationAi = info.m_buildingAI as TransportStationAI;
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

    private void PopulateVehicleListBox(ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level, VehicleInfo.VehicleType vehicleType)
    {
      _vehicleListBox.ClearItems();
      foreach (var prefabData in VehiclePrefabs.instance.GetPrefabs(service, subService, level, vehicleType))
      {
        _vehicleListBox.AddItem(prefabData);
      }
    }
  }
}
