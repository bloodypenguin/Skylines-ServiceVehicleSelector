using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ColossalFramework;
using ColossalFramework.Math;
using Harmony;
using ServiceVehicleSelector2.Detours;

namespace ServiceVehicleSelector2.HarmonyPatches
{
    public class ServiceBuildingAIPatch
    {
        public static MethodInfo getTranspiler(bool vehicleTypeUsed = false)
        {
            return typeof(ServiceBuildingAIPatch).GetMethod(
                vehicleTypeUsed ? nameof(TranspileWithVehicleType) : nameof(TranspileNoVehicleType),
                BindingFlags.Static | BindingFlags.Public);
        }

        private static IEnumerable<CodeInstruction> TranspileNoVehicleType(IEnumerable<CodeInstruction> instructions)
        {
            return Transpile(instructions, false);
        }

        private static IEnumerable<CodeInstruction> TranspileWithVehicleType(IEnumerable<CodeInstruction> instructions)
        {
            return Transpile(instructions, true);
        }

        private static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instructions,
            bool vehicleTypeUsed)
        {
            var codes = new List<CodeInstruction>(instructions);
            var newCodes = new List<CodeInstruction>();
            var timesReplaced = 0;
            foreach (var codeInstruction in codes)
            {
                if (timesReplaced == 2 || //we only need to replace 2 or 1 occurrences
                    codeInstruction.opcode != OpCodes.Callvirt || codeInstruction.operand == null ||
                    !codeInstruction.operand.ToString().Contains(nameof(VehicleManager.GetRandomVehicleInfo)))
                {
                    newCodes.Add(codeInstruction);
                    continue;
                }

                var methodToCall = AccessTools.Method(typeof(ServiceBuildingAIPatch), nameof(GetRandomVehicleInfo));
                var toRemove = vehicleTypeUsed ? 5 : 4;
                newCodes.RemoveRange(newCodes.Count - toRemove, toRemove); //remove pass level (+vehicleType)
                newCodes.Add(new CodeInstruction(OpCodes.Nop)); //pad      
                newCodes.Add(new CodeInstruction(OpCodes.Nop)); //pad   
                if (vehicleTypeUsed)
                {
                    newCodes.Add(new CodeInstruction(OpCodes.Ldc_I4_1));
                }
                newCodes.Add(new CodeInstruction(OpCodes.Ldarg_1));
                newCodes.Add(new CodeInstruction(OpCodes.Ldarg_2));
                newCodes.Add(new CodeInstruction(OpCodes.Call, methodToCall));
                timesReplaced++;
            }

            return newCodes.AsEnumerable();
        }

        private static VehicleInfo GetRandomVehicleInfo(
            VehicleManager instance,
            ref Randomizer r,
            ItemClass.Service service,
            ItemClass.SubService subService,
            ushort buildingID, ref Building data)
        {
            if (!ServiceVehicleSelectorMod.BuildingData.TryGetValue(buildingID, out var source) || source.Count <= 0)
            {
                return Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref r, data.Info.m_class.m_service,
                    data.Info.m_class.m_subService, data.Info.m_class.m_level);
            }

            var array = source.ToArray();
            return VehicleProvider.GetVehicleInfo(ref r, data.Info.m_class.m_service,
                data.Info.m_class.m_subService, data.Info.m_class.m_level, array[r.Int32((uint) array.Length)]);
        }

        private static VehicleInfo GetRandomVehicleInfo(
            VehicleManager instance,
            ref Randomizer r,
            ItemClass.Service service,
            ItemClass.SubService subService,
            VehicleInfo.VehicleType vehicleType,
            ushort buildingID, ref Building data)
        {
            if (!ServiceVehicleSelectorMod.BuildingData.TryGetValue(buildingID, out var source) || source.Count <= 0)
            {
                return Singleton<VehicleManager>.instance.GetRandomVehicleInfo(ref r, data.Info.m_class.m_service,
                    data.Info.m_class.m_subService, data.Info.m_class.m_level);
            }

            var array = source.ToArray();
            return VehicleProvider.GetVehicleInfo(ref r, data.Info.m_class.m_service,
                data.Info.m_class.m_subService, data.Info.m_class.m_level, array[r.Int32((uint) array.Length)],
                vehicleType);
        }
    }
}