using wServer.networking;

namespace wServer.realm.worlds
{
    public class BelladonnasGarden : World
    {
        public BelladonnasGarden()
        {
            Name = "Belladonna's Garden";
            ClientWorldName = "dungeons.BelladonnaAPOSs_Garden";
            Background = 0;
            AllowTeleport = false;
            Difficulty = 5;
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.belladonnasGarden.wmap", MapType.Wmap);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new BelladonnasGarden());
    }
}