using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using ColossalFramework;
using ColossalFramework.Math;
using Harmony;
using UnityEngine;

namespace ServiceVehicleSelector2.HarmonyPatches
{
    public class CargoTruckAIPatch
    {
        public static MethodInfo GetTranspiler()
        {
            return typeof(CargoTruckAIPatch).GetMethod(nameof(Transpile),
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private static IEnumerable<CodeInstruction> Transpile(MethodBase original,
            IEnumerable<CodeInstruction> instructions)
        {
            Debug.Log("Service Vehicle Selector 2: Transpiling method: " + original.DeclaringType + "." + original);
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
                    AccessTools.Method(typeof(CargoTruckAIPatch), nameof(GetTrainVehicleInfo))));
                Debug.Log(
                    "Service Vehicle Selector 2: Transpiled CargoTruckAI.ChangeVehicleType()");
            }

            return newCodes.AsEnumerable();
        }

        private static bool SkipInstruction(CodeInstruction codeInstruction)
        {
            return codeInstruction.opcode != OpCodes.Callvirt || codeInstruction.operand == null ||
                   !codeInstruction.operand.ToString().Contains(nameof(VehicleManager.GetRandomVehicleInfo));
        }

        private static VehicleInfo GetTrainVehicleInfo(
            VehicleManager instance,
            ushort cargoStation1, ushort cargoStation2,
            ItemClass.Service service, ItemClass.SubService subService, ItemClass.Level level)
        {
            var infoFrom = BuildingManager.instance.m_buildings.m_buffer[cargoStation1].Info;
            var infoTo = BuildingManager.instance.m_buildings.m_buffer[cargoStation2].Info;

            var fromOutsideToStation = infoFrom?.m_buildingAI is OutsideConnectionAI &&
                                       infoFrom?.m_class?.m_subService == infoTo?.m_class?.m_subService;
            var cargoStationId = fromOutsideToStation ? cargoStation2 : cargoStation1;
            if (!ServiceVehicleSelectorMod.BuildingData.TryGetValue(cargoStationId, out var source) || source.Count <= 0)
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
    }
}