using wServer.networking;

namespace wServer.realm.worlds
{
    public class TheRegolith : World
    {
        public TheRegolith()
        {
            Name = "The Regolith";
            ClientWorldName = "The Regolith";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = false;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.regolith.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new TheRegolith());
    }
}