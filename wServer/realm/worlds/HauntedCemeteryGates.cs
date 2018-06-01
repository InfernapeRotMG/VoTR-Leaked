using wServer.networking;

namespace wServer.realm.worlds
{
    public class HauntedCemeteryGates : World
    {
        public HauntedCemeteryGates()
        {
            Name = "Haunted Cemetery Gates";
            ClientWorldName = "Haunted Cemetery Gates";
            Background = 0;
            Difficulty = 3;
            AllowTeleport = true;
            SetMusic("ceme");
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.ceme2.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new HauntedCemeteryGates());
    }
}