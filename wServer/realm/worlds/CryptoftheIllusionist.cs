using System;
using wServer.networking;

namespace wServer.realm.worlds
{
    public class CryptoftheIllusionist : World
    {
        public CryptoftheIllusionist()
        {
            Name = "Crypt of the Illusionist";
            ClientWorldName = "Crypt of the Illusionist";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = false;
            SetMusic("manor");
        }

        public override bool NeedsPortalKey => false;

        protected override void Init()
        {
            Random r = new Random();
            LoadMap("wServer.realm.worlds.maps.cryptill" + r.Next(1, 3 + 1).ToString() + ".jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new CryptoftheIllusionist());
    }
}