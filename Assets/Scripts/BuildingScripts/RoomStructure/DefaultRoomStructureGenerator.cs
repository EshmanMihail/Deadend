using System;

namespace Assets.Scripts.BuildingScripts.RoomStructure
{
    public abstract class DefaultRoomStructureGenerator
    {
        protected abstract double ChanceToMakeSidePlatformOfWalls { get; set; }
        protected abstract double ChanceToMakeRoom { get; set; }
        protected abstract double ChanceToMakePlatforms { get; set; }

        private (double chanceToHapen, Action action)[] listOfDefaultStructures { get; }

        public DefaultRoomStructureGenerator()
        {
            ChanceToMakeSidePlatformOfWalls = 0.4f;
            ChanceToMakeRoom = 0.8f;
            ChanceToMakePlatforms = 0.5f;

            listOfDefaultStructures = new (double, Action)[]
            {
                (ChanceToMakeSidePlatformOfWalls, MakeSidePlatformOfWalls),
                (ChanceToMakeRoom, MakeRoom),
                (ChanceToMakePlatforms, MakePlatForms)
            };
        }

        private void MakeSidePlatformOfWalls()
        {
            
        }

        private void MakeRoom()
        {
            
        }

        private void MakePlatForms()
        {
            
        }
    }
}
