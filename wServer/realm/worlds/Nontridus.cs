using wServer.networking;

namespace wServer.realm.worlds
{
    public class Nontridus : World
    {
        public Nontridus()
        {
            Name = "Nontridus";
            ClientWorldName = "Nontridus";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = false;
            SetMusic("dark fallout");
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.nontridus1.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new Nontridus());
    }
}