using System;

namespace wServer.realm
{
    public enum CurrencyType
    {
        Gold = 0,
        Fame = 1,
        GuildFame = 2,
        FortuneTokens = 3,
        Onrane = 4,
        Kantos = 5
    }

    public struct StatsType
    {
        public static readonly StatsType MaximumHP = 0;
        public static readonly StatsType HP = 1;
        public static readonly StatsType Size = 2;
        public static readonly StatsType MaximumMP = 3;
        public static readonly StatsType MP = 4;
        public static readonly StatsType ExperienceGoal = 5;
        public static readonly StatsType Experience = 6;
        public static readonly StatsType Level = 7;
        public static readonly StatsType Inventory0 = 8;
        public static readonly StatsType Inventory1 = 9;
        public static readonly StatsType Inventory2 = 10;
        public static readonly StatsType Inventory3 = 11;
        public static readonly StatsType Inventory4 = 12;
        public static readonly StatsType Inventory5 = 13;
        public static readonly StatsType Inventory6 = 14;
        public static readonly StatsType Inventory7 = 15;
        public static readonly StatsType Inventory8 = 16;
        public static readonly StatsType Inventory9 = 17;
        public static readonly StatsType Inventory10 = 18;
        public static readonly StatsType Inventory11 = 19;
        public static readonly StatsType Attack = 20;
        public static readonly StatsType Defense = 21;
        public static readonly StatsType Speed = 22;
        public static readonly StatsType Vitality = 26;
        public static readonly StatsType Wisdom = 27;
        public static readonly StatsType Dexterity = 28;
        public static readonly StatsType Effects = 29;
        public static readonly StatsType Stars = 30;
        public static readonly StatsType Name = 31; //Is UTF
        public static readonly StatsType Texture1 = 32;
        public static readonly StatsType Texture2 = 33;
        public static readonly StatsType MerchantMerchandiseType = 34;
        public static readonly StatsType Credits = 35;
        public static readonly StatsType SellablePrice = 36;
        public static readonly StatsType PortalUsable = 37;
        public static readonly StatsType AccountId = 38; //Is UTF
        public static readonly StatsType CurrentFame = 39;
        public static readonly StatsType SellablePriceCurrency = 40;
        public static readonly StatsType ObjectConnection = 41;
        /*
         * Mask :F0F0F0F0
         * each byte > type
         * 0:Dot
         * 1:ushortLine
         * 2:L
         * 3:Line
         * 4:T
         * 5:Cross
         * 0x21222112
        */
        public static readonly StatsType MerchantRemainingCount = 42;
        public static readonly StatsType MerchantRemainingMinute = 43;
        public static readonly StatsType MerchantDiscount = 44;
        public static readonly StatsType SellableRankRequirement = 45;
        public static readonly StatsType HPBoost = 46;
        public static readonly StatsType MPBoost = 47;
        public static readonly StatsType AttackBonus = 48;
        public static readonly StatsType DefenseBonus = 49;
        public static readonly StatsType SpeedBonus = 50;
        public static readonly StatsType VitalityBonus = 51;
        public static readonly StatsType WisdomBonus = 52;
        public static readonly StatsType DexterityBonus = 53;
        public static readonly StatsType OwnerAccountId = 54; //Is UTF
        public static readonly StatsType NameChangerStar = 55;
        public static readonly StatsType NameChosen = 56;
        public static readonly StatsType Fame = 57;
        public static readonly StatsType FameGoal = 58;
        public static readonly StatsType Glowing = 59;
        public static readonly StatsType SinkOffset = 60;
        public static readonly StatsType AltTextureIndex = 61;
        public static readonly StatsType Guild = 62; //Is UTF
        public static readonly StatsType GuildRank = 63;
        public static readonly StatsType OxygenBar = 64;
        public static readonly StatsType XpBoosterActive = 65;
        public static readonly StatsType XpBoosterTime = 66;
        public static readonly StatsType LootDropBoostTimer = 67;
        public static readonly StatsType LootTierBoostTimer = 68;
        public static readonly StatsType HealStackCount = 69;
        public static readonly StatsType MagicStackCount = 70;
        public static readonly StatsType Backpack0 = 71;
        public static readonly StatsType Backpack1 = 72;
        public static readonly StatsType Backpack2 = 73;
        public static readonly StatsType Backpack3 = 74;
        public static readonly StatsType Backpack4 = 75;
        public static readonly StatsType Backpack5 = 76;
        public static readonly StatsType Backpack6 = 77;
        public static readonly StatsType Backpack7 = 78;
        public static readonly StatsType Has_Backpack = 79;
        public static readonly StatsType Skin = 80;
        public static readonly StatsType PetId = 81;
        public static readonly StatsType PetSkin = 82; //Is UTF
        public static readonly StatsType PetType = 83;
        public static readonly StatsType PetRarity = 84;
        public static readonly StatsType PetMaximumLevel = 85;
        public static readonly StatsType PetNothing = 86; //This does do nothing in the client
        public static readonly StatsType PetPoints0 = 87;
        public static readonly StatsType PetPoints1 = 88;
        public static readonly StatsType PetPoints2 = 89;
        public static readonly StatsType PetLevel0 = 90;
        public static readonly StatsType PetLevel1 = 91;
        public static readonly StatsType PetLevel2 = 92;
        public static readonly StatsType PetAbilityType0 = 93;
        public static readonly StatsType PetAbilityType1 = 94;
        public static readonly StatsType PetAbilityType2 = 95;
        public static readonly StatsType Effects2 = 96;

