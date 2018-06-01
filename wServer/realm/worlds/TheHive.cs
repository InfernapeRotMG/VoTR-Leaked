using wServer.networking;

namespace wServer.realm.worlds
{
    public class TheHive : World
    {
        public TheHive()
        {
            Name = "The Hive";
            ClientWorldName = "The Hive";
            Background = 0;
            Difficulty = 4;
            AllowTeleport = true;
            SetMusic("hive");
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.hive.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new TheHive());
    }
}