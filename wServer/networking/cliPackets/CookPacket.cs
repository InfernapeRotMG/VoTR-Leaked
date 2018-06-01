using db;
using System.Collections.Generic;
namespace wServer.networking.cliPackets
{
    public class CookPacket : ClientPacket
    {
        public ObjectSlot Fuel { get; set; }
        public ObjectSlot Slot1 { get; set; }
        public ObjectSlot Slot2 { get; set; }
        public ObjectSlot Slot3 { get; set; }
        public ObjectSlot Slot4 { get; set; }
        public ObjectSlot Slot5 { get; set; }
        public ObjectSlot Slot6 { get; set; }
        public ObjectSlot Slot7 { get; set; }
        public ObjectSlot Slot8 { get; set; }
        public ObjectSlot Slot9 { get; set; }

        public override PacketID ID
        {
            get { return PacketID.COOK; }
        }

        public override Packet CreateInstance()
        {
            return new CookPacket();
        }

        protected override void Read(Client client, NReader rdr)
        {
            Fuel = ObjectSlot.Read(client, rdr);
            Slot1 = ObjectSlot.Read(client, rdr);
            Slot2 = ObjectSlot.Read(client, rdr);
            Slot3 = ObjectSlot.Read(client, rdr);
            Slot4 = ObjectSlot.Read(client, rdr);
            Slot5 = ObjectSlot.Read(client, rdr);
            Slot6 = ObjectSlot.Read(client, rdr);
            Slot7 = ObjectSlot.Read(client, rdr);
            Slot8 = ObjectSlot.Read(client, rdr);
            Slot9 = ObjectSlot.Read(client, rdr);

        }

        protected override void Write(Client client, NWriter wtr)
        {
        }
    }
}