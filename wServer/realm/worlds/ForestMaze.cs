using wServer.networking;

namespace wServer.realm.worlds
{
    public class ForestMaze : World
    {
        public ForestMaze()
        {
            Name = "Forest Maze";
            ClientWorldName = "dungeons.Forest_Maze";
            Background = 0;
            Difficulty = 1;
            AllowTeleport = true;
            SetMusic("forest");
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.forestmaze.wmap", MapType.Wmap);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new ForestMaze());
    }
}