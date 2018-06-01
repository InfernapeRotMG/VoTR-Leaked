using wServer.networking;

namespace wServer.realm.worlds
{
    public class TheHollows : World
    {
        public TheHollows()
        {
            Name = "The Hollows";
            ClientWorldName = "The Hollows";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = false;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.thehollows.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new TheHollows());
    }
}