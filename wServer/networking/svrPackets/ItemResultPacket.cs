using db;

namespace wServer.networking.svrPackets
{
    public class ItemResultPacket : ServerPacket
    {
        public uint ItemID { get; set; }

        public override PacketID ID
        {
            get { return PacketID.ITEMRESULT; }
        }

        public override Packet CreateInstance()
        {
            return new ItemResultPacket();
        }

        protected override void Read(Client client, NReader rdr)
        {
            ItemID = rdr.ReadUInt32();
        }

        protected override void Write(Client client, NWriter wtr)
        {
            wtr.Write(ItemID);
        }
    }
}