using wServer.networking;

namespace wServer.realm.worlds
{
    public class Beachzone : World
    {
        public Beachzone()
        {
            Name = "Beachzone";
            ClientWorldName = "{dungeons.Beachzone}";
            Background = 0;
            Difficulty = 0;
            ShowDisplays = true;
            AllowTeleport = false;
            SetMusic("Nexus2");
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.beachzone.wmap", MapType.Wmap);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new Beachzone());
    }
}