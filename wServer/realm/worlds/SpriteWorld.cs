using System;
using wServer.networking;

namespace wServer.realm.worlds
{
    public class SpriteWorld : World
    {
        public SpriteWorld()
        {
            Name = "Sprite World";
            ClientWorldName = "dungeons.Sprite_World";
            Background = 0;
            Difficulty = 2;
            AllowTeleport = true;
            SetMusic("spriteworld");
        }

        public override bool NeedsPortalKey => true;

        protected override void Init()
        {
            Random r = new Random();
            LoadMap("wServer.realm.worlds.maps.spriteworld" + r.Next(1, 5 + 1).ToString() + ".wmap", MapType.Wmap);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new SpriteWorld());
    }
}