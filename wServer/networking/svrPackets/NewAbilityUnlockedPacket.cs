using db;
using db.data;

namespace wServer.networking.svrPackets
{
    public class NewAbilityUnlockedPacket : ServerPacket
    {
        public Ability Type { get; set; }

        public override PacketID ID
        {
            get { return PacketID.NEWABILITYUNLOCKED; }
        }

        public override Packet CreateInstance()
        {
            return new NewAbilityUnlockedPacket();
        }

        protected override void Read(Client client, NReader rdr)
        {
            Type = (Ability)rdr.ReadInt32();
        }

        protected override void Write(Client client, NWriter wtr)
        {
            wtr.Write((int)Type);
        }
    }
}