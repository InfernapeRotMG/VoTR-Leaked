﻿#region

using db;
using db.data;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using wServer.logic;
using wServer.networking;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;

#endregion

namespace wServer.realm.entities.player
{
    internal interface IPlayer
    {
        void Damage(int dmg, Entity chr, Boolean NoDef);

        bool IsVisibleToEnemy();
    }

    public static class ComparableExtension
    {
        public static bool InRange<T>(this T value, T from, T to) where T : IComparable<T>
        {
            return value.CompareTo(from) >= 1 && value.CompareTo(to) <= -1;
        }
    }

    public partial class Player : Character, IContainer, IPlayer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Player));

        private bool dying;

        private Item[] inventory;

        private float hpRegenCounter;
        private float mpRegenCounter;
        private bool resurrecting;

        private byte[,] tiles;
        private int pingSerial;
        private SetTypeSkin setTypeSkin;

        public Player(RealmManager manager, Client psr)
            : base(manager, (ushort)psr.Character.ObjectType, psr.Random)
        {
            try
            {
                Client = psr;
                Manager = psr.Manager;
                StatsManager = new StatsManager(this, psr.Random.CurrentSeed);
                Name = psr.Account.Name;
                AccountId = psr.Account.AccountId;
                FameCounter = new FameCounter(this);
                Tokens = psr.Account.FortuneTokens;
                HpPotionPrice = 5;
                MpPotionPrice = 5;

                Level = psr.Character.Level == 0 ? 1 : psr.Character.Level;
                Experience = psr.Character.Exp;
                ExperienceGoal = GetExpGoal(Level);
                Stars = StarsUtil.GetStars(psr.Account.Stats);
                StarType = psr.Account.Stats.StarType;
                Texture1 = psr.Character.Tex1;
                Texture2 = psr.Character.Tex2;
                Credits = psr.Account.Credits;
                Onrane = psr.Account.Onrane;
                Kantos = psr.Account.Kantos;
                NameChosen = psr.Account.NameChosen;
                CurrentFame = psr.Account.Stats.Fame;
                Fame = psr.Character.CurrentFame;
                XpBoosted = psr.Character.XpBoosted;
                XpBoostTimeLeft = psr.Character.XpTimer;
                xpFreeTimer = XpBoostTimeLeft != -1.0;
                LootDropBoostTimeLeft = psr.Character.LDTimer;
                lootDropBoostFreeTimer = LootDropBoost;
                LootTierBoostTimeLeft = psr.Character.LTTimer;
                lootTierBoostFreeTimer = LootTierBoost;
                MarkId = psr.Character.MarkId;
                var state =
                    psr.Account.Stats.ClassStates.SingleOrDefault(_ => Utils.FromString(_.ObjectType) == ObjectType);
                FameGoal = StarsUtil.GetFameGoal(state?.BestFame ?? 0);
                Glowing = IsUserInLegends();
                Guild = GuildManager.Add(this, psr.Account.Guild);
                HP = psr.Character.HitPoints <= 0 ? psr.Character.MaxHitPoints : psr.Character.HitPoints;
                Mp = psr.Character.MagicPoints;
                ConditionEffects = 0;
                OxygenBar = 100;
                HasBackpack = psr.Character.HasBackpack;
                PlayerSkin = Client.Account.OwnedSkins.Contains(Client.Character.Skin) ? Client.Character.Skin : 0;
                HealthPotions = psr.Character.HealthStackCount < 0 ? 0 : psr.Character.HealthStackCount;
                MagicPotions = psr.Character.MagicStackCount < 0 ? 0 : psr.Character.MagicStackCount;
                EnchantmentLevel = psr.Account.EnchantmentLevel;
                MarksEnabled = psr.Account.MarksEnabled;
                CookingLevel = psr.Account.CookingLevel;

                Locked = psr.Account.Locked ?? new List<string>();
                Ignored = psr.Account.Ignored ?? new List<string>();
                try
                {
                    Manager.Database.DoActionAsync(db =>
                    {
                        Locked = db.GetLockeds(AccountId);
                        Ignored = db.GetIgnoreds(AccountId);
                        Muted = db.IsMuted(AccountId);
                        DailyQuest = psr.Account.DailyQuest;
                    });
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }

                if (HasBackpack == 1)
                {
                    var inv =
                        psr.Character.Equipment.Select(
                                _ =>
                                    _ == -1
                                        ? null
                                        : (Manager.GameData.Items.ContainsKey((ushort)_)
                                            ? Manager.GameData.Items[(ushort)_]
                                            : null))
                            .ToArray();
                    var backpack =
                        psr.Character.Backpack.Select(
                                _ =>
                                    _ == -1
                                        ? null
                                        : (Manager.GameData.Items.ContainsKey((ushort)_)
                                            ? Manager.GameData.Items[(ushort)_]
                                            : null))
                            .ToArray();

                    Inventory = inv.Concat(backpack).ToArray();
                    var xElement = Manager.GameData.ObjectTypeToElement[ObjectType].Element("SlotTypes");
                    if (xElement != null)
                    {
                        var slotTypes =
                            Utils.FromCommaSepString32(
                                xElement.Value);
                        Array.Resize(ref slotTypes, 20);
                        SlotTypes = slotTypes;
                    }
                }
                else
                {
                    Inventory =
                        psr.Character.Equipment.Select(
                                _ =>
                                    _ == -1
                                        ? null
                                        : (Manager.GameData.Items.ContainsKey((ushort)_)
                                            ? Manager.GameData.Items[(ushort)_]
                                            : null))
                            .ToArray();
                    var xElement = Manager.GameData.ObjectTypeToElement[ObjectType].Element("SlotTypes");
                    if (xElement != null)
                        SlotTypes =
                            Utils.FromCommaSepString32(
                                xElement.Value);
                }
                Stats = new[]
                {
                    psr.Character.MaxHitPoints,
                    psr.Character.MaxMagicPoints,
                    psr.Character.Attack,
                    psr.Character.Defense,
                    psr.Character.Speed,
                    psr.Character.HpRegen,
                    psr.Character.MpRegen,
                    psr.Character.Dexterity,
					psr.Character.Might,
					psr.Character.Luck
                };

                Pet = null;

                for (var i = 0; i < SlotTypes.Length; i++)
                    if (SlotTypes[i] == 0)
                        SlotTypes[i] = 10;

                if (Client.Account.Rank >= 3)
                    return;
                for (var i = 0; i < 4; i++)
                    if (Inventory[i]?.SlotType != SlotTypes[i])
                        Inventory[i] = null;
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }

        ~Player()
        {
            WorldInstance = null;
            Quest = null;
        }

        //Stats
        public string AccountId { get; }

        public int[] Boost { get; private set; }

        public Client Client { get; }

        public int Credits { get; set; }
        public int Tokens { get; set; }
        public int CurrentFame { get; set; }
        public int Onrane { get; set; }
        public int Kantos { get; set; }

        public int Experience { get; set; }
        public int ExperienceGoal { get; set; }

        public int Fame { get; set; }

        public FameCounter FameCounter { get; }

        public QuestItem DailyQuest { get; set; }

        public int FameGoal { get; set; }

        public bool Glowing { get; set; }

        public int HasBackpack { get; set; }

        public int HealthPotions { get; set; }

        public List<string> Ignored { get; set; }

        public bool Invited { get; set; }
        public bool Muted { get; set; }

        public int Level { get; set; }

        public List<string> Locked { get; set; }

        public bool LootDropBoost
        {
            get { return LootDropBoostTimeLeft > 0; }
            set { LootDropBoostTimeLeft = value ? LootDropBoostTimeLeft : 0.0f; }
        }

        public float LootDropBoostTimeLeft { get; set; }

        public bool LootTierBoost
        {
            get { return LootTierBoostTimeLeft > 0; }
            set { LootTierBoostTimeLeft = value ? LootTierBoostTimeLeft : 0.0f; }
        }

        public float LootTierBoostTimeLeft { get; set; }

        public bool XpBoosted { get; set; }
        public int MarkId { get; set; }
        public float XpBoostTimeLeft { get; set; }

        public int MagicPotions { get; set; }

        public ushort HpPotionPrice { get; set; }
        public ushort MpPotionPrice { get; set; }

        public bool HpFirstPurchaseTime { get; set; }
        public bool MpFirstPurchaseTime { get; set; }

        public new RealmManager Manager { get; }

        public int MaxHp { get; set; }

        public int MaxMp { get; set; }

        public int Mp { get; set; }

        public bool NameChosen { get; set; }

        public int OxygenBar { get; set; }

        public Pet Pet { get; set; }

        public int PlayerSkin { get; set; }

        public int Stars { get; set; }
        public int StarType { get; set; }

        public int EnchantmentLevel { get; set; }

        public int MarksEnabled { get; set; }

        public int CookingLevel { get; set; }

        public int[] Stats { get; }

        public StatsManager StatsManager { get; }

        public int Texture1 { get; set; }

        public int Texture2 { get; set; }

        public Item[] Inventory
        {
            get { return inventory; }
            set { inventory = value; }
        }

        public GuildManager Guild { get; set; }

        public int[] SlotTypes { get; set; }

        public void Damage(int dmg, Entity chr, Boolean NoDef)
        {
            try
            {
                if (HasConditionEffect(ConditionEffectIndex.Paused) ||
                    HasConditionEffect(ConditionEffectIndex.Stasis) ||
                    HasConditionEffect(ConditionEffectIndex.Invincible))
                    return;

                dmg = (int)StatsManager.GetDefenseDamage(dmg, NoDef);
                if (!HasConditionEffect(ConditionEffectIndex.Invulnerable))
                    HP -= dmg;
                //Specific Legendary Items
                if (CheckResistance())
                {
                    ApplyConditionEffect(new ConditionEffect
                    {
                        Effect = ConditionEffectIndex.SlowedImmune,
                        DurationMS = -1
                    });
                }
                else
                {
                    ApplyConditionEffect(new ConditionEffect
                    {
                        Effect = ConditionEffectIndex.SlowedImmune,
                        DurationMS = 0
                    });
                }
                if (CheckBWAbility() && HP >= MaxHp / 2)
                {
                    ApplyConditionEffect(new ConditionEffect
                    {
                        Effect = ConditionEffectIndex.DazedImmune,
                        DurationMS = -1
                    });
                }
                else
                {
                    ApplyConditionEffect(new ConditionEffect
                    {
                        Effect = ConditionEffectIndex.DazedImmune,
                        DurationMS = 0
                    });
                }
                if (CheckCResistance() && HP <= MaxHp / 2)
                {
                    ApplyConditionEffect(new ConditionEffect
                    {
                        Effect = ConditionEffectIndex.ArmorBreakImmune,
                        DurationMS = -1
                    });
                }
                else
                {
                    ApplyConditionEffect(new ConditionEffect
                    {
                        Effect = ConditionEffectIndex.ArmorBreakImmune,
                        DurationMS = 0
                    });
                }
				if (CheckGResistance() && HP >= MaxHp / 2)
				{
					ApplyConditionEffect(new ConditionEffect
						{
							Effect = ConditionEffectIndex.ArmorBreakImmune,
							DurationMS = -1
						});
				}
				else
				{
					ApplyConditionEffect(new ConditionEffect
						{
							Effect = ConditionEffectIndex.ArmorBreakImmune,
							DurationMS = 0
						});
				}
                UpdateCount++;
                Owner.BroadcastPacket(new DamagePacket
                {
                    TargetId = Id,
                    Effects = 0,
                    Damage = (ushort)dmg,
                    Killed = HP <= 0,
                    BulletId = 0,
                    ObjectId = chr.Id
                }, this);
                SaveToCharacter();

                if (HP <= 0)
                    Death(chr.ObjectDesc.DisplayId, chr.ObjectDesc);
            }
            catch (Exception e)
            {
                log.Error("Error while processing playerDamage: ", e);
            }
        }

        protected override void ExportStats(IDictionary<StatsType, object> stats)
        {
            base.ExportStats(stats);
            stats[StatsType.AccountId] = AccountId;
            stats[StatsType.Name] = Name;


            stats[StatsType.Experience] = Experience - GetLevelExp(Level);
            stats[StatsType.ExperienceGoal] = ExperienceGoal;
            stats[StatsType.Level] = Level;

            stats[StatsType.CurrentFame] = CurrentFame;
            stats[StatsType.Fame] = Fame;
            stats[StatsType.FameGoal] = FameGoal;
            stats[StatsType.Stars] = Stars;
            stats[StatsType.StarType] = StarType;

            stats[StatsType.Guild] = Guild[AccountId].Name;
            stats[StatsType.GuildRank] = Guild[AccountId].Rank;

            stats[StatsType.Credits] = Credits;
            stats[StatsType.Tokens] = Tokens;
            stats[StatsType.Onrane] = Onrane;
            stats[StatsType.Kantos] = Kantos;
            stats[StatsType.NameChosen] = NameChosen ? 1 : 0;
            stats[StatsType.Texture1] = Texture1;
            stats[StatsType.Texture2] = Texture2;

            if (Glowing)
                stats[StatsType.Glowing] = 1;

            stats[StatsType.HP] = HP;
            stats[StatsType.MP] = Mp;

            stats[StatsType.Inventory0] = (int)(Inventory[0]?.ObjectType ?? -1);
            stats[StatsType.Inventory1] = (int)(Inventory[1]?.ObjectType ?? -1);
            stats[StatsType.Inventory2] = (int)(Inventory[2]?.ObjectType ?? -1);
            stats[StatsType.Inventory3] = (int)(Inventory[3]?.ObjectType ?? -1);
            stats[StatsType.Inventory4] = (int)(Inventory[4]?.ObjectType ?? -1);
            stats[StatsType.Inventory5] = (int)(Inventory[5]?.ObjectType ?? -1);
            stats[StatsType.Inventory6] = (int)(Inventory[6]?.ObjectType ?? -1);
            stats[StatsType.Inventory7] = (int)(Inventory[7]?.ObjectType ?? -1);
            stats[StatsType.Inventory8] = (int)(Inventory[8]?.ObjectType ?? -1);
            stats[StatsType.Inventory9] = (int)(Inventory[9]?.ObjectType ?? -1);
            stats[StatsType.Inventory10] = (int)(Inventory[10]?.ObjectType ?? -1);
            stats[StatsType.Inventory11] = (int)(Inventory[11]?.ObjectType ?? -1);

            if (Boost == null)
                CalcBoost();

            if (Boost != null)
            {
                //With Modifiers from Legendaries and Marks
                stats[StatsType.MaximumHP] = Stats[0] + Boost[0];
                stats[StatsType.MaximumMP] = Stats[1] + Boost[1];
				stats[StatsType.Attack] = Stats[2] + Boost[2] + AegisDamage() + MindStat2() + Mark5() + MarkU() + MercyStat();
                stats[StatsType.Defense] = Stats[3] + Boost[3] + MindStat2() + SpikeDefense() + SPAbilityThingy2() + Mark1() + MarkU();
				stats[StatsType.Speed] = Stats[4] + Boost[4] + MindStat2() + AnguishNumber() + Mark4() + MarkU() - MercyStat();
                stats[StatsType.Vitality] = Stats[5] + Boost[5] + MindStat2() + VRAbilityThingy() + Mark2() + MarkU();
                stats[StatsType.Wisdom] = Stats[6] + Boost[6] + MindStat2() + Mark3() + MarkU() + Mark8b();
                stats[StatsType.Dexterity] = Stats[7] + Boost[7] + MindStat2() + SPAbilityThingy() + Mark6() + MarkU() + Mark8a();
				stats[StatsType.Might] = Stats[8] + Boost[8];
				stats[StatsType.Luck] = Stats[9] + Boost[9] + DDSDamage();

                stats[StatsType.HPBoost] = Boost[0];
                stats[StatsType.MPBoost] = Boost[1];
                stats[StatsType.AttackBonus] = Boost[2] + AegisDamage() + MindStat2() + Mark5() + MarkU() + MercyStat();
                stats[StatsType.DefenseBonus] = Boost[3] + MindStat2() + SpikeDefense() + SPAbilityThingy2() + Mark1() + MarkU();
                stats[StatsType.SpeedBonus] = Boost[4] + MindStat2() + AnguishNumber() + Mark4() + MarkU() - MercyStat();
                stats[StatsType.VitalityBonus] = Boost[5] + MindStat2() + VRAbilityThingy() + Mark2() + MarkU();
                stats[StatsType.WisdomBonus] = Boost[6] + MindStat2() + Mark3() + MarkU() + Mark8b();
                stats[StatsType.DexterityBonus] = Boost[7] + MindStat2() + SPAbilityThingy() + Mark6() + MarkU() + Mark8a();
				stats[StatsType.MightBonus] = Boost[8];
				stats[StatsType.LuckBonus] = Boost[9] + DDSDamage();
            }

            stats[StatsType.Size] = setTypeSkin?.Size ?? Size;
            stats[StatsType.Has_Backpack] = HasBackpack;
            if (HasBackpack == 1)
            {
                stats[StatsType.Backpack0] = (int)(Inventory[12]?.ObjectType ?? -1);
                stats[StatsType.Backpack1] = (int)(Inventory[13]?.ObjectType ?? -1);
                stats[StatsType.Backpack2] = (int)(Inventory[14]?.ObjectType ?? -1);
                stats[StatsType.Backpack3] = (int)(Inventory[15]?.ObjectType ?? -1);
                stats[StatsType.Backpack4] = (int)(Inventory[16]?.ObjectType ?? -1);
                stats[StatsType.Backpack5] = (int)(Inventory[17]?.ObjectType ?? -1);
                stats[StatsType.Backpack6] = (int)(Inventory[18]?.ObjectType ?? -1);
                stats[StatsType.Backpack7] = (int)(Inventory[19]?.ObjectType ?? -1);
            }

            stats[StatsType.Skin] = setTypeSkin?.SkinType ?? PlayerSkin;
            stats[StatsType.HealStackCount] = HealthPotions;
            stats[StatsType.MagicStackCount] = MagicPotions;

            if (Owner != null && Owner.Name == "Ocean Trench")
                stats[StatsType.OxygenBar] = OxygenBar;

            stats[StatsType.XpBoosterActive] = XpBoosted ? 1 : 0;
            stats[StatsType.XpBoosterTime] = (int)XpBoostTimeLeft;
            stats[StatsType.LootDropBoostTimer] = (int)LootDropBoostTimeLeft;
            stats[StatsType.LootTierBoostTimer] = (int)LootTierBoostTimeLeft;
            stats[StatsType.MarkId] = MarkId;
            stats[StatsType.CookingLevel] = CookingLevel;
        }

        public void CalcBoost()
        {
            CheckSetTypeSkin();
            if (Boost == null)
                Boost = new int[12];
            else
                for (var i = 0; i < Boost.Length; i++)
                    Boost[i] = 0;
            for (var i = 0; i < 4; i++)
            {
                if (Inventory.Length < i || Inventory.Length == 0)
                    return;
                if (Inventory[i] == null)
                    continue;
                foreach (var pair in Inventory[i].StatsBoost)
                {
                    if (pair.Key == StatsType.MaximumHP)
                        Boost[0] += pair.Value;
                    if (pair.Key == StatsType.MaximumMP)
                        Boost[1] += pair.Value;
                    if (pair.Key == StatsType.Attack)
                        Boost[2] += pair.Value;
                    if (pair.Key == StatsType.Defense)
                        Boost[3] += pair.Value;
                    if (pair.Key == StatsType.Speed)
                        Boost[4] += pair.Value;
                    if (pair.Key == StatsType.Vitality)
                        Boost[5] += pair.Value;
                    if (pair.Key == StatsType.Wisdom)
                        Boost[6] += pair.Value;
                    if (pair.Key == StatsType.Dexterity)
                        Boost[7] += pair.Value;
					if (pair.Key == StatsType.Might)
						Boost[8] += pair.Value;
					if (pair.Key == StatsType.Luck)
						Boost[9] += pair.Value;
                }
            }

            if (setTypeBoosts == null)
                return;
            for (var i = 0; i < 10; i++)
                Boost[i] += setTypeBoosts[i];
        }

        public bool CompareName(string name)
        {
            var rn = name.ToLower();
            return rn.Split(' ')[0].StartsWith("[") || Name.Split(' ').Length == 1
                ? Name.ToLower().StartsWith(rn)
                : Name.Split(' ')[1].ToLower().StartsWith(rn);
        }

        public void Death(string killer, ObjectDesc desc = null)
        {
            if (dying)
                return;
            dying = true;
            switch (Owner.Name)
            {
                case "Arena":
                    {
                        Client.SendPacket(new ArenaDeathPacket
                        {
                            RestartPrice = 100
                        });
                        HP = Client.Character.MaxHitPoints;
                        ApplyConditionEffect(new ConditionEffect
                        {
                            Effect = ConditionEffectIndex.Invulnerable,
                            DurationMS = -1
                        });
                        return;
                    }
            }

            if (Client.Stage == ProtocalStage.Disconnected || resurrecting)
                return;
            if (CheckInAdminArena())
                return;
            if (CheckResurrection())
                return;

            if (Client.Character.Dead)
            {
                Client.Disconnect();
                return;
            }
            GenerateGravestone();
            if (desc != null)
                killer = desc.DisplayId;
            switch (killer)
            {
                case "":
                case "Unknown":
                    break;

                default:
                    Owner.BroadcastPacket(new TextPacket
                    {
                        BubbleTime = 0,
                        Stars = -1,
                        Name = "",
                        Text = "{\"key\":\"server.death\",\"tokens\":{\"player\":\"" + Name + "\",\"level\":\"" +
                               Level + "\",\"enemy\":\"" + killer + "\"}}"
                    }, null);
                    break;
            }

            try
            {
                Manager.Database.DoActionAsync(db =>
                {
                    Client.Character.Dead = true;
                    SaveToCharacter();
                    db.SaveCharacter(Client.Account, Client.Character);
                    db.Death(Manager.GameData, Client.Account, Client.Character, killer);
                });
                if (Owner.Id != -6)
                {
                    Client.SendPacket(new DeathPacket
                    {
                        AccountId = AccountId,
                        CharId = Client.Character.CharacterId,
                        Killer = killer,
                        obf0 = -1,
                        obf1 = -1,
                    });
                    Owner.Timers.Add(new WorldTimer(1000, (w, t) => Client.Disconnect()));
                    Owner.LeaveWorld(this);
                }
                else
                    Client.Disconnect();
            }
            catch (Exception e)
            {
                log.Error(e);
            }
        }

        public void GivePet(PetItem petInfo)
        {
            //if (Name == "ossimc82" || Name == "C453")
            //{
            Pet = new Pet(Manager, petInfo, this);
            Pet.Move(X, Y);
            Owner.EnterWorld(Pet);
            //}
        }

        public override bool HitByProjectile(Projectile projectile, RealmTime time)
        {
            if (projectile.ProjectileOwner is Player ||
                HasConditionEffect(ConditionEffectIndex.Paused) ||
                HasConditionEffect(ConditionEffectIndex.Stasis) ||
                HasConditionEffect(ConditionEffectIndex.Invincible))
                return false;

            return base.HitByProjectile(projectile, time);
        }

        public override void Init(World owner)
        {
            WorldInstance = owner;
            var rand = new Random();
            int x, y;
            do
            {
                x = rand.Next(0, owner.Map.Width);
                y = rand.Next(0, owner.Map.Height);
            } while (owner.Map[x, y].Region != TileRegion.Spawn);
            Move(x + 0.5f, y + 0.5f);
            tiles = new byte[owner.Map.Width, owner.Map.Height];
            SetNewbiePeriod();
            base.Init(owner);

            if (Client.Character.Pet != null)
                GivePet(Client.Character.Pet);

            if (owner.Id == World.NEXUS_ID || owner.Name == "Vault")
            {
                Client.SendPacket(new Global_NotificationPacket
                {
                    Type = 0,
                    Text = Client.Account.Gifts.Count > 0 ? "giftChestOccupied" : "giftChestEmpty"
                });
            }

            SendAccountList(Locked, AccountListPacket.LOCKED_LIST_ID);
            SendAccountList(Ignored, AccountListPacket.IGNORED_LIST_ID);

            WorldTimer[] accTimer = { null };
            owner.Timers.Add(accTimer[0] = new WorldTimer(5000, (w, t) =>
            {
                Manager.Database.DoActionAsync(db =>
                {
                    if (Client?.Account == null)
                        return;
                    Client.Account = db.GetAccount(AccountId, Manager.GameData);
                    Credits = Client.Account.Credits;
                    CurrentFame = Client.Account.Stats.Fame;
                    Tokens = Client.Account.FortuneTokens;
                    Onrane = Client.Account.Onrane;
                    Kantos = Client.Account.Kantos;
                    accTimer[0].Reset();
                    Manager.Logic.AddPendingAction(_ => w.Timers.Add(accTimer[0]), PendingPriority.Creation);
                });
            }));

            WorldTimer[] pingTimer = { null };
            owner.Timers.Add(pingTimer[0] = new WorldTimer(PING_PERIOD, (w, t) =>
            {
                Client.SendPacket(new PingPacket { Serial = pingSerial++ });
                pingTimer[0].Reset();
                Manager.Logic.AddPendingAction(_ => w.Timers.Add(pingTimer[0]), PendingPriority.Creation);
            }));
            Manager.Database.DoActionAsync(db =>
            {
                db.UpdateLastSeen(Client.Account.AccountId, Client.Character.CharacterId, owner.Name);
                db.LockAccount(Client.Account);
            });

            if (Client.Account.IsGuestAccount)
            {
                owner.Timers.Add(new WorldTimer(1000, (w, t) => Client.Disconnect()));
                Client.SendPacket(new networking.svrPackets.FailurePacket
                {
                    ErrorId = 8,
                    ErrorDescription = "Registration needed."
                });
                Client.SendPacket(new PasswordPromtPacket
                {
                    CleanPasswordStatus = PasswordPromtPacket.REGISTER
                });
                return;
            }

            if (!Client.Account.VerifiedEmail && Program.Verify)
            {
                Client.SendPacket(new VerifyEmailDialogPacket());
                owner.Timers.Add(new WorldTimer(1000, (w, t) => Client.Disconnect()));
                return;
            }
            CheckSetTypeSkin();
        }

        public void SaveToCharacter()
        {
            var chr = Client.Character;
            chr.Exp = Experience;
            chr.Level = Level;
            chr.Tex1 = Texture1;
            chr.Tex2 = Texture2;
            chr.Pet = Pet?.Info;
            chr.CurrentFame = Fame;
            chr.HitPoints = HP;
            chr.MagicPoints = Mp;
            switch (Inventory.Length)
            {
                case 12:
                    chr.Equipment = Inventory.Select(_ => _?.ObjectType ?? -1).ToArray();
                    break;

                case 20:
                    var equip = Inventory.Select(_ => _?.ObjectType ?? -1).ToArray();
                    var backpack = new int[8];
                    Array.Copy(equip, 12, backpack, 0, 8);
                    Array.Resize(ref equip, 12);
                    chr.Equipment = equip;
                    chr.Backpack = backpack;
                    break;
            }
            chr.MaxHitPoints = Stats[0];
            chr.MaxMagicPoints = Stats[1];
            chr.Attack = Stats[2];
            chr.Defense = Stats[3];
            chr.Speed = Stats[4];
            chr.HpRegen = Stats[5];
            chr.MpRegen = Stats[6];
            chr.Dexterity = Stats[7];
			chr.Might = Stats[8];
			chr.Luck = Stats[9];
            chr.HealthStackCount = HealthPotions;
            chr.MagicStackCount = MagicPotions;
            chr.HasBackpack = HasBackpack;
            chr.Skin = PlayerSkin;
            chr.XpBoosted = XpBoosted;
            chr.MarkId = MarkId;
            chr.XpTimer = (int)XpBoostTimeLeft;
            chr.LDTimer = (int)LootDropBoostTimeLeft;
            chr.LTTimer = (int)LootTierBoostTimeLeft;
        }

        public void Teleport(RealmTime time, TeleportPacket packet)
        {
            var obj = Client.Player.Owner.GetEntity(packet.ObjectId);
            try
            {
                if (obj == null)
                    return;
                if (!TPCooledDown())
                {
                    SendError("Player.teleportCoolDown");
                    return;
                }
                if (obj.HasConditionEffect(ConditionEffectIndex.Invisible))
                {
                    SendError("server.no_teleport_to_invisible");
                    return;
                }
                if (obj.HasConditionEffect(ConditionEffectIndex.Paused))
                {
                    SendError("server.no_teleport_to_paused");
                    return;
                }
                var player = obj as Player;
                if (player != null && !player.NameChosen)
                {
                    SendError("server.teleport_needs_name");
                    return;
                }
                if (obj.Id == Id)
                {
                    SendError("server.teleport_to_self");
                    return;
                }
                if (!Owner.AllowTeleport)
                {
                    SendError(GetLanguageString("server.no_teleport_in_realm",
                        new KeyValuePair<string, object>("realm", Owner.Name)));
                    return;
                }

                SetTPDisabledPeriod();
                Move(obj.X, obj.Y);
                Pet?.Move(obj.X, obj.X);
                FameCounter.Teleport();
                SetNewbiePeriod();
                UpdateCount++;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                SendError("player.cannotTeleportTo");
                return;
            }
            Owner.BroadcastPacket(new GotoPacket
            {
                ObjectId = Id,
                Position = new Position
                {
                    X = X,
                    Y = Y
                }
            }, null);
            Owner.BroadcastPacket(new ShowEffectPacket
            {
                EffectType = EffectType.Teleport,
                TargetId = Id,
                PosA = new Position
                {
                    X = X,
                    Y = Y
                },
                Color = new ARGB(0xFFFFFFFF)
            }, null);
        }

        public override void Tick(RealmTime time)
        {
            try
            {
                if (Manager.Clients.Count(_ => _.Value.Id == Client.Id) == 0)
                {
                    if (Owner != null)
                        Owner.LeaveWorld(this);
                    else
                        WorldInstance.LeaveWorld(this);
                    Manager.Database.DoActionAsync(db => db.UnlockAccount(Client.Account));
                    return;
                }
                if (Client.Stage == ProtocalStage.Disconnected || (!Client.Account.VerifiedEmail && Program.Verify))
                {
                    if (Owner != null)
                        Owner.LeaveWorld(this);
                    else
                        WorldInstance.LeaveWorld(this);
                    Manager.Database.DoActionAsync(db => db.UnlockAccount(Client.Account));
                    return;
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            if (Stats != null && Boost != null)
            {
                MaxHp = Stats[0] + Boost[0];
                MaxMp = Stats[1] + Boost[1];
            }

            if (!KeepAlive(time))
                return;

            if (Boost == null)
                CalcBoost();

            TradeHandler?.Tick(time);
            HandleRegen(time);
            HandleQuest(time);
            HandleEffects(time);
            HandleGround(time);
            HandleBoosts();
            CheckLevelUp();
            FameCounter.Tick(time);

            //if(pingSerial > 5)
            //    if (!Enumerable.Range(UpdatesSend, 5000).Contains(UpdatesReceived))
            //        Client.Disconnect();

            if (Mp < 0)
                Mp = 0;

            /* try
                * {
                *     psr.Database.SaveCharacter(psr.Account, psr.Character);
                *     UpdateCount++;
                * }
                * catch (ex)
                * {
                * }
            */

            try
            {
                if (Owner != null)
                {
                    SendUpdate(time);
                    if (!Owner.IsPassable((int)X, (int)Y) && Client.Account.Rank < 2)
                    {
                        log.Fatal($"Player {Name} No-Cliped at position: {X}, {Y}");
                        Client.Disconnect();
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            try
            {
                SendNewTick(time);
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            if (HP < 0 && !dying)
            {
                Death("Unknown");
                return;
            }

            base.Tick(time);
        }

        private bool CheckInAdminArena()
        {
            if (Owner.Name == "Admins Arena")
            {
                Owner.BroadcastPacket(new TextPacket
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "",
                    Text = $"Sidon spares his mercy to {Name}"
                }, null);
                Client.Reconnect(new ReconnectPacket
                {
                    Host = "",
                    Port = Program.Settings.GetValue<int>("port"),
                    GameId = World.NEXUS_ID,
                    Name = "Nexus",
                    Key = Empty<byte>.Array,
                });

                resurrecting = true;
                return true;
            }
            return false;
        }

        private bool CheckResurrection()
        {
            for (var i = 0; i < 4; i++)
            {
                var item = Inventory[i];
                if (item == null || !item.Resurrects)
                    continue;

                HP = Stats[0] + Stats[0];
                Mp = Stats[1] + Stats[1];
                Inventory[i] = null;
                Owner.BroadcastPacket(new TextPacket
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "",
                    Text = $"{Name}'s {item.ObjectId} breaks and he disappears"
                }, null);
                Client.Reconnect(new ReconnectPacket
                {
                    Host = "",
                    Port = Program.Settings.GetValue<int>("port"),
                    GameId = World.NEXUS_ID,
                    Name = "Nexus",
                    Key = Empty<byte>.Array,
                });

                resurrecting = true;
                return true;
            }
            return false;
        }

        //Aegis Damage
        public int AegisDamage()
        {
            if (CheckDResistance() && HP <= MaxHp / 2)
            {
                return 40;
            }
            else
            {
                return 0;
            }
        }
        public int DDSDamage()
        {
            if (HasConditionEffect(ConditionEffectIndex.Berserk))
            {
                if (CheckDDSResistance())
                {
                    return 1000 - Boost[9] - Stats[9];
                }
                else
                {
                    return 0;
                }
            }else
            {
                return 0;
            }
        }
                       

//VR Ability
public int VRAbilityThingy()
        {
            if (CheckVRAbility() && HP >= MaxHp / 2)
            {
                return 80;
            }
            else
            {
                return 0;
            }
        }

        //Mark Additions
        public int Mark1()
        {
            if (MarkId == 1)
            {
                int a = Convert.ToInt32(Stats[3] * 0.20);
                return a;
            }
            else
            {
                return 0;
            }
        }

        public int Mark2()
        {
            if (MarkId == 2)
            {
                int a = Convert.ToInt32(Stats[5] * 0.20);
                return a;
            }
            else
            {
                return 0;
            }
        }

        public int Mark3()
        {
            if (MarkId == 3)
            {
                int a = Convert.ToInt32(Stats[6] * 0.20);
                return a;
            }
            else
            {
                return 0;
            }
        }

        public int Mark4()
        {
            if (MarkId == 4)
            {
                int a = Convert.ToInt32(Stats[4] * 0.20);
                return a;
            }
            else
            {
                return 0;
            }
        }

        public int Mark5()
        {
            if (MarkId == 5)
            {
                int a = Convert.ToInt32(Stats[2] * 0.20);
                return a;
            }
            else
            {
                return 0;
            }
        }

        public int Mark6()
        {
            if (MarkId == 6)
            {
                int a = Convert.ToInt32(Stats[7] * 0.20);
                return a;
            }
            else
            {
                return 0;
            }
        }

        public int MarkU()
        {
            if (MarkId == 7)
            {
                int a = 1;
                return a;
            }
            else
            {
                return 0;
            }
        }

        public int Mark8a()
        {
            if (MarkId == 8)
            {
                int a = Convert.ToInt32(Stats[7] * 0.25);
                return a;
            }
            else
            {
                return 0;
            }
        }

        public int Mark8b()
        {
            if (MarkId == 8)
            {
                int a = Convert.ToInt32(Stats[6] * 0.25);
                return a;
            }
            else
            {
                return 0;
            }
        }

        //SP Ability
        public int SPAbilityThingy()
        {
            if (CheckSPAbility() && HP <= MaxHp / 2)
            {
                return StatsType.Dexterity + 10;
            }
            else
            {
                return 0;
            }
        }

        public int SPAbilityThingy2()
        {
            if (CheckSPAbility() && HP <= MaxHp / 2)
            {
                return -22;
            }
            else
            {
                return 0;
            }
        }

        //Spike Defense
        public int SpikeDefense()
        {
            if (CheckPResistance())
            {
                int sp;
                sp = 0;

                Enemy[] targets = this.GetNearestEntitieIsGroup(2, "AHLogic").OfType<Enemy>().ToArray();
                foreach (Enemy e in targets)
                {
                    sp += 1;
                }
                return sp;
            }
            else
            {
                return 0;
            }
        }

        public float AnguishBleeding(RealmTime t)
        {
            if (CheckAResistance())
            {
                return 16 * (t.thisTickTimes / 1000f);
            }
            else
            {
                return 0;
            }
        }

        public int AnguishNumber()
        {
            if (CheckAResistance() && HP <= MaxHp / 3)
            {
                return 14;
            }
            else
            {
                return 0;
            }
        }

        //Mind over Matter item integers
        public int MindStat2()
        {
            if (CheckMResistance() && Mp == MaxMp)
            {
                return 6;
            }
            else
            {
                return 0;
            }
        }

		//Mind over Matter item integers
		public int MercyStat()
		{
			if (CheckMMResistance() && Mp <= MaxMp/4)
			{
				return 40;
			}
			else
			{
				return 0;
			}
		}

        private bool CheckResistance()
        {
            for (var i = 0; i < 4; i++)
            {
                var item = Inventory[i];
                if (item == null || !item.Resistance)
                    continue;
                return true;
            }
            return false;
        }

        private bool CheckSPAbility()
        {
            for (var i = 0; i < 4; i++)
            {
                var item = Inventory[i];
                if (item == null || !item.SPAbility)
                    continue;
                return true;
            }
            return false;
        }

        private bool CheckVRAbility()
        {
            for (var i = 0; i < 4; i++)
            {
                var item = Inventory[i];
                if (item == null || !item.VRAbility)
                    continue;
                return true;
            }
            return false;
        }

        private bool CheckCResistance()
        {
            for (var i = 0; i < 4; i++)
            {
                var item = Inventory[i];
                if (item == null || !item.CResistance)
                    continue;
                return true;
            }
            return false;
        }

		private bool CheckGResistance()
		{
			for (var i = 0; i < 4; i++)
			{
				var item = Inventory[i];
				if (item == null || !item.GResistance)
					continue;
				return true;
			}
			return false;
		}
        public bool SorForgerGold()
        {
            using (var db = new Database())
            {
                for (int d = 0; d < Inventory.Length; d++)
                {
                    if (Inventory[d] == null)
                        continue;
                    if (Inventory[d].ObjectId == "Sor Forger Battery")
                    {
                        for (int i = 0; i < Inventory.Length; i++)
                        {
                            if (Inventory[i] == null)
                                continue;

                            if (Inventory[i].ObjectId == "Sor Crystal")
                            {
                                Inventory[i] = Manager.GameData.Items[0x49e6];
                                Inventory[d] = null;
                                UpdateCount++;
                                SaveToCharacter();
                                Credits = Client.Account.Credits = db.UpdateCredit(Client.Account, -500);
                                SendInfo("You have successfully forged your Sor Crystal into a Legendary Sor Crystal!");
                                return false;
                            }
                        }
                        ;
                    }
                }
            }
            return true;
        }

        private int CLevel(int clevel)
        {
            Random rnd = new Random();
            int levelrate = rnd.Next(1, clevel);
            return levelrate;
        }

        public void DoesLevel()
        {
            using (var db = new Database())
            {
                if (CookingLevel <= 25)
                {
                    if (CLevel(50) == 1)
                    {
                        CookingLevel = db.UpdateCookingLevel(Client.Account, 1);
                        UpdateCount++;
                        SendHelp("You have leveled up cooking!");
                    }
                }
                else
                {
                    if (CLevel(100) == 1)
                    {
                        CookingLevel = db.UpdateCookingLevel(Client.Account, 1);
                        UpdateCount++;
                        SendHelp("You have leveled up cooking!");
                    }
                }
            }
        }

        public bool SorForgerOnrane()
        {
            using (var db = new Database())
            {
                for (int i = 0; i < Inventory.Length; i++)
                {
                    if (Inventory[i] == null)
                        continue;
                    if (Inventory[i].ObjectId == "Sor Crystal")
                    {
                        Inventory[i] = Manager.GameData.Items[0x49e6];
                        UpdateCount++;
                        SaveToCharacter();
                        Onrane = Client.Account.Onrane = db.UpdateOnraneCurrency(Client.Account, -10);
                        SendInfo("You have successfully forged your Sor Crystal into a Legendary Sor Crystal!");
                        return false;
                    }
                }
            }
            ;
            return true;
        }

        private bool CheckBWAbility()
        {
            for (var i = 0; i < 4; i++)
            {
                var item = Inventory[i];
                if (item == null || !item.BWAbility)
                    continue;
                return true;
            }
            return false;
        }

        private bool CheckPResistance()
        {
            for (var i = 0; i < 4; i++)
            {
                var item = Inventory[i];
                if (item == null || !item.PResistance)
                    continue;
                return true;
            }
            return false;
        }

        public bool CheckDResistance()
        {
            for (var i = 0; i < 4; i++)
            {
                var item = Inventory[i];
                if (item == null || !item.DResistance)
                    continue;
                return true;
            }
            return false;
        }

        public bool CheckMResistance()
        {
            for (var i = 0; i < 4; i++)
            {
                var item = Inventory[i];
                if (item == null || !item.MResistance)
                    continue;
                return true;
            }
            return false;
        }

		public bool CheckMMResistance()
		{
			for (var i = 0; i < 4; i++)
			{
				var item = Inventory[i];
				if (item == null || !item.MMResistance)
					continue;
				return true;
			}
			return false;
		}

        public bool CheckAResistance()
        {
            for (var i = 0; i < 4; i++)
            {
                var item = Inventory[i];
                if (item == null || !item.AResistance)
                    continue;
                return true;
            }
            return false;
        }

        public bool CheckDDSResistance()
        {
            for (var i = 0; i < 4; i++)
            {
                var item = Inventory[i];
                if (item == null || !item.DDSResistance)
                    continue;
                return true;
            }
            return false;
        }
        private void GenerateGravestone()
        {
            var maxed = (from i in Manager.GameData.ObjectTypeToElement[ObjectType].Elements("LevelIncrease")
                         let xElement = Manager.GameData.ObjectTypeToElement[ObjectType].Element(i.Value)
                         where xElement != null
                         let limit = int.Parse(xElement.Attribute("max").Value)
                         let idx = StatsManager.StatsNameToIndex(i.Value)
                         where Stats[idx] >= limit
                         select limit).Count();

            ushort objType;
            int? time;
            switch (maxed)
            {
                case 10:
                    objType = 0x585d;
                    time = null;
                    break;

                case 9:
                    objType = 0x585c;
                    time = null;
                    break;

                case 8:
                    objType = 0x0735;
                    time = null;
                    break;

                case 7:
                    objType = 0x0734;
                    time = null;
                    break;

                case 6:
                    objType = 0x072b;
                    time = null;
                    break;

                case 5:
                    objType = 0x072a;
                    time = null;
                    break;

                case 4:
                    objType = 0x0729;
                    time = null;
                    break;

                case 3:
                    objType = 0x0728;
                    time = null;
                    break;

                case 2:
                    objType = 0x0727;
                    time = null;
                    break;

                case 1:
                    objType = 0x0726;
                    time = null;
                    break;

                default:
                    if (Level <= 1)
                    {
                        objType = 0x0723;
                        time = 30 * 1000;
                    }
                    else if (Level < 20)
                    {
                        objType = 0x0724;
                        time = 60 * 1000;
                    }
                    else
                    {
                        objType = 0x0725;
                        time = 5 * 60 * 1000;
                    }
                    break;
            }
            var obj = new StaticObject(Manager, objType, time, true, time != null, false);
            obj.Move(X, Y);
            obj.Name = Name;
            Owner.EnterWorld(obj);
        }

        private void HandleRegen(RealmTime time)
        {
            if (HP == Stats[0] + Boost[0] || !CanHpRegen())
                hpRegenCounter = 0;
            else
            {
                hpRegenCounter += StatsManager.GetHPRegen() * time.thisTickTimes / 1000f;
                var regen = (int)hpRegenCounter;
                if (regen > 0)
                {
                    HP = Math.Min(Stats[0] + Boost[0], HP + regen);
                    hpRegenCounter -= regen;
                    UpdateCount++;
                }
            }

            if (Mp == Stats[1] + Boost[1] || !CanMpRegen())
                mpRegenCounter = 0;
            else
            {
                mpRegenCounter += StatsManager.GetMPRegen() * time.thisTickTimes / 1000f;
                var regen = (int)mpRegenCounter;
                if (regen <= 0)
                    return;
                Mp = Math.Min(Stats[1] + Boost[1], Mp + regen);
                mpRegenCounter -= regen;
                UpdateCount++;
            }
        }

        public new void Dispose()
        {
            tiles = null;
            Guild.Remove(this);
        }
    }
}