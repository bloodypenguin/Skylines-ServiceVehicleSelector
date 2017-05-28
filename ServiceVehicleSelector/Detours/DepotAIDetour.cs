using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ServiceVehicleSelector.RedirectionFramework;
using ServiceVehicleSelector.RedirectionFramework.Attributes;
using UnityEngine;

namespace ServiceVehicleSelector.Detours
{
    [TargetType(typeof(DepotAI))]
    public class DepotAIDetour : DepotAI
    {
        [RedirectMethod]
        public override void StartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason reason, TransferManager.TransferOffer offer)
        {
            //begin mod(+): handle taxi case
            var forceInfo = GetForcedVehicle(buildingID, data, reason);
            //end mod
            if (reason == this.m_transportInfo.m_vehicleReason)
            {
                //begin mod(*): if some vehicle was forced, use the forced one
                VehicleInfo randomVehicleInfo = forceInfo != null ? forceInfo : Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, this.m_transportInfo.m_class.m_service, this.m_transportInfo.m_class.m_subService, this.m_transportInfo.m_class.m_level);
                //end mod
                if (randomVehicleInfo == null)
                    return;
                Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
                Vector3 position;
                Vector3 target;
                this.CalculateSpawnPosition(buildingID, ref data, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, out position, out target);
                ushort vehicle;
                if (!Singleton<VehicleManager>.instance.CreateVehicle(out vehicle, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, position, reason, false, true))
                    return;
                randomVehicleInfo.m_vehicleAI.SetSource(vehicle, ref vehicles.m_buffer[(int)vehicle], buildingID);
                randomVehicleInfo.m_vehicleAI.StartTransfer(vehicle, ref vehicles.m_buffer[(int)vehicle], reason, offer);
            }
            else if (this.m_secondaryTransportInfo != null && reason == this.m_secondaryTransportInfo.m_vehicleReason)
            {
                //begin mod(*): if some vehicle was forced, use the forced one
                VehicleInfo randomVehicleInfo = forceInfo != null ? forceInfo : Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, this.m_secondaryTransportInfo.m_class.m_service, this.m_secondaryTransportInfo.m_class.m_subService, this.m_secondaryTransportInfo.m_class.m_level);
                //end mod
                if (randomVehicleInfo == null)
                    return;
                Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
                Vector3 position;
                Vector3 target;
                this.CalculateSpawnPosition(buildingID, ref data, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, out position, out target);
                ushort vehicle;
                if (!Singleton<VehicleManager>.instance.CreateVehicle(out vehicle, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, position, reason, false, true))
                    return;
                randomVehicleInfo.m_vehicleAI.SetSource(vehicle, ref vehicles.m_buffer[(int)vehicle], buildingID);
                randomVehicleInfo.m_vehicleAI.StartTransfer(vehicle, ref vehicles.m_buffer[(int)vehicle], reason, offer);
            }
            //begin mod (-): no need to call base
            //end mod
        }

        private static VehicleInfo GetForcedVehicle(ushort buildingID, Building data, TransferManager.TransferReason reason)
        {
            VehicleInfo forceInfo = null;
            if (reason == TransferManager.TransferReason.Taxi &&
                ServiceVehicleSelectorMod.BuildingData.TryGetValue(buildingID, out HashSet<string> source) &&
                source.Count > 0)
            {
                if (reason == ((DepotAI)data.Info.m_buildingAI).m_transportInfo.m_vehicleReason ||
                    reason == ((DepotAI)data.Info.m_buildingAI).m_secondaryTransportInfo.m_vehicleReason)
                {
                    string[] array = source.ToArray<string>();
                    int index = Singleton<SimulationManager>.instance.m_randomizer.Int32((uint) array.Length);
                    string prefabName = array[index];
                    forceInfo = VehicleManagerMod.GetVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer,
                        data.Info.m_class.m_service, data.Info.m_class.m_subService, data.Info.m_class.m_level,
                        buildingID,
                        prefabName);
                }
            }
            return forceInfo;
        }
    }
}