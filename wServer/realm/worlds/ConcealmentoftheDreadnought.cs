using wServer.networking;

namespace wServer.realm.worlds
{
    public class ConcealmentoftheDreadnought : World
    {
        public ConcealmentoftheDreadnought()
        {
            Name = "Concealment of the Dreadnought";
            ClientWorldName = "Concealment of the Dreadnought";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = false;
            SetMusic("concealment");
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.concealment.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new ConcealmentoftheDreadnought());
    }
}