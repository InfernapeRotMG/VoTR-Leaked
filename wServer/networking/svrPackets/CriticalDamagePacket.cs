#region

using db;
using db.data;
using System.Collections.Generic;

#endregion

namespace wServer.networking.svrPackets
{
    public class CriticalDamagePacket : ServerPacket
    {
        public bool IsCritical { get; set; }
		public float CriticalHit { get; set; }

        public override PacketID ID
        {
            get { return PacketID.CRITICALDAMAGE; }
        }

        public override Packet CreateInstance()
        {
			return new CriticalDamagePacket();
        }

        protected override void Read(Client psr, NReader rdr)
        {
			IsCritical = rdr.ReadBoolean();
			CriticalHit = rdr.ReadSingle();
        }

        protected override void Write(Client psr, NWriter wtr)
        {
			wtr.Write(IsCritical);
			wtr.Write(CriticalHit);
        }
    }
}