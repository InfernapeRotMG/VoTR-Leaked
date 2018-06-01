using db;

namespace wServer.networking.cliPackets
{
    public class MarketTradePacket : ClientPacket
    {
        public int TradeId { get; set; }

        public override PacketID ID
        {
            get { return PacketID.MARKETTRADE; }
        }

        public override Packet CreateInstance()
        {
            return new MarketTradePacket();
        }

        protected override void Read(Client client, NReader rdr)
        {
            TradeId = rdr.ReadInt32();
        }

        protected override void Write(Client client, NWriter wtr)
        {
            wtr.Write(TradeId);
        }
    }
}