using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ServiceVehicleSelector2.RedirectionFramework.Attributes;

namespace ServiceVehicleSelector2.Detours
{
    [TargetType(typeof(HelicopterDepotAI))]
    public class HelicopterDepotAIDetour : PlayerBuildingAI
    {
        [RedirectMethod]
        public override void StartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            TransferManager.TransferReason transferReason1 = this.GetTransferReason1();
            TransferManager.TransferReason transferReason2 = this.GetTransferReason2();
            if (material != TransferManager.TransferReason.None && (material == transferReason1 || material == transferReason2))
            {
                //BEGIN MOD
                VehicleInfo forcedInfo = DepotAIDetour.GetVehicleInfo(buildingID, data);
                
                VehicleInfo randomVehicleInfo = forcedInfo != null ? forcedInfo : Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, this.m_info.m_class.m_service, this.m_info.m_class.m_subService, this.m_info.m_class.m_level, VehicleInfo.VehicleType.Helicopter);
                //END MOD
                if (randomVehicleInfo == null)
                    return;
                Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
                ushort vehicle;
                if (!Singleton<VehicleManager>.instance.CreateVehicle(out vehicle, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, data.m_position, material, true, false))
                    return;
                randomVehicleInfo.m_vehicleAI.SetSource(vehicle, ref vehicles.m_buffer[(int) vehicle], buildingID);
                randomVehicleInfo.m_vehicleAI.StartTransfer(vehicle, ref vehicles.m_buffer[(int) vehicle], material, offer);
            }
            else
                base.StartTransfer(buildingID, ref data, material, offer);
        }
       
        [RedirectReverse]        
        private TransferManager.TransferReason GetTransferReason1()
        {
            UnityEngine.Debug.LogError("a reverse redirect is called!");
            return TransferManager.TransferReason.None;
        }

        [RedirectReverse]   
        private TransferManager.TransferReason GetTransferReason2()
        {
            UnityEngine.Debug.LogError("a reverse redirect is called!");
            return TransferManager.TransferReason.None;
        }
    }
}