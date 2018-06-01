#region

using db;
using db.data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using wServer.networking;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;

#endregion

namespace wServer.realm.entities.player
{
    partial class Player
    {
        private static readonly ConditionEffect[] NegativeEffs =
        {
            new ConditionEffect
            {
                Effect = ConditionEffectIndex.Slowed,
                DurationMS = 0
            },
            new ConditionEffect
            {
                Effect = ConditionEffectIndex.Paralyzed,
                DurationMS = 0
            },
            new ConditionEffect
            {
                Effect = ConditionEffectIndex.Weak,
                DurationMS = 0
            },
            new ConditionEffect
            {
                Effect = ConditionEffectIndex.Stunned,
                DurationMS = 0
            },
            new ConditionEffect
            {
                Effect = ConditionEffectIndex.Confused,
                DurationMS = 0
            },
            new ConditionEffect
            {
                Effect = ConditionEffectIndex.Blind,
                DurationMS = 0
            },
            new ConditionEffect
            {
                Effect = ConditionEffectIndex.Quiet,
                DurationMS = 0
            },
            new ConditionEffect
            {
                Effect = ConditionEffectIndex.ArmorBroken,
                DurationMS = 0
            },
            new ConditionEffect
            {
                Effect = ConditionEffectIndex.Bleeding,
                DurationMS = 0
            },
            new ConditionEffect
            {
                Effect = ConditionEffectIndex.Dazed,
                DurationMS = 0
            },
            new ConditionEffect
            {
                Effect = ConditionEffectIndex.Sick,
                DurationMS = 0
            },
            new ConditionEffect
            {
                Effect = ConditionEffectIndex.Drunk,
                DurationMS = 0
            },
            new ConditionEffect
            {
                Effect = ConditionEffectIndex.Hallucinating,
                DurationMS = 0
            },
            new ConditionEffect
            {
                Effect = ConditionEffectIndex.Hexed,
                DurationMS = 0
            },
            new ConditionEffect
            {
                Effect = ConditionEffectIndex.Unstable,
                DurationMS = 0
            }
        };

        public static int oldstat { get; set; }

        public static Position targetlink { get; set; }

        public static void ActivateHealHp(Player player, int amount, List<Packet> pkts)
        {
            int maxHp = player.Stats[0] + player.Boost[0];
            int newHp = Math.Min(maxHp, player.HP + amount);
            if (newHp != player.HP)
            {
                pkts.Add(new ShowEffectPacket
                {
                    EffectType = EffectType.Potion,
                    TargetId = player.Id,
                    Color = new ARGB(0xffffffff)
                });
                pkts.Add(new NotificationPacket
                {
                    Color = new ARGB(0xff00ff00),
                    ObjectId = player.Id,
                    Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"+" + (newHp - player.HP) + "\"}}"
                    //"+" + (newHp - player.HP)
                });
                player.HP = newHp;
                player.UpdateCount++;
            }
        }

        public int ManaLeftToDamage()
        {
            return MaxMp - Mp;
        }

        private static void ActivateHealMp(Player player, int amount, List<Packet> pkts)
        {
            if (player.fast == false)
            {
                int maxMp = player.Stats[1] + player.Boost[1];
                int newMp = Math.Min(maxMp, player.Mp + amount);
                if (newMp != player.Mp)
                {
                    pkts.Add(new ShowEffectPacket
                    {
                        EffectType = EffectType.Potion,
                        TargetId = player.Id,
                        Color = new ARGB(0x6084e0)
                    });
                    pkts.Add(new NotificationPacket
                    {
                        Color = new ARGB(0x6084e0),
                        ObjectId = player.Id,
                        Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"+" + (newMp - player.Mp) + "\"}}"
                    });
                    player.Mp = newMp;
                    player.UpdateCount++;
                }
            }
        }

        private static void ActivateHealMp2(Player player, int amount, List<Packet> pkts)
        {
            int maxMp = player.Stats[1] + player.Boost[1];
            int newMp = Math.Min(maxMp, player.Mp + amount);
            if (newMp != player.Mp)
            {
                pkts.Add(new ShowEffectPacket
                {
                    EffectType = EffectType.Potion,
                    TargetId = player.Id,
                    Color = new ARGB(0x00FFFF)
                });
                player.Mp = newMp;
                player.UpdateCount++;
            }
        }

        private static void ActivateBoostStat(Player player, int idxnew, List<Packet> pkts)
        {
            var OriginalStat = 0;
            OriginalStat = player.Stats[idxnew] + OriginalStat;
            oldstat = OriginalStat;
        }

        private void ActivateShoot(RealmTime time, Item item, Position target)
        {
            double arcGap = item.ArcGap * Math.PI / 180;
            double startAngle = Math.Atan2(target.Y - Y, target.X - X) - (item.NumProjectiles - 1) / 2 * arcGap;
            ProjectileDesc prjDesc = item.Projectiles[0]; //Assume only one

            for (int i = 0; i < item.NumProjectiles; i++)
            {
                Projectile proj = CreateProjectile(prjDesc, item.ObjectType,
                    (int)StatsManager.GetAttackDamage(prjDesc.MinDamage, prjDesc.MaxDamage),
                    time.tickTimes, new Position { X = X, Y = Y }, (float)(startAngle + arcGap * i));
                Owner.EnterWorld(proj);
                FameCounter.Shoot(proj);
            }
        }

