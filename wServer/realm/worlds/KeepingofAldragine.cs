using wServer.networking;

namespace wServer.realm.worlds
{
    public class KeepingofAldragine : World
    {
        public KeepingofAldragine()
        {
            Name = "Keeping of Aldragine";
            ClientWorldName = "Keeping of Aldragine";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = false;
            SetMusic("aldragine");
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.keeping.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new KeepingofAldragine());
    }
}