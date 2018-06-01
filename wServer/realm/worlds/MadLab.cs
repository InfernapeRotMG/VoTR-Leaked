using wServer.networking;

namespace wServer.realm.worlds
{
    public class MadLab : World
    {
        public MadLab()
        {
            Name = "Mad Lab";
            ClientWorldName = "dungeons.Mad_Lab";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = true;
            SetMusic("lab");
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap(GeneratorCache.NextLab(Seed));
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new MadLab());
    }
}