using db.data;
using System;
using System.Collections.Generic;

namespace wServer.realm.entities.player
{
    partial class Player
    {
        public Tuple<Item> GetUnboxResult(Item crateItem, Random rand)
        {
            if (rand == null)
                rand = new Random();
            double choice = rand.NextDouble();
            double totalChance = 0;
            foreach (Tuple<double, List<CrateLoot>> i in crateItem.CrateLoot)
            {
                totalChance += i.Item1;
                if (choice < totalChance)
                {
                    var crateLoot = i.Item2.RandomElement(rand);
                    return GetCrateLoot(crateLoot, rand);
                }
            }
            Item item = Manager.GameData.Items[Manager.GameData.IdToObjectType["Gold Medal"]];
            return Tuple.Create(item);
        }

        public Tuple<Item> GetCrateLoot(CrateLoot crateLoot, Random rand)
        {
            if (rand == null)
                rand = new Random();
            Item item = null;
            switch (crateLoot.Type)
            {
                case CrateLootTypes.Item:
                    {
                        item = Manager.GameData.Items[Manager.GameData.IdToObjectType[crateLoot.Name]];
                    }
                    break;
            }
            return Tuple.Create(item);
        }
    }
}