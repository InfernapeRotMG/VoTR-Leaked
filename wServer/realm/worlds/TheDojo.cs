using wServer.networking;

namespace wServer.realm.worlds
{
    public class TheDojo : World
    {
        public TheDojo()
        {
            Name = "The Dojo";
            ClientWorldName = "The Dojo";
            Background = 0;
            Difficulty = 3;
            AllowTeleport = true;
            SetMusic("dojo");
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.dojo.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new TheDojo());
    }
}