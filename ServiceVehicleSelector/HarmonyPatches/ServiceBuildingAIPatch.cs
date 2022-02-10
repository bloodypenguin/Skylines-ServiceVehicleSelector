using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ColossalFramework.Math;
using HarmonyLib;
using ServiceVehicleSelector2.Util;
using UnityEngine;

namespace ServiceVehicleSelector2.HarmonyPatches
{
    public class ServiceBuildingAIPatch
    {
        public static void Apply()
        {
            Transpile(typeof(TransportStationAI), "CreateOutgoingVehicle");
            Transpile(typeof(TransportStationAI), "CreateIncomingVehicle");
            Transpile(typeof(PrivateAirportAI), "CheckVehicles");
            Transpile(typeof(TourBuildingAI), "CheckVehicles");
            Transpile(typeof(PostOfficeAI), nameof(PostOfficeAI.StartTransfer));
            Transpile(typeof(CableCarStationAI), "CreateVehicle");
            Transpile(typeof(LandfillSiteAI), nameof(LandfillSiteAI.StartTransfer));
            Transpile(typeof(CemeteryAI), nameof(CemeteryAI.StartTransfer));
            Transpile(typeof(PoliceStationAI), nameof(PoliceStationAI.StartTransfer));
            Transpile(typeof(HospitalAI), nameof(HospitalAI.StartTransfer));
            Transpile(typeof(SnowDumpAI), nameof(SnowDumpAI.StartTransfer));
            Transpile(typeof(MaintenanceDepotAI), nameof(MaintenanceDepotAI.StartTransfer));
            Transpile(typeof(FireStationAI), nameof(FireStationAI.StartTransfer));
            Transpile(typeof(HelicopterDepotAI), nameof(HelicopterDepotAI.StartTransfer));
            Transpile(typeof(DisasterResponseBuildingAI), nameof(DisasterResponseBuildingAI.StartTransfer));
            Transpile(typeof(FishingHarborAI), nameof(FishingHarborAI.TrySpawnBoat));
            Transpile(typeof(WaterFacilityAI), nameof(WaterFacilityAI.StartTransfer));
        }

        public static void Undo()
        {
            Restore(typeof(TransportStationAI), "CreateOutgoingVehicle");
            Restore(typeof(TransportStationAI), "CreateIncomingVehicle");
            Restore(typeof(PrivateAirportAI), "CheckVehicles");
            Restore(typeof(TourBuildingAI), "CheckVehicles");
            Restore(typeof(PostOfficeAI), nameof(PostOfficeAI.StartTransfer));
            Restore(typeof(CableCarStationAI), "CreateVehicle");
            Restore(typeof(LandfillSiteAI), nameof(LandfillSiteAI.StartTransfer));
            Restore(typeof(CemeteryAI), nameof(CemeteryAI.StartTransfer));
            Restore(typeof(PoliceStationAI), nameof(PoliceStationAI.StartTransfer));
            Restore(typeof(HospitalAI), nameof(HospitalAI.StartTransfer));
            Restore(typeof(SnowDumpAI), nameof(SnowDumpAI.StartTransfer));
            Restore(typeof(MaintenanceDepotAI), nameof(MaintenanceDepotAI.StartTransfer));
            Restore(typeof(FireStationAI), nameof(FireStationAI.StartTransfer));
            Restore(typeof(HelicopterDepotAI), nameof(HelicopterDepotAI.StartTransfer));
            Restore(typeof(DisasterResponseBuildingAI), nameof(DisasterResponseBuildingAI.StartTransfer));
            Restore(typeof(FishingHarborAI), nameof(FishingHarborAI.TrySpawnBoat));
            Restore(typeof(WaterFacilityAI), nameof(WaterFacilityAI.StartTransfer));
        }


        private static IEnumerable<CodeInstruction> TranspileMethod(MethodBase original,
            IEnumerable<CodeInstruction> instructions)
        {
            Debug.Log("SVS2: Transpiling method: " + original.DeclaringType + "." + original);
            if (original.GetParameters()[0].ParameterType != typeof(ushort))
            {
                throw new Exception("SVS2: parameter 0 type is not ushort: " +
                                    original.GetParameters()[0].ParameterType);
            }

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

                ProcessOccurence(declaringType, occurrences, newCodes, codeInstruction);
                occurrences++;
            }

