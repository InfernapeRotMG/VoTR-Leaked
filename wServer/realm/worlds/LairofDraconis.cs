using wServer.networking;

namespace wServer.realm.worlds
{
    public class LairofDraconis : World
    {
        public LairofDraconis()
        {
            Name = "Lair of Draconis";
            ClientWorldName = "Lair of Draconis";
            Background = 0;
            Difficulty = 4;
            AllowTeleport = false;
            SetMusic("draconis");
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.draconis.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new LairofDraconis());
    }
}