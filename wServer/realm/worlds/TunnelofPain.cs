using wServer.networking;

namespace wServer.realm.worlds
{
    public class TunnelofPain : World
    {
        public TunnelofPain()
        {
            Name = "Tunnel of Pain";
            ClientWorldName = "Tunnel of Pain";
            Background = 0;
            Difficulty = 4;
            AllowTeleport = true;
            SetMusic("tunnel");
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.tunnel.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new TunnelofPain());
    }
}