            return newCodes.AsEnumerable();
        }
        
        private static void ProcessOccurence(Type declaringType, int occurrences, List<CodeInstruction> newCodes,
            CodeInstruction codeInstruction)
        {
            var methodToCall =
                AccessTools.Method(typeof(ServiceBuildingAIPatch), nameof(GetVehicleInfoWithoutType));
            int patchIndexOffset; //how many instructions the randomization code includes
            if (declaringType == typeof(TransportStationAI))
            {
                if (occurrences == 0)
                {
                    patchIndexOffset = 14;
                } else if (occurrences == 1)
                {
                    patchIndexOffset = 17;
                } else {
                    newCodes.Add(codeInstruction);
                    return;
                }
                methodToCall = AccessTools.Method(typeof(ServiceBuildingAIPatch), nameof(GetVehicleInfoWithType));
            } else if (declaringType == typeof(CableCarStationAI) ||
                declaringType == typeof(LandfillSiteAI) ||
                declaringType == typeof(CemeteryAI) ||
                declaringType == typeof(PoliceStationAI) ||
                declaringType == typeof(HospitalAI) ||
                declaringType == typeof(SnowDumpAI) ||
                declaringType == typeof(MaintenanceDepotAI) ||
                declaringType == typeof(PrivateAirportAI)
            )
            {
                if (occurrences > 1)
                {
                    newCodes.Add(codeInstruction);
                    return;
                }

                patchIndexOffset = 14;
            }
            else if (declaringType == typeof(PostOfficeAI))
            {
                if (occurrences > 1)
                {
                    newCodes.Add(codeInstruction);
                    return;
                }

                patchIndexOffset = 11;
            }
            else if (declaringType == typeof(FireStationAI) || declaringType == typeof(HelicopterDepotAI))
            {
                if (occurrences > 2)
                {
                    newCodes.Add(codeInstruction);
                    return;
                }

                patchIndexOffset = 15;
                methodToCall = AccessTools.Method(typeof(ServiceBuildingAIPatch), nameof(GetVehicleInfoWithType));
            }
            else if (declaringType == typeof(DisasterResponseBuildingAI))
            {
                if (occurrences < 1 || occurrences > 3)
                {
                    newCodes.Add(codeInstruction);
                    return;
                }

                patchIndexOffset = 15;
                methodToCall = AccessTools.Method(typeof(ServiceBuildingAIPatch), nameof(GetVehicleInfoWithType));
            }
            else if (declaringType == typeof(FishingHarborAI))
            {
                if (occurrences > 2)
                {
                    newCodes.Add(codeInstruction);
                    return;
                }

                patchIndexOffset = 12;
                methodToCall = AccessTools.Method(typeof(ServiceBuildingAIPatch), nameof(GetVehicleInfoWithType));
            }
            else if (declaringType == typeof(WaterFacilityAI) || declaringType == typeof(TourBuildingAI))
            {
                if (occurrences > 0)
                {
                    newCodes.Add(codeInstruction);
                    return;
                }

                patchIndexOffset = 14;
                methodToCall = AccessTools.Method(typeof(ServiceBuildingAIPatch), nameof(GetVehicleInfoWithoutType));
            }
            else
            {
                throw new NotImplementedException("SVS2: unsupported patched type: " +
                                                  declaringType);
            }

            ChangeInstructions(newCodes, methodToCall, patchIndexOffset);
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
            if (!SerializableDataExtension.BuildingData().TryGetValue(buildingID, out var source) || source.Count <= 0)
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
            if (!SerializableDataExtension.BuildingData().TryGetValue(buildingID, out var source) || source.Count <= 0)
            {
                return instance.GetRandomVehicleInfo(ref SimulationManager.instance.m_randomizer, service, subService,
                    level, vehicleType);
            }

            return GetRandomVehicleInfoOverride(ref SimulationManager.instance.m_randomizer, service, subService, level,
                vehicleType,
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

        private static void Transpile(Type type, string methodName)
        {
            PatchUtil.Patch(new PatchUtil.MethodDefinition(type, methodName),
                null, null,
                new PatchUtil.MethodDefinition(typeof(ServiceBuildingAIPatch), nameof(TranspileMethod)));
        }

        private static void Restore(Type type, string methodName)
        {
            PatchUtil.Unpatch(new PatchUtil.MethodDefinition(type, methodName));
        }
    }
}