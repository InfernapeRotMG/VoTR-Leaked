using wServer.networking;

namespace wServer.realm.worlds
{
    public class SincryersGate : World
    {
        public SincryersGate()
        {
            Name = "Sincryer's Gate";
            ClientWorldName = "Sincryer's Gate";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = false;
            SetMusic("domain");
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.sincryersgate.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new SincryersGate());
    }
}