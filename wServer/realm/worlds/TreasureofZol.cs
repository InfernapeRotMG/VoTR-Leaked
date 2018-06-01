using wServer.networking;

namespace wServer.realm.worlds
{
    public class TreasureofZol : World
    {
        public TreasureofZol()
        {
            Name = "Treasure of Zol";
            ClientWorldName = "Treasure of Zol";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = false;
            SetMusic("safe");
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.keepingtroom.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new TreasureofZol());
    }
}