using db;

namespace wServer.networking.cliPackets
{
    public class SorForgePacket : ClientPacket
    {
        public int Currency { get; set; }

        public override PacketID ID
        {
            get { return PacketID.SOR_FORGE; }
        }

        public override Packet CreateInstance()
        {
            return new SorForgePacket();
        }

        protected override void Read(Client client, NReader rdr)
        {
            Currency = rdr.ReadInt32();
        }

        protected override void Write(Client client, NWriter wtr)
        {
            wtr.Write(Currency);
        }
    }
}