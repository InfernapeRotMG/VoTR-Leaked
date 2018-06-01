using wServer.networking;

namespace wServer.realm.worlds
{
    public class CoreoftheHideout : World
    {
        public CoreoftheHideout()
        {
            Name = "Core of the Hideout";
            ClientWorldName = "Core of the Hideout";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = false;
            SetMusic("coreofthehideout");
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.core.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new CrimsonShowdown());
    }
}