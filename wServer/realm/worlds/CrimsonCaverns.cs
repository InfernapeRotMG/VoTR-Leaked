using wServer.networking;

namespace wServer.realm.worlds
{
    public class CrimsonCaverns : World
    {
        public CrimsonCaverns()
        {
            Name = "Crimson Caverns";
            ClientWorldName = "Crimson Caverns";
            Background = 0;
            Difficulty = 4;
            AllowTeleport = false;
            SetMusic("caverns");
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.crimsoncaverns.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new CrimsonCaverns());
    }
}