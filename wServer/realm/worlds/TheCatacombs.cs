using wServer.networking;

namespace wServer.realm.worlds
{
    public class TheCatacombs : World
    {
        public TheCatacombs()
        {
            Name = "The Catacombs";
            ClientWorldName = "The Catacombs";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = false;
            SetMusic("catacombs");
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.catacombs.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new TheCatacombs());
    }
}