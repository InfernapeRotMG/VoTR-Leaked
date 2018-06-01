using wServer.networking;

namespace wServer.realm.worlds
{
    public class WoodlandLabyrinth : World
    {
        public WoodlandLabyrinth()
        {
            Name = "Woodland Labyrinth";
            ClientWorldName = "Woodland Labyrinth";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = true;
            SetMusic("forest");
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.woodlandlabyrinth.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new WoodlandLabyrinth());
    }
}