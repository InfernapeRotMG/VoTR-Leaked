using wServer.networking;

namespace wServer.realm.worlds
{
    public class HauntedCemeteryFinalBattle : World
    {
        public HauntedCemeteryFinalBattle()
        {
            Name = "Haunted Cemetery Final Battle";
            ClientWorldName = "Haunted Cemetery Final Battle";
            Background = 0;
            Difficulty = 4;
            AllowTeleport = true;
            SetMusic("ceme");
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.cemefinal.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new HauntedCemeteryFinalBattle());
    }
}