using wServer.networking;

namespace wServer.realm.worlds
{
    public class DomainoftheDarkKnight : World
    {
        public DomainoftheDarkKnight()
        {
            Name = "Domain of the Dark Knight";
            ClientWorldName = "Domain of the Dark Knight";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = false;
            SetMusic("unspeakableboss");
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.domain.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new DomainoftheDarkKnight());
    }
}