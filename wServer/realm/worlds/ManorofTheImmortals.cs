using wServer.networking;

namespace wServer.realm.worlds
{
    public class ManoroftheImmortals : World
    {
        public ManoroftheImmortals()
        {
            Name = "Manor of the Immortals";
            ClientWorldName = "Manor of the Immortals";
            Background = 0;
            Difficulty = 4;
            AllowTeleport = true;
            SetMusic("manor");
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.manor.wmap", MapType.Wmap);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new ManoroftheImmortals());
    }
}