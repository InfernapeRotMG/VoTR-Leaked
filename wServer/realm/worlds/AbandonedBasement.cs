using wServer.networking;

namespace wServer.realm.worlds
{
    public class AbandonedBasement : World
    {
        public AbandonedBasement()
        {
            Name = "Abandoned Basement";
            ClientWorldName = "Abandoned Basement";
            Background = 0;
            Difficulty = 4;
            AllowTeleport = true;
            SetMusic("abandoned");
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.basement.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new AbandonedBasement());
    }
}