using wServer.networking;

namespace wServer.realm.worlds
{
    public class SpiderDen : World
    {
        public SpiderDen()
        {
            Name = "Spider Den";
            ClientWorldName = "dungeons.Spider_Den";
            Background = 0;
            Difficulty = 2;
            AllowTeleport = true;
            SetMusic("spiderden");
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.spiderden.wmap", MapType.Wmap);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new SpiderDen());
    }
}