#region

using db.data;
using System.Collections.Generic;
using wServer.networking;
using wServer.networking.svrPackets;
using wServer.realm.entities.player;

#endregion

namespace wServer.realm.entities
{
    internal class Banner : StaticObject
    {
        private readonly float radius;
        private int lifetime;
        private readonly int duration;
        private int p;
        private int p2;
        private Player player;
        private int t;

        public Banner(Player player, float radius, int lifetime, int duration)
            : base(player.Manager, 0x0711, lifetime * 1000, true, true, false)
        {
            this.player = player;
            this.radius = radius;
            this.lifetime = lifetime;
            this.duration = duration;
        }

        public override void Tick(RealmTime time)
        {
            if (t / 500 == p2)
            {
                Owner.BroadcastPacket(new ShowEffectPacket
                {
                    EffectType = EffectType.Trap,
                    Color = new ARGB(0x0000ff),
                    TargetId = Id,
                    PosA = new Position { X = radius }
                }, null);
                p2++;
                //Stuff
            }
            if (t / 2000 == p)
            {
                List<Packet> pkts = new List<Packet>();
                List<Player> players = new List<Player>();
                this.Aoe(radius, true, player =>
                {
                    players.Add(player as Player);
                    player.ApplyConditionEffect(new ConditionEffect
                    {
                        Effect = ConditionEffectIndex.Empowered,
                        DurationMS = duration
                    });
                    player.ApplyConditionEffect(new ConditionEffect
                    {
                        Effect = ConditionEffectIndex.StunImmune,
                        DurationMS = duration
                    });
                    player.ApplyConditionEffect(new ConditionEffect
                    {
                        Effect = ConditionEffectIndex.ParalyzeImmune,
                        DurationMS = duration
                    });
                });

                Owner.BroadcastPackets(pkts, null);
                p++;
            }
            t += time.thisTickTimes;
            base.Tick(time);
        }
    }
}