using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ServiceVehicleSelector2.RedirectionFramework.Attributes;

namespace ServiceVehicleSelector2.Detours
{
    [TargetType(typeof(LandfillSiteAI))]
    public class LandfillSiteAIDetour : LandfillSiteAI
    {
        [RedirectMethod]
        public override void StartTransfer(ushort buildingID, ref Building data, TransferManager.TransferReason material, TransferManager.TransferOffer offer)
        {
            ItemClass.SubService subService = ItemClass.SubService.None;
            if (material != TransferManager.TransferReason.Lumber)
            {
                if (material != TransferManager.TransferReason.GarbageMove)
                {
                    if (material != TransferManager.TransferReason.Garbage)
                    {
                        if (material != TransferManager.TransferReason.Coal)
                        {
                            if (material == TransferManager.TransferReason.Petrol)
                                subService = ItemClass.SubService.IndustrialOil;
                            else
                                base.StartTransfer(buildingID, ref data, material, offer);
                        }
                        else
                            subService = ItemClass.SubService.IndustrialOre;
                    }
                    else
                    {
                        VehicleInfo randomVehicleInfo;
                        //begin mod
                        HashSet<string> source;
                        if (ServiceVehicleSelectorMod.BuildingData.TryGetValue(buildingID, out source) && source.Count > 0)
                        {
                            string[] array = source.ToArray<string>();
                            int index = Singleton<SimulationManager>.instance.m_randomizer.Int32((uint)array.Length);
                            string prefabName = array[index];
                            randomVehicleInfo = VehicleManagerMod.GetVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, data.Info.m_class.m_service, data.Info.m_class.m_subService, data.Info.m_class.m_level, prefabName);
                        }
                        else
                            //end mod
                            randomVehicleInfo = Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, this.m_info.m_class.m_service, this.m_info.m_class.m_subService, this.m_info.m_class.m_level);
                        if (randomVehicleInfo != null)
                        {
                            Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
                            ushort vehicle;
                            if (Singleton<VehicleManager>.instance.CreateVehicle(out vehicle, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, data.m_position, material, true, false))
                            {
                                randomVehicleInfo.m_vehicleAI.SetSource(vehicle, ref vehicles.m_buffer[(int)vehicle], buildingID);
                                randomVehicleInfo.m_vehicleAI.StartTransfer(vehicle, ref vehicles.m_buffer[(int)vehicle], material, offer);
                            }
                        }
                    }
                }
                else
                {
                    VehicleInfo randomVehicleInfo;
                    //begin mod
                    HashSet<string> source;
                    if (ServiceVehicleSelectorMod.BuildingData.TryGetValue(buildingID, out source) && source.Count > 0)
                    {
                        string[] array = source.ToArray<string>();
                        int index = Singleton<SimulationManager>.instance.m_randomizer.Int32((uint)array.Length);
                        string prefabName = array[index];
                        randomVehicleInfo = VehicleManagerMod.GetVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, data.Info.m_class.m_service, data.Info.m_class.m_subService, data.Info.m_class.m_level, prefabName);
                    }
                    else
                        //end mod
                        randomVehicleInfo = Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, this.m_info.m_class.m_service, this.m_info.m_class.m_subService, this.m_info.m_class.m_level);
                if (randomVehicleInfo != null)
                    {
                        Array16<Vehicle> vehicles = Singleton<VehicleManager>.instance.m_vehicles;
                        ushort vehicle;
                        if (Singleton<VehicleManager>.instance.CreateVehicle(out vehicle, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo, data.m_position, material, false, true))
                        {
                            randomVehicleInfo.m_vehicleAI.SetSource(vehicle, ref vehicles.m_buffer[(int)vehicle], buildingID);
                            randomVehicleInfo.m_vehicleAI.StartTransfer(vehicle, ref vehicles.m_buffer[(int)vehicle], material, offer);
                        }
                    }
                }
            }
            else
                subService = ItemClass.SubService.IndustrialForestry;
            if (subService == ItemClass.SubService.None)
                return;
            VehicleInfo randomVehicleInfo1 = Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref Singleton<SimulationManager>.instance.m_randomizer, ItemClass.Service.Industrial, subService, ItemClass.Level.Level1);
            if (randomVehicleInfo1 == null)
                return;
            Array16<Vehicle> vehicles1 = Singleton<VehicleManager>.instance.m_vehicles;
            ushort vehicle1;
            if (!Singleton<VehicleManager>.instance.CreateVehicle(out vehicle1, ref Singleton<SimulationManager>.instance.m_randomizer, randomVehicleInfo1, data.m_position, material, false, true))
                return;
            randomVehicleInfo1.m_vehicleAI.SetSource(vehicle1, ref vehicles1.m_buffer[(int)vehicle1], buildingID);
            randomVehicleInfo1.m_vehicleAI.StartTransfer(vehicle1, ref vehicles1.m_buffer[(int)vehicle1], material, offer);
            ushort building = offer.Building;
            if ((int)building != 0 && (Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)building].m_flags & Building.Flags.IncomingOutgoing) != Building.Flags.None)
            {
                int size;
                int max;
                randomVehicleInfo1.m_vehicleAI.GetSize(vehicle1, ref vehicles1.m_buffer[(int)vehicle1], out size, out max);
                CommonBuildingAI.ExportResource(buildingID, ref data, material, size);
            }
            data.m_outgoingProblemTimer = (byte)0;
        }
    }
}