using db;

namespace wServer.networking.svrPackets
{
    public class VerifyEmailDialogPacket : ServerPacket
    {
        public override PacketID ID
        {
            get { return PacketID.VERIFYEMAILDIALOG; }
        }

        public override Packet CreateInstance()
        {
            return new VerifyEmailDialogPacket();
        }

        protected override void Read(Client client, NReader rdr)
        {
        }

        protected override void Write(Client client, NWriter wtr)
        {
        }
    }
}