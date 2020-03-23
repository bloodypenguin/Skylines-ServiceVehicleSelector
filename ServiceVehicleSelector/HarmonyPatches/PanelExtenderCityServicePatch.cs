using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using UnityEngine;

namespace ServiceVehicleSelector2.HarmonyPatches
{
    public class PanelExtenderCityServicePatch
    {
        public static MethodInfo getTranspiler()
        {
            return typeof(PanelExtenderCityServicePatch).GetMethod(nameof(Transpile),
                BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (var index = 0; index < codes.Count; index++)
            {
                var codeInstruction = codes[index];
                if (codeInstruction.opcode != OpCodes.Call || codeInstruction.operand == null ||
                    !codeInstruction.operand.ToString().Contains("ReleaseVehicles"))
                {
                    continue;
                }

                codes[index] = new CodeInstruction(OpCodes.Call,
                    AccessTools.Method(typeof(TransportStationAI), "ReleaseVehicles"));
                Debug.Log(
                    "Service Vehicle Selector 2: Transpiled PanelExtenderCityService.ReleaseVehicles()");
            }

            return codes.AsEnumerable();
        }
    }
}