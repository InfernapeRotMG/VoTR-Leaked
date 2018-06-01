namespace wServer.realm.worlds
{
    public class StormPalace : World
    {
        public StormPalace()
        {
            Name = "Storm Palace";
            ClientWorldName = "Storm Palace";
            Background = 0;
            Difficulty = 3;
            AllowTeleport = true;
            SetMusic("stormypalace");
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.storm.jm", MapType.Json);
        }
    }
}