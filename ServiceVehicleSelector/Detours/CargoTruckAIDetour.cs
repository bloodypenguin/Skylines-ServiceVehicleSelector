using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Math;
using ServiceVehicleSelector2.RedirectionFramework.Attributes;
using UnityEngine;

namespace ServiceVehicleSelector2.Detours
{
  [TargetType(typeof(CargoTruckAI))]
  public class CargoTruckAIDetour : CargoTruckAI
  {

    
  [RedirectMethod]
  public new static bool ChangeVehicleType(VehicleInfo vehicleInfo, ushort vehicleID, ref Vehicle vehicleData, PathUnit.Position pathPos, uint laneID)
  {
    if ((vehicleData.m_flags & (Vehicle.Flags.TransferToSource | Vehicle.Flags.GoingBack)) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
      return false;
    VehicleManager instance1 = Singleton<VehicleManager>.instance;
    NetManager instance2 = Singleton<NetManager>.instance;
    BuildingManager instance3 = Singleton<BuildingManager>.instance;
    NetInfo info1 = instance2.m_segments.m_buffer[(int) pathPos.m_segment].Info;
    Vector3 position1 = instance2.m_lanes.m_buffer[laneID].CalculatePosition(0.5f);
    Vector3 lastPos = position1;
    if (!CargoTruckAIDetour.SkipNonCarPaths(ref vehicleData.m_path, ref vehicleData.m_pathPositionIndex, ref vehicleData.m_lastPathOffset, ref lastPos))
      return false;
    ushort cargoStation1 = CargoTruckAIDetour.FindCargoStation(position1, info1.m_class.m_service);
    ushort cargoStation2 = CargoTruckAIDetour.FindCargoStation(lastPos, info1.m_class.m_service);
    if ((int) cargoStation2 == (int) cargoStation1)
      return true;
    bool flag1 = false;
    if (cargoStation1 != (ushort) 0 && (instance3.m_buildings.m_buffer[(int) cargoStation1].m_flags & Building.Flags.Active) != Building.Flags.None)
      flag1 = true;
    bool flag2 = false;
    if (cargoStation2 != (ushort) 0 && (instance3.m_buildings.m_buffer[(int) cargoStation2].m_flags & Building.Flags.Active) != Building.Flags.None)
      flag2 = true;
    ushort vehicle1;
    if (flag1 && flag2 && instance1.CreateVehicle(out vehicle1, ref Singleton<SimulationManager>.instance.m_randomizer, vehicleInfo, position1, (TransferManager.TransferReason) vehicleData.m_transferType, false, true))
    {
      if (vehicleData.m_targetBuilding != (ushort) 0)
      {
        instance1.m_vehicles.m_buffer[(int) vehicle1].m_targetBuilding = vehicleData.m_targetBuilding;
        instance1.m_vehicles.m_buffer[(int) vehicle1].m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive;
        instance3.m_buildings.m_buffer[(int) vehicleData.m_targetBuilding].AddGuestVehicle(vehicle1, ref instance1.m_vehicles.m_buffer[(int) vehicle1]);
      }
      instance1.m_vehicles.m_buffer[(int) vehicle1].m_transferSize = vehicleData.m_transferSize;
      instance1.m_vehicles.m_buffer[(int) vehicle1].m_path = vehicleData.m_path;
      instance1.m_vehicles.m_buffer[(int) vehicle1].m_pathPositionIndex = vehicleData.m_pathPositionIndex;
      instance1.m_vehicles.m_buffer[(int) vehicle1].m_lastPathOffset = vehicleData.m_lastPathOffset;
      instance1.m_vehicles.m_buffer[(int) vehicle1].m_flags |= vehicleData.m_flags & (Vehicle.Flags.Importing | Vehicle.Flags.Exporting);
      vehicleData.m_path = 0U;
      ushort vehicle2 = CargoTruckAIDetour.FindCargoParent(cargoStation1, cargoStation2, info1.m_class.m_service, info1.m_class.m_subService);
      VehicleInfo info2;
      if (vehicle2 != (ushort) 0)
      {
        info2 = instance1.m_vehicles.m_buffer[(int) vehicle2].Info;
      }
      else
      {
        //begin mod
        if (ServiceVehicleSelectorMod.BuildingData.TryGetValue(cargoStation1, out var source) && source.Count > 0)
        {
          string[] array = source.ToArray<string>();
          int index = Singleton<SimulationManager>.instance.m_randomizer.Int32((uint) array.Length);
          string prefabName = array[index];
          info2 = VehicleManagerMod.GetVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, info1.m_class.m_service, info1.m_class.m_subService, ItemClass.Level.Level4, prefabName);
        }
        else
        {
          info2 = null;
        }
        if(info2 == null)
        //end mod
        info2 = instance1.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, info1.m_class.m_service, info1.m_class.m_subService, ItemClass.Level.Level4);
        if (info2 != null && instance1.CreateVehicle(out vehicle2, ref Singleton<SimulationManager>.instance.m_randomizer, info2, position1, TransferManager.TransferReason.None, false, true))
        {
          info2.m_vehicleAI.SetSource(vehicle2, ref instance1.m_vehicles.m_buffer[(int) vehicle2], cargoStation1);
          info2.m_vehicleAI.SetTarget(vehicle2, ref instance1.m_vehicles.m_buffer[(int) vehicle2], cargoStation2);
        }
      }
      if (vehicle2 != (ushort) 0)
      {
        int size;
        int max;
        info2.m_vehicleAI.GetSize(vehicle2, ref instance1.m_vehicles.m_buffer[(int) vehicle2], out size, out max);
        instance1.m_vehicles.m_buffer[(int) vehicle1].m_cargoParent = vehicle2;
        instance1.m_vehicles.m_buffer[(int) vehicle1].m_nextCargo = instance1.m_vehicles.m_buffer[(int) vehicle2].m_firstCargo;
        instance1.m_vehicles.m_buffer[(int) vehicle2].m_firstCargo = vehicle1;
        instance1.m_vehicles.m_buffer[(int) vehicle2].m_transferSize = (ushort) ++size;
        if (size >= max && info2.m_vehicleAI.CanSpawnAt(position1))
        {
          instance1.m_vehicles.m_buffer[(int) vehicle2].m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive;
          instance1.m_vehicles.m_buffer[(int) vehicle2].m_waitCounter = (byte) 0;
          info2.m_vehicleAI.SetTarget(vehicle2, ref instance1.m_vehicles.m_buffer[(int) vehicle2], cargoStation2);
        }
      }
      else
        instance1.ReleaseVehicle(vehicle1);
    }
    vehicleData.m_transferSize = (ushort) 0;
    if (cargoStation1 != (ushort) 0)
    {
      vehicleData.Unspawn(vehicleID);
      BuildingInfo info2 = instance3.m_buildings.m_buffer[(int) cargoStation1].Info;
      Randomizer randomizer = new Randomizer((int) vehicleID);
      Vector3 position2;
      Vector3 target;
      info2.m_buildingAI.CalculateSpawnPosition(cargoStation1, ref instance3.m_buildings.m_buffer[(int) cargoStation1], ref randomizer, vehicleInfo, out position2, out target);
      Quaternion rotation = Quaternion.identity;
      Vector3 forward = target - position2;
      if ((double) forward.sqrMagnitude > 0.00999999977648258)
        rotation = Quaternion.LookRotation(forward);
      vehicleData.m_frame0 = new Vehicle.Frame(position2, rotation);
      vehicleData.m_frame1 = vehicleData.m_frame0;
      vehicleData.m_frame2 = vehicleData.m_frame0;
      vehicleData.m_frame3 = vehicleData.m_frame0;
      vehicleData.m_targetPos0 = (Vector4) position2;
      vehicleData.m_targetPos0.w = 2f;
      vehicleData.m_targetPos1 = (Vector4) target;
      vehicleData.m_targetPos1.w = 2f;
      vehicleData.m_targetPos2 = vehicleData.m_targetPos1;
      vehicleData.m_targetPos3 = vehicleData.m_targetPos1;
      if ((int) cargoStation1 == (int) vehicleData.m_sourceBuilding)
      {
        if (vehicleData.m_targetBuilding != (ushort) 0)
        {
          Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) vehicleData.m_targetBuilding].RemoveGuestVehicle(vehicleID, ref vehicleData);
          vehicleData.m_targetBuilding = (ushort) 0;
        }
        vehicleData.m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive;
        vehicleData.m_flags |= Vehicle.Flags.GoingBack;
        vehicleData.m_waitCounter = (byte) 0;
        return true;
      }
    }
    else
    {
      vehicleData.m_targetPos1 = vehicleData.m_targetPos0;
      vehicleData.m_targetPos2 = vehicleData.m_targetPos0;
      vehicleData.m_targetPos3 = vehicleData.m_targetPos0;
    }
    vehicleInfo.m_vehicleAI.SetTarget(vehicleID, ref vehicleData, (ushort) 0);
    return true;
  }
    
    
    

      [RedirectReverse]
      private static bool SkipNonCarPaths(ref uint path, ref byte pathPositionIndex, ref byte lastPathOffset,
          ref Vector3 lastPos)
      {
            UnityEngine.Debug.Log("SkipNonCarPaths");
          return false;
      }

      [RedirectReverse]
        private static ushort FindCargoParent(ushort sourceBuilding, ushort targetBuilding, ItemClass.Service service,
          ItemClass.SubService subService)
      {
          UnityEngine.Debug.Log("FindCargoParent");
            return 0;
      }

      [RedirectReverse]
        private static void RemoveTarget(CargoTruckAI ai, ushort vehicleID, ref Vehicle data)
      {
          UnityEngine.Debug.Log("RemoveTarget");
        }
    
    [RedirectReverse]
      private static ushort FindCargoStation(Vector3 position, ItemClass.Service service)
      {
        UnityEngine.Debug.Log("FindCargoStation");
        return 0;
      }
    }
}
