using System.Reflection;
using ColossalFramework;
using ServiceVehicleSelector2.RedirectionFramework.Attributes;

namespace ServiceVehicleSelector2.Detours
{
  [TargetType(typeof(PostOfficeAI))]
    public class PostOfficeAIDetour : PlayerBuildingAI
    {
      [RedirectMethod]
          public override void StartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
  {
    switch (material)
    {
      case TransferManager.TransferReason.Mail:
        //begin mod(*): force vehicle info
        var forceInfo1 = DepotAIDetour.GetVehicleInfo(buildingID, data);
        VehicleInfo randomVehicleInfo1 = forceInfo1 != null ? forceInfo1 : Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, this.m_info.m_class.m_service, this.m_info.m_class.m_subService, ItemClass.Level.Level2);
        //end mod
       
        if (randomVehicleInfo1 == null)
          break;
        Vehicle data1 = new Vehicle();
        int size1;
        int max1;
        randomVehicleInfo1.m_vehicleAI.GetSize((ushort) 0, ref data1, out size1, out max1);
        if ((int) data.m_customBuffer2 * 1000 < max1)
          break;
        Array16<Vehicle> vehicles1 = Singleton<VehicleManager>.instance.m_vehicles;
        ushort vehicle1;
        if (!Singleton<VehicleManager>.instance.CreateVehicle(out vehicle1, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo1, data.m_position, material, true, false))
          break;
        randomVehicleInfo1.m_vehicleAI.SetSource(vehicle1, ref vehicles1.m_buffer[(int) vehicle1], buildingID);
        randomVehicleInfo1.m_vehicleAI.StartTransfer(vehicle1, ref vehicles1.m_buffer[(int) vehicle1], material, offer);
        break;
      case TransferManager.TransferReason.UnsortedMail:
      case TransferManager.TransferReason.SortedMail:
      case TransferManager.TransferReason.OutgoingMail:
      case TransferManager.TransferReason.IncomingMail:
        int unsortedMail = 0;
        int sortedMail = 0;
        int unsortedCapacity = 0;
        int sortedCapacity = 0;
        int ownVanCount = 0;
        int ownTruckCount = 0;
        int import = 0;
        int export = 0;
        this.CalculateVehicles(buildingID, ref data, ref unsortedMail, ref sortedMail, ref unsortedCapacity, ref sortedCapacity, ref ownVanCount, ref ownTruckCount, ref import, ref export);
        if (ownTruckCount >= this.m_postTruckCount)
          break;
        bool transferToSource = this.m_sortingRate == 0 ? material != TransferManager.TransferReason.UnsortedMail : material == TransferManager.TransferReason.UnsortedMail;
        //begin mod(*): force vehicle info
        var forceInfo2 = DepotAIDetour.GetVehicleInfo(buildingID, data);
        VehicleInfo randomVehicleInfo2 = forceInfo2 != null ? forceInfo2 : Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, this.m_info.m_class.m_service, this.m_info.m_class.m_subService, ItemClass.Level.Level5);
        //end mod
        if (randomVehicleInfo2 == null)
          break;
        Array16<Vehicle> vehicles2 = Singleton<VehicleManager>.instance.m_vehicles;
        ushort vehicle2;
        if (!Singleton<VehicleManager>.instance.CreateVehicle(out vehicle2, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo2, data.m_position, material, transferToSource, !transferToSource))
          break;
        randomVehicleInfo2.m_vehicleAI.SetSource(vehicle2, ref vehicles2.m_buffer[(int) vehicle2], buildingID);
        randomVehicleInfo2.m_vehicleAI.StartTransfer(vehicle2, ref vehicles2.m_buffer[(int) vehicle2], material, offer);
        ushort building = offer.Building;
        if (building == (ushort) 0 || (Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) building].m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.None)
          break;
        int size2;
        int max2;
        randomVehicleInfo2.m_vehicleAI.GetSize(vehicle2, ref vehicles2.m_buffer[(int) vehicle2], out size2, out max2);
        CommonBuildingAI.ExportResource(buildingID, ref data, material, size2);
        break;
      default:
        base.StartTransfer(buildingID, ref data, material, offer);
        break;
    }
  }

      [RedirectReverse]
      private void CalculateVehicles(ushort buildingID, ref Building data, ref int unsortedMail, ref int sortedMail,
        ref int unsortedCapacity, ref int sortedCapacity, ref int ownVanCount, ref int ownTruckCount, ref int import,
        ref int export)
      {
          UnityEngine.Debug.LogWarning("CalculateVehicles");
      }

      private int m_postTruckCount => (int)typeof(PostOfficeAI)
        .GetField("m_postTruckCount", BindingFlags.Instance | BindingFlags.Public).GetValue(this);
      
      private int m_sortingRate => (int)typeof(PostOfficeAI)
        .GetField("m_sortingRate", BindingFlags.Instance | BindingFlags.Public).GetValue(this);
    }
}