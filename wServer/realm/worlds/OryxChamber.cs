namespace wServer.realm.worlds
{
    public class OryxChamber : World
    {
        public OryxChamber()
        {
            Name = "Oryx's Chamber";
            ClientWorldName = "Oryx's Chamber";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = false;
            SetMusic("oryxchamber");
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.oryxchamber.jm", MapType.Json);
        }
    }
}