using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Harmony;
using ServiceVehicleSelector2.Util;
using UnityEngine;

namespace ServiceVehicleSelector2.HarmonyPatches
{
    public class PanelExtenderCityServicePatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(PanelExtenderCityService), "OnSelectedPrefabsChanged"),
                null, null,
                new PatchUtil.MethodDefinition(typeof(PanelExtenderCityServicePatch), nameof(Transpile)));
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(new PatchUtil.MethodDefinition(typeof(PanelExtenderCityService),
                "OnSelectedPrefabsChanged"));
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
                    "SVS2: Transpiled PanelExtenderCityService.ReleaseVehicles()");
            }

            return codes.AsEnumerable();
        }
    }
}