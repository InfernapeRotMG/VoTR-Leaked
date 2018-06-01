using db;

namespace wServer.networking.svrPackets
{
    public class UnboxResultPacket : ServerPacket
    {
        public ushort[] Items { get; set; }

        public override PacketID ID
        {
            get { return PacketID.UNBOXRESULT; }
        }

        public override Packet CreateInstance()
        {
            return new UnboxResultPacket();
        }

        protected override void Read(Client client, NReader rdr)
        {
            Items = new ushort[rdr.ReadInt16()];
            for (int i = 0; i < Items.Length; i++)
                Items[i] = (ushort)rdr.ReadInt32();
        }

        protected override void Write(Client client, NWriter wtr)
        {
            wtr.Write((short)Items.Length);
            for (int i = 0; i < Items.Length; i++)
                wtr.Write((int)Items[i]);
        }
    }
}