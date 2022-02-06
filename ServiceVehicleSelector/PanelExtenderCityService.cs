using System.Collections.Generic;
using ColossalFramework;
using ColossalFramework.UI;
using ServiceVehicleSelector2.HarmonyPatches;
using UnityEngine;

namespace ServiceVehicleSelector2
{
  public class PanelExtenderCityService : MonoBehaviour
  {
    private  const float VerticalOffset = 40f; //TODO: needed due to the UI issue, revert if CO fixes the panel
    
    private bool _initialized;
    private ushort _cachedBuildingID;
    private int _cachedIndex = -1;
    private ItemClass _cachedItemClass;
    private ItemClass _cachedItemVehicleClass;
    private ItemClass _cachedSecondaryVehicleClass; //for cargo stations
    private CityServiceWorldInfoPanel _cityServiceWorldInfoPanel;
    private UIPanel _prefabPanel;
    private UIDropDown _headerDropDown;
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
        
        if(itemClass.m_service == ItemClass.Service.Water && itemClass.m_level == ItemClass.Level.Level1 && buildingInfo.m_buildingAI is WaterFacilityAI wf && wf.m_pumpingVehicles > 0)
        {
          canSelectVehicle = true;
          if (_cachedItemClass != itemClass)
          {
            _prefabPanel.relativePosition = new Vector3(_prefabPanel.parent.width + 1f, VerticalOffset);
            _headerDropDown.items = new [] {"Truck types"};
            _headerDropDown.selectedIndex = 0;
            PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, itemClass.m_level, VehicleInfo.VehicleType.None);
            _cachedItemClass = itemClass;
          }
        } else  if (itemClass.m_service == ItemClass.Service.PublicTransport && buildingInfo.m_buildingAI is CargoStationAI &&
                    (itemClass.m_level == ItemClass.Level.Level4 && itemClass.m_subService is ItemClass.SubService.PublicTransportTrain or ItemClass.SubService.PublicTransportPlane or ItemClass.SubService.PublicTransportShip ||
                     itemClass.m_level == ItemClass.Level.Level5 && itemClass.m_subService == ItemClass.SubService.PublicTransportShip)) //the last condition is for barges
        {
          var cargoStationTransportInfos = CargoTruckAIChangeVehicleTypePatch.GetCargoStationTransportInfos(buildingInfo);
          var primaryVehicle = cargoStationTransportInfos.Primary?.m_vehicleType ?? VehicleInfo.VehicleType.None;
          var secondaryVehicle = cargoStationTransportInfos.Secondary?.m_vehicleType ?? VehicleInfo.VehicleType.None;;
          var secondaryClass = cargoStationTransportInfos.Secondary?.m_class;

          var isHub = primaryVehicle != VehicleInfo.VehicleType.None && secondaryVehicle != VehicleInfo.VehicleType.None && primaryVehicle != secondaryVehicle;
          canSelectVehicle = true;
          if (_cachedItemClass != itemClass || _cachedSecondaryVehicleClass != secondaryClass || _cachedIndex != _headerDropDown.selectedIndex)
          {
            _prefabPanel.relativePosition = new Vector3(_prefabPanel.parent.width + 1f, VerticalOffset);
            if (_cachedItemClass != itemClass || _cachedSecondaryVehicleClass != secondaryClass)
            {
              _headerDropDown.items = isHub ? new[] { $"{primaryVehicle} types", $"{secondaryVehicle} types" } : new[] { $"{primaryVehicle} types" };
              _headerDropDown.selectedIndex = 0;
            }
            if (_headerDropDown.selectedIndex == 0)
            {
              PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, itemClass.m_level,
                VehicleInfo.VehicleType.None);
            }
            else
            {
              PopulateVehicleListBox(secondaryClass.m_service, secondaryClass.m_subService, ItemClass.Level.Level4,
                VehicleInfo.VehicleType.None); //TODO: this makes custom cargo modes (using Level5) not possible for secondary
            }

            _cachedSecondaryVehicleClass = secondaryClass;
            _cachedItemClass = itemClass;
          }
        }
        else if (buildingInfo.m_buildingAI is PrivateAirportAI ||
                 itemClass.m_service == ItemClass.Service.Beautification && itemClass.m_subService == ItemClass.SubService.BeautificationParks && itemClass.m_level == ItemClass.Level.Level2 && buildingInfo.m_buildingAI is MaintenanceDepotAI ||
                 itemClass.m_service == ItemClass.Service.HealthCare && buildingInfo.m_buildingAI is HospitalAI or HelicopterDepotAI or CemeteryAI ||
                 itemClass.m_service == ItemClass.Service.FireDepartment && buildingInfo.m_buildingAI is not (FirewatchTowerAI or HelicopterDepotAI) ||
                 itemClass.m_service == ItemClass.Service.Garbage && !buildingInfo.m_isFloating ||
                 itemClass.m_service == ItemClass.Service.PoliceDepartment ||
                 itemClass.m_service == ItemClass.Service.Road && buildingInfo.m_buildingAI is not TollBoothAI|| 
                 itemClass.m_subService == ItemClass.SubService.PublicTransportTaxi && buildingInfo.m_buildingAI is DepotAI ||
                 itemClass.m_subService == ItemClass.SubService.PublicTransportTours && itemClass.m_level == ItemClass.Level.Level4 && buildingInfo.m_buildingAI is TourBuildingAI ||
                 itemClass.m_subService == ItemClass.SubService.PublicTransportCableCar && buildingInfo.m_buildingAI is CableCarStationAI)
        {
          canSelectVehicle = true;
          if (_cachedItemClass != itemClass)
          {
            _prefabPanel.relativePosition = new Vector3(_prefabPanel.parent.width + 1f, VerticalOffset);
            _headerDropDown.items = new [] {"Vehicle types"};
            _headerDropDown.selectedIndex = 0;
            PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, itemClass.m_level, VehicleInfo.VehicleType.None);
            _cachedItemClass = itemClass;
          }
        }
        else if (itemClass.m_subService == ItemClass.SubService.PublicTransportPost)
        {
          canSelectVehicle = true;
          
          
          if (_cachedItemClass != itemClass || _cachedIndex != _headerDropDown.selectedIndex)
          {
            _prefabPanel.relativePosition = new Vector3(_prefabPanel.parent.width + 1f, VerticalOffset);

            if (_cachedItemClass != itemClass)
            {
              if (itemClass.m_level == ItemClass.Level.Level5)
              {
                _headerDropDown.items = new[] { "Truck types" };
              }
              else
              {
                _headerDropDown.items = new[] { "Van types", "Truck types" };
              }
              _headerDropDown.selectedIndex = 0;
            }
            if (_headerDropDown.selectedIndex == 0)
            {
              PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, itemClass.m_level, VehicleInfo.VehicleType.None); 
            }
            else //trucks for sorting
            {
              PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, ItemClass.Level.Level5, VehicleInfo.VehicleType.None);
            }
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
            _headerDropDown.items = new [] {"Intercity types"};
            _headerDropDown.selectedIndex = 0;

            var buildingAi = buildingInfo.m_buildingAI as TransportStationAI;
            PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, itemClass.m_level, buildingAi.m_transportInfo.m_vehicleType);
            _cachedItemClass = itemClass;
          }
        }       
        else if (itemClass.m_service == ItemClass.Service.FireDepartment && buildingInfo.m_buildingAI is HelicopterDepotAI)
        {
          canSelectVehicle = true;
          if (_cachedItemClass != itemClass)
          {
            _prefabPanel.relativePosition = new Vector3(_prefabPanel.parent.width + 1f, VerticalOffset);
            _headerDropDown.items = new [] {"Helicopter types"};
            _headerDropDown.selectedIndex = 0;
            PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, itemClass.m_level, VehicleInfo.VehicleType.Helicopter);
            _cachedItemClass = itemClass;
          }
        }
        else if (itemClass.m_service == ItemClass.Service.Disaster && buildingInfo.m_buildingAI is DisasterResponseBuildingAI)
        {
          canSelectVehicle = true;
          if (_cachedItemClass != itemClass || _cachedIndex != _headerDropDown.selectedIndex)
          {
            _prefabPanel.relativePosition = new Vector3(_prefabPanel.parent.width + 1f, VerticalOffset);
            if (_cachedItemClass != itemClass)
            {
              _headerDropDown.items = new [] {"Helicopter types", "Truck types"};
              _headerDropDown.selectedIndex = 0;
            }

            if (_headerDropDown.selectedIndex == 0)
            {
              PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, itemClass.m_level, VehicleInfo.VehicleType.Helicopter); 
            }
            else
            {
              PopulateVehicleListBox(itemClass.m_service, itemClass.m_subService, itemClass.m_level, VehicleInfo.VehicleType.Car);   
            }
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
            _headerDropDown.items = new [] {"Boat types"};
            _headerDropDown.selectedIndex = 0;
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
          if (_cachedBuildingID != buildingId || _cachedIndex != _headerDropDown.selectedIndex)
          {
            if (SerializableDataExtension.BuildingData(_headerDropDown.selectedIndex).TryGetValue(buildingId, out var stringSet) && stringSet.Count > 0)
              _vehicleListBox.SelectedItems = stringSet;
            else
              _vehicleListBox.SetSelectionStateToAll(false);
          }
          _prefabPanel.Show();
        }
        else
          _prefabPanel.Hide();
        _cachedIndex = _headerDropDown.selectedIndex;
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
      var uiDropDown = UIHelper.CreateDropDown(uiPanel);
      uiDropDown.font = UIHelper.Font;
      uiDropDown.relativePosition = new Vector3(3f, 3f);
      _headerDropDown = uiDropDown;
      _headerDropDown.width = uiPanel.width - 6f;
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
      if (!SerializableDataExtension.BuildingData(_headerDropDown.selectedIndex).TryGetValue(building, out _))
        SerializableDataExtension.BuildingData(_headerDropDown.selectedIndex).Add(building, selectedItems);
      else
        SerializableDataExtension.BuildingData(_headerDropDown.selectedIndex)[building] = selectedItems;
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
