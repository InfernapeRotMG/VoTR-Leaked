using wServer.networking;

namespace wServer.realm.worlds
{
    public class AldraginesHideout : World
    {
        public AldraginesHideout()
        {
            Name = "Aldragine's Hideout";
            ClientWorldName = "Aldragine's Hideout";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = false;
            SetMusic("magic");
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.aldragine.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new AldraginesHideout());
    }
}