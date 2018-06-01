using wServer.networking;

namespace wServer.realm.worlds
{
    public class TheUnspeakable : World
    {
        public TheUnspeakable()
        {
            Name = "The Unspeakable";
            ClientWorldName = "The Unspeakable";
            Background = 0;
            Difficulty = 4;
            AllowTeleport = true;
            SetMusic("unspeakable");
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.unspeakable.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new TheUnspeakable());
    }
}