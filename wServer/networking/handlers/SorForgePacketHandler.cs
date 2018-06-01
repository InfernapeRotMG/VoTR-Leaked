#region

using db;
using wServer.networking.cliPackets;

#endregion

namespace wServer.networking.handlers
{
    internal class SorForgePacketHandler : PacketHandlerBase<SorForgePacket>
    {
        public override PacketID ID
        {
            get { return PacketID.SOR_FORGE; }
        }

        protected override void HandlePacket(Client client, SorForgePacket packet)
        {
            using (Database db = new Database())
            {
                var p = client.Player;
                var inv = p.Inventory;

                if (packet.Currency == 0)
                {
                    p.SorForgerGold();
                }
                else
                {
                    p.SorForgerOnrane();
                }
                client.Save();
            }
        }
    }
}