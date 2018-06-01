using wServer.networking;

namespace wServer.realm.worlds
{
    public class AdminsArena : World
    {
        public AdminsArena()
        {
            Name = "Admins Arena";
            ClientWorldName = "Admins Arena";
            Background = 0;
            Difficulty = 0;
            AllowTeleport = true;
            SetMusic("Nexus2");
            Dungeon = false;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.adminsarena.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new AdminsArena());
    }
}