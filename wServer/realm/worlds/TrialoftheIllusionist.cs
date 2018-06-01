using System;
using wServer.networking;

namespace wServer.realm.worlds
{
    public class TrialoftheIllusionist : World
    {
        public TrialoftheIllusionist()
        {
            Name = "Trial of the Illusionist";
            ClientWorldName = "Trial of the Illusionist";
            Background = 0;
            Difficulty = 4;
            AllowTeleport = false;
            SetMusic("manor");
        }

        public override bool NeedsPortalKey => false;

        protected override void Init()
        {
            Random r = new Random();
            LoadMap("wServer.realm.worlds.maps.trial" + r.Next(1, 3 + 1).ToString() + ".jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new TrialoftheIllusionist());
    }
}