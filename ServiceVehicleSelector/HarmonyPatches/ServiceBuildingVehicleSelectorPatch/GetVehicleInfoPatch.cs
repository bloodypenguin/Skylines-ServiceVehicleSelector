using ServiceVehicleSelector2.Util;

namespace ServiceVehicleSelector2.HarmonyPatches.ServiceBuildingVehicleSelectorPatch
{
    public class GetVehicleInfoPatch
    {
        public static void Apply()
        {
            PatchUtil.Patch(
                new PatchUtil.MethodDefinition(typeof(ServiceBuildingVehicleSelector),
                    "GetVehicleInfo"),
                new PatchUtil.MethodDefinition(typeof(GetVehicleInfoPatch),
                    nameof(Prefix))
            );
        }

        public static void Undo()
        {
            PatchUtil.Unpatch(
                new PatchUtil.MethodDefinition(typeof(ServiceBuildingVehicleSelector),
                    "GetVehicleInfo")
            );
        }

        public static bool Prefix(ServiceBuildingVehicleSelector __instance)
        {
            __instance.component.Hide();
            return false;
        }
    }
}