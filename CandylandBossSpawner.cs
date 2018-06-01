using System;
using db.data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wServer.realm;

namespace wServer.logic.behaviors
{
    internal class CandylandBossSpawner : Behavior
    {
        private static readonly string[] Ground = { "Gigacorn", "Desire Troll", "Spoiled Creampuff", "Megarototo", "Swoll Fairy" };
        protected override void TickCore(Entity host, RealmTime time, ref object state)
        {
            
        }
        protected override void OnStateEntry(Entity host, RealmTime time, ref object state)
        {
            Entity gigacorn = Entity.Resolve(host.Manager, "Gigacorn");
            Entity desireTroll = Entity.Resolve(host.Manager, "Desire Troll");
            Entity spoiledCreampuff = Entity.Resolve(host.Manager, "Spoiled Creampuff");
            Entity megarototo = Entity.Resolve(host.Manager, "Megarototo");
            Entity swollFairy = Entity.Resolve(host.Manager, "Swoll Fairy");
            var boss = "";
            Random rand = new Random();
            var r = rand.NextDouble();
            if (r > 0 && r <= 0.2)
                boss = "Gigacorn";
            if (r > 0.2 && r <= 0.4)
                boss = "Desire Troll";
            if (r > 0.4 && r <= 0.6)
                boss = "Spoiled Creampuff";
            if (r > 0.6 && r <= 0.8)
                boss = "Megarototo";
            if (r > 0.8 && r <= 1)
                boss = "Swoll Fairy";
            
            switch (boss)
            {
                case "Gigacorn":
                    gigacorn.Move(host.X, host.Y);
                    host.Owner.EnterWorld(gigacorn);
                    
                    break;
                case "Desire Troll":
                    desireTroll.Move(host.X, host.Y);
                    host.Owner.EnterWorld(desireTroll);
                    break;
                case "Spoiled Creampuff":
                    spoiledCreampuff.Move(host.X, host.Y);
                    host.Owner.EnterWorld(spoiledCreampuff);
                    break;
                case "Megarototo":
                    megarototo.Move(host.X, host.Y);
                    host.Owner.EnterWorld(megarototo);
                    break;
                case "Swoll Fairy":
                    swollFairy.Move(host.X, host.Y);
                    host.Owner.EnterWorld(swollFairy);
                    break;
            }
        }
        
    }
}
