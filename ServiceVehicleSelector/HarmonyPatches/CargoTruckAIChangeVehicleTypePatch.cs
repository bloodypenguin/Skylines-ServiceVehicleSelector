using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ColossalFramework;
using ColossalFramework.Math;
using HarmonyLib;
using ServiceVehicleSelector2.Util;
using UnityEngine;

namespace ServiceVehicleSelector2.HarmonyPatches
{
    public class CargoTruckAIChangeVehicleTypePatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(CargoTruckAI), nameof(CargoTruckAI.ChangeVehicleType),
                    BindingFlags.Static | BindingFlags.Public),
                null, null,
                new PatchUtil.MethodDefinition(typeof(CargoTruckAIChangeVehicleTypePatch), (nameof(Transpile))));
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(new PatchUtil.MethodDefinition(typeof(CargoTruckAI),
                nameof(CargoTruckAI.ChangeVehicleType),
                BindingFlags.Static | BindingFlags.Public));
        }

        private static IEnumerable<CodeInstruction> Transpile(MethodBase original,
            IEnumerable<CodeInstruction> instructions)
        {
            Debug.Log("SVS2: Transpiling method: " + original.DeclaringType + "." + original);
            var codes = new List<CodeInstruction>(instructions);
            var newCodes = new List<CodeInstruction>();
            foreach (var codeInstruction in codes)
            {
                if (SkipInstruction(codeInstruction))
                {
                    newCodes.Add(codeInstruction);
                    continue;
                }

                var patchIndex = newCodes.Count - 9;
                newCodes.RemoveRange(patchIndex, 2); //remove randomizer
                newCodes.Insert(patchIndex, new CodeInstruction(OpCodes.Ldloc_S, 6));
                newCodes.Insert(patchIndex + 1, new CodeInstruction(OpCodes.Ldloc_S, 7));
                newCodes.Add(new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(CargoTruckAIChangeVehicleTypePatch), nameof(GetCargoVehicleInfo))));
                Debug.Log(
                    "SVS2: Transpiled CargoTruckAI.ChangeVehicleType()");
            }

            return newCodes.AsEnumerable();
        }

        private static bool SkipInstruction(CodeInstruction codeInstruction)
        {
            return codeInstruction.opcode != OpCodes.Callvirt || codeInstruction.operand == null ||
                   !codeInstruction.operand.ToString().Contains(nameof(VehicleManager.GetRandomVehicleInfo));
        }

        private static VehicleInfo GetCargoVehicleInfo(
            VehicleManager instance,
            ushort fromBuilding, ushort toBuilding,
            ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
        {
            var infoFrom = BuildingManager.instance.m_buildings.m_buffer[fromBuilding].Info;

            var fromOutsideToStation = infoFrom?.m_buildingAI is OutsideConnectionAI;
            var buildingDataId = fromOutsideToStation ? toBuilding : fromBuilding; //if from station to station then source station settings are used
            if (infoFrom?.m_class?.name == "Ferry Cargo Facility") //to support Cargo Ferries
            {
                level = ItemClass.Level.Level5;
            }
            
            if (!SerializableDataExtension.BuildingData(GetBuildingDataIndex(buildingDataId, service, subService, level))
                    .TryGetValue(buildingDataId, out var source) ||
                source.Count <= 0)
            {
                return instance.GetRandomVehicleInfo(
                    ref Singleton<SimulationManager>.instance.m_randomizer, service, subService, level);
            }

            var array = source.ToArray();
            return GetRandomVehicleInfoOverride(ref SimulationManager.instance.m_randomizer, service, subService, level,
                array);
        }

        private static VehicleInfo GetRandomVehicleInfoOverride(ref Randomizer r, ItemClass.Service service,
            ItemClass.SubService subService, ItemClass.Level level, IList<string> array)
        {
            return VehicleProvider.GetVehicleInfo(ref r, service, subService, level,
                array[r.Int32((uint) array.Count)]);
        }
        
        private static int GetBuildingDataIndex(ushort buildingId,
            ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
        {
            var buildingInfo = BuildingManager.instance.m_buildings.m_buffer[buildingId].Info;
            var buildingAi = buildingInfo?.m_buildingAI;
            if (buildingAi == null)
            {
                return 0;
            }
            
            if (buildingAi is CargoStationAI)
            {
                var cargoStationTransportInfos = GetCargoStationTransportInfos(buildingInfo);
                //TODO: take levels into account
                if (cargoStationTransportInfos.Secondary != null && cargoStationTransportInfos.Secondary?.m_class
                                                                     .m_service == service
                                                                 && cargoStationTransportInfos.Secondary?.m_class
                                                                     .m_subService == subService)
                {
                    return 1;
                }

                return 0;
            }
            
            //TODO: add support for post office 
            return 0;
        }

        public static TransportInfos GetCargoStationTransportInfos(BuildingInfo buildingInfo)
        {
            var cargoStationAI = buildingInfo?.m_buildingAI as CargoStationAI;
            if (cargoStationAI == null)
            {
                throw new Exception($"The AI of {buildingInfo.name} is not a CargoStationAI");
            }
            var itemClass = buildingInfo.m_class;
            //we cannot check for item class equality as cargo airport item class has different level than its primary transport info's class
            //TODO: support case when just level differs. Ex: cargo plane to cargo helicopter hub
            if (cargoStationAI.m_transportInfo2 == null || cargoStationAI.m_transportInfo?.m_class.m_service == itemClass.m_service && cargoStationAI.m_transportInfo?.m_class.m_subService == itemClass.m_subService)
            {
                return new TransportInfos(cargoStationAI.m_transportInfo, cargoStationAI.m_transportInfo2);
            }

            return new TransportInfos(cargoStationAI.m_transportInfo2, cargoStationAI.m_transportInfo);
        }
        
        
        public struct TransportInfos
        {
            public TransportInfos(TransportInfo primary, TransportInfo secondary)
            {
                Primary = primary;
                Secondary = secondary;
            }

            public TransportInfo Primary { get;  }
            public TransportInfo Secondary { get;  }
        }
    }
}