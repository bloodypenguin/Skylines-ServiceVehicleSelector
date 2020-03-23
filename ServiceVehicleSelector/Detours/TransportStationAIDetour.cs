using ServiceVehicleSelector2.RedirectionFramework.Attributes;
using UnityEngine;

namespace ServiceVehicleSelector2.Detours
{
    [TargetType(typeof(TransportStationAI))]
    public class TransportStationAIDetour : TransportStationAI
    {
        [RedirectReverse]
        public static void ReleaseVehicles(TransportStationAI ai, ushort buildingID, ref Building data)
        {
            Debug.Log("ReleaseVehicles");
        }
    }
}