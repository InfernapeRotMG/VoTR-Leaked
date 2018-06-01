using db.data;
using Mono.Game;
using System.Linq;
using wServer.realm;
using wServer.realm.entities;
using wServer.realm.entities.player;

namespace wServer.logic.behaviors.PetBehaviors
{
    internal class PetSavage : Behavior
    {
        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            state = 0;
        }

        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            if (state == null)
                return;
            int cool = (int)state;

            if (cool <= 0)
            {
                PetLevel level = null;
                if (host is Pet)
                {
                    Pet p = host as Pet;
                    level = p.GetPetLevelFromAbility(Ability.Savage, true);
                }
                else
                    return;
                if (level == null)
                    return;
                Enemy[] targets = host.GetNearestEntities(7).OfType<Enemy>().ToArray();
                foreach (Enemy e in targets)
                {
                    var vectt = new Vector2(e.X - host.X, e.Y - host.Y);
                    var pet = (Pet)host;
                    var player = host.GetEntity(pet.PlayerOwner.Id) as Player;
                    var vecttt = new Vector2(player.X - host.X, player.Y - host.Y);
                    var distt = host.GetSpeed(0.5f);

                    if (vectt.Length < 6 && vecttt.Length < 6 && vectt.Length > 1)
                    {
                        distt = host.GetSpeed(1);
                        vectt.Normalize();
                        host.Move(e.X, e.Y);
                        host.UpdateCount++;
                    }
                    else if (vecttt.Length > 5)
                    {
                        distt = host.GetSpeed(0.5f);

                        host.UpdateCount++;
                    }
                }
                cool = getCooldown(host as Pet, level);
            }
            else
                cool -= time.thisTickTimes;

            state = cool;
        }

        private int getCooldown(Pet host, PetLevel type)
        {
            if (type.Level <= 30)
            {
                double cool = 10000;
                for (int i = 0; i < type.Level; i++)
                    cool -= 16.6666666666666;
                return (int)cool;
            }
            else if (type.Level > 89)
            {
                double cool = 3000;
                for (int i = 0; i < type.Level - 90; i++)
                    cool -= 40;
                return (int)cool;
            }
            else
            {
                double cool = 6000;
                for (int i = 0; i < type.Level - 30; i++)
                    cool -= 25;
                return (int)cool;
            }
        }
    }
}