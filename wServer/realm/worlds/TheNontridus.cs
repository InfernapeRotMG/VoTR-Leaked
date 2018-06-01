using System;
using wServer.networking;

namespace wServer.realm.worlds
{
    public class TheNontridus : World
    {
        public TheNontridus()
        {
            Name = "The Nontridus";
            ClientWorldName = "The Nontridus";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = false;
            SetMusic("dark fallout");
        }

        protected override void Init()
        {
            Random r = new Random();
            LoadMap("wServer.realm.worlds.maps.nontridus" + r.Next(1, 3 + 1).ToString() + ".jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new TheNontridus());
    }
}