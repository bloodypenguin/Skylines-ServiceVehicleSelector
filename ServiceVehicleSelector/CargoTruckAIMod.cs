// Decompiled with JetBrains decompiler
// Type: ServiceVehicleSelector.CargoTruckAIMod
// Assembly: ServiceVehicleSelector, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D0EBD243-0D3C-4ED4-95A5-A73C88972683
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\519691655\ServiceVehicleSelector.dll

using ColossalFramework;
using ColossalFramework.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ServiceVehicleSelector
{
  public class CargoTruckAIMod
  {
    private static bool _isDeployed;
    private static CargoTruckAIMod.SkipNonCarPathsCallback SkipNonCarPaths;
    private static CargoTruckAIMod.FindCargoParentCallback FindCargoParent;
    private static CargoTruckAIMod.RemoveTargetCallback RemoveTarget;
    private static RedirectCallsState _redirectCallsState;

    public static void Init()
    {
      if (CargoTruckAIMod._isDeployed)
        return;
      CargoTruckAIMod.SkipNonCarPaths = (CargoTruckAIMod.SkipNonCarPathsCallback) Delegate.CreateDelegate(typeof (CargoTruckAIMod.SkipNonCarPathsCallback), (object) null, typeof (CargoTruckAI).GetMethod("SkipNonCarPaths", BindingFlags.Static | BindingFlags.NonPublic));
      CargoTruckAIMod.FindCargoParent = (CargoTruckAIMod.FindCargoParentCallback) Delegate.CreateDelegate(typeof (CargoTruckAIMod.FindCargoParentCallback), (object) null, typeof (CargoTruckAI).GetMethod("FindCargoParent", BindingFlags.Static | BindingFlags.NonPublic));
      CargoTruckAIMod.RemoveTarget = (CargoTruckAIMod.RemoveTargetCallback) Delegate.CreateDelegate(typeof (CargoTruckAIMod.RemoveTargetCallback), (object) null, typeof (CargoTruckAI).GetMethod("RemoveTarget", BindingFlags.Instance | BindingFlags.NonPublic));
      Utils.Log((object) "Detouring CargoTruckAI.ChangeVehicleType to CargoTruckAIMod.ChangeVehicleType");
      CargoTruckAIMod._redirectCallsState = RedirectionHelper.RedirectCalls(typeof (CargoTruckAI).GetMethod("ChangeVehicleType", BindingFlags.Instance | BindingFlags.NonPublic), typeof (CargoTruckAIMod).GetMethod("ChangeVehicleType", BindingFlags.Instance | BindingFlags.NonPublic));
      CargoTruckAIMod._isDeployed = true;
    }

    public static void Deinit()
    {
      if (!CargoTruckAIMod._isDeployed)
        return;
      CargoTruckAIMod.SkipNonCarPaths = (CargoTruckAIMod.SkipNonCarPathsCallback) null;
      CargoTruckAIMod.FindCargoParent = (CargoTruckAIMod.FindCargoParentCallback) null;
      CargoTruckAIMod.RemoveTarget = (CargoTruckAIMod.RemoveTargetCallback) null;
      Utils.Log((object) "Reverting detour for ChangeVehicleType");
      RedirectionHelper.RevertRedirect(typeof (CargoTruckAI).GetMethod("ChangeVehicleType", BindingFlags.Instance | BindingFlags.NonPublic), CargoTruckAIMod._redirectCallsState);
      CargoTruckAIMod._isDeployed = false;
    }

    private bool ChangeVehicleType(ushort vehicleID, ref Vehicle vehicleData, PathUnit.Position pathPos, uint laneID)
    {
      if ((vehicleData.m_flags & (Vehicle.Flags.TransferToSource | Vehicle.Flags.GoingBack)) != ~(Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.WaitingTarget | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive))
        return false;
      VehicleManager instance1 = Singleton<VehicleManager>.instance;
      NetManager instance2 = Singleton<NetManager>.instance;
      BuildingManager instance3 = Singleton<BuildingManager>.instance;
      CargoTruckAI vehicleAi = vehicleData.Info.m_vehicleAI as CargoTruckAI;
      NetInfo info1 = instance2.m_segments.m_buffer[(int) pathPos.m_segment].Info;
      Vector3 position1 = instance2.m_lanes.m_buffer[(int) laneID].CalculatePosition(0.5f);
      Vector3 lastPos = position1;
      if (!CargoTruckAIMod.SkipNonCarPaths(ref vehicleData.m_path, ref vehicleData.m_pathPositionIndex, ref vehicleData.m_lastPathOffset, ref lastPos))
        return false;
      ushort building1 = instance3.FindBuilding(position1, 100f, info1.m_class.m_service, ItemClass.SubService.None, Building.Flags.None, Building.Flags.None);
      ushort building2 = instance3.FindBuilding(lastPos, 100f, info1.m_class.m_service, ItemClass.SubService.None, Building.Flags.None, Building.Flags.None);
      if ((int) building2 == (int) building1)
        return true;
      bool flag1 = false;
      if ((int) building1 != 0 && (instance3.m_buildings.m_buffer[(int) building1].m_flags & Building.Flags.Active) != Building.Flags.None)
        flag1 = true;
      bool flag2 = false;
      if ((int) building2 != 0 && (instance3.m_buildings.m_buffer[(int) building2].m_flags & Building.Flags.Active) != Building.Flags.None)
        flag2 = true;
      ushort vehicle1;
      if (flag1 & flag2 && instance1.CreateVehicle(out vehicle1, ref Singleton<SimulationManager>.instance.m_randomizer, vehicleAi.m_info, position1, (TransferManager.TransferReason) vehicleData.m_transferType, false, true))
      {
        if ((int) vehicleData.m_targetBuilding != 0)
        {
          instance1.m_vehicles.m_buffer[(int) vehicle1].m_targetBuilding = vehicleData.m_targetBuilding;
          instance1.m_vehicles.m_buffer[(int) vehicle1].m_flags &= Vehicle.Flags.Created | Vehicle.Flags.Deleted | Vehicle.Flags.Spawned | Vehicle.Flags.Inverted | Vehicle.Flags.TransferToTarget | Vehicle.Flags.TransferToSource | Vehicle.Flags.Emergency1 | Vehicle.Flags.Emergency2 | Vehicle.Flags.WaitingPath | Vehicle.Flags.Stopped | Vehicle.Flags.Leaving | Vehicle.Flags.Arriving | Vehicle.Flags.Reversed | Vehicle.Flags.TakingOff | Vehicle.Flags.Flying | Vehicle.Flags.Landing | Vehicle.Flags.WaitingSpace | Vehicle.Flags.WaitingCargo | Vehicle.Flags.GoingBack | Vehicle.Flags.Importing | Vehicle.Flags.Exporting | Vehicle.Flags.Parking | Vehicle.Flags.CustomName | Vehicle.Flags.OnGravel | Vehicle.Flags.WaitingLoading | Vehicle.Flags.Congestion | Vehicle.Flags.DummyTraffic | Vehicle.Flags.Underground | Vehicle.Flags.Transition | Vehicle.Flags.InsideBuilding | Vehicle.Flags.LeftHandDrive;
          instance3.m_buildings.m_buffer[(int) vehicleData.m_targetBuilding].AddGuestVehicle(vehicle1, ref instance1.m_vehicles.m_buffer[(int) vehicle1]);
        }
        instance1.m_vehicles.m_buffer[(int) vehicle1].m_transferSize = vehicleData.m_transferSize;
        instance1.m_vehicles.m_buffer[(int) vehicle1].m_path = vehicleData.m_path;
        instance1.m_vehicles.m_buffer[(int) vehicle1].m_pathPositionIndex = vehicleData.m_pathPositionIndex;
        instance1.m_vehicles.m_buffer[(int) vehicle1].m_lastPathOffset = vehicleData.m_lastPathOffset;
        instance1.m_vehicles.m_buffer[(int) vehicle1].m_flags = instance1.m_vehicles.m_buffer[(int) vehicle1].m_flags | vehicleData.m_flags & (Vehicle.Flags.Importing | Vehicle.Flags.Exporting);
        vehicleData.m_path = 0U;
        ushort vehicle2 = CargoTruckAIMod.FindCargoParent(building1, building2, info1.m_class.m_service, info1.m_class.m_subService);
        VehicleInfo info2;
        if ((int) vehicle2 != 0)
        {
          info2 = instance1.m_vehicles.m_buffer[(int) vehicle2].Info;
        }
        else
        {
          HashSet<string> source;
          if (info1.m_class.m_subService == ItemClass.SubService.PublicTransportTrain && ServiceVehicleSelectorMod.BuildingData.TryGetValue(building1, out source) && source.Count > 0)
          {
            string[] array = source.ToArray<string>();
            int index = Singleton<SimulationManager>.instance.m_randomizer.Int32((uint) array.Length);
            string prefabName = array[index];
            info2 = VehicleManagerMod.GetVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, info1.m_class.m_service, info1.m_class.m_subService, ItemClass.Level.Level4, building1, prefabName);
          }
          else
            info2 = instance1.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, info1.m_class.m_service, info1.m_class.m_subService, ItemClass.Level.Level4);
          if ((UnityEngine.Object) info2 != (UnityEngine.Object) null && instance1.CreateVehicle(out vehicle2, ref Singleton<SimulationManager>.instance.m_randomizer, info2, position1, TransferManager.TransferReason.None, false, true))
          {
            info2.m_vehicleAI.SetSource(vehicle2, ref instance1.m_vehicles.m_buffer[(int) vehicle2], building1);
            info2.m_vehicleAI.SetTarget(vehicle2, ref instance1.m_vehicles.m_buffer[(int) vehicle2], building2);
          }
        }
        if ((int) vehicle2 != 0)
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
            info2.m_vehicleAI.SetTarget(vehicle2, ref instance1.m_vehicles.m_buffer[(int) vehicle2], building2);
          }
        }
        else
          instance1.ReleaseVehicle(vehicle1);
      }
      vehicleData.m_transferSize = (ushort) 0;
      if ((int) building1 != 0)
      {
        vehicleData.Unspawn(vehicleID);
        Randomizer randomizer = new Randomizer((int) vehicleID);
        Vector3 position2;
        Vector3 target;
        instance3.m_buildings.m_buffer[(int) building1].Info.m_buildingAI.CalculateSpawnPosition(building1, ref instance3.m_buildings.m_buffer[(int) building1], ref randomizer, vehicleAi.m_info, out position2, out target);
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
        if ((int) building1 == (int) vehicleData.m_sourceBuilding)
        {
          CargoTruckAIMod.RemoveTarget(vehicleAi, vehicleID, ref vehicleData);
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
      vehicleAi.SetTarget(vehicleID, ref vehicleData, (ushort) 0);
      return true;
    }

    private delegate bool SkipNonCarPathsCallback(ref uint path, ref byte pathPositionIndex, ref byte lastPathOffset, ref Vector3 lastPos);

    private delegate ushort FindCargoParentCallback(ushort sourceBuilding, ushort targetBuilding, ItemClass.Service service, ItemClass.SubService subService);

    private delegate void RemoveTargetCallback(CargoTruckAI ai, ushort vehicleID, ref Vehicle data);
  }
}
