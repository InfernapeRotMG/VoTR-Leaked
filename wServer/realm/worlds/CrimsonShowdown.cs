using wServer.networking;

namespace wServer.realm.worlds
{
    public class CrimsonShowdown : World
    {
        public CrimsonShowdown()
        {
            Name = "Crimson Showdown";
            ClientWorldName = "Crimson Showdown";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = true;
            SetMusic("crimson");
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.crimson.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new CrimsonShowdown());
    }
}