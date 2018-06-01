#region

using db.data;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace wServer.realm.entities.merchant
{
    internal class MerchantLists
    {
        public static int[] AccessoryClothList;
        public static int[] AccessoryDyeList;
        public static int[] ClothingClothList;
        public static int[] ClothingDyeList;
        public static bool TM = Program.TestingMerchants;

        public static Dictionary<int, Tuple<int, CurrencyType>> prices = new Dictionary<int, Tuple<int, CurrencyType>>
        {
            {0xb41, new Tuple<int, CurrencyType>(0, CurrencyType.Fame)},
            {0xbab, new Tuple<int, CurrencyType>(0, CurrencyType.Fame)},
            {0xbad, new Tuple<int, CurrencyType>(0, CurrencyType.Fame)},

            #region Weapons
            {0xa07, new Tuple<int, CurrencyType>(TM ? 1 : 200, TM ? CurrencyType.Fame : CurrencyType.Fame)},  // (T8) Wand of Death - 51 Fame
            {0xa85, new Tuple<int, CurrencyType>(TM ? 1 : 600, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T9) Wand of Deep Sorcery - 150 Fame
            {0xa86, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T10) Wand of Shadow - 225 Fame
            {0xa87, new Tuple<int, CurrencyType>(TM ? 1 : 1500, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T11) Wand of Ancient Warning - 450 Fame
            {0xaf6, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T12) Wand of Recompense - 550 Fame

            {0xa1e, new Tuple<int, CurrencyType>(TM ? 1 : 200, TM ? CurrencyType.Fame : CurrencyType.Fame)},  // (T8) Fameen Bow - 51 Fame
            {0xa8b, new Tuple<int, CurrencyType>(TM ? 1 : 600, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T9) Verdant Bow - 150 Fame
            {0xa8c, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T10) Bow of Fey Magic - 225 Fame
            {0xa8d, new Tuple<int, CurrencyType>(TM ? 1 : 1500, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T11) Bow of Innocent Blood - 450 Fame
            {0xb02, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T12) Bow of Covert Havens - 600 Fame

            {0xa19, new Tuple<int, CurrencyType>(TM ? 1 : 200, TM ? CurrencyType.Fame : CurrencyType.Fame)},  // (T8) Fire Dagger - 51 Fame
            {0xa88, new Tuple<int, CurrencyType>(TM ? 1 : 600, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T9) Ragetalon Dagger - 150 Fame
            {0xa89, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T10) Emeraldshard Dagger - 225 Fame
            {0xa8a, new Tuple<int, CurrencyType>(TM ? 1 : 1500, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T11) Agateclaw Dagger - 450 Fame
            {0xaff, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T12) Dagger of Foul Malevolence - 650 Fame

            {0xa82, new Tuple<int, CurrencyType>(TM ? 1 : 200, TM ? CurrencyType.Fame : CurrencyType.Fame)},  // (T8) Ravenheart Sword - 51 Fame
            {0xa83, new Tuple<int, CurrencyType>(TM ? 1 : 600, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T9) Dragonsoul Sword - 150 Fame
            {0xa84, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T10) Archon Sword - 225 Fame
            {0xa47, new Tuple<int, CurrencyType>(TM ? 1 : 1500, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T11) Skysplitter Sword - 450 Fame
            {0xb0b, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T12) Sword of Acclaim - 900 Fame

            {0xa9f, new Tuple<int, CurrencyType>(TM ? 1 : 200, TM ? CurrencyType.Fame : CurrencyType.Fame)},  // (T8) Staff of Horror - 51 Fame
            {0xaa0, new Tuple<int, CurrencyType>(TM ? 1 : 600, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T9) Staff of Necrotic Arcana - 150 Fame
            {0xaa1, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T10) Staff of Diabolic Secrets - 225 Fame
            {0xaa2, new Tuple<int, CurrencyType>(TM ? 1 : 1500, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T11) Staff of Astral Knowledge - 450 Fame
            {0xb08, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T12) Staff of the Cosmic Whole - 900 Fame

            {0xc4c, new Tuple<int, CurrencyType>(TM ? 1 : 200, TM ? CurrencyType.Fame : CurrencyType.Fame)},  // (T8) Demon Edge - 51 Fame
            {0xc4d, new Tuple<int, CurrencyType>(TM ? 1 : 600, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T9) Jewel Eye Katana - 150 Fame
            {0xc4e, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T10) Ichimonji - 225 Fame
            {0xc4f, new Tuple<int, CurrencyType>(TM ? 1 : 1500, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T11) Muramasa - 450 Fame
            {0xc50, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T12) Masamune - 700 Fame
            #endregion

            #region Rings
            {0xabf, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Fame)},  // (T4) Ring of Paramount Attack - 300 Fame
            {0xac0, new Tuple<int, CurrencyType>(TM ? 1 : 750, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T4) Ring of Paramount Defense - 225 Fame
            {0xac1, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Fame)},  // (T4) Ring of Paramount Speed - 300 Fame
            {0xac2, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Fame)},  // (T4) Ring of Paramount Vitality - 300 Fame
            {0xac3, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Fame)},  // (T4) Ring of Paramount Wisdom - 300 Fame
            {0xac4, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Fame)},  // (T4) Ring of Paramount Dexterity - 300 Fame
            {0xac5, new Tuple<int, CurrencyType>(TM ? 1 : 750, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T4) Ring of Paramount Health - 225 Fame
            {0xac6, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T4) Ring of Paramount Magic - 225 Fame

            {0xac7, new Tuple<int, CurrencyType>(TM ? 1 : 600, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T5) Ring of Exalted Attack - 600 Fame
            {0xac8, new Tuple<int, CurrencyType>(TM ? 1 : 1500, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T5) Ring of Exalted Defense - 360 Fame
            {0xac9, new Tuple<int, CurrencyType>(TM ? 1 : 600, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T5) Ring of Exalted Speed - 600 Fame
            {0xaca, new Tuple<int, CurrencyType>(TM ? 1 : 600, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T5) Ring of Exalted Vitality - 600 Fame
            {0xacb, new Tuple<int, CurrencyType>(TM ? 1 : 600, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T5) Ring of Exalted Wisdom - 600 Fame
            {0xacc, new Tuple<int, CurrencyType>(TM ? 1 : 600, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T5) Ring of Exalted Dexterity - 600 Fame
            {0xacd, new Tuple<int, CurrencyType>(TM ? 1 : 1500, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T5) Ring of Exalted Health - 360 Fame
            {0xace, new Tuple<int, CurrencyType>(TM ? 1 : 600, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // (T5) Ring of Exalted Magic - 360 Fame
            #endregion

            #region Armor
            {0xa13, new Tuple<int, CurrencyType>(TM ? 1 : 200, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xa60, new Tuple<int, CurrencyType>(TM ? 1 : 200, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xa8e, new Tuple<int, CurrencyType>(TM ? 1 : 600, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xa8f, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xa90, new Tuple<int, CurrencyType>(TM ? 1 : 1500, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xa91, new Tuple<int, CurrencyType>(TM ? 1 : 600, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xa92, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xa93, new Tuple<int, CurrencyType>(TM ? 1 : 1500, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xa94, new Tuple<int, CurrencyType>(TM ? 1 : 600, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xa95, new Tuple<int, CurrencyType>(TM ? 1 : 600, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xa96, new Tuple<int, CurrencyType>(TM ? 1 : 1500, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xad3, new Tuple<int, CurrencyType>(TM ? 1 : 200, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xaf9, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xafc, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xb05, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            #endregion

            #region ABILITIES
            {0xa0c, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xa30, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xa46, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xa55, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xa5b, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xa65, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xa6b, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xaa8, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xaaf, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xab6, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xae1, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xb20, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xb22, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xb23, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xb24, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xb25, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xb26, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xb27, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xb28, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xb29, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xb2a, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xb2b, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xb2c, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xb2d, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xb32, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xb33, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xc58, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0xc59, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0x185f, new Tuple<int, CurrencyType>(TM ? 1 : 1000, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // sheath t5
            {0x1862, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Fame)}, // sheath t6
            #endregion

            #region Pet food
            {0xcc4, new Tuple<int, CurrencyType>(TM ? 1 : 110, TM ? CurrencyType.Fame : CurrencyType.Gold)},
            {0xcc5, new Tuple<int, CurrencyType>(TM ? 1 : 75, TM ? CurrencyType.Fame : CurrencyType.Gold)},
            {0xcc6, new Tuple<int, CurrencyType>(TM ? 1 : 50, TM ? CurrencyType.Fame : CurrencyType.Gold)},
            {0xcc7, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Gold)},
            {0xcc8, new Tuple<int, CurrencyType>(TM ? 1 : 200, TM ? CurrencyType.Fame : CurrencyType.Gold)},
            {0xcc9, new Tuple<int, CurrencyType>(TM ? 1 : 10, TM ? CurrencyType.Fame : CurrencyType.Gold)},
            {0xcca, new Tuple<int, CurrencyType>(TM ? 1 : 150, TM ? CurrencyType.Fame : CurrencyType.Gold)},
            {0xccb, new Tuple<int, CurrencyType>(TM ? 1 : 25, TM ? CurrencyType.Fame : CurrencyType.Gold)},
            {0xccc, new Tuple<int, CurrencyType>(TM ? 1 : 500, TM ? CurrencyType.Fame : CurrencyType.Gold)},
            #endregion

            #region Eggs
            {0xc86, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //uncommon feline egg
            {0xc87, new Tuple<int, CurrencyType>(TM ? 1 : 500, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //rare feline egg
            {0xc8a, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //uncommon canine egg
            {0xc8b, new Tuple<int, CurrencyType>(TM ? 1 : 500, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //rare canine egg
            {0xc8e, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //uncommon avian egg
            {0xc8f, new Tuple<int, CurrencyType>(TM ? 1 : 500, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //rare avian egg
            {0xc92, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //uncommon exotic egg
            {0xc93, new Tuple<int, CurrencyType>(TM ? 1 : 500, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //rare exotic egg
            {0xc96, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //uncommon farm egg
            {0xc97, new Tuple<int, CurrencyType>(TM ? 1 : 500, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //rare farm egg
            {0xc9a, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //uncommon woodland egg
            {0xc9b, new Tuple<int, CurrencyType>(TM ? 1 : 500, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //rare woodland egg
            {0xc9e, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //uncommon reptile egg
            {0xc9f, new Tuple<int, CurrencyType>(TM ? 1 : 500, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //rare reptile egg
            {0xca2, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //uncommon insect egg
            {0xca3, new Tuple<int, CurrencyType>(TM ? 1 : 500, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //rare insect egg
            {0xca6, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //uncommon pinguin egg
            {0xca7, new Tuple<int, CurrencyType>(TM ? 1 : 500, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //rare pinguin egg
            {0xcaa, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //uncommon aquatic egg
            {0xcab, new Tuple<int, CurrencyType>(TM ? 1 : 500, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //rare aquatic egg
            {0xcae, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //uncommon spooky egg
            {0xcaf, new Tuple<int, CurrencyType>(TM ? 1 : 500, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //rare spooky egg
            {0xcb2, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //uncommon humanoid egg
            {0xcb3, new Tuple<int, CurrencyType>(TM ? 1 : 500, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //rare humanoid egg
            {0xcb6, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //uncommon ???? egg
            {0xcb7, new Tuple<int, CurrencyType>(TM ? 1 : 500, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //rare ???? egg
            {0xcba, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //uncommon automaton egg
            {0xcbb, new Tuple<int, CurrencyType>(TM ? 1 : 500, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //rare automaton egg
            {0xcbe, new Tuple<int, CurrencyType>(TM ? 1 : 240, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //uncommon mystery egg
            {0xcbf, new Tuple<int, CurrencyType>(TM ? 1 : 400, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //rare mystery egg
            {0xc85, new Tuple<int, CurrencyType>(TM ? 1 : 50, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //commmon feline egg
            {0xc88, new Tuple<int, CurrencyType>(TM ? 1 : 800, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //legendary feline egg
            {0xc89, new Tuple<int, CurrencyType>(TM ? 1 : 50, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //commmon canine egg
            {0xc8c, new Tuple<int, CurrencyType>(TM ? 1 : 800, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //legendary canine egg
            {0xc8d, new Tuple<int, CurrencyType>(TM ? 1 : 50, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //commmon avian egg
            {0xc90, new Tuple<int, CurrencyType>(TM ? 1 : 800, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //legendary avian egg
            {0xc91, new Tuple<int, CurrencyType>(TM ? 1 : 50, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //commmon exotic egg
            {0xc94, new Tuple<int, CurrencyType>(TM ? 1 : 800, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //legendary exotic egg
            {0xc95, new Tuple<int, CurrencyType>(TM ? 1 : 50, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //commmon farm egg
            {0xc98, new Tuple<int, CurrencyType>(TM ? 1 : 800, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //legendary farm egg
            {0xc99, new Tuple<int, CurrencyType>(TM ? 1 : 50, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //commmon woodland egg
            {0xc9c, new Tuple<int, CurrencyType>(TM ? 1 : 800, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //legendary woodland egg
            {0xc9d, new Tuple<int, CurrencyType>(TM ? 1 : 50, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //commmon reptile egg
            {0xca0, new Tuple<int, CurrencyType>(TM ? 1 : 800, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //legendary reptile egg
            {0xca1, new Tuple<int, CurrencyType>(TM ? 1 : 50, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //commmon insect egg
            {0xca4, new Tuple<int, CurrencyType>(TM ? 1 : 800, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //legendary insect egg
            {0xca5, new Tuple<int, CurrencyType>(TM ? 1 : 50, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //commmon pinguin egg
            {0xca8, new Tuple<int, CurrencyType>(TM ? 1 : 800, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //legendary pinguin egg
            {0xca9, new Tuple<int, CurrencyType>(TM ? 1 : 50, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //commmon aquatic egg
            {0xcac, new Tuple<int, CurrencyType>(TM ? 1 : 800, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //legendary aquatic egg
            {0xcad, new Tuple<int, CurrencyType>(TM ? 1 : 50, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //commmon spooky egg
            {0xcb0, new Tuple<int, CurrencyType>(TM ? 1 : 800, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //legendary spooky egg
            {0xcb1, new Tuple<int, CurrencyType>(TM ? 1 : 50, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //commmon humanoid egg
            {0xcb4, new Tuple<int, CurrencyType>(TM ? 1 : 800, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //legendary humanoid egg
            {0xcb5, new Tuple<int, CurrencyType>(TM ? 1 : 50, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //commmon ???? egg
            {0xcb8, new Tuple<int, CurrencyType>(TM ? 1 : 800, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //legendary ???? egg
            {0xcb9, new Tuple<int, CurrencyType>(TM ? 1 : 50, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //commmon automaton egg
            {0xcbc, new Tuple<int, CurrencyType>(TM ? 1 : 800, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //legendary automaton egg
            {0xcbd, new Tuple<int, CurrencyType>(TM ? 1 : 40, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //commmon mystery egg
            {0xcc0, new Tuple<int, CurrencyType>(TM ? 1 : 640, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //legendary mystery egg
            #endregion

            #region backpacks
            {0xc6c, new Tuple<int, CurrencyType>(TM ? 1 : 2000, TM ? CurrencyType.Fame : CurrencyType.Gold)},
            //only 1 price can be made for item{0xc6c, new Tuple<int, CurrencyType>(TM ? 1 : 200, TM ? CurrencyType.Gold : CurrencyType.Gold)},
            #endregion

            #region Keys
            {0x2290, new Tuple<int, CurrencyType>(TM ? 1 : 240, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Bella's Key - just temponary for testing
            {0x701, new Tuple<int, CurrencyType>(TM ? 1 : 220, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Undead lair key
            {0x705, new Tuple<int, CurrencyType>(TM ? 1 : 90, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Pirate cave key
            {0x70a, new Tuple<int, CurrencyType>(TM ? 1 : 225, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Abyss of demons key
            {0x70b, new Tuple<int, CurrencyType>(TM ? 1 : 200, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Snake pit key
            {0x710, new Tuple<int, CurrencyType>(TM ? 1 : 390, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Tomb of the ancients key
            {0x71f, new Tuple<int, CurrencyType>(TM ? 1 : 170, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Sprite World Key
            {0xc11, new Tuple<int, CurrencyType>(TM ? 1 : 390, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Ocean Trench Key
            {0xc19, new Tuple<int, CurrencyType>(TM ? 1 : 200, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Totem Key
            {0xc23, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Manor Key
            {0xc2e, new Tuple<int, CurrencyType>(TM ? 1 : 300, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Daby's Key
            {0xc2f, new Tuple<int, CurrencyType>(TM ? 1 : 350, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Lab Key
            {0xccf, new Tuple<int, CurrencyType>(TM ? 1 : 290, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Woodland Labyrinth Key
            {0xcda, new Tuple<int, CurrencyType>(TM ? 1 : 290, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //The crawling depths key
            {0xcdd, new Tuple<int, CurrencyType>(TM ? 1 : 210, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Shatters key
            {0x158a, new Tuple<int, CurrencyType>(TM ? 1 : 260, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Hidden Temple key
            {0x1855, new Tuple<int, CurrencyType>(TM ? 1 : 280, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Elemental Ruins key
            {0x1669, new Tuple<int, CurrencyType>(TM ? 1 : 200, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Stormy Palace key
            {0xcce, new Tuple<int, CurrencyType>(TM ? 1 : 290, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Deadwater Docks Palace key
            {0x199e, new Tuple<int, CurrencyType>(TM ? 1 : 200, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Tunnel of Pain key
            {0x5149, new Tuple<int, CurrencyType>(TM ? 1 : 360, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Heavenly Rift Palace key
            {0x5121, new Tuple<int, CurrencyType>(TM ? 1 : 390, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Concealment key
            {0x5050, new Tuple<int, CurrencyType>(TM ? 1 : 390, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Unspeakable key
            {0x47ff, new Tuple<int, CurrencyType>(TM ? 1 : 2100, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Galactic Plateau key
            {0x45dc, new Tuple<int, CurrencyType>(TM ? 1 : 280, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Prison of Time
            {0x46fb, new Tuple<int, CurrencyType>(TM ? 1 : 260, TM ? CurrencyType.Fame : CurrencyType.Gold)}, //Hive Key

            {0x51aa, new Tuple<int, CurrencyType>(TM ? 1 : 1200, TM ? CurrencyType.Fame : CurrencyType.Fame)},

            {0x5172, new Tuple<int, CurrencyType>(TM ? 1 : 3, TM ? CurrencyType.Fame : CurrencyType.FortuneTokens)},
            {0x5173, new Tuple<int, CurrencyType>(TM ? 1 : 6, TM ? CurrencyType.Fame : CurrencyType.FortuneTokens)},
            {0x5174, new Tuple<int, CurrencyType>(TM ? 1 : 16, TM ? CurrencyType.Fame : CurrencyType.FortuneTokens)},
            {0x5175, new Tuple<int, CurrencyType>(TM ? 1 : 18, TM ? CurrencyType.Fame : CurrencyType.FortuneTokens)},
            {0x5176, new Tuple<int, CurrencyType>(TM ? 1 : 22, TM ? CurrencyType.Fame : CurrencyType.FortuneTokens)},

            {0x49a8, new Tuple<int, CurrencyType>(TM ? 1 : 4, TM ? CurrencyType.Fame : CurrencyType.Onrane)},
			{0x56b1, new Tuple<int, CurrencyType>(TM ? 1 : 100, TM ? CurrencyType.Fame : CurrencyType.Kantos)},
            {0x55f1, new Tuple<int, CurrencyType>(TM ? 1 : 1200, TM ? CurrencyType.Fame : CurrencyType.Kantos)},
            {0x49e8, new Tuple<int, CurrencyType>(TM ? 1 : 6200, TM ? CurrencyType.Fame : CurrencyType.Fame)},
            {0x58f6, new Tuple<int, CurrencyType>(TM ? 1 : 600, TM ? CurrencyType.Fame : CurrencyType.Kantos)},
            //Aldragine Legendaries
            {0x32c2, new Tuple<int, CurrencyType>(TM ? 1 : 62, TM ? CurrencyType.Fame : CurrencyType.Onrane)},
            {0x45d8, new Tuple<int, CurrencyType>(TM ? 1 : 50, TM ? CurrencyType.Fame : CurrencyType.Onrane)},
            {0x43a4, new Tuple<int, CurrencyType>(TM ? 1 : 58, TM ? CurrencyType.Fame : CurrencyType.Onrane)},
            {0x43a7, new Tuple<int, CurrencyType>(TM ? 1 : 48, TM ? CurrencyType.Fame : CurrencyType.Onrane)},
            {0x48fe, new Tuple<int, CurrencyType>(TM ? 1 : 68, TM ? CurrencyType.Fame : CurrencyType.Onrane)},
            {0x1468, new Tuple<int, CurrencyType>(TM ? 1 : 60, TM ? CurrencyType.Fame : CurrencyType.Onrane)},
            {0x53c8, new Tuple<int, CurrencyType>(TM ? 1 : 70, TM ? CurrencyType.Fame : CurrencyType.Onrane)},
            {0x55b8, new Tuple<int, CurrencyType>(TM ? 1 : 70, TM ? CurrencyType.Fame : CurrencyType.Onrane)},

            #endregion
        };

		public static int[] store10List = { 0x49e8, 0x56b1, 0x55f1, 0x49a8, 0x58f6 };
        public static int[] store11List = { 0x32c2, 0x45d8, 0x43a4, 0x43a7, 0x48fe, 0x1468, 0x53c8, 0x55b8 }; //Aldragine Legendary Shop
        public static int[] store12List = { 0xb41, 0xbab, 0xbad, 0xbac };
        public static int[] store13List = { 0xb41, 0xbab, 0xbad, 0xbac };
        public static int[] store14List = { 0xb41, 0xbab, 0xbad, 0xbac };
        public static int[] store15List = { 0xb41, 0xbab, 0xbad, 0xbac };
        public static int[] store16List = { 0xb41, 0xbab, 0xbad, 0xbac };
        public static int[] store17List = { 0xb41, 0xbab, 0xbad, 0xbac };
        public static int[] store18List = { 0xb41, 0xbab, 0xbad, 0xbac };
        public static int[] store19List = { 0xb41, 0xbab, 0xbad, 0xbac };

        public static int[] store1List =
        {
            0xcdd, 0xcda, 0xccf, 0xcce, 0xc2f, 0xc2e, 0xc23, 0xc19, 0xc11, 0x71f, 0x710,
            0x70b, 0x70a, 0x705, 0x701, 0x2290, 0x1855, 0x1669, 0x158a, 0x199e, 0x5149, 0x5121,
            0x5050, 0x47ff, 0x45dc, 0x46fb
        };

        public static int[] store20List = { 0xb41, 0xbab, 0xbad, 0xbac };

        //keys need to add etcetc
        public static int[] store2List =
        {
            0xcbf, 0xcbe, 0xcbb, 0xcba, 0xcb7, 0xcb6, 0xcb2, 0xcb3, 0xcae, 0xcaf, 0xcab,
            0xcaa, 0xca7, 0xca6, 0xca3, 0xca2, 0xc9f, 0xc9e, 0xc9b, 0xc9a, 0xc97, 0xc96, 0xc93, 0xc92, 0xc8f, 0xc8e,
            0xc8b, 0xc8a, 0xc87, 0xc86, 0xc85, 0xc88, 0xc89, 0xc8c, 0xc8d, 0xc91, 0xc94, 0xc95, 0xc98, 0xc99, 0xc9c, 0xc9d, 0xca0, 0xca1, 0xca4, 0xca5,
            0xca8, 0xca9, 0xcac, 0xcad, 0xcb0, 0xcb1, 0xcb4, 0xcb5, 0xcb8, 0xcb9, 0xcbc, 0xcbd, 0xcb0
        };

        //pet eggs
        public static int[] store3List = { 0xccc, 0xccb, 0xcca, 0xcc9, 0xcc8, 0xcc7, 0xcc6, 0xcc5, 0xcc4 };

        //pet food
        public static int[] store4List =
        {
            0xb25, 0xa5b, 0xb22, 0xa0c, 0xb24, 0xa30, 0xb26, 0xa55, 0xb27, 0xae1, 0xb28,
            0xa65, 0xb29, 0xa6b, 0xb2a, 0xaa8, 0xb2b, 0xaaf, 0xb2c, 0xab6, 0xb2d, 0xa46, 0xb23, 0xb20, 0xb33, 0xb32,
            0xc59, 0xc58, 0x185f, 0x1862
        };

        //abilities
        public static int[] store5List =
        {
            0xb05, 0xa96, 0xa95, 0xa94, 0xa60, 0xafc, 0xa93, 0xa92, 0xa91, 0xa13, 0xaf9,
            0xa90, 0xa8f, 0xa8e, 0xad3
        };

        //armors
        public static int[] store6List =
        {
            0xaf6, 0xa87, 0xa86, 0xa85, 0xa07, 0xb02, 0xa8d, 0xa8c, 0xa8b, 0xa1e, 0xb08,
            0xaa2, 0xaa1, 0xaa0, 0xa9f
        };

        //Wands&staves&bows
        public static int[] store7List =
        {
            0xb0b, 0xa47, 0xa84, 0xa83, 0xa82, 0xaff, 0xa8a, 0xa89, 0xa88, 0xa19, 0xc50,
            0xc4f, 0xc4e, 0xc4d, 0xc4c
        };

        //Swords&daggers&samurai shit
        public static int[] store8List =
        {
            0xabf, 0xac0, 0xac1, 0xac2, 0xac3, 0xac4, 0xac5, 0xac6, 0xac7, 0xac8, 0xac9,
            0xaca, 0xacb, 0xacc, 0xacd, 0xace
        };

        // rings
        public static int[] store9List = { 0x5172, 0x5173, 0x5174, 0x5175, 0x5176, 0x51aa };

        private static readonly ILog log = LogManager.GetLogger(typeof(MerchantLists));

        public static void InitMerchatLists(XmlData data)
        {
            log.Info("Loading merchant lists...");
            List<int> accessoryDyeList = new List<int>();
            List<int> clothingDyeList = new List<int>();
            List<int> accessoryClothList = new List<int>();
            List<int> clothingClothList = new List<int>();

            foreach (KeyValuePair<ushort, Item> item in data.Items.Where(_ => noShopCloths.All(i => i != _.Value.ObjectId)))
            {
                if (item.Value.Texture1 != 0 && item.Value.ObjectId.Contains("Clothing") && item.Value.Class == "Dye")
                {
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(150, CurrencyType.Fame));
                    clothingDyeList.Add(item.Value.ObjectType);
                }

                if (item.Value.Texture2 != 0 && item.Value.ObjectId.Contains("Accessory") && item.Value.Class == "Dye")
                {
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(75, CurrencyType.Fame));
                    accessoryDyeList.Add(item.Value.ObjectType);
                }

                if (item.Value.Texture1 != 0 && item.Value.ObjectId.Contains("Cloth") &&
                    item.Value.ObjectId.Contains("Large"))
                {
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(150, CurrencyType.Gold));
                    clothingClothList.Add(item.Value.ObjectType);
                }

                if (item.Value.Texture2 != 0 && item.Value.ObjectId.Contains("Cloth") &&
                    item.Value.ObjectId.Contains("Small"))
                {
                    prices.Add(item.Value.ObjectType, new Tuple<int, CurrencyType>(75, CurrencyType.Gold));
                    accessoryClothList.Add(item.Value.ObjectType);
                }
            }

            ClothingDyeList = clothingDyeList.ToArray();
            ClothingClothList = clothingClothList.ToArray();
            AccessoryClothList = accessoryClothList.ToArray();
            AccessoryDyeList = accessoryDyeList.ToArray();
            log.Info("Merchat lists added.");
        }

        private static readonly string[] noShopCloths =
        {
            "Large Ivory Dragon Scale Cloth", "Small Ivory Dragon Scale Cloth",
            "Large Green Dragon Scale Cloth", "Small Green Dragon Scale Cloth",
            "Large Midnight Dragon Scale Cloth", "Small Midnight Dragon Scale Cloth",
            "Large Blue Dragon Scale Cloth", "Small Blue Dragon Scale Cloth",
            "Large Red Dragon Scale Cloth", "Small Red Dragon Scale Cloth",
            "Large Jester Argyle Cloth", "Small Jester Argyle Cloth",
            "Large Alchemist Cloth", "Small Alchemist Cloth",
            "Large Mosaic Cloth", "Small Mosaic Cloth",
            "Large Spooky Cloth", "Small Spooky Cloth",
            "Large Flame Cloth", "Small Flame Cloth",
            "Large Heavy Chainmail Cloth", "Small Heavy Chainmail Cloth",
        };
    }
}