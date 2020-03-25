using System;
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
        public static MethodInfo GetTranspiler()
        {
            return typeof(ServiceBuildingAIPatch).GetMethod(nameof(Transpile),
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }


        private static IEnumerable<CodeInstruction> Transpile(MethodBase original,
            IEnumerable<CodeInstruction> instructions)
        {
            Debug.Log("Service Vehicle Selector 2: Transpiling method: " + original.DeclaringType + "." + original);
            if (original.GetParameters()[0].ParameterType != typeof(ushort))
            {
                throw new Exception("Service Vehicle Selector 2: parameter 0 type is not ushort: " +
                                    original.GetParameters()[0].ParameterType);
            }

            var declaringType = original.DeclaringType;
            var codes = new List<CodeInstruction>(instructions);
            var newCodes = new List<CodeInstruction>();
            var occurrences = 0;
            var patchIndexOffset = 14;
            foreach (var codeInstruction in codes)
            {
                if (SkipInstruction(codeInstruction))
                {
                    newCodes.Add(codeInstruction);
                    continue;
                }

                var methodToCall =
                    AccessTools.Method(typeof(ServiceBuildingAIPatch), nameof(GetVehicleInfoWithoutType));
                if (declaringType == typeof(CableCarStationAI) ||
                    declaringType == typeof(LandfillSiteAI) ||
                    declaringType == typeof(CemeteryAI) ||
                    declaringType == typeof(PoliceStationAI) ||
                    declaringType == typeof(HospitalAI) ||
                    declaringType == typeof(SnowDumpAI) ||
                    declaringType == typeof(MaintenanceDepotAI) ||
                    declaringType == typeof(TransportStationAI)
                )
                {
                    if (occurrences > 1)
                    {
                        newCodes.Add(codeInstruction);
                        continue;
                    }
                }
                else if (declaringType == typeof(DepotAI))
                {
                    if (occurrences != 1)
                    {
                        newCodes.Add(codeInstruction);
                        continue;
                    }

                    methodToCall = AccessTools.Method(typeof(ServiceBuildingAIPatch), nameof(GetVehicleInfoForDepot));
                }
                else if (declaringType == typeof(PostOfficeAI))
                {
                    if (occurrences > 1)
                    {
                        newCodes.Add(codeInstruction);
                        continue;
                    }

                    patchIndexOffset = 11;
                }
                else if (declaringType == typeof(FireStationAI) || declaringType == typeof(HelicopterDepotAI))
                {
                    if (occurrences > 2)
                    {
                        newCodes.Add(codeInstruction);
                        continue;
                    }

                    patchIndexOffset = 15;
                    methodToCall = AccessTools.Method(typeof(ServiceBuildingAIPatch), nameof(GetVehicleInfoWithType));
                }
                else if (declaringType == typeof(DisasterResponseBuildingAI))
                {
                    if (occurrences < 1 || occurrences > 3)
                    {
                        newCodes.Add(codeInstruction);
                        continue;
                    }

                    patchIndexOffset = 15;
                    methodToCall = AccessTools.Method(typeof(ServiceBuildingAIPatch), nameof(GetVehicleInfoWithType));
                }
                else if (declaringType == typeof(FishingHarborAI))
                {
                    if (occurrences > 2)
                    {
                        newCodes.Add(codeInstruction);
                        continue;
                    }

                    patchIndexOffset = 12;
                    methodToCall = AccessTools.Method(typeof(ServiceBuildingAIPatch), nameof(GetVehicleInfoWithType));
                }
                else
                {
                    throw new NotImplementedException("Service Vehicle Selector 2: unsupported patched type: " +
                                                      declaringType);
                }

                ChangeInstructions(newCodes, methodToCall, patchIndexOffset);
                occurrences++;
            }

            return newCodes.AsEnumerable();
        }

        private static bool SkipInstruction(CodeInstruction codeInstruction)
        {
            return codeInstruction.opcode != OpCodes.Callvirt || codeInstruction.operand == null ||
                   !codeInstruction.operand.ToString().Contains(nameof(VehicleManager.GetRandomVehicleInfo));
        }

        private static void ChangeInstructions(List<CodeInstruction> newCodes, MethodInfo methodToCall,
            int patchIndexOffset)
        {
            var patchIndex = newCodes.Count - patchIndexOffset;
            newCodes.RemoveRange(patchIndex, 2); //remove randomizer
            newCodes.Insert(patchIndex, new CodeInstruction(OpCodes.Ldarg_1));
            newCodes.Insert(patchIndex + 1, new CodeInstruction(OpCodes.Nop)); //pad
            newCodes.Add(new CodeInstruction(OpCodes.Call, methodToCall));
        }

        private static VehicleInfo GetVehicleInfoWithoutType(
            VehicleManager instance,
            ushort buildingID,
            ItemClass.Service service,
            ItemClass.SubService subService,
            ItemClass.Level level)
        {
            if (!ServiceVehicleSelectorMod.BuildingData.TryGetValue(buildingID, out var source) || source.Count <= 0)
            {
                return instance.GetRandomVehicleInfo(ref SimulationManager.instance.m_randomizer, service, subService,
                    level);
            }

            return GetRandomVehicleInfoOverride(ref SimulationManager.instance.m_randomizer, service, subService, level,
                source.ToArray());
        }

        private static VehicleInfo GetVehicleInfoWithType(
            VehicleManager instance,
            ushort buildingID,
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

            return GetRandomVehicleInfoOverride(ref SimulationManager.instance.m_randomizer, service, subService, level,
                vehicleType,
                source.ToArray());
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
                !ServiceVehicleSelectorMod.BuildingData.TryGetValue(buildingID, out var source) || source.Count <= 0)
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

        private static VehicleInfo GetRandomVehicleInfoOverride(ref Randomizer r,
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