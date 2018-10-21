using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using ServiceVehicleSelector2.RedirectionFramework;
using ServiceVehicleSelector2.RedirectionFramework.Attributes;
using UnityEngine;

namespace ServiceVehicleSelector2.Detours
{
  public class ServiceBuildingAIMod
  {
    private static bool _isDeployed;
    private static RedirectCallsState _redirectCallsStateCemeteryAI;
    private static RedirectCallsState _redirectCallsStateFireStationAI;
    private static RedirectCallsState _redirectCallsStateHospitalAI;
    private static RedirectCallsState _redirectCallsStatePoliceStationAI;
    private static RedirectCallsState _redirectCallsStateMaintenanceDepotAI;
    private static RedirectCallsState _redirectCallsStateSnowDumpAI;

    public static void Init()
    {
      if (ServiceBuildingAIMod._isDeployed)
        return;
      Utils.Log((object) "Detouring AI.StartTransfer to ServiceBuildingAIMod.StartTransfer");
      MethodInfo method1 = typeof (CemeteryAI).GetMethod("StartTransfer");
      MethodInfo method2 = typeof (ServiceBuildingAIMod).GetMethod("StartTransfer");
      MethodInfo to = method2;
      ServiceBuildingAIMod._redirectCallsStateCemeteryAI = RedirectionHelper.RedirectCalls(method1, to);
      ServiceBuildingAIMod._redirectCallsStateFireStationAI = RedirectionHelper.RedirectCalls(typeof (FireStationAI).GetMethod("StartTransfer"), method2);
      ServiceBuildingAIMod._redirectCallsStateHospitalAI = RedirectionHelper.RedirectCalls(typeof (HospitalAI).GetMethod("StartTransfer"), method2);
      ServiceBuildingAIMod._redirectCallsStatePoliceStationAI = RedirectionHelper.RedirectCalls(typeof (PoliceStationAI).GetMethod("StartTransfer"), method2);
      ServiceBuildingAIMod._redirectCallsStateMaintenanceDepotAI = RedirectionHelper.RedirectCalls(typeof (MaintenanceDepotAI).GetMethod("StartTransfer"), method2);
      ServiceBuildingAIMod._redirectCallsStateSnowDumpAI = RedirectionHelper.RedirectCalls(typeof (SnowDumpAI).GetMethod("StartTransfer"), method2);
      ServiceBuildingAIMod._isDeployed = true;
    }

    public static void Deinit()
    {
      if (!ServiceBuildingAIMod._isDeployed)
        return;
      Utils.Log((object) "Reverting detour for StartTransfer");
      RedirectionHelper.RevertRedirect(typeof (CemeteryAI).GetMethod("StartTransfer"), ServiceBuildingAIMod._redirectCallsStateCemeteryAI);
      RedirectionHelper.RevertRedirect(typeof (FireStationAI).GetMethod("StartTransfer"), ServiceBuildingAIMod._redirectCallsStateFireStationAI);
      RedirectionHelper.RevertRedirect(typeof (HospitalAI).GetMethod("StartTransfer"), ServiceBuildingAIMod._redirectCallsStateHospitalAI);
      RedirectionHelper.RevertRedirect(typeof (PoliceStationAI).GetMethod("StartTransfer"), ServiceBuildingAIMod._redirectCallsStatePoliceStationAI);
      RedirectionHelper.RevertRedirect(typeof (MaintenanceDepotAI).GetMethod("StartTransfer"), ServiceBuildingAIMod._redirectCallsStateMaintenanceDepotAI);
      RedirectionHelper.RevertRedirect(typeof (SnowDumpAI).GetMethod("StartTransfer"), ServiceBuildingAIMod._redirectCallsStateSnowDumpAI);
      ServiceBuildingAIMod._isDeployed = false;
    }

    public void StartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
    {
      bool transferToSource = true;
      bool transferToTarget = false;
      if (material == TransferManager.TransferReason.DeadMove || material == TransferManager.TransferReason.GarbageMove || material == TransferManager.TransferReason.SnowMove)
      {
        transferToSource = false;
        transferToTarget = true;
      }
      HashSet<string> source;
      VehicleInfo info;
      if (ServiceVehicleSelectorMod.BuildingData.TryGetValue(buildingID, out source) && source.Count > 0)
      {
        string[] array = source.ToArray<string>();
        int index = Singleton<SimulationManager>.instance.m_randomizer.Int32((uint) array.Length);
        string prefabName = array[index];
        info = VehicleManagerMod.GetVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, data.Info.m_class.m_service, data.Info.m_class.m_subService, data.Info.m_class.m_level, prefabName);
      }
      else
        info = Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, data.Info.m_class.m_service, data.Info.m_class.m_subService, data.Info.m_class.m_level);
      if (!((Object) info != (Object) null))
        return;
      Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
      ushort vehicle;
      if (!Singleton<VehicleManager>.instance.CreateVehicle(out vehicle, ref Singleton<SimulationManager>.instance.m_randomizer, info, data.m_position, material, transferToSource, transferToTarget))
        return;
      info.m_vehicleAI.SetSource(vehicle, ref vehicles.m_buffer[(int) vehicle], buildingID);
      info.m_vehicleAI.StartTransfer(vehicle, ref vehicles.m_buffer[(int) vehicle], material, offer);
    }
  }
}
