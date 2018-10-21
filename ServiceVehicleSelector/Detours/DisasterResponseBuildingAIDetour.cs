using ColossalFramework;
using ServiceVehicleSelector2.RedirectionFramework.Attributes;

namespace ServiceVehicleSelector2.Detours
{
    [TargetType(typeof(DisasterResponseBuildingAI))]
    public class DisasterResponseBuildingAIDetour : PlayerBuildingAI
    {
      
        [RedirectMethod]
        public override void StartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
          switch (material)
          {
            case TransferManager.TransferReason.Collapsed:
              VehicleInfo randomVehicleInfo1 = Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, this.m_info.m_class.m_service, this.m_info.m_class.m_subService, this.m_info.m_class.m_level, VehicleInfo.VehicleType.Car);
              if (randomVehicleInfo1 == null)
                break;
              Array16<Vehicle> vehicles1 = Singleton<VehicleManager>.instance.m_vehicles;
              ushort vehicle1;
              if (!Singleton<VehicleManager>.instance.CreateVehicle(out vehicle1, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo1, data.m_position, material, true, false))
                break;
              randomVehicleInfo1.m_vehicleAI.SetSource(vehicle1, ref vehicles1.m_buffer[(int) vehicle1], buildingID);
              randomVehicleInfo1.m_vehicleAI.StartTransfer(vehicle1, ref vehicles1.m_buffer[(int) vehicle1], material, offer);
              break;
            case TransferManager.TransferReason.Collapsed2:
              //BEGIN MOD
              VehicleInfo forcedInfo = DepotAIDetour.GetVehicleInfo(buildingID, data);
              VehicleInfo randomVehicleInfo2 = forcedInfo != null ? forcedInfo : Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, this.m_info.m_class.m_service, this.m_info.m_class.m_subService, this.m_info.m_class.m_level, VehicleInfo.VehicleType.Helicopter);
              //END MOD
              if (randomVehicleInfo2 == null)
                break;
              Array16<Vehicle> vehicles2 = Singleton<VehicleManager>.instance.m_vehicles;
              ushort vehicle2;
              if (!Singleton<VehicleManager>.instance.CreateVehicle(out vehicle2, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo2, data.m_position, material, true, false))
                break;
              randomVehicleInfo2.m_vehicleAI.SetSource(vehicle2, ref vehicles2.m_buffer[(int) vehicle2], buildingID);
              randomVehicleInfo2.m_vehicleAI.StartTransfer(vehicle2, ref vehicles2.m_buffer[(int) vehicle2], material, offer);
              break;
            default:
              base.StartTransfer(buildingID, ref data, material, offer);
              break;
          }
        } 
    }
}