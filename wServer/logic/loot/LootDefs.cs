#region

using db.data;
using System;
using System.Collections.Generic;
using System.Linq;
using wServer.realm;
using wServer.realm.entities;
using wServer.realm.entities.player;

#endregion

namespace wServer.logic.loot
{
    public interface ILootDef
    {
        string Lootstate { get; set; }

        void Populate(RealmManager manager, Enemy enemy, Tuple<Player, int> playerDat,
            Random rand, string lootState, IList<LootDef> lootDefs);
    }

    public class ItemLoot : ILootDef
    {
        private readonly string item;
        private readonly double probability;

        public string Lootstate { get; set; }

        public ItemLoot(string item, double probability)
        {
            this.item = item;
            this.probability = probability;
        }

        public void Populate(RealmManager manager, Enemy enemy, Tuple<Player, int> playerDat,
            Random rand, string lootState, IList<LootDef> lootDefs)
        {
            Lootstate = lootState;
            if (playerDat != null)
                return;
            XmlData dat = manager.GameData;
            lootDefs.Add(new LootDef(dat.Items[dat.IdToObjectType[item]], probability, lootState));
        }
    }

    public class LootState : ILootDef
    {
        private readonly ILootDef[] children;

        public string Lootstate { get; set; }

        public LootState(string subState, params ILootDef[] lootDefs)
        {
            children = lootDefs;
            Lootstate = subState;
        }

        public void Populate(RealmManager manager, Enemy enemy, Tuple<Player, int> playerDat, Random rand, string lootState, IList<LootDef> lootDefs)
        {
            foreach (ILootDef i in children)
                i.Populate(manager, enemy, playerDat, rand, Lootstate, lootDefs);
        }
    }

    public enum ItemType
    {
        Weapon,
        Ability,
        Armor,
        Ring,
        Potion
    }

