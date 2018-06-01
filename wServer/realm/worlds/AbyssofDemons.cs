#region

using wServer.networking;

#endregion

namespace wServer.realm.worlds
{
    public class AbyssofDemons : World
    {
        public AbyssofDemons()
        {
            Name = "Abyss of Demons";
            ClientWorldName = "{dungeons.Abyss_of_Demons}";
            Dungeon = true;
            Background = 0;
            AllowTeleport = true;
            SetMusic("abyss");
            Difficulty = 4;
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            LoadMap(GeneratorCache.NextAbyss(Seed));
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new AbyssofDemons());
    }
}