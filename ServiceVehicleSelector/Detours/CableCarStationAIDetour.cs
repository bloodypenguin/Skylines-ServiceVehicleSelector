using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ServiceVehicleSelector2.RedirectionFramework.Attributes;
using UnityEngine;

namespace ServiceVehicleSelector2.Detours
{
    [TargetType(typeof(CableCarStationAI))]
    public class CableCarStationAIDetour : CableCarStationAI
    {
        [RedirectMethod]
        private ushort CreateVehicle(ushort buildingID, ref Building buildingData, ushort sourceNode, ushort targetNode)
        {
            //begin mod
            VehicleManager instance = Singleton<VehicleManager>.instance;
            VehicleInfo forceInfo = null;
            if (ServiceVehicleSelectorMod.BuildingData.TryGetValue(buildingID, out HashSet<string> source) &&
                source.Count > 0)
            {
                string[] array = source.ToArray<string>();
                int index = Singleton<SimulationManager>.instance.m_randomizer.Int32((uint) array.Length);
                string prefabName = array[index];
                forceInfo = VehicleManagerMod.GetVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer,
                    buildingData.Info.m_class.m_service, buildingData.Info.m_class.m_subService,
                    buildingData.Info.m_class.m_level,
                    buildingID,
                    prefabName);
            }
            VehicleInfo randomVehicleInfo = forceInfo != null ? forceInfo : instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, this.m_info.m_class.m_service, this.m_info.m_class.m_subService, this.m_info.m_class.m_level);
            //end mod
            if (randomVehicleInfo != null)
            {
                Vector3 position = Singleton<NetManager>.instance.m_nodes.m_buffer[(int)sourceNode].m_position;
                ushort vehicle;
                if (randomVehicleInfo.m_vehicleAI.CanSpawnAt(position) && instance.CreateVehicle(out vehicle, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, position, this.m_transportInfo.m_vehicleReason, false, true))
                {
                    randomVehicleInfo.m_vehicleAI.SetSource(vehicle, ref instance.m_vehicles.m_buffer[(int)vehicle], buildingID);
                    randomVehicleInfo.m_vehicleAI.SetTarget(vehicle, ref instance.m_vehicles.m_buffer[(int)vehicle], targetNode);
                    return vehicle;
                }
            }
            return 0;
        }
    }
}