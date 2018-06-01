using db.data;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace db
{
    public class ItemData
    {
        [DefaultValue("")]
        public string NamePrefix = "";

        [DefaultValue("")]
        public string Name = "";

        [DefaultValue("")]
        public string Description = "";

        public uint NameColor = 0xFFFFFF;
        public int DmgPercentage = 0;
        public bool Soulbound = false;
        public bool MultiUse = false;
        public int Doses = 0;

        [DefaultValue("")]
        public string TextureFile = "";

        public uint TextureIndex = 0;

        public bool Strange = false;
        public int Kills = 0;

        public ConcurrentDictionary<string, int> StrangeParts = new ConcurrentDictionary<string, int>();

        [DefaultValue("")]
        public string Effect = "";

        [DefaultValue("")]
        public string FullEffect = "";

        //Gauntlet Enchantments



        #region Weapons
        public int FameBonus = 0; //Added percentage
        public int MinDamage = 0; //Added min damage
        public int MaxDamage = 0; //Added max damage
        public double Range = 0; //Added range (converted to LifetimeMS)

        public int CondChance = 0; //Percentage to apply CondEffect
        public int CondEffect = 0; //Condition effect index

        #endregion Weapons

        #region Abilities

        public int MPCost = 0; //Added MP Cost

        #endregion Abilities

        public string GetJson()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            return JsonConvert.SerializeObject(this, settings);
        }

        public void ResetStrange()
        {
            if (!this.Strange)
                return;
            this.NamePrefix = "Strange";
            this.Kills = 0;
            foreach (var i in this.StrangeParts)
                this.StrangeParts[i.Key] = 0;
        }

        public static ItemData CreateData(string js)
        {
            return JsonConvert.DeserializeObject<ItemData>(js);
        }
    }

    public class ItemDataList
    {
        public ItemData[] datas;

        public static string GetJson(ItemData[] datas)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            return JsonConvert.SerializeObject(new ItemDataList
            {
                datas = datas
            }, settings);
        }

        public static ItemData[] CreateData(string js)
        {
            return JsonConvert.DeserializeObject<ItemDataList>(js).datas;
        }
    }

    public class JsonItem
    {
        public ushort item;
        public ItemData data;

        public JsonItem(Item item, ItemData data)
        {
            this.item = item.ObjectType;
            this.data = data;
        }

        public string GetJson()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            return JsonConvert.SerializeObject(this, settings);
        }
    }

    public class JsonItemList
    {
        public JsonItem[] items;

        public static string GetJson(JsonItem[] items)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.DefaultValueHandling = DefaultValueHandling.Ignore;
            return JsonConvert.SerializeObject(new JsonItemList
            {
                items = items
            }, settings);
        }

        public static string GetJson(Item[] items, ItemData[] datas)
        {
            var jsItems = new JsonItem[items.Length];
            for (int i = 0; i < items.Length; i++)
                if (items[i] != null)
                    jsItems[i] = new JsonItem(items[i], datas[i]);
                else
                    jsItems[i] = null;
            return GetJson(jsItems);
        }

        public static JsonItem[] CreateJsonItems(string js)
        {
            return JsonConvert.DeserializeObject<JsonItemList>(js).items;
        }
    }

    public class UnusualEffects
    {
        public const int CurrentSeries = 1;

        public static void Init()
        {
            //Normal series
            AddSeries(1, "Tinted Trail", "Sporadic Sparkles", "Ring of Fire", "Lovestruck", "Dusty Disaster");
            AddSeries(2, "Orbiting Fire", "Stormy Storm", "Blizzardy Storm", "Rising Bubbles", "Misty Skulls");

            //Special series
            AddSeries(-1, "Realm Riches", "Rainbow Rain", "Premium Stars");
        }

        public static Dictionary<string, string> Types = new Dictionary<string, string>()
        {
            {"Realm Riches", "<Effect rate=\"5\" life=\"1\" lifeVariance=\".1\" speed=\"0\" speedVariance=\"0.5\" size=\"50\" rise=\"-1.6\" riseVariance=\".1\" rangeX=\"0.6\" rangeY=\"0.6\" zOffset=\"2\" fadeIn=\"0.1\" fadeOut=\"0.9\" bitmapFile=\"customEffects8x8\" bitmapIndex=\"0x0\" bitmapIndexMax=\"0x3\" attached=\"true\">CustomParticles</Effect>"},
            {"Rainbow Rain", "<Effects><Effect color=\"0xf40d07\" rate=\"4.1\" life=\"1\" lifeVariance=\".1\" speed=\"0\" speedVariance=\"0.5\" size=\"6\" growth=\"-5\" rise=\"-1.6\" riseVariance=\".1\" rangeX=\"0.8\" rangeY=\"0.8\" zOffset=\"2\">CustomParticles</Effect><Effect color=\"0xee9417\" rate=\"4.3\" life=\"1\" lifeVariance=\".1\" speed=\"0\" speedVariance=\"0.5\" size=\"6\" growth=\"-5\" rise=\"-1.6\" riseVariance=\".1\" rangeX=\"0.8\" rangeY=\"0.8\" zOffset=\"2\">CustomParticles</Effect><Effect color=\"0xf9ff08\" rate=\"4.4\" life=\"1\" lifeVariance=\".1\" speed=\"0\" speedVariance=\"0.5\" size=\"6\" growth=\"-5\" rise=\"-1.6\" riseVariance=\".1\" rangeX=\"0.8\" rangeY=\"0.8\" zOffset=\"2\">CustomParticles</Effect><Effect color=\"0x43b448\" rate=\"4.6\" life=\"1\" lifeVariance=\".1\" speed=\"0\" speedVariance=\"0.5\" size=\"6\" growth=\"-5\" rise=\"-1.6\" riseVariance=\".1\" rangeX=\"0.8\" rangeY=\"0.8\" zOffset=\"2\">CustomParticles</Effect><Effect color=\"0x0a256a\" rate=\"4.7\" life=\"1\" lifeVariance=\".1\" speed=\"0\" speedVariance=\"0.5\" size=\"6\" growth=\"-5\" rise=\"-1.6\" riseVariance=\".1\" rangeX=\"0.8\" rangeY=\"0.8\" zOffset=\"2\">CustomParticles</Effect><Effect color=\"0x44005c\" rate=\"4.9\" life=\"1\" lifeVariance=\".1\" speed=\"0\" speedVariance=\"0.5\" size=\"6\" growth=\"-5\" rise=\"-1.6\" riseVariance=\".1\" rangeX=\"0.8\" rangeY=\"0.8\" zOffset=\"2\">CustomParticles</Effect><Effect color=\"0xa259a4\" rate=\"4.3\" life=\"1\" lifeVariance=\".1\" speed=\"0\" speedVariance=\"0.5\" size=\"6\" growth=\"-5\" rise=\"-1.6\" riseVariance=\".1\" rangeX=\"0.8\" rangeY=\"0.8\" zOffset=\"2\">CustomParticles</Effect></Effects>"},
            {"Premium Stars", "<Effects><Effect size=\"20\" speed=\"180\" radius=\"0.75\" zOffset=\"0.3\" bitmapFile=\"customEffects16x16\" bitmapIndex=\"2\">Orbiting</Effect><Effect size=\"20\" speed=\"270\" radius=\"0.75\" zOffset=\"0.6\" bitmapFile=\"customEffects16x16\" bitmapIndex=\"2\" angleOffset=\"180\">Orbiting</Effect><Effect size=\"20\" speed=\"360\" radius=\"0.75\" zOffset=\"0.9\" bitmapFile=\"customEffects16x16\" bitmapIndex=\"2\" angleOffset=\"270\">Orbiting</Effect></Effects>"},
            {"Tinted Trail", "<Effect rate=\"0.85\" color=\"{0}\">Gas</Effect>"},
            {"Sporadic Sparkles", "<Effect color=\"{0}\" rate=\"6.5\" life=\"1\" lifeVariance=\"0\" speed=\"0\" speedVariance=\"0\" size=\"7\" rise=\"0\" growth=\"-7\" rangeX=\"0.5\" rangeY=\"0.5\" rangeZ=\"0.75\" attached=\"true\">CustomParticles</Effect>"},
            {"Ring of Fire", "<Effect rate=\"20\" life=\"1\" lifeVariance=\"0.25\" speed=\"0\" speedVariance=\"0\" size=\"40\" growth=\"-40\" rise=\"0.01\" riseVariance=\"0\" rangeX=\"1\" rangeY=\"1\" zOffset=\"0.15\" bitmapFile=\"lofiObjBig\" bitmapIndex=\"10\">CustomParticles</Effect>"},
            {"Rising Bubbles", "<Effect color=\"{0}\" rate=\"20\" life=\".45\" lifeVariance=\".1\" speed=\"0\" speedVariance=\".5\" size=\"5\" growth=\"-5\" rise=\"1\" riseVariance=\".1\" rangeX=\"1\" rangeY=\"1\">CustomParticles</Effect>"},
            {"Dusty Disaster", "<Effect color=\"0x777777\" rate=\"50\" life=\"0.5\" lifeVariance=\"0\" speed=\"0\" speedVariance=\"0.1\" size=\"6\" growth=\"-6\" rise=\"0\" riseVariance=\"0\" rangeX=\"0.5\" rangeY=\"0.5\" zOffset=\"0\" rangeZ=\"0.75\">CustomParticles</Effect>"},
            {"Stormy Storm", "<Effects><Effect rate=\"25\" life=\"1\" lifeVariance=\".1\" speed=\"0\" speedVariance=\"0.2\" size=\"45\" rise=\"0\" riseVariance=\".1\" rangeX=\"0.75\" rangeY=\"0.75\" zOffset=\"2.5\" bitmapFile=\"customEffects16x16\" bitmapIndex=\"0x0\">CloudParticles</Effect><Effect rate=\"40\" life=\".66\" lifeVariance=\".16\" speed=\".06\" speedVariance=\"0.55\" size=\"15\" growth=\"1\" rise=\"-2.9\" riseVariance=\".1\" rangeX=\"0.5\" rangeY=\"0.5\" zOffset=\"2.5\" bitmapFile=\"lofiObj\" bitmapIndex=\"0x64\">CustomParticles</Effect></Effects>"},
            {"Blizzardy Storm", "<Effects><Effect rate=\"25\" life=\"1\" lifeVariance=\".1\" speed=\"0\" speedVariance=\"0.2\" size=\"45\" rise=\"0\" riseVariance=\".1\" rangeX=\"0.75\" rangeY=\"0.75\" zOffset=\"2.5\" bitmapFile=\"customEffects16x16\" bitmapIndex=\"0x1\">CloudParticles</Effect><Effect rate=\"20\" life=\"2\" lifeVariance=\".16\" speed=\".06\" speedVariance=\"0.55\" size=\"20\" growth=\"-4\" rise=\"-1\" riseVariance=\".1\" rangeX=\"0.5\" rangeY=\"0.5\" zOffset=\"2.5\" bitmapFile=\"essences\" bitmapIndex=\"0\">CustomParticles</Effect></Effects>"},
            {"Lovestruck", "<Effect rate=\"4\" life=\"1\" lifeVariance=\"0\" speed=\"0\" speedvariance=\"0.33\" size=\"10\" rise=\"0.33\" riseVariance=\".1\" growth=\"35\" rangeX=\"0.6\" rangeY=\"0.6\" rangeZ=\"0.75\" fadeOut=\"0.9\" bitmapFile=\"lofiObj\" bitmapIndex=\"0x8f\">CustomParticles</Effect>"},
            {"Orbiting Fire", "<Effects><Effect size=\"30\" speed=\"270\" radius=\"0.75\" zOffset=\"0.5\" bitmapFile=\"lofiObjBig\" bitmapIndex=\"10\">Orbiting</Effect><Effect color=\"0xFF0000\" size=\"12\" speed=\"270\" radius=\"0.75\" zOffset=\"0.45\" angleOffset=\"-10\">Orbiting</Effect><Effect color=\"0xFF5500\" size=\"8\" speed=\"270\" radius=\"0.75\" zOffset=\"0.45\" angleOffset=\"-20\">Orbiting</Effect><Effect color=\"0xFFAA00\" size=\"6\" speed=\"270\" radius=\"0.75\" zOffset=\"0.45\" angleOffset=\"-30\">Orbiting</Effect><Effect color=\"0xFFFF00\" size=\"4\" speed=\"270\" radius=\"0.75\" zOffset=\"0.45\" angleOffset=\"-40\">Orbiting</Effect></Effects>"},
            {"Misty Skulls", "<Effect rate=\"2\" life=\"1.5\" lifeVariance=\"0.1\" speed=\"0\" speedVariance=\"0\" rise=\"0\" size=\"0\" growth=\"60\" fadeOut=\"0.5\" bitmapFile=\"lofiObj6\" bitmapIndex=\"0\" rangeX=\"0.5\" rangeY=\"0.5\" rangeZ=\"0.75\" attached=\"true\">CustomParticles</Effect>"}
        };

        public static string Save(string name)
        {
            if (!Types.ContainsKey(name))
                return name;
            Random rand = new Random();
            List<string> args = new List<string>();
            args.Add(name);
            switch (name)
            {
                case "Sporadic Sparkles":
                case "Tinted Trail":
                case "Rising Bubbles":
                    args.Add(rand.Next(0, 0xFFFFFF + 1).ToString());
                    break;
            }
            return Utils.GetCommaSepString(args.ToArray());
        }

        public static string GetXML(string effectString)
        {
            string[] args = Utils.FromCommaSepString(effectString);
            if (args.Length == 0)
                return "";
            if (!Types.ContainsKey(args[0]))
                return "";
            if (args.Length == 1)
                return Types[args[0]];
            if (args.Length == 2)
                return string.Format(Types[args[0]], args[1]);
            return string.Format(Types[args[0]], args.Skip(1));
        }

        public static Dictionary<int, List<string>> Series = new Dictionary<int, List<string>>();

        protected static void AddSeries(int num, params string[] effects)
        {
            Series.Add(num, effects.ToList());
        }
    }
}