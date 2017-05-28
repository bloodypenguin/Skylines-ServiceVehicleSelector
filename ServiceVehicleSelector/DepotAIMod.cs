// Decompiled with JetBrains decompiler
// Type: ServiceVehicleSelector.DepotAIMod
// Assembly: ServiceVehicleSelector, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D0EBD243-0D3C-4ED4-95A5-A73C88972683
// Assembly location: C:\Games\Steam\steamapps\workshop\content\255710\519691655\ServiceVehicleSelector.dll

using ColossalFramework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ServiceVehicleSelector
{
  public class DepotAIMod : DepotAI
  {
    private static bool _isDeployed;
    private static RedirectCallsState _redirectCallsStateDepotAI;

    public static void Init()
    {
      if (DepotAIMod._isDeployed)
        return;
      Utils.Log((object) "Detouring DepotAI.StartTransfer to DepotAIMod.StartTransfer");
      DepotAIMod._redirectCallsStateDepotAI = RedirectionHelper.RedirectCalls(typeof (DepotAI).GetMethod("StartTransfer"), typeof (DepotAIMod).GetMethod("StartTransfer"));
      DepotAIMod._isDeployed = true;
    }

    public static void Deinit()
    {
      if (!DepotAIMod._isDeployed)
        return;
      Utils.Log((object) "Reverting detour for DepotAI.StartTransfer");
      RedirectionHelper.RevertRedirect(typeof (DepotAI).GetMethod("StartTransfer"), DepotAIMod._redirectCallsStateDepotAI);
      DepotAIMod._isDeployed = false;
    }

    public override void StartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason reason, TransferManager.TransferOffer offer)
    {
      if (reason != this.m_transportInfo.m_vehicleReason)
        return;
      HashSet<string> source;
      VehicleInfo info;
      if (reason == TransferManager.TransferReason.Taxi && ServiceVehicleSelectorMod.BuildingData.TryGetValue(buildingID, out source) && source.Count > 0)
      {
        string[] array = source.ToArray<string>();
        int index = Singleton<SimulationManager>.instance.m_randomizer.Int32((uint) array.Length);
        string prefabName = array[index];
        info = VehicleManagerMod.GetVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, data.Info.m_class.m_service, data.Info.m_class.m_subService, data.Info.m_class.m_level, buildingID, prefabName);
      }
      else
        info = Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, this.m_info.m_class.m_service, this.m_info.m_class.m_subService, this.m_info.m_class.m_level);
      if (!((Object) info != (Object) null))
        return;
      Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
      Vector3 position;
      Vector3 target;
      this.CalculateSpawnPosition(buildingID, ref data, ref Singleton<SimulationManager>.instance.m_randomizer, info, out position, out target);
      ushort vehicle;
      if (!Singleton<VehicleManager>.instance.CreateVehicle(out vehicle, ref Singleton<SimulationManager>.instance.m_randomizer, info, position, reason, false, true))
        return;
      info.m_vehicleAI.SetSource(vehicle, ref vehicles.m_buffer[(int) vehicle], buildingID);
      info.m_vehicleAI.StartTransfer(vehicle, ref vehicles.m_buffer[(int) vehicle], reason, offer);
    }
  }
}
