using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColossalFramework.Math;
using ServiceVehicleSelector2.Util;

namespace ServiceVehicleSelector2.HarmonyPatches.DepotAIPatches
{
    public class CreateVehiclePatch
    {

        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(DepotAI), "CreateVehicle",
                    BindingFlags.Instance | BindingFlags.NonPublic),
                new PatchUtil.MethodDefinition(typeof(CreateVehiclePatch), nameof(Prefix)));
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(new PatchUtil.MethodDefinition(typeof(DepotAI), "CreateVehicle",
                BindingFlags.Instance | BindingFlags.NonPublic));
        }


        private static bool Prefix(DepotAI __instance, ushort buildingID,
            ref VehicleInfo vehicleInfo,
            TransferManager.TransferReason reason)
        {
            if (reason != TransferManager.TransferReason.Taxi && reason != TransferManager.TransferReason.CableCar)
            {
                return true;
            }

            vehicleInfo = GetVehicleInfoForDepot(VehicleManager.instance, buildingID, __instance.m_transportInfo.m_class.m_service,
                __instance.m_transportInfo.m_class.m_subService, __instance.m_transportInfo.m_class.m_level);

            return true;
        }
        
        private static VehicleInfo GetVehicleInfoForDepot( //only for taxi and cable cars - and only for 1 of 4 cases
            VehicleManager instance,
            ushort buildingID,
            ItemClass.Service service,
            ItemClass.SubService subService,
            ItemClass.Level level)
        {
            if (
                service != ItemClass.Service.PublicTransport ||
                subService != ItemClass.SubService.PublicTransportTaxi &&
                subService != ItemClass.SubService.PublicTransportCableCar ||
                !SerializableDataExtension.BuildingData().TryGetValue(buildingID, out var source) || source.Count <= 0)
            {
                return instance.GetRandomVehicleInfo(ref SimulationManager.instance.m_randomizer, service, subService,
                    level);
            }

            return GetRandomVehicleInfoOverride(ref SimulationManager.instance.m_randomizer, service, subService, level,
                source.ToArray());
        }
        
        private static VehicleInfo GetRandomVehicleInfoOverride(ref Randomizer r,
            ItemClass.Service service,
            ItemClass.SubService subService,
            ItemClass.Level level,
            IList<string> array)
        {
            return VehicleProvider.GetVehicleInfo(ref r, service, subService, level,
                array[r.Int32((uint) array.Count)]);
        }
    }
}