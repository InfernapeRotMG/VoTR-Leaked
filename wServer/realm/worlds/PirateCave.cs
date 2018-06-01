using wServer.networking;

namespace wServer.realm.worlds
{
    public class PirateCave : World
    {
        public PirateCave()
        {
            Name = "Pirate Cave";
            ClientWorldName = "dungeons.Pirate_Cave";
            Background = 0;
            Difficulty = 1;
            AllowTeleport = true;
            SetMusic("pirates");
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap(GeneratorCache.NextPirateCave(Seed));
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new PirateCave());
    }
}