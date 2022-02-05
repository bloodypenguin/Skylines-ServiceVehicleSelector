using ICities;

namespace ServiceVehicleSelector2
{
    public class BuildingExtension : BuildingExtensionBase
    {
        public override void OnBuildingReleased(ushort id)
        {
            base.OnBuildingReleased(id);
            if (!SerializableDataExtension.BuildingData.Remove(id))
            {
                return;
            }
            Utils.Log($"SVS2 - Demolished building {id} was removed from the building config");
        }
    }
}