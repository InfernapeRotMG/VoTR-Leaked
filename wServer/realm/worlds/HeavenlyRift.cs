using wServer.networking;

namespace wServer.realm.worlds
{
    public class HeavenlyRift : World
    {
        public HeavenlyRift()
        {
            Name = "Heavenly Rift";
            ClientWorldName = "Heavenly Rift";
            Background = 0;
            Difficulty = 4;
            AllowTeleport = true;
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.heaven.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new HeavenlyRift());
    }
}