using wServer.networking;

namespace wServer.realm.worlds
{
    public class IceCave : World
    {
        public IceCave()
        {
            Name = "Ice Cave";
            ClientWorldName = "{dungeons.Ice_Cave}";
            Background = 0;
            Difficulty = 5;
            ShowDisplays = true;
            AllowTeleport = true;
            SetMusic("icecave");
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.icecave.wmap", MapType.Wmap);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new IceCave());
    }
}