        private void PoisonEnemy(Enemy enemy, ActivateEffect eff)
        {
            try
            {
                if (eff.ConditionEffect != null)
                    enemy.ApplyConditionEffect(new[]
                    {
                        new ConditionEffect
                        {
                            Effect = (ConditionEffectIndex) eff.ConditionEffect,
                            DurationMS = (int) eff.EffectDuration
                        }
                    });
                int remainingDmg =
                    (int)StatsManager.GetDefenseDamage(enemy, eff.TotalDamage, enemy.ObjectDesc.Defense);
                int perDmg = remainingDmg * 1000 / eff.DurationMS;
                WorldTimer tmr = null;
                int x = 0;
                tmr = new WorldTimer(100, (w, t) =>
                {
                    if (enemy.Owner == null)
                        return;
                    w.BroadcastPacket(new ShowEffectPacket
                    {
                        EffectType = EffectType.Dead,
                        TargetId = enemy.Id,
                        Color = new ARGB(0xffddff00)
                    }, null);

                    if (x % 10 == 0)
                    {
                        int thisDmg;
                        if (remainingDmg < perDmg)
                            thisDmg = remainingDmg;
                        else
                            thisDmg = perDmg;

                        enemy.Damage(this, t, thisDmg, true);
                        remainingDmg -= thisDmg;
                        if (remainingDmg <= 0)
                            return;
                    }
                    x++;

                    tmr.Reset();

                    Manager.Logic.AddPendingAction(_ => w.Timers.Add(tmr), PendingPriority.Creation);
                });
                Owner.Timers.Add(tmr);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public bool Activate(RealmTime time, Item item, UseItemPacket pkt)
        {
            bool endMethod = false;
            Position target = pkt.ItemUsePos;
            Mp -= item.MpCost;
            if (HP < item.HpCost)
                HP = item.HpCost;
            else
                HP -= item.HpCost;
            IContainer con = Owner.GetEntity(pkt.SlotObject.ObjectId) as IContainer;
            if (con == null)
                return true;

            if (pkt.SlotObject.SlotId != 255 && pkt.SlotObject.SlotId != 254 &&
                con.Inventory[pkt.SlotObject.SlotId] != item)
            {
                log.FatalFormat("Cheat engine detected for player {0},\nItem should be {1}, but its {2}.",
                    Name, Inventory[pkt.SlotObject.SlotId].ObjectId, item.ObjectId);
                foreach (Player player in Owner.Players.Values)
                    if (player.Client.Account.Rank >= 2)
                        player.SendInfo(String.Format(
                            "Cheat engine detected for player {0},\nItem should be {1}, but its {2}.",
                            Name, Inventory[pkt.SlotObject.SlotId].ObjectId, item.ObjectId));
                Client.Disconnect();
                return true;
            }

            if (item.IsBackpack)
            {
                if (HasBackpack != 0)
                    return true;
                Client.Character.Backpack = new[] { -1, -1, -1, -1, -1, -1, -1, -1 };
                HasBackpack = 1;
                Client.Character.HasBackpack = 1;
                Manager.Database.DoActionAsync(db =>
                    db.SaveBackpacks(Client.Character, Client.Account));
                Array.Resize(ref inventory, 20);
                int[] slotTypes =
                    Utils.FromCommaSepString32(
                        Manager.GameData.ObjectTypeToElement[ObjectType].Element("SlotTypes").Value);
                Array.Resize(ref slotTypes, 20);
                for (int i = 0; i < slotTypes.Length; i++)
                    if (slotTypes[i] == 0)
                        slotTypes[i] = 10;
                SlotTypes = slotTypes;
                return false;
            }
            if (item.XpBooster)
            {
                if (!XpBoosted)
                {
                    XpBoostTimeLeft = (float)item.Timer;
                    XpBoosted = item.XpBooster;
                    xpFreeTimer = (float)item.Timer == -1.0 ? false : true;
                    return false;
                }
                else
                {
                    SendInfo("You have already an active XP Booster.");
                    return true;
                }
            }

            if (item.LootDropBooster)
            {
                if (!LootDropBoost)
                {
                    LootDropBoostTimeLeft = (float)item.Timer;
                    lootDropBoostFreeTimer = (float)item.Timer == -1.0 ? false : true;
                    return false;
                }
                else
                {
                    SendInfo("You have already an active Loot Drop Booster.");
                    return true;
                }
            }

            if (item.LootTierBooster)
            {
                if (!LootTierBoost)
                {
                    LootTierBoostTimeLeft = (float)item.Timer;
                    lootTierBoostFreeTimer = (float)item.Timer == -1.0 ? false : true;
                    return false;
                }
                else
                {
                    SendInfo("You have already an active Loot Tier Booster.");
                    return true;
                }
            }

            foreach (ActivateEffect eff in item.ActivateEffects)
            {
                switch (eff.Effect)
                {
                    case ActivateEffects.BulletNova:
                        {
                            ProjectileDesc prjDesc = item.Projectiles[0]; //Assume only one
                            Packet[] batch = new Packet[21];
                            uint s = Random.CurrentSeed;
                            Random.CurrentSeed = (uint)(s * time.tickTimes);
                            for (int i = 0; i < 20; i++)
                            {
                                Projectile proj = CreateProjectile(prjDesc, item.ObjectType,
                                    (int)StatsManager.GetAttackDamage(prjDesc.MinDamage, prjDesc.MaxDamage),
                                    time.tickTimes, target, (float)(i * (Math.PI * 2) / 20));
                                Owner.EnterWorld(proj);
                                FameCounter.Shoot(proj);
                                batch[i] = new Shoot2Packet()
                                {
                                    BulletId = proj.ProjectileId,
                                    OwnerId = Id,
                                    ContainerType = item.ObjectType,
                                    StartingPos = target,
                                    Angle = proj.Angle,
                                    Damage = (short)proj.Damage
                                };
                            }
                            Random.CurrentSeed = s;
                            batch[20] = new ShowEffectPacket()
                            {
                                EffectType = EffectType.Trail,
                                PosA = target,
                                TargetId = Id,
                                Color = new ARGB(0xFFFF00AA)
                            };
                            BroadcastSync(batch, p => this.Dist(p) < 35);
                        }
                        break;

                    case ActivateEffects.Shoot:
                        {
                            ActivateShoot(time, item, target);
                        }
                        break;

                    case ActivateEffects.StatBoostSelf:
                        {
                            int idx = -1;

                            if (eff.Stats == StatsType.MaximumHP)
                                idx = 0;
                            else if (eff.Stats == StatsType.MaximumMP)
                                idx = 1;
                            else if (eff.Stats == StatsType.Attack)
                                idx = 2;
                            else if (eff.Stats == StatsType.Defense)
                                idx = 3;
                            else if (eff.Stats == StatsType.Speed)
                                idx = 4;
                            else if (eff.Stats == StatsType.Vitality)
                                idx = 5;
                            else if (eff.Stats == StatsType.Wisdom)
                                idx = 6;
                            else if (eff.Stats == StatsType.Dexterity)
                                idx = 7;
							else if (eff.Stats == StatsType.Might)
								idx = 8;
							else if (eff.Stats == StatsType.Luck)
								idx = 9;

                            List<Packet> pkts = new List<Packet>();

                            ActivateBoostStat(this, idx, pkts);
                            int OGstat = oldstat;
                            int bit = idx + 39;

                            int s = eff.Amount;
                            Boost[idx] += s;
                            ApplyConditionEffect(new ConditionEffect
                            {
                                DurationMS = eff.DurationMS,
                                Effect = (ConditionEffectIndex)bit
                            });
                            UpdateCount++;
                            Owner.Timers.Add(new WorldTimer(eff.DurationMS, (world, t) =>
                            {
                                Boost[idx] = OGstat;
                                UpdateCount++;
                            }));
                            Owner.BroadcastPacket(new ShowEffectPacket
                            {
                                EffectType = EffectType.Potion,
                                TargetId = Id,
                                Color = new ARGB(0xffffffff)
                            }, null);
                        }
                        break;

                    case ActivateEffects.StatBoostAura:
                        {
                            int idx = -1;

                            if (eff.Stats == StatsType.MaximumHP)
                                idx = 0;
                            if (eff.Stats == StatsType.MaximumMP)
                                idx = 1;
                            if (eff.Stats == StatsType.Attack)
                                idx = 2;
                            if (eff.Stats == StatsType.Defense)
                                idx = 3;
                            if (eff.Stats == StatsType.Speed)
                                idx = 4;
                            if (eff.Stats == StatsType.Vitality)
                                idx = 5;
                            if (eff.Stats == StatsType.Wisdom)
                                idx = 6;
                            if (eff.Stats == StatsType.Dexterity)
                                idx = 7;
							if (eff.Stats == StatsType.Might)
								idx = 8;
							if (eff.Stats == StatsType.Luck)
								idx = 9;

                            int bit = idx + 39;

                            var amountSBA = eff.Amount;
                            var durationSBA = eff.DurationMS;
                            var rangeSBA = eff.Range;
                            if (eff.UseWisMod)
                            {
                                amountSBA = (int)UseWisMod(eff.Amount, 0);
                                durationSBA = (int)(UseWisMod(eff.DurationSec) * 1000);
                                rangeSBA = UseWisMod(eff.Range);
                            }

                            if (HasConditionEffect(ConditionEffectIndex.HPBoost))
                            {
                                if (amountSBA >= 0)
                                {
                                    amountSBA = 0;
                                }
                            }

                            var q = Boost[idx]; //Sets 'q' to Stat just prior to StatBoostAura being used.

                            this.Aoe(rangeSBA, true, player =>
                            {
                                ApplyConditionEffect(new ConditionEffect
                                {
                                    DurationMS = durationSBA,
                                    Effect = (ConditionEffectIndex)bit
                                });
                                (player as Player).Boost[idx] += amountSBA;
                                player.UpdateCount++;
                                Owner.Timers.Add(new WorldTimer(durationSBA,
                                    (world, t)
                                        => //With durationSBA set to 0, time should not overlap. Auras should no longer stack.
                                    {
                                        if (Boost[idx] > q)
                                        {
                                            (player as Player).Boost[idx] -= amountSBA;
                                            player.UpdateCount++;
                                        }
                                        else
                                        {
                                        }
                                    }));
                            });
                            BroadcastSync(new ShowEffectPacket()
                            {
                                EffectType = EffectType.AreaBlast,
                                TargetId = Id,
                                Color = new ARGB(0xffffffff),
                                PosA = new Position() { X = rangeSBA }
                            }, p => this.Dist(p) < 25);
                        }

                        /*
						 * current health -(Health after boost - health before boost)
						 *
						 */

                        break;

                    case ActivateEffects.ConditionEffectSelf:
                        {
                            var durationCES = eff.DurationMS;
                            if (eff.UseWisMod)
                                durationCES = (int)(UseWisMod(eff.DurationSec) * 1000);

                            var color = 0xffffffff;
                            switch (eff.ConditionEffect.Value)
                            {
                                case ConditionEffectIndex.Damaging:
                                    color = 0xffff0000;
                                    break;

                                case ConditionEffectIndex.Berserk:
                                    color = 0x808080;
                                    break;
                            }

                            ApplyConditionEffect(new ConditionEffect
                            {
                                Effect = eff.ConditionEffect.Value,
                                DurationMS = durationCES
                            });
                            Owner.BroadcastPacket(new ShowEffectPacket
                            {
                                EffectType = EffectType.AreaBlast,
                                TargetId = Id,
                                Color = new ARGB(color),
                                PosA = new Position { X = 2F }
                            }, null);
                        }
                        break;

                    case ActivateEffects.LootboxActivate:
                        {
                            Random rnd = new Random();

                            using (var db = new Database())
                            {
                                int a = eff.Amount;
                                string LootboxName = eff.LootboxName;
                                var acc = Client.Account;
                                if (acc.Onrane >= a)
                                {
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == LootboxName && acc.Onrane >= a)
                                        {
                                            Random rand1 = new Random();
                                            Item originalItem = Inventory[i];

                                            ushort[] items = new ushort[50];
                                            for (int x = 0; x < 50; x++)
                                            {
                                                var result = GetUnboxResult(originalItem, rand1);
                                                items[x] = result.Item1.ObjectType;
                                            }

                                            Inventory[i] = Manager.GameData.Items[items[45]];
                                            SaveToCharacter();
                                            Credits = db.UpdateOnraneCurrency(acc, -a);
                                            UpdateCount++;

                                            Client.SendPacket(new UnboxResultPacket
                                            {
                                                Items = items
                                            });
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    SendError("You need at least " + a + " onrane in order to open this Lootbox!");
                                }
                            }
                        }
                        break;

                    case ActivateEffects.PLootboxActivate:
                        {
                            Random rnd = new Random();

                            using (var db = new Database())
                            {
                                int a = eff.Amount;
                                string LootboxName = eff.LootboxName;
                                var acc = Client.Account;
                                if (acc.Kantos >= a)
                                {
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == LootboxName && acc.Kantos >= a)
                                        {
                                            Random rand1 = new Random();
                                            Item originalItem = Inventory[i];

                                            ushort[] items = new ushort[50];
                                            for (int x = 0; x < 50; x++)
                                            {
                                                var result = GetUnboxResult(originalItem, rand1);
                                                items[x] = result.Item1.ObjectType;
                                            }

                                            Inventory[i] = Manager.GameData.Items[items[45]];
                                            SaveToCharacter();
                                            Kantos = db.UpdateKantosCurrency(acc, -a);
                                            UpdateCount++;

                                            Client.SendPacket(new UnboxResultPacket
                                            {
                                                Items = items
                                            });
                                            return false;
                                        }
                                    }
                                }
                                else
                                {
                                    SendError("You need at least " + a + " kantos in order to open this Lootbox!");
                                }
                            }
                        }
                        break;

                    case ActivateEffects.ConditionEffectAura:
                        {
                            var durationCEA = eff.DurationMS;
                            var rangeCEA = eff.Range;
                            if (eff.UseWisMod)
                            {
                                durationCEA = (int)(UseWisMod(eff.DurationSec) * 1000);
                                rangeCEA = UseWisMod(eff.Range);
                            }

                            this.Aoe(rangeCEA, true, player =>
                            {
                                player.ApplyConditionEffect(new ConditionEffect
                                {
                                    Effect = eff.ConditionEffect.Value,
                                    DurationMS = durationCEA
                                });
                            });

                            var color = 0xffffffff;
                            switch (eff.ConditionEffect.Value)
                            {
                                case ConditionEffectIndex.Damaging:
                                    color = 0xffff0000;
                                    break;

                                case ConditionEffectIndex.Berserk:
                                    color = 0x808080;
                                    break;
                            }

                            BroadcastSync(new ShowEffectPacket
                            {
                                EffectType = EffectType.AreaBlast,
                                TargetId = Id,
                                Color = new ARGB(color),
                                PosA = new Position { X = rangeCEA }
                            }, p => this.Dist(p) < 25);
                        }
                        break;

                    case ActivateEffects.Heal:
                        {
                            List<Packet> pkts = new List<Packet>();
                            ActivateHealHp(this, eff.Amount, pkts);
                            Owner.BroadcastPackets(pkts, null);
                        }
                        break;

                    case ActivateEffects.HealNova:
                        {
                            var amountHN = eff.Amount;
                            var rangeHN = eff.Range;
                            if (eff.UseWisMod)
                            {
                                amountHN = (int)UseWisMod(eff.Amount, 0);
                                rangeHN = UseWisMod(eff.Range);
                            }

                            List<Packet> pkts = new List<Packet>();
                            this.Aoe(rangeHN, true, player => { ActivateHealHp(player as Player, amountHN, pkts); });
                            pkts.Add(new ShowEffectPacket
                            {
                                EffectType = EffectType.AreaBlast,
                                TargetId = Id,
                                Color = new ARGB(0xffffffff),
                                PosA = new Position { X = rangeHN }
                            });
                            BroadcastSync(pkts, p => this.Dist(p) < 25);
                        }
                        break;

                    case ActivateEffects.HealNovaSigil:
                        {
                            ActivateShoot(time, item, target);
                            var amountHN = eff.Amount;
                            var rangeHN = eff.Range;
                            if (eff.UseWisMod)
                            {
                                amountHN = (int)UseWisMod(eff.Amount, 0);
                                rangeHN = UseWisMod(eff.Range);
                            }

                            List<Packet> pkts = new List<Packet>();
                            this.Aoe(rangeHN, true, player => { ActivateHealHp(player as Player, amountHN, pkts); });
                            pkts.Add(new ShowEffectPacket
                            {
                                EffectType = EffectType.AreaBlast,
                                TargetId = Id,
                                Color = new ARGB(0xffffffff),
                                PosA = new Position { X = rangeHN }
                            });
                            BroadcastSync(pkts, p => this.Dist(p) < 25);
                        }
                        break;

                    case ActivateEffects.Magic:
                        {
                            List<Packet> pkts = new List<Packet>();
                            ActivateHealMp(this, eff.Amount, pkts);
                            Owner.BroadcastPackets(pkts, null);
                        }
                        break;

                    case ActivateEffects.Banner:
                        {
                            BroadcastSync(new ShowEffectPacket
                            {
                                EffectType = EffectType.Throw,
                                Color = new ARGB(0x0000ff),
                                TargetId = Id,
                                PosA = target
                            }, p => this.Dist(p) < 25);
                            Owner.Timers.Add(new WorldTimer(1500, (world, t) =>
                            {
                                Banner banner = new Banner(this, eff.Range, eff.Amount, eff.DurationMS);
                                banner.Move(target.X, target.Y);
                                world.EnterWorld(banner);
                            }));
                        }
                        break;

                    case ActivateEffects.MagicNova:
                        {
                            List<Packet> pkts = new List<Packet>();
                            this.Aoe(eff.Range / 2, true,
                                player => { ActivateHealMp(player as Player, eff.Amount, pkts); });
                            pkts.Add(new ShowEffectPacket
                            {
                                EffectType = EffectType.AreaBlast,
                                TargetId = Id,
                                Color = new ARGB(0xffffffff),
                                PosA = new Position { X = eff.Range }
                            });
                            Owner.BroadcastPackets(pkts, null);
                        }
                        break;
				case ActivateEffects.DiceActivate:
					{
						Random rnd = new Random();
						int roll = rnd.Next (1, 7);
                            switch (roll)
                            {
                                case 1:
                                    ApplyConditionEffect(new ConditionEffect
                                    {
                                        Effect = ConditionEffectIndex.Damaging,
                                        DurationMS = eff.DurationMS
                                    });
                                    break;
                                case 2:
                                    ApplyConditionEffect(new ConditionEffect
                                    {
                                        Effect = ConditionEffectIndex.Bravery,
                                        DurationMS = eff.DurationMS
                                    });
                                    break;
                                case 3:
                                    ApplyConditionEffect(new ConditionEffect
                                    {
                                        Effect = ConditionEffectIndex.Healing,
                                        DurationMS = eff.DurationMS
                                    });
                                    break;
                                case 4:
                                    ApplyConditionEffect(new ConditionEffect
                                    {
                                        Effect = ConditionEffectIndex.Berserk,
                                        DurationMS = eff.DurationMS
                                    });
                                    break;

                                case 5:
                                    ApplyConditionEffect(new ConditionEffect
                                    {
                                        Effect = ConditionEffectIndex.Speedy,
                                        DurationMS = eff.DurationMS
                                    });
                                    break;

                                case 6:
                                    ApplyConditionEffect(new ConditionEffect
                                    {
                                        Effect = ConditionEffectIndex.ArmorBroken,
                                        DurationMS = eff.DurationMS
                                    });
                                    break;
                            }
					}
					break;
                    case ActivateEffects.Teleport:
                        {
                            Move(target.X, target.Y);
                            UpdateCount++;
                            Owner.BroadcastPackets(new Packet[]
                            {
                            new GotoPacket
                            {
                                ObjectId = Id,
                                Position = new Position
                                {
                                    X = X,
                                    Y = Y
                                }
                            },
                            new ShowEffectPacket
                            {
                                EffectType = EffectType.Teleport,
                                TargetId = Id,
                                PosA = new Position
                                {
                                    X = X,
                                    Y = Y
                                },
                                Color = new ARGB(0xFFFFFFFF)
                            }
                            }, null);
                        }
                        break;

                    case ActivateEffects.VampireBlast:
                        {
                            List<Packet> pkts = new List<Packet>();
                            pkts.Add(new ShowEffectPacket
                            {
                                EffectType = EffectType.Trail,
                                TargetId = Id,
                                PosA = target,
                                Color = new ARGB(0xFFFF0000)
                            });
                            pkts.Add(new ShowEffectPacket
                            {
                                EffectType = EffectType.Diffuse,
                                Color = new ARGB(0xFFFF0000),
                                TargetId = Id,
                                PosA = target,
                                PosB = new Position { X = target.X + eff.Radius, Y = target.Y }
                            });

                            int totalDmg = 0;
                            List<Enemy> enemies = new List<Enemy>();
                            Owner.Aoe(target, eff.Radius, false, enemy =>
                            {
                                enemies.Add(enemy as Enemy);
                                totalDmg += (enemy as Enemy).Damage(this, time, eff.TotalDamage, false);
                            });
                            List<Player> players = new List<Player>();
                            this.Aoe(eff.Radius, true, player =>
                            {
                                players.Add(player as Player);
                                ActivateHealHp(player as Player, totalDmg, pkts);
                            });

                            if (enemies.Count > 0)
                            {
                                Random rand = new Random();
                                for (int i = 0; i < 5; i++)
                                {
                                    Enemy a = enemies[rand.Next(0, enemies.Count)];
                                    Player b = players[rand.Next(0, players.Count)];
                                    pkts.Add(new ShowEffectPacket
                                    {
                                        EffectType = EffectType.Flow,
                                        TargetId = b.Id,
                                        PosA = new Position { X = a.X, Y = a.Y },
                                        Color = new ARGB(0xffffffff)
                                    });
                                }
                            }

                            BroadcastSync(pkts, p => this.Dist(p) < 25);
                        }
                        break;

                    case ActivateEffects.PZara:
                        {
                            Manager.Database.DoActionAsync(db =>
                            {
                                var cmd = db.CreateQuery();
                                cmd.CommandText = "UPDATE `accounts` SET `marksEnabled`=@mar WHERE name=@name";
                                cmd.Parameters.AddWithValue("@mar", 1);
                                cmd.Parameters.AddWithValue("@name", Name);
                                if (cmd.ExecuteNonQuery() == 0)
                                {
                                    SendError("Error!");
                                }
                                else
                                {
                                    UpdateCount++;
                                    SendInfo("Marks can now be used on your account!");
                                }
                            });
                        }
                        break;

                    case ActivateEffects.Trap:
                        {
                            BroadcastSync(new ShowEffectPacket
                            {
                                EffectType = EffectType.Throw,
                                Color = new ARGB(0xff9000ff),
                                TargetId = Id,
                                PosA = target
                            }, p => this.Dist(p) < 25);
                            Owner.Timers.Add(new WorldTimer(1500, (world, t) =>
                            {
                                Trap trap = new Trap(
                                    this,
                                    eff.Radius,
                                    eff.TotalDamage,
                                    eff.ConditionEffect ?? ConditionEffectIndex.Slowed,
                                    eff.EffectDuration);
                                trap.Move(target.X, target.Y);
                                world.EnterWorld(trap);
                            }));
                        }
                        break;

                    case ActivateEffects.RoyalTrap:
                        {
                            BroadcastSync(new ShowEffectPacket
                            {
                                EffectType = EffectType.Throw,
                                Color = new ARGB(0xff9900),
                                TargetId = Id,
                                PosA = target
                            }, p => this.Dist(p) < 25);
                            Owner.Timers.Add(new WorldTimer(1500, (world, t) =>
                            {
                                Trap trap = new Trap(
                                    this,
                                    eff.Radius,
                                    eff.TotalDamage,
                                    eff.ConditionEffect ?? ConditionEffectIndex.Slowed,
                                    eff.EffectDuration);
                                trap.Move(target.X, target.Y);
                                world.EnterWorld(trap);
                            }));
                        }
                        break;

                    case ActivateEffects.StasisBlast:
                        {
                            List<Packet> pkts = new List<Packet>();

                            pkts.Add(new ShowEffectPacket
                            {
                                EffectType = EffectType.Concentrate,
                                TargetId = Id,
                                PosA = target,
                                PosB = new Position { X = target.X + 3, Y = target.Y },
                                Color = new ARGB(0xFF00D0)
                            });
                            Owner.Aoe(target, 3, false, enemy =>
                            {
                                if (IsSpecial(enemy.ObjectType))
                                    return;

                                if (enemy.HasConditionEffect(ConditionEffectIndex.StasisImmune))
                                {
                                    if (!enemy.HasConditionEffect(ConditionEffectIndex.Invincible))
                                    {
                                        pkts.Add(new NotificationPacket
                                        {
                                            ObjectId = enemy.Id,
                                            Color = new ARGB(0xff00ff00),
                                            Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"Immune\"}}"
                                        });
                                    }
                                }
                                else if (!enemy.HasConditionEffect(ConditionEffectIndex.Stasis))
                                {
                                    enemy.ApplyConditionEffect(new ConditionEffect
                                    {
                                        Effect = ConditionEffectIndex.Stasis,
                                        DurationMS = eff.DurationMS
                                    });
                                    Owner.Timers.Add(new WorldTimer(eff.DurationMS, (world, t) =>
                                    {
                                        enemy.ApplyConditionEffect(new ConditionEffect
                                        {
                                            Effect = ConditionEffectIndex.StasisImmune,
                                            DurationMS = 3000
                                        });
                                    }));
                                    pkts.Add(new NotificationPacket
                                    {
                                        ObjectId = enemy.Id,
                                        Color = new ARGB(0xffff0000),
                                        Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"Stasis\"}}"
                                    });
                                }
                            });
                            BroadcastSync(pkts, p => this.Dist(p) < 25);
                        }
                        break;

                    case ActivateEffects.BigStasisBlast:
                        {
                            List<Packet> pkts = new List<Packet>();

                            pkts.Add(new ShowEffectPacket
                            {
                                EffectType = EffectType.Concentrate,
                                TargetId = Id,
                                PosA = target,
                                PosB = new Position { X = target.X + 6, Y = target.Y },
                                Color = new ARGB(0x00FF00)
                            });
                            Owner.Aoe(target, 6, false, enemy =>
                            {
                                if (IsSpecial(enemy.ObjectType))
                                    return;

                                if (enemy.HasConditionEffect(ConditionEffectIndex.StasisImmune))
                                {
                                    if (!enemy.HasConditionEffect(ConditionEffectIndex.Invincible))
                                    {
                                        pkts.Add(new NotificationPacket
                                        {
                                            ObjectId = enemy.Id,
                                            Color = new ARGB(0xff00ff00),
                                            Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"Immune\"}}"
                                        });
                                    }
                                }
                                else if (!enemy.HasConditionEffect(ConditionEffectIndex.Stasis))
                                {
                                    enemy.ApplyConditionEffect(new ConditionEffect
                                    {
                                        Effect = ConditionEffectIndex.Stasis,
                                        DurationMS = eff.DurationMS
                                    });
                                    Owner.Timers.Add(new WorldTimer(eff.DurationMS, (world, t) =>
                                    {
                                        enemy.ApplyConditionEffect(new ConditionEffect
                                        {
                                            Effect = ConditionEffectIndex.StasisImmune,
                                            DurationMS = 3000
                                        });
                                    }));
                                    pkts.Add(new NotificationPacket
                                    {
                                        ObjectId = enemy.Id,
                                        Color = new ARGB(0xffff0000),
                                        Text = "{\"key\":\"blank\",\"tokens\":{\"data\":\"Stasis\"}}"
                                    });
                                }
                            });
                            BroadcastSync(pkts, p => this.Dist(p) < 25);
                        }
                        break;

                    case ActivateEffects.Decoy:
                        {
                            Decoy decoy = new Decoy(Manager, this, eff.DurationMS, StatsManager.GetSpeed());
                            decoy.Move(X, Y);
                            Owner.EnterWorld(decoy);
                        }
                        break;

                    case ActivateEffects.Lightning:
                        {
                            Enemy start = null;
                            double angle = Math.Atan2(target.Y - Y, target.X - X);
                            double diff = Math.PI / 3;
                            Owner.Aoe(target, 6, false, enemy =>
                            {
                                if (!(enemy is Enemy))
                                    return;
                                double x = Math.Atan2(enemy.Y - Y, enemy.X - X);
                                if (Math.Abs(angle - x) < diff)
                                {
                                    start = enemy as Enemy;
                                    diff = Math.Abs(angle - x);
                                }
                            });
                            if (start == null)
                                break;

                            Enemy current = start;
                            Enemy[] targets = new Enemy[eff.MaxTargets];
                            for (int i = 0; i < targets.Length; i++)
                            {
                                targets[i] = current;
                                Enemy next = current.GetNearestEntity(8, false,
                                    enemy =>
                                        enemy is Enemy &&
                                        Array.IndexOf(targets, enemy) == -1 &&
                                        this.Dist(enemy) <= 6) as Enemy;

                                if (next == null)
                                    break;
                                current = next;
                            }

                            List<Packet> pkts = new List<Packet>();
                            for (int i = 0; i < targets.Length; i++)
                            {
                                if (targets[i] == null)
                                    break;
                                if (targets[i].HasConditionEffect(ConditionEffectIndex.Invincible))
                                    continue;
                                Entity prev = i == 0 ? (Entity)this : targets[i - 1];
                                targets[i].Damage(this, time, eff.TotalDamage, false);
                                if (eff.ConditionEffect != null)
                                    targets[i].ApplyConditionEffect(new ConditionEffect
                                    {
                                        Effect = eff.ConditionEffect.Value,
                                        DurationMS = (int)(eff.EffectDuration * 1000)
                                    });
                                pkts.Add(new ShowEffectPacket
                                {
                                    EffectType = EffectType.Lightning,
                                    TargetId = prev.Id,
                                    Color = new ARGB(0xffff0088),
                                    PosA = new Position
                                    {
                                        X = targets[i].X,
                                        Y = targets[i].Y
                                    },
                                    PosB = new Position { X = 350 }
                                });
                            }
                            BroadcastSync(pkts, p => this.Dist(p) < 25);
                        }
                        break;

                    case ActivateEffects.PoisonGrenade:
                        {
                            try
                            {
                                BroadcastSync(new ShowEffectPacket
                                {
                                    EffectType = EffectType.Throw,
                                    Color = new ARGB(0xffddff00),
                                    TargetId = Id,
                                    PosA = target
                                }, p => this.Dist(p) < 25);
                                Placeholder x = new Placeholder(Manager, 1500);
                                x.Move(target.X, target.Y);
                                Owner.EnterWorld(x);
                                try
                                {
                                    Owner.Timers.Add(new WorldTimer(1500, (world, t) =>
                                    {
                                        world.BroadcastPacket(new ShowEffectPacket
                                        {
                                            EffectType = EffectType.AreaBlast,
                                            Color = new ARGB(0xffddff00),
                                            TargetId = x.Id,
                                            PosA = new Position { X = eff.Radius }
                                        }, null);
                                        world.Aoe(target, eff.Radius, false,
                                            enemy => PoisonEnemy(enemy as Enemy, eff));
                                    }));
                                }
                                catch (Exception ex)
                                {
                                    log.ErrorFormat("Poison ShowEffect:\n{0}", ex);
                                }
                            }
                            catch (Exception ex)
                            {
                                log.ErrorFormat("Poisons General:\n{0}", ex);
                            }
                        }
                        break;

                    case ActivateEffects.RemoveNegativeConditions:
                        {
                            this.Aoe(eff.Range / 2, true, player => { ApplyConditionEffect(NegativeEffs); });
                            BroadcastSync(new ShowEffectPacket
                            {
                                EffectType = EffectType.AreaBlast,
                                TargetId = Id,
                                Color = new ARGB(0xffffffff),
                                PosA = new Position { X = eff.Range / 2 }
                            }, p => this.Dist(p) < 25);
                        }
                        break;

                    case ActivateEffects.RemoveNegativeConditionsSelf:
                        {
                            ApplyConditionEffect(NegativeEffs);
                            Owner.BroadcastPacket(new ShowEffectPacket
                            {
                                EffectType = EffectType.AreaBlast,
                                TargetId = Id,
                                Color = new ARGB(0xffffffff),
                                PosA = new Position { X = 1 }
                            }, null);
                        }
                        break;

                    case ActivateEffects.IncrementStat:
                        {
                            int idx = -1;

                            if (eff.Stats == StatsType.MaximumHP)
                                idx = 0;
                            else if (eff.Stats == StatsType.MaximumMP)
                                idx = 1;
                            else if (eff.Stats == StatsType.Attack)
                                idx = 2;
                            else if (eff.Stats == StatsType.Defense)
                                idx = 3;
                            else if (eff.Stats == StatsType.Speed)
                                idx = 4;
                            else if (eff.Stats == StatsType.Vitality)
                                idx = 5;
                            else if (eff.Stats == StatsType.Wisdom)
                                idx = 6;
                            else if (eff.Stats == StatsType.Dexterity)
                                idx = 7;
							else if (eff.Stats == StatsType.Might)
								idx = 8;
							else if (eff.Stats == StatsType.Luck)
								idx = 9;

                            Stats[idx] += eff.Amount;
                            int limit =
                                int.Parse(
                                    Manager.GameData.ObjectTypeToElement[ObjectType].Element(
                                            StatsManager.StatsIndexToName(idx))
                                        .Attribute("max")
                                        .Value);
                            if (Stats[idx] > limit)
                                Stats[idx] = limit;
                            UpdateCount++;
                        }
                        break;

                    case ActivateEffects.OPBUFF:
                        {
                            fast = false;
                            if (!ninjaShoot)
                            {
                                ApplyConditionEffect(new ConditionEffect
                                {
                                    Effect = ConditionEffectIndex.Damaging,
                                    DurationMS = -1
                                });
                                ninjaFreeTimer = true;
                                ninjaShoot = true;
                            }
                            else
                            {
                                ApplyConditionEffect(new ConditionEffect
                                {
                                    Effect = ConditionEffectIndex.Armored,
                                    DurationMS = -1
                                });
                                ApplyConditionEffect(new ConditionEffect
                                {
                                    Effect = ConditionEffectIndex.Damaging,
                                    DurationMS = 0
                                });
                                ushort obj;
                                Manager.GameData.IdToObjectType.TryGetValue(item.ObjectId, out obj);
                                if (Mp >= item.MpEndCost)
                                {
                                    ActivateShoot(time, item, pkt.ItemUsePos);
                                    Mp -= (int)item.MpEndCost;
                                }
                                targetlink = target;
                                ninjaShoot = false;
                            }
                        }
                        break;

                    case ActivateEffects.TreasureActivate:
                        {
                            using (var db = new Database())
                            {
                                int a = eff.Amount;

                                Credits = db.UpdateCredit(Client.Account, a);
                                UpdateCount++;
                            }
                        }
                        break;

                    case ActivateEffects.MarkActivate:
                        {
                            if (MarksEnabled == 1)
                            {
                                int a = eff.Amount;
                                MarkId = a;
                                UpdateCount++;
                                SendInfo("The mark has been activated!");
                            }
                            else
                            {
                                SendError(
                                    "Your account can't have marks enabled yet! You must have used the Page of Zaragon in order to enable marks.");
                            }
                        }
                        break;

                    case ActivateEffects.PetStoneActivate:
                        {
                            var p = Pet;

                            if (p == null)
                            {
                                SendError("Pet not found");
                                return false;
                            }
                            Manager.Database.DoActionAsync(db =>
                            {
                                MySqlCommand cmd = db.CreateQuery();
                                cmd.CommandText = "UPDATE pets SET skin=@skin WHERE petId=@petId AND accId=@accId;";
                                cmd.Parameters.AddWithValue("@accId", int.Parse(AccountId));
                                cmd.Parameters.AddWithValue("@petId", Pet.PetId);
                                cmd.Parameters.AddWithValue("@skin", eff.Amount);
                                cmd.ExecuteNonQuery();
                                db.Dispose();
                            });
                            SendHelp("Your pet's skin has been trasformed!");
                            SaveToCharacter();
                            UpdateCount++;
                            
                        }
                        break;

                    case ActivateEffects.OnraneActivate:
                        {
                            using (var db = new Database())
                            {
                                int a = eff.Amount;

                                Onrane = db.UpdateOnraneCurrency(Client.Account, a);
                                UpdateCount++;
                            }
                        }
                        break;

                    case ActivateEffects.BuildTower:
                        {
                            Entity ezn = Entity.Resolve(Manager, eff.ObjectId);
                            ezn.Move(X, Y);
                            ezn.SetPlayerOwner(this);
                            Owner.EnterWorld(ezn);
                            Owner.Timers.Add(new WorldTimer(eff.DurationMS * 1000, (w, t) =>
                            {
                                w.LeaveWorld(ezn);
                            }));
                        }
                        break;

                    case ActivateEffects.FameActivate:
                        {
                            using (var db = new Database())
                            {
                                int a = eff.Amount;

                                CurrentFame = db.UpdateFame(Client.Account, a);
                                UpdateCount++;
                            }
                        }
                        break;

                    case ActivateEffects.UnScroll:
                        {
                            List<Packet> pkts = new List<Packet>();
                            int ScrollChance = Random.Next(0, 4);

                            switch (ScrollChance)
                            {
                                case 0:

                                    this.Aoe(6, true, player => { ActivateHealMp(player as Player, 300, pkts); });
                                    pkts.Add(new ShowEffectPacket
                                    {
                                        EffectType = EffectType.AreaBlast,
                                        TargetId = Id,
                                        Color = new ARGB(0x00000fff),
                                        PosA = new Position { X = 6 }
                                    });
                                    Owner.BroadcastPackets(pkts, null);
                                    SendInfo("An area around you is magical!");

                                    break;

                                case 1:

                                    this.Aoe(6, true, player => { ActivateHealHp(player as Player, 300, pkts); });
                                    pkts.Add(new ShowEffectPacket
                                    {
                                        EffectType = EffectType.AreaBlast,
                                        TargetId = Id,
                                        Color = new ARGB(0xffffffff),
                                        PosA = new Position { X = 6 }
                                    });
                                    BroadcastSync(pkts, p => this.Dist(p) < 25);
                                    SendInfo("You heal everybody around you!");
                                    break;

                                case 2:
                                    SendInfo("The scroll did absolutely nothing.");
                                    break;

                                case 3:
                                    HP -= 100;
                                    SendInfo("Your hp lowered by 100!");
                                    break;

                                case 4:
                                    Mp -= 100;
                                    SendInfo("Your mp lowered by 100!");
                                    break;

                                case 5:
                                    Mp += 100;
                                    SendInfo("Your mp increased by 100!");
                                    break;

                                case 6:
                                    HP += 100;
                                    SendInfo("Your Hp increased by 100!");
                                    break;
                            }
                        }
                        break;

                    case ActivateEffects.BlackScroll:
                        {
                            Move(Quest.X + 1.0f, Quest.Y + 1.0f);
                            if (Pet != null)
                                Pet.Move(Quest.X + 1.0f, Quest.Y + 1.0f);
                            UpdateCount++;
                            Owner.BroadcastPacket(new GotoPacket
                            {
                                ObjectId = Id,
                                Position = new Position
                                {
                                    X = Quest.X,
                                    Y = Quest.Y
                                }
                            }, null);
                        }
                        break;

                    case ActivateEffects.SpiderTrap:
                        {
                            BroadcastSync(new ShowEffectPacket
                            {
                                EffectType = EffectType.Throw,
                                Color = new ARGB(0xFFFFFF),
                                TargetId = Id,
                                PosA = target
                            }, p => this.Dist(p) < 25);
                            Owner.Timers.Add(new WorldTimer(1500, (world, t) =>
                            {
                                Trap trap = new Trap(
                                    this,
                                    eff.Radius,
                                    eff.TotalDamage,
                                    eff.ConditionEffect ?? ConditionEffectIndex.Slowed,
                                    eff.EffectDuration);
                                trap.Move(target.X, target.Y);
                                world.EnterWorld(trap);
                            }));
                        }
                        break;

                    case ActivateEffects.BrownScroll:
                        {
                            int BrownScrollChance = Random.Next(0, 3);
                            switch (BrownScrollChance)
                            {
                                case 0:
                                    {
                                        using (var db = new Database())
                                        {
                                            CurrentFame = db.UpdateFame(Client.Account, 20);
                                            UpdateCount++;
                                        }
                                    }
                                    break;

                                case 1:
                                    {
                                        using (var db = new Database())
                                        {
                                            CurrentFame = db.UpdateFame(Client.Account, 40);
                                            UpdateCount++;
                                        }
                                    }
                                    break;

                                case 2:
                                    {
                                        using (var db = new Database())
                                        {
                                            CurrentFame = db.UpdateFame(Client.Account, 60);
                                            UpdateCount++;
                                        }
                                    }
                                    break;
                            }
                        }
                        break;

                    case ActivateEffects.RandomGold:
                        {
                            int GoldChance = Random.Next(0, 3);
                            switch (GoldChance)
                            {
                                case 0:
                                    {
                                        using (var db = new Database())
                                        {
                                            Credits = db.UpdateCredit(Client.Account, 500);
                                            UpdateCount++;
                                        }
                                        SendHelp("You have acquired 500 gold!");
                                    }
                                    break;

                                case 1:
                                    {
                                        using (var db = new Database())
                                        {
                                            Credits = db.UpdateCredit(Client.Account, 1000);
                                            UpdateCount++;
                                        }
                                        SendHelp("You have acquired 1000 gold!");
                                    }
                                    break;

                                case 2:
                                    {
                                        using (var db = new Database())
                                        {
                                            Credits = db.UpdateCredit(Client.Account, 1500);
                                            UpdateCount++;
                                        }
                                        SendHelp("You have acquired 1500 gold!");
                                    }
                                    break;
                            }
                        }
                        break;

                    case ActivateEffects.RandomOnrane:
                        {
                            int OnraneChance = Random.Next(0, 5);
                            switch (OnraneChance)
                            {
                                case 0:
                                    {
                                        using (var db = new Database())
                                        {
                                            Onrane = db.UpdateOnraneCurrency(Client.Account, 2);
                                            UpdateCount++;
                                        }
                                        SendHelp("You have acquired 2 onrane!");
                                    }
                                    break;

                                case 1:
                                    {
                                        using (var db = new Database())
                                        {
                                            Onrane = db.UpdateOnraneCurrency(Client.Account, 4);
                                            UpdateCount++;
                                        }
                                        SendHelp("You have acquired 4 onrane!");
                                    }
                                    break;

                                case 2:
                                    {
                                        using (var db = new Database())
                                        {
                                            Onrane = db.UpdateOnraneCurrency(Client.Account, 6);
                                            UpdateCount++;
                                        }
                                        SendHelp("You have acquired 6 onrane!");
                                    }
                                    break;

                                case 3:
                                    {
                                        using (var db = new Database())
                                        {
                                            Onrane = db.UpdateOnraneCurrency(Client.Account, 8);
                                            UpdateCount++;
                                        }
                                        SendHelp("You have acquired 8 onrane!");
                                    }
                                    break;

                                case 4:
                                    {
                                        using (var db = new Database())
                                        {
                                            Onrane = db.UpdateOnraneCurrency(Client.Account, 10);
                                            UpdateCount++;
                                        }
                                        SendHelp("You have acquired 10 onrane!");
                                    }
                                    break;
                            }
                        }
                        break;

                    case ActivateEffects.URandomOnrane:
                        {
                            int OnraneChance = Random.Next(0, 5);
                            switch (OnraneChance)
                            {
                                case 0:
                                    {
                                        using (var db = new Database())
                                        {
                                            Onrane = db.UpdateOnraneCurrency(Client.Account, 12);
                                            UpdateCount++;
                                        }
                                        SendHelp("You have acquired 12 onrane!");
                                    }
                                    break;

                                case 1:
                                    {
                                        using (var db = new Database())
                                        {
                                            Onrane = db.UpdateOnraneCurrency(Client.Account, 14);
                                            UpdateCount++;
                                        }
                                        SendHelp("You have acquired 14 onrane!");
                                    }
                                    break;

                                case 2:
                                    {
                                        using (var db = new Database())
                                        {
                                            Onrane = db.UpdateOnraneCurrency(Client.Account, 16);
                                            UpdateCount++;
                                        }
                                        SendHelp("You have acquired 16 onrane!");
                                    }
                                    break;

                                case 3:
                                    {
                                        using (var db = new Database())
                                        {
                                            Onrane = db.UpdateOnraneCurrency(Client.Account, 18);
                                            UpdateCount++;
                                        }
                                        SendHelp("You have acquired 18 onrane!");
                                    }
                                    break;

                                case 4:
                                    {
                                        using (var db = new Database())
                                        {
                                            Onrane = db.UpdateOnraneCurrency(Client.Account, 20);
                                            UpdateCount++;
                                        }
                                        SendHelp("You have acquired 20 onrane!");
                                    }
                                    break;
                            }
                        }
                        break;

                    case ActivateEffects.RandomKantos:
                        {
                            int KantosChance = Random.Next(0, 3);
                            switch (KantosChance)
                            {
                                case 0:
                                    {
                                        using (var db = new Database())
                                        {
                                            Kantos = db.UpdateKantosCurrency(Client.Account, 100);
                                            UpdateCount++;
                                        }
                                        SendHelp("You have acquired 100 kantos!");
                                    }
                                    break;

                                case 1:
                                    {
                                        using (var db = new Database())
                                        {
                                            Kantos = db.UpdateKantosCurrency(Client.Account, 200);
                                            UpdateCount++;
                                        }
                                        SendHelp("You have acquired 200 kantos!");
                                    }
                                    break;

                                case 2:
                                    {
                                        using (var db = new Database())
                                        {
                                            Kantos = db.UpdateKantosCurrency(Client.Account, 300);
                                            UpdateCount++;
                                        }
                                        SendHelp("You have acquired 300 kantos!");
                                    }
                                    break;
                            }
                        }
                        break;

                    case ActivateEffects.IdScroll:
                        {
                            int ScrollChance = Random.Next(0, 15);
                            switch (ScrollChance)
                            {
                                case 0:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == "Unidentified Scroll")
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x5065];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            SendHelp("Your scroll has been identified!");
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 1:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == "Unidentified Scroll")
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x5066];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            SendHelp("Your scroll has been identified!");
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 2:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == "Unidentified Scroll")
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x506b];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            SendHelp("Your scroll has been identified!");
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 3:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == "Unidentified Scroll")
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x5067];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            SendHelp("Your scroll has been identified!");
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 4:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == "Unidentified Scroll")
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x5068];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            SendHelp("Your scroll has been identified!");
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 5:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == "Unidentified Scroll")
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x5069];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            SendHelp("Your scroll has been identified!");
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 6:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == "Unidentified Scroll")
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x506a];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            SendHelp("Your scroll has been identified!");
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 7:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == "Unidentified Scroll")
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x506c];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            SendHelp("Your scroll has been identified!");
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 8:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == "Unidentified Scroll")
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x506d];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            SendHelp("Your scroll has been identified!");
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 9:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == "Unidentified Scroll")
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x506e];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            SendHelp("Your scroll has been identified!");
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 10:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == "Unidentified Scroll")
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x507a];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            SendHelp("Your scroll has been identified!");
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 11:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == "Unidentified Scroll")
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x507b];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            SendHelp("Your scroll has been identified!");
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 12:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == "Unidentified Scroll")
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x507c];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            SendHelp("Your scroll has been identified!");
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 13:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == "Unidentified Scroll")
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x507d];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            SendHelp("Your scroll has been identified!");
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 14:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == "Unidentified Scroll")
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x507e];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            SendHelp("Your scroll has been identified!");
                                            return false;
                                        }
                                    }
                                    ;
                                    break;
                            }
                        }
                        break;

                    case ActivateEffects.SamuraiAbility:
                        {
                            fast = false;
                            if (!ninjaShoot)
                            {
                                ApplyConditionEffect(new ConditionEffect
                                {
                                    Effect = ConditionEffectIndex.Berserk,
                                    DurationMS = -1
                                });
                                ninjaFreeTimer = true;
                                ninjaShoot = true;
                            }
                            else
                            {
                                ApplyConditionEffect(new ConditionEffect
                                {
                                    Effect = ConditionEffectIndex.Berserk,
                                    DurationMS = 0
                                });
                                ushort obj;
                                Manager.GameData.IdToObjectType.TryGetValue(item.ObjectId, out obj);
                                if (Mp >= item.MpEndCost)
                                {
                                    List<Packet> pkts = new List<Packet>();
                                    this.Aoe(eff.Range / 2, false, enemy =>
                                    {
                                        (enemy as Enemy).Damage(this, time,
                                            (int)this.StatsManager.GetAttackDamage(eff.TotalDamage, eff.TotalDamage),
                                            false, new ConditionEffect[0]);
                                    });
                                    pkts.Add(new ShowEffectPacket()
                                    {
                                        EffectType = EffectType.AreaBlast,
                                        TargetId = Id,
                                        Color = new ARGB(eff.Color ?? 0xffffffff),
                                        PosA = new Position() { X = eff.Range / 2 }
                                    });
                                    BroadcastSync(pkts, p => this.Dist(p) < 25);
                                    Mp -= (int)item.MpEndCost;
                                }
                                targetlink = target;
                                ninjaShoot = false;
                            }
                        }
                        break;

                    case ActivateEffects.SiphonAbility:
                        {
                            int wisBoost = Stats[6]+Boost[6] * 5;

                            fast = true;
                            if (!ninjaShoot)
                            {
                                List<Packet> pkts = new List<Packet>();
                                ActivateHealMp2(this, 9999, pkts);
                                ninjaFreeTimer = true;
                                ninjaShoot = true;
                            }
                            else
                            {
                                ushort obj;
                                Manager.GameData.IdToObjectType.TryGetValue(item.ObjectId, out obj);
                                if (HP >= item.HpEndCost)
                                {
                                    List<Packet> pkts = new List<Packet>();
                                    pkts.Add(new ShowEffectPacket
                                    {
                                        EffectType = EffectType.Flow,
                                        TargetId = Id,
                                        PosA = target,
                                        Color = new ARGB(0xFFA500)
                                    });

                                    pkts.Add(new ShowEffectPacket()
                                    {
                                        EffectType = EffectType.Diffuse,
                                        TargetId = Id,
                                        Color = new ARGB(0xFFA500),
                                        PosA = target,
                                        PosB = new Position { X = target.X + eff.Range, Y = target.Y }
                                    });
                                    int manaDmg = ManaLeftToDamage();
                                    List<Enemy> enemies = new List<Enemy>();

                                    Owner.Aoe(target, eff.Range, false, enemy =>
                                    {
                                        (enemy as Enemy).Damage(this, time, manaDmg * 6 + wisBoost, false,
                                            new ConditionEffect[0]);
                                    });
                                    BroadcastSync(pkts, p => this.Dist(p) < 25);
                                    HP -= (int)item.HpEndCost;

                                    ActivateHealMp2(this, 9999, pkts); //Recharges Siphon Instantly
                                    Owner.BroadcastPackets(pkts, null);
                                }

                                targetlink = target;
                                ninjaShoot = false;
                            }
                        }
                        break;

                    case ActivateEffects.AstonAbility:
                        {
                            fast = false;
                            if (!ninjaShoot)
                            {
                                ApplyConditionEffect(new ConditionEffect
                                {
                                    Effect = ConditionEffectIndex.Berserk,
                                    DurationMS = -1
                                });
                                ninjaFreeTimer = true;
                                ninjaShoot = true;
                            }
                            else
                            {
                                ApplyConditionEffect(new ConditionEffect
                                {
                                    Effect = ConditionEffectIndex.Berserk,
                                    DurationMS = 0
                                });
                                ushort obj;
                                Manager.GameData.IdToObjectType.TryGetValue(item.ObjectId, out obj);
                                if (Mp >= item.MpEndCost)
                                {
                                    ActivateShoot(time, item, pkt.ItemUsePos);
                                    Mp -= (int)item.MpEndCost;
                                }
                                targetlink = target;
                                ninjaShoot = false;
                            }
                        }
                        break;

                    case ActivateEffects.SamuraiAbility2:
                        {
                            var ydist = target.Y - Y;
                            var xdist = target.X - X;
                            var xwalkable = target.X + xdist / 2;
                            var ywalkable = target.Y + ydist / 2;
                            var tile = Owner.Map[(int)xwalkable, (int)ywalkable];
                            Owner.BroadcastPacket(new ShowEffectPacket
                            {
                                EffectType = EffectType.Diffuse,
                                Color = new ARGB(0xFFFF0000),
                                TargetId = Id,
                                PosA = target,
                                PosB = new Position { X = target.X + eff.Radius, Y = target.Y }
                            }, null);
                            Owner.Aoe(target, eff.Radius, false, enemy =>
                            {
                                (enemy as Enemy).Damage(this, time, eff.TotalDamage, false, new ConditionEffect
                                {
                                    Effect = ConditionEffectIndex.Bleeding,
                                    DurationMS = eff.DurationMS
                                });
                            });
                            Move(target.X + xdist / 2, target.Y + ydist / 2);
                            UpdateCount++;

                            Owner.BroadcastPackets(new Packet[]
                            {
                            new GotoPacket
                            {
                                ObjectId = Id,
                                Position = new Position
                                {
                                    X = X,
                                    Y = Y
                                }
                            },
                            new ShowEffectPacket
                            {
                                EffectType = EffectType.Teleport,
                                TargetId = Id,
                                PosA = new Position
                                {
                                    X = X,
                                    Y = Y
                                },
                                Color = new ARGB(0xFFFFFFFF)
                            }
                            }, null);
                            ApplyConditionEffect(new ConditionEffect
                            {
                                Effect = ConditionEffectIndex.Paralyzed,
                                DurationMS = eff.DurationMS2
                            });
                        }
                        break;

                    case ActivateEffects.AsiHeal:
                        {
                            var amountHN = eff.Amount;
                            var rangeHN = eff.Range;
                            if (eff.UseWisMod)
                            {
                                amountHN = (int)UseWisMod(eff.Amount, 0);
                                rangeHN = UseWisMod(eff.Range);
                            }

                            List<Packet> pkts = new List<Packet>();
                            this.Aoe(rangeHN, true, player => { ActivateHealHp(player as Player, amountHN, pkts); });
                            pkts.Add(new ShowEffectPacket
                            {
                                EffectType = EffectType.AreaBlast,
                                TargetId = Id,
                                Color = new ARGB(0xff8000),
                                PosA = new Position { X = rangeHN }
                            });
                            pkts.Add(new ShowEffectPacket
                            {
                                EffectType = EffectType.AreaBlast,
                                TargetId = Id,
                                Color = new ARGB(0xffffffff),
                                PosA = new Position { X = rangeHN }
                            });
                            BroadcastSync(pkts, p => this.Dist(p) < 25);
                        }
                        break;

                    case ActivateEffects.SorMachine:
                        {
                            int LGITEM = Random.Next(0, 22);
                            string Crystal = "Legendary Sor Crystal";
                            switch (LGITEM)
                            {
                                case 0:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x42fa];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 1:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x42fc];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 2:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x47f8];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 3:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x5435];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 4:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x42f4];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 5:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x42b7];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 6:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x42c5];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 7:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x42c7];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 8:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x49e4];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 9:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x542b];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 10:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x47cb];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 11:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x45d1];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 12:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x48a6];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 13:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x1644];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 14:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x5437];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 15:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x42f2];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 16:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x43a6];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 17:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x46d7];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 18:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x46d8];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 19:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x43a2];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 20:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
											Inventory[i] = Manager.GameData.Items[0x5834];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;

                                case 21:
                                    for (int i = 0; i < Inventory.Length; i++)
                                    {
                                        if (Inventory[i] == null)
                                            continue;
                                        if (Inventory[i].ObjectId == Crystal)
                                        {
                                            Inventory[i] = Manager.GameData.Items[0x585b];
                                            UpdateCount++;
                                            SaveToCharacter();
                                            return false;
                                        }
                                    }
                                    ;
                                    break;
                            }
                        }
                        break;

                    case ActivateEffects.UnlockPortal:

                        Portal portal =
                            this.GetNearestEntity(5, Manager.GameData.IdToObjectType[eff.LockedName]) as Portal;

                        Packet[] packets = new Packet[3];
                        packets[0] = new ShowEffectPacket
                        {
                            EffectType = EffectType.AreaBlast,
                            Color = new ARGB(0xFFFFFF),
                            PosA = new Position { X = 5 },
                            TargetId = Id
                        };
                        if (portal == null)
                            break;

                        portal.Unlock(eff.DungeonName);

                        packets[1] = new NotificationPacket
                        {
                            Color = new ARGB(0x00FF00),
                            Text =
                                "{\"key\":\"blank\",\"tokens\":{\"data\":\"Unlocked by " +
                                Name + "\"}}",
                            ObjectId = Id
                        };

                        packets[2] = new TextPacket
                        {
                            BubbleTime = 0,
                            Stars = -1,
                            Name = "",
                            Text = eff.DungeonName + " Unlocked by " + Name + "."
                        };

                        BroadcastSync(packets);

                        break;

                    case ActivateEffects.FUnlockPortal:

                        portal = this.GetNearestEntity(5, Manager.GameData.IdToObjectType[eff.LockedName]) as Portal;

                        packets = new Packet[3];
                        packets[0] = new ShowEffectPacket
                        {
                            EffectType = EffectType.AreaBlast,
                            Color = new ARGB(0xFFFFFF),
                            PosA = new Position { X = 5 },
                            TargetId = Id
                        };
                        if (portal == null)
                            break;

                        portal.AldragineUnlock(eff.DungeonName);

                        packets[1] = new NotificationPacket
                        {
                            Color = new ARGB(0x0000FF),
                            Text =
                                "{\"key\":\"blank\",\"tokens\":{\"data\":\"Unlocked by " +
                                Name + "\"}}",
                            ObjectId = Id
                        };

                        packets[2] = new TextPacket
                        {
                            BubbleTime = 0,
                            Stars = -1,
                            Name = "",
                            Text = eff.DungeonName + " Unlocked by " + Name + "."
                        };

                        BroadcastSync(packets);

                        break;

                    case ActivateEffects.Create: //this is a portal
                        {
                            ushort objType;
                            if (!Manager.GameData.IdToObjectType.TryGetValue(eff.Id, out objType) ||
                                !Manager.GameData.Portals.ContainsKey(objType))
                                break; // object not found, ignore
                            Entity entity = Resolve(Manager, objType);
                            World w = Manager.GetWorld(Owner.Id); //can't use Owner here, as it goes out of scope
                            int TimeoutTime = Manager.GameData.Portals[objType].TimeoutTime;
                            string DungName = Manager.GameData.Portals[objType].DungeonName;

                            ARGB c = new ARGB(0x00FF00);

                            entity.Move(X, Y);
                            w.EnterWorld(entity);

                            w.BroadcastPacket(new NotificationPacket
                            {
                                Color = c,
                                Text =
                                    "{\"key\":\"blank\",\"tokens\":{\"data\":\"" + DungName + " opened by " +
                                    Client.Account.Name + "\"}}",
                                ObjectId = Client.Player.Id
                            }, null);

                            w.BroadcastPacket(new TextPacket
                            {
                                BubbleTime = 0,
                                Stars = -1,
                                Name = "",
                                Text = DungName + " opened by " + Client.Account.Name
                            }, null);
                            w.Timers.Add(new WorldTimer(TimeoutTime * 1000,
                                (world, t) => //default portal close time * 1000
                                {
                                    try
                                    {
                                        w.LeaveWorld(entity);
                                    }
                                    catch (Exception ex)
                                //couldn't remove portal, Owner became null. Should be fixed with RealmManager implementation
                                {
                                        log.ErrorFormat("Couldn't despawn portal.\n{0}", ex);
                                    }
                                }));
                        }
                        break;

                    case ActivateEffects.CreateGauntlet: //this is a gauntlet portal
                        {
                            if (Owner.Name != "Nexus")
                            {
                                SendError("You can not open a Gauntlet anywhere other than the nexus.");
                            }
                            else
                            {
                                ushort objType;
                                if (!Manager.GameData.IdToObjectType.TryGetValue(eff.Id, out objType) ||
                                    !Manager.GameData.Portals.ContainsKey(objType))
                                    break; // object not found, ignore
                                Entity entity = Resolve(Manager, objType);
                                World w = Manager.GetWorld(Owner.Id); //can't use Owner here, as it goes out of scope
                                int TimeoutTime = Manager.GameData.Portals[objType].TimeoutTime;
                                string DungName = Manager.GameData.Portals[objType].DungeonName;

                                ARGB c = new ARGB(0x0000FF);

                                entity.Move(X, Y);
                                w.EnterWorld(entity);

                                w.BroadcastPacket(new NotificationPacket
                                {
                                    Color = c,
                                    Text =
                                        "{\"key\":\"blank\",\"tokens\":{\"data\":\"" + DungName + " opened by " +
                                        Client.Account.Name + "\"}}",
                                    ObjectId = Client.Player.Id
                                }, null);

                                w.BroadcastPacket(new TextPacket
                                {
                                    BubbleTime = 0,
                                    Stars = -1,
                                    Name = "",
                                    Text = DungName + " opened by " + Client.Account.Name
                                }, null);

                                foreach (Client i in Manager.Clients.Values)
                                {
                                    i.SendPacket(new TextPacket
                                    {
                                        BubbleTime = 0,
                                        Stars = -1,
                                        Name = "",
                                        Text = "The Gauntlet has been opened in the Nexus!"
                                    });
                                }
                                w.Timers.Add(new WorldTimer(TimeoutTime * 1000,
                                    (world, t) => //default portal close time * 1000
                                    {
                                        try
                                        {
                                            w.LeaveWorld(entity);
                                        }
                                        catch (Exception ex)
                                    //couldn't remove portal, Owner became null. Should be fixed with RealmManager implementation
                                    {
                                            log.ErrorFormat("Couldn't despawn portal.\n{0}", ex);
                                        }
                                    }));
                            }
                        }
                        break;

                    case ActivateEffects.Dye:
                        {
                            if (item.Texture1 != 0)
                            {
                                Texture1 = item.Texture1;
                            }
                            if (item.Texture2 != 0)
                            {
                                Texture2 = item.Texture2;
                            }
                            SaveToCharacter();
                        }
                        break;

                    case ActivateEffects.ShurikenAbility:
                        {
                            fast = false;
                            if (!ninjaShoot)
                            {
                                ApplyConditionEffect(new ConditionEffect
                                {
                                    Effect = ConditionEffectIndex.Speedy,
                                    DurationMS = -1
                                });
                                ninjaFreeTimer = true;
                                ninjaShoot = true;
                            }
                            else
                            {
                                ApplyConditionEffect(new ConditionEffect
                                {
                                    Effect = ConditionEffectIndex.Speedy,
                                    DurationMS = 0
                                });
                                ushort obj;
                                Manager.GameData.IdToObjectType.TryGetValue(item.ObjectId, out obj);
                                if (Mp >= item.MpEndCost)
                                {
                                    ActivateShoot(time, item, pkt.ItemUsePos);
                                    Mp -= (int)item.MpEndCost;
                                }
                                targetlink = target;
                                ninjaShoot = false;
                            }
                        }
                        break;

                    case ActivateEffects.UnlockSkin:
                        if (!Client.Account.OwnedSkins.Contains(item.ActivateEffects[0].SkinType))
                        {
                            Manager.Database.DoActionAsync(db =>
                            {
                                Client.Account.OwnedSkins.Add(item.ActivateEffects[0].SkinType);
                                MySqlCommand cmd = db.CreateQuery();
                                cmd.CommandText = "UPDATE accounts SET ownedSkins=@ownedSkins WHERE id=@id";
                                cmd.Parameters.AddWithValue("@ownedSkins",
                                    Utils.GetCommaSepString(Client.Account.OwnedSkins.ToArray()));
                                cmd.Parameters.AddWithValue("@id", AccountId);
                                cmd.ExecuteNonQuery();
                                SendInfo(
                                    "New skin unlocked successfully. Change skins in your Vault, or start a new character to use.");
                                Client.SendPacket(new UnlockedSkinPacket
                                {
                                    SkinID = item.ActivateEffects[0].SkinType
                                });
                            });
                            endMethod = false;
                            break;
                        }
                        SendInfo("Error.alreadyOwnsSkin");
                        endMethod = true;
                        break;

                    case ActivateEffects.PermaPet: //Doesnt exist anymore
                        {
                            //psr.Character.Pet = XmlDatas.IdToType[eff.ObjectId];
                            //GivePet(XmlDatas.IdToType[eff.ObjectId]);
                            //UpdateCount++;
                        }
                        break;

                    case ActivateEffects.Pet:
                        Entity en = Entity.Resolve(Manager, eff.ObjectId);
                        en.Move(X, Y);
                        en.SetPlayerOwner(this);
                        Owner.EnterWorld(en);
                        Owner.Timers.Add(new WorldTimer(30 * 1000, (w, t) =>
                        {
                            w.LeaveWorld(en);
                        }));
                        break;

                    case ActivateEffects.CreatePet:
                        if (!Owner.Name.StartsWith("Pet Yard"))
                        {
                            SendInfo("server.use_in_petyard");
                            return true;
                        }
                        if (item.Rarity == Rarity.Common)
                        {
                            Pet.Create(Manager, this, item);
                            break;
                        }
                        else if (item.Rarity == Rarity.Uncommon && Owner.ClientWorldName == "{nexus.Pet_Yard_2}" ||
                                 item.Rarity == Rarity.Uncommon && Owner.ClientWorldName == "{nexus.Pet_Yard_3}" ||
                                 item.Rarity == Rarity.Uncommon && Owner.ClientWorldName == "{nexus.Pet_Yard_4}" ||
                                 item.Rarity == Rarity.Uncommon && Owner.ClientWorldName == "{nexus.Pet_Yard_5}")
                        {
                            Pet.Create(Manager, this, item);
                            break;
                        }
                        else if (item.Rarity == Rarity.Rare && Owner.ClientWorldName == "{nexus.Pet_Yard_3}" ||
                                 item.Rarity == Rarity.Rare && Owner.ClientWorldName == "{nexus.Pet_Yard_4}" ||
                                 item.Rarity == Rarity.Rare && Owner.ClientWorldName == "{nexus.Pet_Yard_5}")
                        {
                            Pet.Create(Manager, this, item);
                            break;
                        }
                        else if (item.Rarity == Rarity.Legendary && Owner.ClientWorldName == "{nexus.Pet_Yard_4}" ||
                                 item.Rarity == Rarity.Legendary && Owner.ClientWorldName == "{nexus.Pet_Yard_5}")
                        {
                            Pet.Create(Manager, this, item);
                            break;
                        }
                        SendInfo("You need to upgrade your Pet Yard first.");
                        return true;

                        break;

                    case ActivateEffects.MysteryPortal:
                        string[] dungeons = new[]
                        {
                            "Pirate Cave Portal",
                            "Forest Maze Portal",
                            "Spider Den Portal",
                            "Snake Pit Portal",
                            "Glowing Portal",
                            "Forbidden Jungle Portal",
                            "Candyland Portal",
                            "Haunted Cemetery Portal",
                            "Undead Lair Portal",
                            "Davy Jones' Locker Portal",
                            "Manor of the Immortals Portal",
                            "Abyss of Demons Portal",
                            "Lair of Draconis Portal",
                            "Mad Lab Portal",
                            "Ocean Trench Portal",
                            "Tomb of the Ancients Portal",
                            "Beachzone Portal",
                            "The Shatters",
                            "Deadwater Docks",
                            "Woodland Labyrinth",
                            "The Crawling Depths",
                            "Treasure Cave Portal",
                            "Battle Nexus Portal",
                            "Belladonna's Garden Portal",
                            "Lair of Shaitan Portal"
                        };

                        var descs = Manager.GameData.Portals.Where(_ => dungeons.Contains<string>(_.Value.ObjectId))
                            .Select(_ => _.Value).ToArray();
                        var portalDesc = descs[Random.Next(0, descs.Count())];
                        Entity por = Entity.Resolve(Manager, portalDesc.ObjectId);
                        por.Move(this.X, this.Y);
                        Owner.EnterWorld(por);

                        Client.SendPacket(new NotificationPacket
                        {
                            Color = new ARGB(0x00FF00),
                            Text =
                                "{\"key\":\"blank\",\"tokens\":{\"data\":\"" + portalDesc.DungeonName + " opened by " +
                                Client.Account.Name + "\"}}",
                            ObjectId = Client.Player.Id
                        });

                        Owner.BroadcastPacket(new TextPacket
                        {
                            BubbleTime = 0,
                            Stars = -1,
                            Name = "",
                            Text = portalDesc.ObjectId + " opened by " + Name
                        }, null);

                        Owner.Timers.Add(new WorldTimer(portalDesc.TimeoutTime * 1000,
                            (w, t) => //default portal close time * 1000
                            {
                                try
                                {
                                    w.LeaveWorld(por);
                                }
                                catch (Exception ex)
                                {
                                    log.ErrorFormat("Couldn't despawn portal.\n{0}", ex);
                                }
                            }));
                        break;

                    case ActivateEffects.GenericActivate:
                        var targetPlayer = eff.Target.Equals("player");
                        var centerPlayer = eff.Center.Equals("player");
                        var duration = (eff.UseWisMod) ? (int)(UseWisMod(eff.DurationSec) * 1000) : eff.DurationMS;
                        var range = (eff.UseWisMod)
                            ? UseWisMod(eff.Range)
                            : eff.Range;

                        Owner.Aoe((eff.Center.Equals("mouse")) ? target : new Position { X = X, Y = Y }, range,
                            targetPlayer, entity =>
                            {
                                if (IsSpecial(entity.ObjectType))
                                    return;
                                if (!entity.HasConditionEffect(ConditionEffectIndex.Stasis) &&
                                    !entity.HasConditionEffect(ConditionEffectIndex.Invincible))
                                {
                                    entity.ApplyConditionEffect(
                                        new ConditionEffect()
                                        {
                                            Effect = eff.ConditionEffect.Value,
                                            DurationMS = duration
                                        });
                                }
                            });

                        // replaced this last bit with what I had, never noticed any issue with it. Perhaps I'm wrong?
                        BroadcastSync(new ShowEffectPacket()
                        {
                            EffectType = (EffectType)eff.VisualEffect,
                            TargetId = Id,
                            Color = new ARGB(eff.Color ?? 0xffffffff),
                            PosA = centerPlayer ? new Position { X = range } : target,
                            PosB = new Position(target.X - range, target.Y) //Its the range of the diffuse effect
                        }, p => this.DistSqr(p) < 25);
                        break;
                }
            }
            UpdateCount++;
            return endMethod;
        }

        private float UseWisMod(float value, int offset = 1)
        {
            double totalWisdom = Stats[6] + 2 * Boost[6];

            if (totalWisdom < 30)
                return value;

            double m = (value < 0) ? -1 : 1;
            double n = (value * totalWisdom / 150) + (value * m);
            n = Math.Floor(n * Math.Pow(10, offset)) / Math.Pow(10, offset);
            if (n - (int)n * m >= 1 / Math.Pow(10, offset) * m)
            {
                return ((int)(n * 10)) / 10.0f;
            }

            return (int)n;
        }

        private static bool IsSpecial(ushort objType)
        {
            return objType == 0x750d || objType == 0x750e || objType == 0x222c || objType == 0x222d;
        }
    }
}