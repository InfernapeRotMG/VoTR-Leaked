using db.data;
using Mono.Game;
using System.Linq;
using wServer.realm;
using wServer.realm.entities;
using wServer.realm.entities.player;
using wServer.networking.svrPackets;

namespace wServer.logic.behaviors.Drakes
{
    internal class TowerAttack : Behavior
    {
        private readonly int amount;
        private Cooldown coolDown;
        private uint color;
        private bool trueDamage;

        public TowerAttack(int amount, Cooldown coolDown = new Cooldown(), uint color = 0xFF0000, bool trueDamage = false)
        {
            this.amount = amount;
            this.coolDown = coolDown.Normalize();
            this.color = color;
            this.trueDamage = trueDamage;
        }

        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            state = 0;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            int cool = (int)state;
            if (cool <= 0)
            {

                Enemy en = null;
				Enemy[] targets = host.GetNearestEntities(7).OfType<Enemy>().ToArray();
				foreach (Enemy e in targets) {
					if (e is Enemy) {
						en = e as Enemy;
						break;
					}
				}

                if (en != null & en.ObjectDesc.Enemy)
                {
                    en.Owner.BroadcastPacket(new ShowEffectPacket
                    {
                        EffectType = EffectType.AreaBlast,
                        Color = new ARGB(color),
                        TargetId = en.Id,
                        PosA = new Position { X = 1, }
                    }, null);
                    en.Owner.BroadcastPacket(new ShowEffectPacket
                    {
                        EffectType = EffectType.Trail,
                        TargetId = host.Id,
                        PosA = new Position { X = en.X, Y = en.Y },
                        Color = new ARGB(color)
                    }, null);
                    en.Damage(host.GetPlayerOwner(), time, amount, trueDamage, new ConditionEffect[] { });
                }
                cool = 200;
            }
            else
                cool -= time.thisTickTimes;

            state = cool;
        }
    }
}