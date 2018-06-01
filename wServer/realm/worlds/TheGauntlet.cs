using wServer.networking;

namespace wServer.realm.worlds
{
    public class TheGauntlet : World
    {
        public TheGauntlet()
        {
            Name = "The Gauntlet";
            ClientWorldName = "The Gauntlet";
            Background = 0;
            AllowTeleport = false;
            SetMusic("desolate");
            Dungeon = false;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.gauntlet.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new TheGauntlet());
    }
}