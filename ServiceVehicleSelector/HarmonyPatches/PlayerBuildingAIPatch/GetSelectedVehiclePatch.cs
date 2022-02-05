using ServiceVehicleSelector2.HarmonyPatches.ServiceBuildingVehicleSelectorPatch;
using ServiceVehicleSelector2.Util;

namespace ServiceVehicleSelector2.HarmonyPatches.PlayerBuildingAIPatch
{
    public class GetSelectedVehiclePatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(PlayerBuildingAI),
                    nameof(PlayerBuildingAI.GetSelectedVehicle)),
                new PatchUtil.MethodDefinition(typeof(GetSelectedVehiclePatch),
                    nameof(Prefix))
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(PlayerBuildingAI),
                    nameof(PlayerBuildingAI.GetSelectedVehicle))
            );
        }

        public static bool Prefix(PlayerBuildingAI __instance, ref VehicleInfo __result)
        {
            __result = null;
            return false;
        }
    }
}