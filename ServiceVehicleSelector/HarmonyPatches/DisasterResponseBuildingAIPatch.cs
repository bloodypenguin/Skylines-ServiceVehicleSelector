using System.Collections.Generic;
using System.Reflection;
using Harmony;

namespace ServiceVehicleSelector2.HarmonyPatches
{
    public class DisasterResponseBuildingAIPatch //we only want to change helicopter for now
    {
        public static MethodInfo getTranspiler()
        {
            return typeof(DisasterResponseBuildingAIPatch).GetMethod(nameof(Transpile),
                BindingFlags.Static | BindingFlags.Public);
        }

        private static IEnumerable<CodeInstruction> Transpile(IEnumerable<CodeInstruction> instructions)
        {
            return ServiceBuildingAIPatch.Transpile(instructions, true, 2, 3);
        }
    }
}