using wServer.networking;

namespace wServer.realm.worlds
{
    public class ZolSecretShop : World
    {
        public ZolSecretShop()
        {
            Name = "Zol Secret Shop";
            ClientWorldName = "Zol Secret Shop";
            Background = 0;
            Difficulty = 5;
            AllowTeleport = false;
            SetMusic("safe");
        }

        protected override void Init()
        {
            LoadMap("wServer.realm.worlds.maps.ashop.jm", MapType.Json);
        }

        public override World GetInstance(Client psr) => Manager.AddWorld(new ZolSecretShop());
    }
}