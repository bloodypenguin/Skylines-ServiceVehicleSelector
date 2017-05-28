using ColossalFramework;
using ColossalFramework.Math;
using ServiceVehicleSelector2.RedirectionFramework.Attributes;
using UnityEngine;

namespace ServiceVehicleSelector2.Detours
{
    [TargetType(typeof(TransportStationAI))]
    public class TransportStationAIDetour : TransportStationAI
    {
        [RedirectMethod]
        private bool CreateOutgoingVehicle(ushort buildingID, ref Building buildingData, ushort startStop,
            int gateIndex)
        {
            if (this.m_transportLineInfo != null &&
                (int) FindConnectionVehicle(this, buildingID, ref buildingData, startStop, 3000f) == 0)
            {
                //begin mod(*): force vehicle info
                var forceInfo = DepotAIDetour.GetVehicleInfo(buildingID, buildingData);
                VehicleInfo randomVehicleInfo = forceInfo != null ? forceInfo : Singleton<VehicleManager>.instance.GetRandomVehicleInfo(
                    ref Singleton<SimulationManager>.instance.m_randomizer, this.m_transportLineInfo.m_class.m_service,
                    this.m_transportLineInfo.m_class.m_subService, this.m_transportLineInfo.m_class.m_level);
                //end mod
                if (randomVehicleInfo != null)
                {
                    Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
                    Vector3 position;
                    Vector3 target;
                    var randomizer = new Randomizer()
                    {
                        seed = (ulong) gateIndex
                    };
                    this.CalculateSpawnPosition(buildingID, ref buildingData, ref randomizer, randomVehicleInfo,
                        out position, out target);
                    TransportInfo transportInfo = this.m_transportInfo;
                    if (this.m_secondaryTransportInfo != null && this.m_secondaryTransportInfo.m_class.m_subService ==
                        this.m_transportLineInfo.m_class.m_subService)
                        transportInfo = this.m_secondaryTransportInfo;
                    ushort vehicle;
                    if (randomVehicleInfo.m_vehicleAI.CanSpawnAt(position) &&
                        Singleton<VehicleManager>.instance.CreateVehicle(out vehicle,
                            ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, position,
                            transportInfo.m_vehicleReason, false, true))
                    {
                        vehicles.m_buffer[(int) vehicle].m_gateIndex = (byte) gateIndex;
                        vehicles.m_buffer[(int) vehicle].m_flags |= Vehicle.Flags.Importing | Vehicle.Flags.Exporting;
                        randomVehicleInfo.m_vehicleAI.SetSource(vehicle, ref vehicles.m_buffer[(int) vehicle],
                            buildingID);
                        randomVehicleInfo.m_vehicleAI.SetTarget(vehicle, ref vehicles.m_buffer[(int) vehicle],
                            startStop);
                        return true;
                    }
                }
            }
            return false;
        }

        [RedirectMethod]
        private bool CreateIncomingVehicle(ushort buildingID, ref Building buildingData, ushort startStop,
            int gateIndex)
        {
            if (this.m_transportLineInfo != null &&
                (int) FindConnectionVehicle(this, buildingID, ref buildingData, startStop, 3000f) == 0)
            {
                //begin mod(*): force vehicle info
                var forceInfo = DepotAIDetour.GetVehicleInfo(buildingID, buildingData);
                    VehicleInfo randomVehicleInfo = forceInfo != null ? forceInfo : Singleton<VehicleManager>.instance.GetRandomVehicleInfo(
                    ref Singleton<SimulationManager>.instance.m_randomizer, this.m_transportLineInfo.m_class.m_service,
                    this.m_transportLineInfo.m_class.m_subService, this.m_transportLineInfo.m_class.m_level);
                //end mod
                if (randomVehicleInfo != null)
                {
                    ushort connectionBuilding = FindConnectionBuilding(this, startStop);
                    if ((int) connectionBuilding != 0)
                    {
                        Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
                        Vector3 position;
                        Vector3 target;
                        var randomizer = new Randomizer()
                        {
                            seed = (ulong) gateIndex
                        };
                        Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) connectionBuilding]
                            .Info.m_buildingAI.CalculateSpawnPosition(connectionBuilding,
                                ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int) connectionBuilding],
                                ref randomizer, randomVehicleInfo, out position, out target);
                        TransportInfo transportInfo = this.m_transportInfo;
                        if (this.m_secondaryTransportInfo != null &&
                            this.m_secondaryTransportInfo.m_class.m_subService ==
                            this.m_transportLineInfo.m_class.m_subService)
                            transportInfo = this.m_secondaryTransportInfo;
                        ushort vehicle;
                        if (randomVehicleInfo.m_vehicleAI.CanSpawnAt(position) &&
                            Singleton<VehicleManager>.instance.CreateVehicle(out vehicle,
                                ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, position,
                                transportInfo.m_vehicleReason, true, false))
                        {
                            vehicles.m_buffer[(int) vehicle].m_gateIndex = (byte) gateIndex;
                            vehicles.m_buffer[(int) vehicle].m_flags |=
                                Vehicle.Flags.Importing | Vehicle.Flags.Exporting;
                            randomVehicleInfo.m_vehicleAI.SetSource(vehicle, ref vehicles.m_buffer[(int) vehicle],
                                connectionBuilding);
                            randomVehicleInfo.m_vehicleAI.SetSource(vehicle, ref vehicles.m_buffer[(int) vehicle],
                                buildingID);
                            randomVehicleInfo.m_vehicleAI.SetTarget(vehicle, ref vehicles.m_buffer[(int) vehicle],
                                startStop);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        [RedirectReverse]
        private static ushort FindConnectionVehicle(TransportStationAI ai, ushort buildingID, ref Building buildingData,
            ushort targetStop, float maxDistance)
        {
            UnityEngine.Debug.Log("FindConnectionVehicle");
            return 0;
        }

        [RedirectReverse]
        private static ushort FindConnectionBuilding(TransportStationAI ai, ushort stop)
        {
            UnityEngine.Debug.Log("FindConnectionBuilding");
            return 0;
        }
    }
}