    public enum EggRarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary
    }

    public class EggLoot : ILootDef
    {
        private readonly EggRarity rarity;
        private readonly double probability;

        public string Lootstate { get; set; }

        public EggLoot(EggRarity rarity, double probability)
        {
            this.probability = probability;
            this.rarity = rarity;
        }

        public void Populate(RealmManager manager, Enemy enemy, Tuple<Player, int> playerDat,
            Random rand, string lootState, IList<LootDef> lootDefs)
        {
            Lootstate = lootState;
            if (playerDat != null)
                return;
            Item[] candidates = manager.GameData.Items
                .Where(item => item.Value.SlotType == 26)
                .Where(item => item.Value.Tier == (int)rarity)
                .Select(item => item.Value)
                .ToArray();
            foreach (Item i in candidates)
                lootDefs.Add(new LootDef(i, probability / candidates.Length, lootState));
        }
    }

    public class TierLoot : ILootDef
    {
        public static readonly int[] WeaponT = { 1, 2, 3, 8, 17, 24, 29 };
        public static readonly int[] AbilityT = { 4, 5, 11, 12, 13, 15, 16, 18, 19, 20, 21, 22, 23, 25, 27, 28, 30, 31, 32, 33 };
        public static readonly int[] ArmorT = { 6, 7, 14 };
        public static readonly int[] RingT = { 9 };
        public static readonly int[] PotionT = { 10 };
        private readonly double probability;

        private readonly byte tier;
        private readonly int[] types;

        public string Lootstate { get; set; }

        public TierLoot(byte tier, ItemType type, double probability)
        {
            this.tier = tier;
            switch (type)
            {
                case ItemType.Weapon:
                    types = WeaponT;
                    break;

                case ItemType.Ability:
                    types = AbilityT;
                    break;

                case ItemType.Armor:
                    types = ArmorT;
                    break;

                case ItemType.Ring:
                    types = RingT;
                    break;

                case ItemType.Potion:
                    types = PotionT;
                    break;

                default:
                    throw new NotSupportedException(type.ToString());
            }
            this.probability = probability;
        }

        public void Populate(RealmManager manager, Enemy enemy, Tuple<Player, int> playerDat,
            Random rand, string lootState, IList<LootDef> lootDefs)
        {
            Lootstate = lootState;
            if (playerDat != null)
                return;
            Item[] candidates = manager.GameData.Items
                .Where(item => Array.IndexOf(types, item.Value.SlotType) != -1)
                .Where(item => item.Value.Tier == tier)
                .Select(item => item.Value)
                .ToArray();
            foreach (Item i in candidates)
                lootDefs.Add(new LootDef(i, probability / candidates.Length, lootState));
        }
    }

    public class Threshold : ILootDef
    {
        private readonly ILootDef[] children;
        private readonly double threshold;

        public string Lootstate { get; set; }

        public Threshold(double threshold, params ILootDef[] children)
        {
            this.threshold = threshold;
            this.children = children;
        }

        public void Populate(RealmManager manager, Enemy enemy, Tuple<Player, int> playerDat, Random rand,
            string lootState, IList<LootDef> lootDefs)
        {
            Lootstate = lootState;
            if (playerDat != null && playerDat.Item2 / enemy.ObjectDesc.MaxHP >= threshold)
            {
                foreach (ILootDef i in children)
                    i.Populate(manager, enemy, null, rand, lootState, lootDefs);
            }
        }
    }

    internal class MostDamagers : ILootDef
    {
        private readonly ILootDef[] loots;
        private readonly int amount;

        public MostDamagers(int amount, params ILootDef[] loots)
        {
            this.amount = amount;
            this.loots = loots;
        }

        public string Lootstate { get; set; }

        public void Populate(RealmManager manager, Enemy enemy, Tuple<Player, int> playerDat, Random rand, string lootState, IList<LootDef> lootDefs)
        {
            var data = enemy.DamageCounter.GetPlayerData();
            var mostDamage = GetMostDamage(data);
            foreach (var loot in mostDamage.Where(pl => pl.Equals(playerDat)).SelectMany(pl => loots))
                loot.Populate(manager, enemy, null, rand, lootState, lootDefs);
        }

        private IEnumerable<Tuple<Player, int>> GetMostDamage(IEnumerable<Tuple<Player, int>> data)
        {
            var damages = data.Select(_ => _.Item2).ToList();
            var len = damages.Count < amount ? damages.Count : amount;
            for (var i = 0; i < len; i++)
            {
                var val = damages.Max();
                yield return data.FirstOrDefault(_ => _.Item2 == val);
                damages.Remove(val);
            }
        }
    }

    public class OnlyOne : ILootDef
    {
        private readonly ILootDef[] loots;

        public OnlyOne(params ILootDef[] loots)
        {
            this.loots = loots;
        }

        public string Lootstate { get; set; }

        public void Populate(RealmManager manager, Enemy enemy, Tuple<Player, int> playerDat, Random rand, string lootState, IList<LootDef> lootDefs)
        {
            loots[rand.Next(0, loots.Length)].Populate(manager, enemy, playerDat, rand, lootState, lootDefs);
        }
    }

    public static class LootTemplates
    {
        public static ILootDef[] DefaultEggLoot(EggRarity maxRarity)
        {
            switch (maxRarity)
            {
                case EggRarity.Common:
                    return new ILootDef[1] { new EggLoot(EggRarity.Common, 0.1) };

                case EggRarity.Uncommon:
                    return new ILootDef[2] { new EggLoot(EggRarity.Common, 0.1), new EggLoot(EggRarity.Uncommon, 0.05) };

                case EggRarity.Rare:
                    return new ILootDef[3] { new EggLoot(EggRarity.Common, 0.1), new EggLoot(EggRarity.Uncommon, 0.05), new EggLoot(EggRarity.Rare, 0.01) };

                case EggRarity.Legendary:
                    return new ILootDef[4] { new EggLoot(EggRarity.Common, 0.1), new EggLoot(EggRarity.Uncommon, 0.05), new EggLoot(EggRarity.Rare, 0.01), new EggLoot(EggRarity.Legendary, 0.001) };

                default:
                    throw new InvalidOperationException("Not a valid Egg Rarity");
            }
        }

        public static ILootDef[] StatIncreasePotionsLoot()
        {
            return new ILootDef[]
            {
                new OnlyOne(
                    new ItemLoot("Potion of Defense", 1),
                    new ItemLoot("Potion of Attack", 1),
                    new ItemLoot("Potion of Speed", 1),
                    new ItemLoot("Potion of Vitality", 1),
                    new ItemLoot("Potion of Wisdom", 1),
                    new ItemLoot("Potion of Dexterity", 1)
                )
             };
        }

        public static ILootDef[] GStatIncreasePotionsLoot()
        {
            return new ILootDef[]
            {
                new OnlyOne(
                    new ItemLoot("Greater Potion of Defense", 1),
                    new ItemLoot("Greater Potion of Attack", 1),
                    new ItemLoot("Greater Potion of Speed", 1),
                    new ItemLoot("Greater Potion of Vitality", 1),
                    new ItemLoot("Greater Potion of Wisdom", 1),
                    new ItemLoot("Greater Potion of Dexterity", 1)
                )
             };
        }

        public static ILootDef[] RaidTokens()
        {
            return new ILootDef[]
            {
                new OnlyOne(
                    new ItemLoot("Sor Forger Battery", 0.005)
                ),
                new OnlyOne(
                    new ItemLoot("Mark of Unity", 0.005)
                ),
                new OnlyOne(
                    new ItemLoot("Questing Package", 0.25)
                ),
                new OnlyOne(
                    new ItemLoot("The Zol Awakening (Token)", 0.1),
                    new ItemLoot("Calling of the Titan (Token)", 0.1),
                    new ItemLoot("A Fallen Light (Token)", 0.1),
                    new ItemLoot("Sidon's Fall (Token)", 0.1),
                    new ItemLoot("War of Decades (Token)", 0.1)
                ),
             };
        }

        //Hideout Fabled Dungeon
        public static ILootDef[] FabledItemsLoot1()
        {
            return new ILootDef[]
            {
                new OnlyOne(
                    new ItemLoot("Kismet Seal", 0.75),
                    new ItemLoot("Soundpiercer Shuriken", 0.75),
                    new ItemLoot("Doomgrazer", 0.75),
                    new ItemLoot("Age of Zol", 0.75),
                    new ItemLoot("Wrath of Aldragine", 0.75),
                    new ItemLoot("Bane of the Vision", 0.75),
                    new ItemLoot("Spirit of the Heart", 0.75),
                    new ItemLoot("The Grand Finale", 0.75),
                    new ItemLoot("Merit of Rebellion", 0.75),
                    new ItemLoot("Enigma Wand", 0.75),
                    new ItemLoot("Spear of the Unforgiven", 0.75),
                    new ItemLoot("Dagger of Corruption", 0.75)
                ),
                //Have another chance to get fabled iten
                new OnlyOne(
                    new ItemLoot("Kismet Seal", 0.5),
                    new ItemLoot("Soundpiercer Shuriken", 0.5),
                    new ItemLoot("Doomgrazer", 0.5),
                    new ItemLoot("Age of Zol", 0.5),
                    new ItemLoot("Wrath of Aldragine", 0.5),
                    new ItemLoot("Bane of the Vision", 0.5),
                    new ItemLoot("Spirit of the Heart", 0.5),
                    new ItemLoot("The Grand Finale", 0.5),
                    new ItemLoot("Merit of Rebellion", 0.5),
                    new ItemLoot("Enigma Wand", 0.5),
                    new ItemLoot("Spear of the Unforgiven", 0.5),
                    new ItemLoot("Dagger of Corruption", 0.5)
                ),
                //You have a 100% chance to get attack or wisdom eon
                new OnlyOne(
                    new ItemLoot("Attack Eon", 1),
                    new ItemLoot("Wisdom Eon", 1)
                ),
                //You have a 25% chance to get ANOTHER attack or wisdom eon
                new OnlyOne(
                    new ItemLoot("Attack Eon", 0.25),
                    new ItemLoot("Wisdom Eon", 0/25)
                ),
                //High chance to get 2 cloth
                new OnlyOne(
                    new ItemLoot("Large Zol Cloth", 0.6),
                    new ItemLoot("Small Zol Cloth", 0.6)
                ),
                new OnlyOne(
                    new ItemLoot("Large Vortex Cloth", 0.6),
                    new ItemLoot("Small Vortex Cloth", 0.6)
                    ),
                new OnlyOne(
                    new ItemLoot("Large Aura Cloth", 0.6),
                    new ItemLoot("Small Aura Cloth", 0.6)
                    ),
                new OnlyOne(
                    new ItemLoot("Sword of Dark Retribution", 0.8),
                    new ItemLoot("Helm of Dark Retribution", 0.8),
                    new ItemLoot("Armor of Dark Retribution", 0.8),
                    new ItemLoot("Ring of Dark Retribution", 0.8)
                    ),
                new OnlyOne(
                    new ItemLoot("Sword of Dark Retribution", 0.8),
                    new ItemLoot("Helm of Dark Retribution", 0.8),
                    new ItemLoot("Armor of Dark Retribution", 0.8),
                    new ItemLoot("Ring of Dark Retribution", 0.8)
                    )
            };
        }

        //Sincryer (Hideout) Fabled Dungeon
        public static ILootDef[] FabledItemsLoot2()
        {
            return new ILootDef[]
            {
                new OnlyOne(
                    new ItemLoot("Kismet Seal", 0.75),
                    new ItemLoot("Soundpiercer Shuriken", 0.75),
                    new ItemLoot("Doomgrazer", 0.75),
                    new ItemLoot("Age of Zol", 0.75),
                    new ItemLoot("Wrath of Aldragine", 0.75),
                    new ItemLoot("Bane of the Vision", 0.75),
                    new ItemLoot("Spirit of the Heart", 0.75),
                    new ItemLoot("The Grand Finale", 0.75),
                    new ItemLoot("Merit of Rebellion", 0.75),
                    new ItemLoot("Enigma Wand", 0.75),
                    new ItemLoot("Spear of the Unforgiven", 0.75),
                    new ItemLoot("Dagger of Corruption", 0.75)
                ),
                new OnlyOne(
                    new ItemLoot("Sword of Dark Retribution", 0.8),
                    new ItemLoot("Helm of Dark Retribution", 0.8),
                    new ItemLoot("Armor of Dark Retribution", 0.8),
                    new ItemLoot("Ring of Dark Retribution", 0.8)
                    )
            };
        }
    }
}