using wServer.networking;

namespace wServer.realm.worlds
{
    public class HiddenTemple : World
    {
        public HiddenTemple()
        {
            Name = "Hidden Temple";
            ClientWorldName = "Hidden Temple";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = false;
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.hiddentemple.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new HiddenTemple());
    }
}