        //case StatData._-04I:
        //   param1.condition_[ConditionEffect._-KL]=_loc8_;
        //   break;
        public static readonly StatsType Tokens = 97;

        public static readonly StatsType Onrane = 98;

        public static readonly StatsType Kantos = 99;

        public static readonly StatsType MarkId = 100;

        public static readonly StatsType CookingLevel = 101;
        public static readonly StatsType StarType = 102;
		public static readonly StatsType Might = 103;
		public static readonly StatsType Luck = 104;
		public static readonly StatsType MightBonus = 105;
		public static readonly StatsType LuckBonus = 106;
        private byte _type;

        private StatsType(byte type)
        {
            this._type = type;
        }

        public bool IsUTF()
        {
            if (this == StatsType.Name || this == StatsType.AccountId || this == StatsType.OwnerAccountId
               || this == StatsType.Guild || this == StatsType.PetSkin)
                return true;
            return false;
        }

        public static implicit operator StatsType(int type)
        {
            if (type > byte.MaxValue)
                throw new Exception("Not a valid StatData number.");
            return new StatsType((byte)type);
        }

        public static implicit operator StatsType(byte type)
        {
            return new StatsType(type);
        }

        public static bool operator ==(StatsType type, int id)
        {
            if (id > byte.MaxValue)
                throw new Exception("Not a valid StatData number.");
            return type._type == (byte)id;
        }

        public static bool operator ==(StatsType type, byte id)
        {
            return type._type == id;
        }

        public static bool operator !=(StatsType type, int id)
        {
            if (id > byte.MaxValue)
                throw new Exception("Not a valid StatData number.");
            return type._type != (byte)id;
        }

        public static bool operator !=(StatsType type, byte id)
        {
            return type._type != id;
        }

        public static bool operator ==(StatsType type, StatsType id)
        {
            return type._type == id._type;
        }

        public static bool operator !=(StatsType type, StatsType id)
        {
            return type._type != id._type;
        }

        public static implicit operator int(StatsType type)
        {
            return type._type;
        }

        public static implicit operator byte(StatsType type)
        {
            return type._type;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is StatsType))
                return false;
            return this == (StatsType)obj;
        }

        public override string ToString()
        {
            return _type.ToString();
        }
    }
}