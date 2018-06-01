using wServer.networking;

namespace wServer.realm.worlds
{
    public class SnakePit : World
    {
        public SnakePit()
        {
            Name = "Snake Pit";
            ClientWorldName = "dungeons.Snake_Pit";
            Dungeon = true;
            Background = 0;
            AllowTeleport = true;
            SetMusic("snakepit");
            Difficulty = 1;
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.snakepit.wmap", MapType.Wmap);
        }

        public override World GetInstance(Client client)
        {
            return Manager.AddWorld(new SnakePit());
        }
    }
}