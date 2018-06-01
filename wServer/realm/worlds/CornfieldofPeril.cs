using wServer.networking;

namespace wServer.realm.worlds
{
    public class CornfieldofPeril : World
    {
        public CornfieldofPeril()
        {
            Name = "Cornfield of Peril";
            ClientWorldName = "Cornfield of Peril";
            Background = 0;
            Difficulty = 4;
            AllowTeleport = false;
            SetMusic("Spooky", "Spooky2");
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.cornfield.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new CornfieldofPeril());
    }
}