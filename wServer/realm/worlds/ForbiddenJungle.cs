using wServer.networking;

namespace wServer.realm.worlds
{
    public class ForbiddenJungle : World
    {
        public ForbiddenJungle()
        {
            Name = "Forbidden Jungle";
            ClientWorldName = "Forbidden Jungle";
            Background = 0;
            Difficulty = 2;
            AllowTeleport = true;
            SetMusic("jungle");
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.jungles.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new ForbiddenJungle());
    }
}