using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ColossalFramework.Math;
using Harmony;
using UnityEngine;

namespace ServiceVehicleSelector2.HarmonyPatches
{
    public class ServiceBuildingAIPatch
    {
        public static MethodInfo getTranspiler()
        {
            return typeof(ServiceBuildingAIPatch).GetMethod(nameof(Transpile),
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }


        private static IEnumerable<CodeInstruction> Transpile(MethodBase original,
            IEnumerable<CodeInstruction> instructions)
        {
            Debug.Log("Service Vehicle Selector 2: Transpling method: " + original);
            var declaringType = original.DeclaringType;
            var codes = new List<CodeInstruction>(instructions);
            var newCodes = new List<CodeInstruction>();
            var occurrences = 0;
            foreach (var codeInstruction in codes)
            {
                if (SkipInstruction(codeInstruction))
                {
                    newCodes.Add(codeInstruction);
                    continue;
                }

                var vehicleTypeUsed = false;
                if (declaringType == typeof(CableCarStationAI) ||
                    declaringType == typeof(LandfillSiteAI) ||
                    declaringType == typeof(CemeteryAI) ||
                    declaringType == typeof(PoliceStationAI) ||
                    declaringType == typeof(HospitalAI) ||
                    declaringType == typeof(SnowDumpAI) ||
                    declaringType == typeof(MaintenanceDepotAI))
                {
                    if (occurrences > 1)
                    {
                        newCodes.Add(codeInstruction);
                        continue;
                    }
                }
                else if (declaringType == typeof(FireStationAI) || declaringType == typeof(HelicopterDepotAI))
                {
                    if (occurrences > 2)
                    {
                        newCodes.Add(codeInstruction);
                        continue;
                    }

                    vehicleTypeUsed = true;
                }
                else if (declaringType == typeof(DisasterResponseBuildingAI))
                {
                    if (occurrences < 1 || occurrences > 3)
                    {
                        newCodes.Add(codeInstruction);
                        continue;
                    }

                    vehicleTypeUsed = true;
                }

                ChangeInstructions(newCodes, vehicleTypeUsed);
                occurrences++;
            }

            return newCodes.AsEnumerable();
        }

        private static bool SkipInstruction(CodeInstruction codeInstruction)
        {
            return codeInstruction.opcode != OpCodes.Callvirt || codeInstruction.operand == null ||
                   !codeInstruction.operand.ToString().Contains(nameof(VehicleManager.GetRandomVehicleInfo));
        }

        private static void ChangeInstructions(List<CodeInstruction> newCodes, bool vehicleTypeUsed)
        {
            var methodToCall = vehicleTypeUsed
                ? AccessTools.Method(typeof(ServiceBuildingAIPatch), nameof(GetVehicleInfoWithType))
                : AccessTools.Method(typeof(ServiceBuildingAIPatch), nameof(GetVehicleInfoWithoutType));
            var patchIndexOffset = vehicleTypeUsed ? 15 : 14;
            var patchIndex = newCodes.Count - patchIndexOffset;
            newCodes.RemoveRange(patchIndex, 2); //remove randomizer
            newCodes.Insert(patchIndex, new CodeInstruction(OpCodes.Ldarg_1));
            newCodes.Insert(patchIndex + 1, new CodeInstruction(OpCodes.Ldarg_2));
            newCodes.Add(new CodeInstruction(OpCodes.Call, methodToCall));
        }

        private static VehicleInfo GetVehicleInfoWithoutType(
            VehicleManager instance,
            ushort buildingID, ref Building data,
            ItemClass.Service service,
            ItemClass.SubService subService,
            ItemClass.Level level)
        {
            if (!ServiceVehicleSelectorMod.BuildingData.TryGetValue(buildingID, out var source) || source.Count <= 0)
            {
                return instance.GetRandomVehicleInfo(ref SimulationManager.instance.m_randomizer, service, subService,
                    level);
            }

            return GetRandomVehicleInfo(ref SimulationManager.instance.m_randomizer, service, subService, level,
                source.ToArray());
        }

        private static VehicleInfo GetVehicleInfoWithType(
            VehicleManager instance,
            ushort buildingID, ref Building data,
            ItemClass.Service service,
            ItemClass.SubService subService,
            ItemClass.Level level,
            VehicleInfo.VehicleType vehicleType)
        {
            if (!ServiceVehicleSelectorMod.BuildingData.TryGetValue(buildingID, out var source) || source.Count <= 0)
            {
                return instance.GetRandomVehicleInfo(ref SimulationManager.instance.m_randomizer, service, subService,
                    level, vehicleType);
            }

            return GetRandomVehicleInfo(ref SimulationManager.instance.m_randomizer, service, subService, level,
                vehicleType,
                source.ToArray());
        }


        private static VehicleInfo GetRandomVehicleInfo(ref Randomizer r,
            ItemClass.Service service,
            ItemClass.SubService subService,
            ItemClass.Level level,
            IList<string> array)
        {
            return VehicleProvider.GetVehicleInfo(ref r, service, subService, level,
                array[r.Int32((uint) array.Count)]);
        }

        private static VehicleInfo GetRandomVehicleInfo(ref Randomizer r,
            ItemClass.Service service,
            ItemClass.SubService subService,
            ItemClass.Level level,
            VehicleInfo.VehicleType vehicleType,
            IList<string> array)
        {
            return VehicleProvider.GetVehicleInfo(ref r, service, subService, level,
                array[r.Int32((uint) array.Count)], vehicleType);
        }
    }
}