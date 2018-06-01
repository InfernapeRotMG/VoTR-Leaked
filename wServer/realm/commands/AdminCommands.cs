#region

using db;
using db.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using wServer.networking;
using wServer.networking.svrPackets;
using wServer.realm.entities;
using wServer.realm.entities.player;
using wServer.realm.setpieces;
using wServer.realm.worlds;

#endregion

namespace wServer.realm.commands
{
    internal class StarTypeCommand : Command
    {
        /// <inheritdoc />
        public StarTypeCommand() : base("startype", 3)
        {
        }

        /// <inheritdoc />
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendInfo(ListStarTypes());
                return true;
            }
            if (args.Length > 0 && TryGetStarType(string.Join(" ", args), out var starType))
            {
                player.StarType = starType;
                player.UpdateCount++;

                player.Manager.Database.DoActionAsync(db =>
                {
                    var cmd = db.CreateQuery();
                    cmd.CommandText = "UPDATE stats SET starType=@st WHERE accId=@accId;";
                    cmd.Parameters.AddWithValue("@accId", player.AccountId);
                    cmd.Parameters.AddWithValue("@st", starType);
                    cmd.ExecuteNonQuery();
                });
                player.SendInfo(
                    $"Stars: {player.Stars} (Override: {player.Client.Account.Stats.StarsOverride}) Type: {player.StarType}");
                return true;
            }
            player.SendInfo(ListStarTypes());
            return false;
        }

        private string ListStarTypes()
        {
            var type = typeof(StarsUtil);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.IsLiteral && !f.IsInitOnly).ToArray();
            return string.Join(", ", fields.Select(f => $"{f.Name.Replace("_TYPE", "")} ({f.GetRawConstantValue()})"));
        }

        private bool TryGetStarType(string str, out int starType)
        {
            var type = typeof(StarsUtil);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.IsLiteral && !f.IsInitOnly
                && (string.Equals(f.Name.Replace("_TYPE", ""), str.Replace("_TYPE", ""), StringComparison.OrdinalIgnoreCase)
                || int.TryParse(str, out int d) && d == (int)f.GetRawConstantValue())).ToArray();

            if (fields.Length == 0)
            {
                starType = 0;
                return false;
            }
            starType = (int)fields.First().GetRawConstantValue();
            return true;
        }
    }

    internal class MaxStarsCommand : Command
    {
        /// <inheritdoc />
        public MaxStarsCommand() : base("maxstars", 7)
        {
        }

        /// <inheritdoc />
        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            var stars = player.Manager.GameData.ObjectDescs.Count(od => od.Value.Player) * StarsUtil.StarCount;
            player.Stars = stars;
            player.UpdateCount++;

            player.Manager.Database.DoActionAsync(db =>
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "UPDATE stats SET starOverride=@st WHERE accId=@accId;";
                cmd.Parameters.AddWithValue("@accId", player.AccountId);
                cmd.Parameters.AddWithValue("@st", stars);
                cmd.ExecuteNonQuery();
            });
            return true;
        }
    }
		

    internal class HealthPotionCommand : Command
    {
        public HealthPotionCommand()
            : base("HPot", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /HPot <ammount>");
                    return false;
                }
                if (args.Length == 1)
                {
                    player.HealthPotions = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
                return false;
            }
            return true;
        }
    }

    internal class PetMaxCommand : Command
    {
        public PetMaxCommand()
            : base("petmax", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            Item feed = player.Manager.GameData.Items[0x9d4];
            for (int i = 0; i < 50; i++)
            {
                player.Pet.Feed(feed);
                player.Pet.UpdateCount++;
            }
            return true;
        }
    }

    internal class UpdateMarkCommand : Command
    {
        public UpdateMarkCommand()
            : base("um", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /um <ammount>");
                    return false;
                }
                if (args.Length == 1)
                {
                    player.MarkId = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
                return false;
            }
            return true;
        }
    }

    internal class MagicPotionCommand : Command
    {
        public MagicPotionCommand()
            : base("MPot", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /MPot <ammount>");
                    return false;
                }
                if (args.Length == 1)
                {
                    player.MagicPotions = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
                return false;
            }
            return true;
        }
    }

    internal class TheHoodTranformation : Command
    {
        public TheHoodTranformation()
            : base("hood", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /hood");
                return false;
            }

            foreach (Client i in player.Manager.Clients.Values)
            {
                i.Player.Client.Character.Tex1 = 16777216;
                i.Player.Client.Player.Texture1 = 16777216;
                i.Player.Client.Character.Tex2 = 16777216;
                i.Player.Client.Player.Texture2 = 16777216;
                i.Player.SaveToCharacter();
                i.Player.UpdateCount++;
                i.Player.SendInfo("You are now black!");
            }
            return true;
        }
    }

    internal class ThrowEffCommand : Command
    {
        public ThrowEffCommand()
            : base("throweff", 6)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length < 2)
            {
                player.SendInfo("Usage: /throweff <player> <effect>");
                player.SendInfo("Or");
                player.SendInfo("Usage: /throweff <player> <effect> <duration>");
                return false;
            }
            foreach (Client i in player.Manager.Clients.Values)
            {
                if (i.Account.Name.EqualsIgnoreCase(args[0]))
                {
                    if (args.Length == 2)
                    {
                        i.Player.ApplyConditionEffect(new ConditionEffect
                        {
                            Effect = (ConditionEffectIndex)Enum.Parse(typeof(ConditionEffectIndex), args[1].Trim(),
                                true),
                            DurationMS = -1
                        });
                        player.SendInfo("You threw " + args[1] + " at " + args[0] + "!");
                        i.Player.SendInfo(player.Name + " threw " + args[1] + " at you!");
                        return true;
                    }
                    else
                    {
                        i.Player.ApplyConditionEffect(new ConditionEffect
                        {
                            Effect = (ConditionEffectIndex)Enum.Parse(typeof(ConditionEffectIndex), args[1].Trim(),
                                true),
                            DurationMS = int.Parse(args[2]) * 1000
                        });
                        player.SendInfo("You threw " + args[1] + " at " + args[0] + " for " + args[2] + " seconds!");
                        i.Player.SendInfo(player.Name + " threw " + args[1] + " at you for " + args[2] + " seconds!");
                        return true;
                    }
                }
            }
            player.SendError(string.Format("Player '{0}' could not be found!", args));
            return false;
        }
    }

    internal class SizeCommand : Command
    {
        public SizeCommand() : base("size", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            player.Size = int.Parse(args[0]);
            player.UpdateCount++;
            player.SendInfo("Your size has been changed!");
            return true;
        }
    }

    internal class PetSizeCommand : Command
    {
        public PetSizeCommand()
            : base("PetSize", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /petsize <Pet Size>");
                    return false;
                }
                if (args.Length == 1)
                {
                    if (int.Parse(args[0]) > 200)
                    {
                        player.SendInfo("The size was too high!");
                    }
                    else
                    {
                        player.Pet.Size = int.Parse(args[0]);
                        player.UpdateCount++;
                        player.SendInfo("Success!");
                    }
                }
            }
            catch
            {
                player.SendError("Error!");
                return false;
            }
            return true;
        }
    }
	internal class SpawnOnCommand : Command    {
		public SpawnOnCommand()
			: base("spawnon", 6)
		{
		}


		protected override bool Process(Player player, RealmTime time, string[] args)
		{
			if (string.IsNullOrEmpty(args[0]))
			{
				player.SendHelp("Usage: /spawnon <Player> <Int> <Entity>");
				return false;
			}


			foreach (Client i in player.Manager.Clients.Values)
			{


				if (i.Account.Name.EqualsIgnoreCase(args[0]))
				{
					int num;
					if (args[1].Length > 0 && int.TryParse(args[1], out num)) //multi
					{
						string name = string.Join(" ", args.Skip(2).ToArray());
						ushort objType;
						Dictionary<string, ushort> icdatas = new Dictionary<string, ushort>(
							player.Manager.GameData.IdToObjectType,
							StringComparer.OrdinalIgnoreCase);
						if (!icdatas.TryGetValue(name, out objType) ||
							!player.Manager.GameData.ObjectDescs.ContainsKey(objType))
						{
							player.SendInfo("Unknown entity!");
							return false;
						}
						else
						{
							for (int u = 0; u < num; u++)
							{
								Entity entity = Entity.Resolve(player.Manager, objType);
								entity.Move(i.Player.X, i.Player.Y);
								i.Player.Owner.EnterWorld(entity);
							}
							player.SendInfo("Sent " + i.Player.Name + " " + string.Join(" ", args.Skip(1).ToArray()) + "'s");
							return true;
						}
					}
				}
				else
				{
					if (i.Account.Name.EqualsIgnoreCase(args[0]))
					{
						{
							string name = string.Join(" ", args.Skip(1).ToArray());
							ushort objType;
							Dictionary<string, ushort> icdatas = new Dictionary<string, ushort>(
								player.Manager.GameData.IdToObjectType,
								StringComparer.OrdinalIgnoreCase);
							if (!icdatas.TryGetValue(name, out objType) ||
								!player.Manager.GameData.ObjectDescs.ContainsKey(objType))
							{ }
							Entity entity = Entity.Resolve(player.Manager, objType);
							entity.Move(i.Player.X, i.Player.Y);
							i.Player.Owner.EnterWorld(entity);
						}
						player.SendInfo("Sent " + i.Player.Name + " " + string.Join(" ", args.Skip(1).ToArray()));
						return true;
					}
				}
			}
			return false;
		}

	}
    internal class VisitCommand : Command
    {
        public VisitCommand()
            : base("visit", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            foreach (KeyValuePair<string, Client> i in player.Manager.Clients)
                if (i.Value.Player.Owner is PetYard)
                {
                    player.SendInfo("You cant visit players in that world.");
                    return false;
                }
            foreach (KeyValuePair<string, Client> i in player.Manager.Clients)
            {
                if (i.Value.Player.Name.EqualsIgnoreCase(args[0]))
                {
                    Packet pkt;
                    if (i.Value.Player.Owner == player.Owner)
                    {
                        player.Move(i.Value.Player.X, i.Value.Player.Y);
                        pkt = new GotoPacket
                        {
                            ObjectId = player.Id,
                            Position = new Position(i.Value.Player.X, i.Value.Player.Y)
                        };
                        i.Value.Player.UpdateCount++;
                        player.SendInfo("He is here already. git fast.");
                    }
                    else
                    {
                        player.Client.Reconnect(new ReconnectPacket()
                        {
                            GameId = i.Value.Player.Owner.Id,
                            Host = "",
                            IsFromArena = false,
                            Key = Empty<byte>.Array,
                            KeyTime = -1,
                            Name = i.Value.Player.Owner.Name,
                            Port = -1,
                        });
                        player.SendInfo("You are visiting " + i.Value.Player.Owner.Id + " now");
                    }

                    return true;
                }
            }
            player.SendError(string.Format("Player '{0}' could not be found!", args));
            return false;
        }
    }

    internal class GodCommand : Command
    {
        public GodCommand()
            : base("god", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.HasConditionEffect(ConditionEffects.Invincible))
            {
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Invincible,
                    DurationMS = 0,
                });
                player.SendInfo("Godmode Deactivated");
                return false;
            }
            else
            {
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Invincible,
                    DurationMS = -1
                });
                player.SendInfo("Godmode Activated");
            }
            return true;
        }
    }

    internal class SpectateCommand : Command
    {
        public SpectateCommand()
            : base("spectate", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.HasConditionEffect(ConditionEffectIndex.Stasis))
            {
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Stasis,
                    DurationMS = 0,
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Stunned,
                    DurationMS = 0,
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Invisible,
                    DurationMS = 0,
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Invincible,
                    DurationMS = 0,
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Quiet,
                    DurationMS = 0,
                });

                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Speedy,
                    DurationMS = 0,
                });
                if (player.Pet != null)
                    player.Owner.EnterWorld(player.Pet);
                if (player.Pet != null)
                    player.Pet.Move(player.X, player.Y);
                player.SendInfo("You aren't spectating anymore!");
                return false;
            }
            else
            {
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Stasis,
                    DurationMS = -1
                });

                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Stunned,
                    DurationMS = -1
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Invisible,
                    DurationMS = -1
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Invincible,
                    DurationMS = -1
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Quiet,
                    DurationMS = -1
                });
                player.ApplyConditionEffect(new ConditionEffect()
                {
                    Effect = ConditionEffectIndex.Speedy,
                    DurationMS = -1
                });

                if (player.Pet != null)
                    player.Owner.LeaveWorld(player.Pet);
                player.SendInfo("You are now spectating!");
            }
            return true;
        }
    }

    internal class TestCommand : Command
    {
        public TestCommand()
            : base("t", 7)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            Entity en = Entity.Resolve(player.Manager, "Zombie Wizard");
            en.Move(player.X, player.Y);
            player.Owner.EnterWorld(en);
            player.UpdateCount++;
            //player.Client.SendPacket(new DeathPacket
            //{
            //    AccountId = player.AccountId,
            //    CharId = player.Client.Character.CharacterId,
            //    Killer = "mountains.beholder",
            //    obf0 = 10000,
            //    obf1 = 10000
            //});
            return true;
        }
    }

    internal class AddGiftCodeCommand : Command
    {
        public AddGiftCodeCommand()
            : base("gcode", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(args[0]))
                    player.Manager.FindPlayer(args[0])?.Client.GiftCodeReceived("LevelUp");
                else
                    player.Client.GiftCodeReceived("LevelUp");
            }
            catch (Exception)
            {
            }
            return true;
        }
    }

    internal class posCmd : Command
    {
        public posCmd()
            : base("p", 0)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            player.SendInfo("X: " + (int)player.X + " - Y: " + (int)player.Y);
            return true;
        }
    }

    internal class BanCommand : Command
    {
        public BanCommand() :
            base("ban", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            var p = player.Manager.FindPlayer(args[1]);
            if (p == null)
            {
                player.SendError("Player not found");
                return false;
            }
            player.Manager.Database.DoActionAsync(db =>
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "UPDATE accounts SET banned=1 WHERE id=@accId;";
                cmd.Parameters.AddWithValue("@accId", p.AccountId);
                cmd.ExecuteNonQuery();
            });
            return true;
        }
    }

    internal class AddWorldCommand : Command
    {
        public AddWorldCommand()
            : base("addworld", 6)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            Task.Factory.StartNew(() => GameWorld.AutoName(1, true))
                .ContinueWith(_ => player.Manager.AddWorld(_.Result), TaskScheduler.Default);
            return true;
        }
    }

    internal class GodLandsCommand : Command
    {
        public GodLandsCommand()
            : base("Gland", 0)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            {
                if (!(player.Owner is GameWorld))
                {
                    player.SendError("There isn't a gangster lands here.");
                    return false;
                }
            }
            {
                int x, y;
                try
                {
                    x = 1000;
                    y = 1000;
                }
                catch
                {
                    player.SendError("Invalid coordinates!");
                    return false;
                }
                player.Move(x + 0.5f, y + 0.5f);
                if (player.Pet != null)
                    player.Pet.Move(x + 0.5f, y + 0.5f);
                player.UpdateCount++;
                player.Owner.BroadcastPacket(new GotoPacket
                {
                    ObjectId = player.Id,
                    Position = new Position
                    {
                        X = player.X,
                        Y = player.Y
                    }
                }, null);
            }
            return true;
        }
    }

    internal class SpawnCommand : Command
    {
        public SpawnCommand()
            : base("spawn", 6)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Owner.Name == "Nexus")
            {
                player.SendError("No you silly goose!");
                return false;
            }

            int num;
            if (args.Length > 0 && int.TryParse(args[0], out num)) //multi
            {
                string name = string.Join(" ", args.Skip(1).ToArray());
                ushort objType;
                //creates a new case insensitive dictionary based on the XmlDatas
                Dictionary<string, ushort> icdatas = new Dictionary<string, ushort>(
                    player.Manager.GameData.IdToObjectType,
                    StringComparer.OrdinalIgnoreCase);
                if (name == "vault chest" || name == "Vault Chest")
                {
                    player.SendError("Cannot spawn this object!");
                    return false;
                }
                if (!icdatas.TryGetValue(name, out objType) ||
                    !player.Manager.GameData.ObjectDescs.ContainsKey(objType))
                {
                    player.SendInfo("Unknown entity!");
                    return false;
                }
                int c = int.Parse(args[0]);
                if (!(player.Client.Account.Rank > 2) && c > 200)
                {
                    player.SendError("Maximum spawn count is set to 200!");
                    return false;
                }
                if (player.Client.Account.Rank > 2 && c > 200)
                {
                    player.SendInfo("Bypass made!");
                }
                for (int i = 0; i < num; i++)
                {
                    Entity entity = Entity.Resolve(player.Manager, objType);
                    entity.Move(player.X, player.Y);
                    player.Owner.EnterWorld(entity);
                }
                player.SendInfo("Success!");
            }
            else
            {
                string name = string.Join(" ", args);
                ushort objType;
                //creates a new case insensitive dictionary based on the XmlDatas
                Dictionary<string, ushort> icdatas = new Dictionary<string, ushort>(
                    player.Manager.GameData.IdToObjectType,
                    StringComparer.OrdinalIgnoreCase);
                if (!icdatas.TryGetValue(name, out objType) ||
                    !player.Manager.GameData.ObjectDescs.ContainsKey(objType))
                {
                    player.SendHelp("Usage: /spawn <entityname>");
                    return false;
                }
                Entity entity = Entity.Resolve(player.Manager, objType);
                entity.Move(player.X, player.Y);
                player.Owner.EnterWorld(entity);
            }
            return true;
        }
    }

    internal class AddEffCommand : Command
    {
        public AddEffCommand()
            : base("addeff", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /addeff <Effectname or Effectnumber>");
                return false;
            }
            try
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = (ConditionEffectIndex)Enum.Parse(typeof(ConditionEffectIndex), args[0].Trim(), true),
                    DurationMS = -1
                });
                {
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Invalid effect!");
                return false;
            }
            return true;
        }
    }

    internal class RemoveEffCommand : Command
    {
        public RemoveEffCommand()
            : base("remeff", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /remeff <Effectname or Effectnumber>");
                return false;
            }
            try
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = (ConditionEffectIndex)Enum.Parse(typeof(ConditionEffectIndex), args[0].Trim(), true),
                    DurationMS = 0
                });
                player.SendInfo("Success!");
            }
            catch
            {
                player.SendError("Invalid effect!");
                return false;
            }
            return true;
        }
    }

    internal class GiveCommand : Command
    {
        public GiveCommand()
            : base("give", 6)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /give <Itemname>");
                return false;
            }
            string name = string.Join(" ", args.ToArray()).Trim();
            ushort objType;
            //creates a new case insensitive dictionary based on the XmlDatas
            Dictionary<string, ushort> icdatas = new Dictionary<string, ushort>(player.Manager.GameData.IdToObjectType,
                StringComparer.OrdinalIgnoreCase);
            if (!icdatas.TryGetValue(name, out objType))
            {
                player.SendError("Unknown type!");
                return false;
            }
            if (!player.Manager.GameData.Items[objType].Secret || player.Client.Account.Rank >= 4)
            {
                for (int i = 0; i < player.Inventory.Length; i++)
                    if (player.Inventory[i] == null)
                    {
                        player.Inventory[i] = player.Manager.GameData.Items[objType];
                        player.UpdateCount++;
                        player.SaveToCharacter();
                        player.SendInfo("Success!");
                        break;
                    }
            }
            else
            {
                player.SendError("Item cannot be given!");
                return false;
            }
            return true;
        }
    }

    internal class TpCommand : Command
    {
        public TpCommand()
            : base("tp", 6)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0 || args.Length == 1)
            {
                player.SendHelp("Usage: /tp <X coordinate> <Y coordinate>");
            }
            else
            {
                int x, y;
                try
                {
                    x = int.Parse(args[0]);
                    y = int.Parse(args[1]);
                }
                catch
                {
                    player.SendError("Invalid coordinates!");
                    return false;
                }
                player.Move(x + 0.5f, y + 0.5f);
                if (player.Pet != null)
                    player.Pet.Move(x + 0.5f, y + 0.5f);
                player.UpdateCount++;
                player.Owner.BroadcastPacket(new GotoPacket
                {
                    ObjectId = player.Id,
                    Position = new Position
                    {
                        X = player.X,
                        Y = player.Y
                    }
                }, null);
            }
            return true;
        }
    }

    internal class KillAll : Command
    {
        public KillAll()
            : base("killAll", 7)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Owner is Nexus)
            {
                player.SendInfo("You cant killall in this world.");
                return false;
            }
            var iterations = 0;
            var lastKilled = -1;
            var killed = 0;

            var mobName = args.Aggregate((s, a) => string.Concat(s, " ", a));
            while (killed != lastKilled)
            {
                lastKilled = killed;
                foreach (var i in player.Owner.Enemies.Values.Where(e =>
                    e.ObjectDesc?.ObjectId != null && e.ObjectDesc.ObjectId.ContainsIgnoreCase(mobName)))
                {
                    i.Death(time);
                    killed++;
                }
                if (++iterations >= 5)
                    break;
            }

            player.SendInfo($"{killed} enemy killed!");
            return true;
        }
    }

    internal class SetCommand : Command
    {
        public SetCommand()
            : base("setStat", 5)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 2)
            {
                try
                {
                    string stat = args[0].ToLower();
                    int amount = int.Parse(args[1]);
                    switch (stat)
                    {
                        case "health":
                        case "hp":
                            player.Stats[0] = amount;
                            break;

                        case "mana":
                        case "mp":
                            player.Stats[1] = amount;
                            break;

                        case "atk":
                        case "attack":
                            player.Stats[2] = amount;
                            break;

                        case "def":
                        case "defence":
                            player.Stats[3] = amount;
                            break;

                        case "spd":
                        case "speed":
                            player.Stats[4] = amount;
                            break;

                        case "vit":
                        case "vitality":
                            player.Stats[5] = amount;
                            break;

                        case "wis":
                        case "wisdom":
                            player.Stats[6] = amount;
                            break;

                        case "dex":
                        case "dexterity":
                            player.Stats[7] = amount;
                            break;

						case "mgt":
						case "might":
							player.Stats[8] = amount;
							break;

						case "luc":
						case "luck":
							player.Stats[9] = amount;
							break;

                        default:
                            player.SendError("Invalid Stat");
                            player.SendHelp("Stats: Health, Mana, Attack, Defence, Speed, Vitality, Wisdom, Dexterity, Might, Luck");
                            player.SendHelp("Shortcuts: Hp, Mp, Atk, Def, Spd, Vit, Wis, Dex, Mgt, Luc");
                            return false;
                    }
                    player.SaveToCharacter();
                    player.Client.Save();
                    player.UpdateCount++;
                    player.SendInfo("Success");
                }
                catch
                {
                    player.SendError("Error while setting stat");
                    return false;
                }
                return true;
            }
            else if (args.Length == 3)
            {
                foreach (Client i in player.Manager.Clients.Values)
                {
                    if (i.Account.Name.EqualsIgnoreCase(args[0]))
                    {
                        try
                        {
                            string stat = args[1].ToLower();
                            int amount = int.Parse(args[2]);
                            switch (stat)
                            {
                                case "health":
                                case "hp":
                                    i.Player.Stats[0] = amount;
                                    break;

                                case "mana":
                                case "mp":
                                    i.Player.Stats[1] = amount;
                                    break;

                                case "atk":
                                case "attack":
                                    i.Player.Stats[2] = amount;
                                    break;

                                case "def":
                                case "defence":
                                    i.Player.Stats[3] = amount;
                                    break;

                                case "spd":
                                case "speed":
                                    i.Player.Stats[4] = amount;
                                    break;

                                case "vit":
                                case "vitality":
                                    i.Player.Stats[5] = amount;
                                    break;

                                case "wis":
                                case "wisdom":
                                    i.Player.Stats[6] = amount;
                                    break;

                                case "dex":
                                case "dexterity":
                                    i.Player.Stats[7] = amount;
                                    break;

                                default:
                                    player.SendError("Invalid Stat");
                                    player.SendHelp(
                                        "Stats: Health, Mana, Attack, Defence, Speed, Vitality, Wisdom, Dexterity");
                                    player.SendHelp("Shortcuts: Hp, Mp, Atk, Def, Spd, Vit, Wis, Dex");
                                    return false;
                            }
                            i.Player.SaveToCharacter();
                            i.Player.Client.Save();
                            i.Player.UpdateCount++;
                            player.SendInfo("Success");
                        }
                        catch
                        {
                            player.SendError("Error while setting stat");
                            return false;
                        }
                        return true;
                    }
                }
                player.SendError(string.Format("Player '{0}' could not be found!", args));
                return false;
            }
            else
            {
                player.SendHelp("Usage: /setStat <Stat> <Amount>");
                player.SendHelp("or");
                player.SendHelp("Usage: /setStat <Player> <Stat> <Amount>");
                player.SendHelp("Shortcuts: Hp, Mp, Atk, Def, Spd, Vit, Wis, Dex");
                return false;
            }
        }
    }

    internal class SetGoldCommand : Command
    {
        public SetGoldCommand() : base("setgold", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (string.IsNullOrEmpty(args[0]))
            {
                player.SendHelp("Usage: /setgold <gold>");
                return false;
            }
            player.Manager.Database.DoActionAsync(db =>
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "UPDATE `stats` SET `credits`=@cre WHERE accId=@accId";
                cmd.Parameters.AddWithValue("@cre", args[0]);
                cmd.Parameters.AddWithValue("@accId", player.AccountId);
                if (cmd.ExecuteNonQuery() == 0)
                {
                    player.SendError("Error setting gold!");
                }
                else
                {
                    player.SendInfo("Success!");
                }
            });
            return true;
        }
    }

    internal class SetELevel : Command
    {
        public SetELevel() : base("setelevel", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (string.IsNullOrEmpty(args[0]))
            {
                player.SendHelp("Usage: /setelevel <enchantmentlevel>");
                return false;
            }
            player.Manager.Database.DoActionAsync(db =>
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "UPDATE `accounts` SET `enchantmentLevel`=@ele WHERE name=@name";
                cmd.Parameters.AddWithValue("@ele", args[0]);
                cmd.Parameters.AddWithValue("@name", player.Name);
                if (cmd.ExecuteNonQuery() == 0)
                {
                    player.SendError("Error setting enchantment level!");
                }
                else
                {
                    player.SendInfo("Success, setting Enchantment Level to " + args[0]);
                }
            });
            return true;
        }
    }

    internal class Kick : Command
    {
        public Kick()
            : base("kick", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /kick <playername>");
                return false;
            }
            try
            {
                foreach (KeyValuePair<int, Player> i in player.Owner.Players)
                {
                    if (i.Value.Name.ToLower() == args[0].ToLower().Trim())
                    {
                        player.SendInfo("Player Disconnected");
                        i.Value.Client.Disconnect();
                    }
                }
            }
            catch
            {
                player.SendError("Cannot kick!");
                return false;
            }
            return true;
        }
    }

    internal class Mute : Command
    {
        public Mute()
            : base("mute", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /mute <playername>");
                return false;
            }
            try
            {
                foreach (KeyValuePair<int, Player> i in player.Owner.Players)
                {
                    if (i.Value.Name.ToLower() == args[0].ToLower().Trim())
                    {
                        i.Value.Muted = true;
                        i.Value.Manager.Database.DoActionAsync(db => db.MuteAccount(i.Value.AccountId));
                        player.SendInfo("Player Muted.");
                    }
                }
            }
            catch
            {
                player.SendError("Cannot mute!");
                return false;
            }
            return true;
        }
    }

    internal class Max : Command
    {
        public Max()
            : base("max", 5)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                player.Stats[0] = player.ObjectDesc.MaxHitPoints;
                player.Stats[1] = player.ObjectDesc.MaxMagicPoints;
                player.Stats[2] = player.ObjectDesc.MaxAttack;
                player.Stats[3] = player.ObjectDesc.MaxDefense;
                player.Stats[4] = player.ObjectDesc.MaxSpeed;
                player.Stats[5] = player.ObjectDesc.MaxHpRegen;
                player.Stats[6] = player.ObjectDesc.MaxMpRegen;
                player.Stats[7] = player.ObjectDesc.MaxDexterity;
				player.Stats[8] = player.ObjectDesc.MaxMight;
				player.Stats[9] = player.ObjectDesc.MaxLuck;
                player.SaveToCharacter();
                player.Client.Save();
                player.UpdateCount++;
                player.SendInfo("Success");
            }
            catch
            {
                player.SendError("Error while maxing stats");
                return false;
            }
            return true;
        }
    }
	internal class CheckML : Command
	{
		public CheckML()
			: base("cmlzz", 5)
		{
		}

		protected override bool Process(Player player, RealmTime time, string[] args)
		{
			try
			{
				player.SendInfo(" Might:" + player.Boost[8] + " Luck:" + player.Boost[9] + " Wisdom:" + player.Boost[6]);
			}
			catch
			{
				player.SendError("Error while maxing stats");
				return false;
			}
			return true;
		}
	}

    internal class UnMute : Command
    {
        public UnMute()
            : base("unmute", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /unmute <playername>");
                return false;
            }
            try
            {
                foreach (KeyValuePair<int, Player> i in player.Owner.Players)
                {
                    if (i.Value.Name.ToLower() == args[0].ToLower().Trim())
                    {
                        i.Value.Muted = true;
                        i.Value.Manager.Database.DoActionAsync(db => db.UnmuteAccount(i.Value.AccountId));
                        player.SendInfo("Player Unmuted.");
                    }
                }
            }
            catch
            {
                player.SendError("Cannot unmute!");
                return false;
            }
            return true;
        }
    }

    internal class OryxSay : Command
    {
        public OryxSay()
            : base("osay", 3)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /oryxsay <saytext>");
                return false;
            }
            string saytext = string.Join(" ", args);
            player.SendEnemy("Oryx the Mad God", saytext);
            return true;
        }
    }

    internal class SWhoCommand : Command //get all players from all worlds (this may become too large!)
    {
        public SWhoCommand()
            : base("swho", 0)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            StringBuilder sb = new StringBuilder("All conplayers: ");

            foreach (KeyValuePair<int, World> w in player.Manager.Worlds)
            {
                World world = w.Value;
                if (w.Key != 0)
                {
                    Player[] copy = world.Players.Values.ToArray();
                    if (copy.Length != 0)
                    {
                        for (int i = 0; i < copy.Length; i++)
                        {
                            sb.Append(copy[i].Name);
                            sb.Append(", ");
                        }
                    }
                }
            }
            string fixedString = sb.ToString().TrimEnd(',', ' '); //clean up trailing ", "s

            player.SendInfo(fixedString);
            return true;
        }
    }

    internal class Announcement : Command
    {
        public Announcement()
            : base("a", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (args.Length == 0)
            {
                player.SendHelp("Usage: /announce <saytext>");
                return false;
            }
            string saytext = string.Join(" ", args);

            foreach (Client i in player.Manager.Clients.Values)
            {
                i.SendPacket(new TextPacket
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "@ANNOUNCEMENT",
                    Text = " " + saytext
                });
            }
            return true;
        }
    }

    internal class Summon : Command
    {
        public Summon()
            : base("summon", 5)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Owner is Vault || player.Owner is PetYard)
            {
                player.SendInfo("You cant summon in this world.");
                return false;
            }
            foreach (KeyValuePair<string, Client> i in player.Manager.Clients)
            {
                if (i.Value.Player.Name.EqualsIgnoreCase(args[0]))
                {
                    Packet pkt;
                    if (i.Value.Player.Owner == player.Owner)
                    {
                        i.Value.Player.Move(player.X, player.Y);
                        pkt = new GotoPacket
                        {
                            ObjectId = i.Value.Player.Id,
                            Position = new Position(player.X, player.Y)
                        };
                        i.Value.Player.UpdateCount++;
                        player.SendInfo("Player summoned!");
                    }
                    else
                    {
                        pkt = new ReconnectPacket
                        {
                            GameId = player.Owner.Id,
                            Host = "",
                            IsFromArena = false,
                            Key = player.Owner.PortalKey,
                            KeyTime = -1,
                            Name = player.Owner.Name,
                            Port = -1
                        };
                        player.SendInfo("Player will connect to you now!");
                    }

                    i.Value.SendPacket(pkt);

                    return true;
                }
            }
            player.SendError(string.Format("Player '{0}' could not be found!", args));
            return false;
        }
    }

    internal class KillPlayerCommand : Command
    {
        public KillPlayerCommand()
            : base("kill", 7)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            foreach (Client i in player.Manager.Clients.Values)
            {
                if (i.Account.Name.EqualsIgnoreCase(args[0]))
                {
                    i.Player.HP = 0;
                    i.Player.Death("Admin");
                    player.SendInfo("Player killed!");
                    return true;
                }
            }
            player.SendError(string.Format("Player '{0}' could not be found!", args));
            return false;
        }
    }

    internal class RestartCommand : Command
    {
        public RestartCommand()
            : base("restart", 7)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                foreach (KeyValuePair<int, World> w in player.Manager.Worlds)
                {
                    World world = w.Value;
                    if (w.Key != 0)
                    {
                        world.BroadcastPacket(new TextPacket
                        {
                            Name = "@ANNOUNCEMENT",
                            Stars = -1,
                            BubbleTime = 0,
                            Text =
                                "Server restarting soon. Please be ready to disconnect. Estimated server down time: 30 Seconds - 1 Minute"
                        }, null);
                    }
                }
            }
            catch
            {
                player.SendError("Cannot say that in announcement!");
                return false;
            }
            return true;
        }
    }

    //class VitalityCommand : ICommand
    //{
    //    public string Command { get { return "vit"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /vit <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[5] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class DefenseCommand : ICommand
    //{
    //    public string Command { get { return "def"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /def <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[3] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class AttackCommand : ICommand
    //{
    //    public string Command { get { return "att"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /att <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[2] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class DexterityCommand : ICommand
    //{
    //    public string Command { get { return "dex"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /dex <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[7] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class LifeCommand : ICommand
    //{
    //    public string Command { get { return "hp"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /hp <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[0] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class ManaCommand : ICommand
    //{
    //    public string Command { get { return "mp"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /mp <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[1] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class SpeedCommand : ICommand
    //{
    //    public string Command { get { return "spd"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /spd <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[4] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class WisdomCommand : ICommand
    //{
    //    public string Command { get { return "wis"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /spd <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stats[6] = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    //class Ban : ICommand
    //{
    //    public string Command { get { return "ban"; } }
    //    public int RequiredRank { get { return 4; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        if (args.Length == 0)
    //        {
    //            player.SendHelp("Usage: /ban <username>");
    //        }
    //        try
    //        {
    //            using (Database dbx = new Database())
    //            {
    //                var cmd = dbx.CreateQuery();
    //                cmd.CommandText = "UPDATE accounts SET banned=1, rank=0 WHERE name=@name";
    //                cmd.Parameters.AddWithValue("@name", args[0]);
    //                if (cmd.ExecuteNonQuery() == 0)
    //                {
    //                    player.SendInfo("Could not ban");
    //                }
    //                else
    //                {
    //                    foreach (var i in player.Owner.Players)
    //                    {
    //                        if (i.Value.Name.ToLower() == args[0].ToLower().Trim())
    //                        {
    //                            i.Value.Client.Disconnect();
    //                            player.SendInfo("Account successfully Banned");
    //                            log.InfoFormat(args[0] + " was Banned.");
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        catch
    //        {
    //            player.SendInfo("Could not ban");
    //        }
    //    }
    //}

    //class UnBan : ICommand
    //{
    //    public string Command { get { return "unban"; } }
    //    public int RequiredRank { get { return 4; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        if (args.Length == 0)
    //        {
    //            player.SendHelp("Usage: /unban <username>");
    //        }
    //        try
    //        {
    //            using (Database dbx = new Database())
    //            {
    //                var cmd = dbx.CreateQuery();
    //                cmd.CommandText = "UPDATE accounts SET banned=0, rank=1 WHERE name=@name";
    //                cmd.Parameters.AddWithValue("@name", args[0]);
    //                if (cmd.ExecuteNonQuery() == 0)
    //                {
    //                    player.SendInfo("Could not unban");
    //                }
    //                else
    //                {
    //                    player.SendInfo("Account successfully Unbanned");
    //                    log.InfoFormat(args[1] + " was Unbanned.");

    //                }
    //            }
    //        }
    //        catch
    //        {
    //            player.SendInfo("Could not unban, please unban in database");
    //        }
    //    }
    //}

    //class Rank : ICommand
    //{
    //    public string Command { get { return "rank"; } }
    //    public int RequiredRank { get { return 4; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        if (args.Length < 2)
    //        {
    //            player.SendHelp("Usage: /rank <username> <number>\n0: Player\n1: Donator\n2: Game Master\n3: Developer\n4: Head Developer\n5: Admin");
    //        }
    //        else
    //        {
    //            try
    //            {
    //                using (Database dbx = new Database())
    //                {
    //                    var cmd = dbx.CreateQuery();
    //                    cmd.CommandText = "UPDATE accounts SET rank=@rank WHERE name=@name";
    //                    cmd.Parameters.AddWithValue("@rank", args[1]);
    //                    cmd.Parameters.AddWithValue("@name", args[0]);
    //                    if (cmd.ExecuteNonQuery() == 0)
    //                    {
    //                        player.SendInfo("Could not change rank");
    //                    }
    //                    else
    //                        player.SendInfo("Account rank successfully changed");
    //                }
    //            }
    //            catch
    //            {
    //                player.SendInfo("Could not change rank, please change rank in database");
    //            }
    //        }
    //    }
    //}
    //class GuildRank : ICommand
    //{
    //    public string Command { get { return "grank"; } }
    //    public int RequiredRank { get { return 4; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        if (args.Length < 2)
    //        {
    //            player.SendHelp("Usage: /grank <username> <number>");
    //        }
    //        else
    //        {
    //            try
    //            {
    //                using (Database dbx = new Database())
    //                {
    //                    var cmd = dbx.CreateQuery();
    //                    cmd.CommandText = "UPDATE accounts SET guildRank=@guildRank WHERE name=@name";
    //                    cmd.Parameters.AddWithValue("@guildRank", args[1]);
    //                    cmd.Parameters.AddWithValue("@name", args[0]);
    //                    if (cmd.ExecuteNonQuery() == 0)
    //                    {
    //                        player.SendInfo("Could not change guild rank. Use 10, 20, 30, 40, or 50 (invisible)");
    //                    }
    //                    else
    //                        player.SendInfo("Guild rank successfully changed");
    //                    log.InfoFormat(args[1] + "'s guild rank has been changed");
    //                }
    //            }
    //            catch
    //            {
    //                player.SendInfo("Could not change rank, please change rank in database");
    //            }
    //        }
    //    }
    //}
    //class ChangeGuild : ICommand
    //{
    //    public string Command { get { return "setguild"; } }
    //    public int RequiredRank { get { return 4; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        if (args.Length < 2)
    //        {
    //            player.SendHelp("Usage: /setguild <username> <guild id>");
    //        }
    //        else
    //        {
    //            try
    //            {
    //                using (Database dbx = new Database())
    //                {
    //                    var cmd = dbx.CreateQuery();
    //                    cmd.CommandText = "UPDATE accounts SET guild=@guild WHERE name=@name";
    //                    cmd.Parameters.AddWithValue("@guild", args[1]);
    //                    cmd.Parameters.AddWithValue("@name", args[0]);
    //                    if (cmd.ExecuteNonQuery() == 0)
    //                    {
    //                        player.SendInfo("Could not change guild.");
    //                    }
    //                    else
    //                        player.SendInfo("Guild successfully changed");
    //                    log.InfoFormat(args[1] + "'s guild has been changed");
    //                }
    //            }
    //            catch
    //            {
    //                player.SendInfo("Could not change guild, please change in database.                                Use /setguild <username> <guild id>");
    //            }
    //        }
    //    }
    //}
    internal class Lefttomax : Command
    {
        public Lefttomax()
            : base("lefttomax", 0)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            {
                player.SendInfo("Your level " + player.Level + " " + player.ObjectDesc.ObjectId + " needs");
                if (((player.ObjectDesc.MaxHitPoints - player.Stats[0]) / 5) * 5 <=
                    ((player.ObjectDesc.MaxHitPoints - player.Stats[0]) / 5))
                {
                    player.SendInfo((player.ObjectDesc.MaxHitPoints - player.Stats[0]) + " (" +
                                    (((player.ObjectDesc.MaxHitPoints - player.Stats[0])) / 5) + ") to max health");
                }
                else if (((player.ObjectDesc.MaxHitPoints - player.Stats[0]) / 5) * 5 >=
                         ((player.ObjectDesc.MaxHitPoints - player.Stats[0]) / 5))
                {
                    player.SendInfo((player.ObjectDesc.MaxHitPoints - player.Stats[0]) + " (" +
                                    (((player.ObjectDesc.MaxHitPoints - player.Stats[0])) / 5 + 1) + ") to max health");
                }
                if (((player.ObjectDesc.MaxMagicPoints - player.Stats[1]) / 5) * 5 <=
                    ((player.ObjectDesc.MaxMagicPoints - player.Stats[1]) / 5))
                {
                    player.SendInfo((player.ObjectDesc.MaxMagicPoints - player.Stats[1]) + " (" +
                                    (((player.ObjectDesc.MaxMagicPoints - player.Stats[1])) / 5) + ") to max mana");
                }
                else if (((player.ObjectDesc.MaxMagicPoints - player.Stats[1]) / 5) * 5 >=
                         ((player.ObjectDesc.MaxMagicPoints - player.Stats[1]) / 5))
                {
                    player.SendInfo((player.ObjectDesc.MaxMagicPoints - player.Stats[1]) + " (" +
                                    (((player.ObjectDesc.MaxMagicPoints - player.Stats[1])) / 5 + 1) + ") to max mana");
                }
                player.SendInfo((player.ObjectDesc.MaxAttack - player.Stats[2]) + " (" +
                                (((player.ObjectDesc.MaxAttack - player.Stats[2]))) + ") to max attack");
                player.SendInfo((player.ObjectDesc.MaxDefense - player.Stats[3]) + " (" +
                                (((player.ObjectDesc.MaxDefense - player.Stats[3]))) + ") to max defense");
                player.SendInfo((player.ObjectDesc.MaxSpeed - player.Stats[4]) + " (" +
                                (((player.ObjectDesc.MaxSpeed - player.Stats[4]))) + ") to max speed");
                player.SendInfo((player.ObjectDesc.MaxHpRegen - player.Stats[5]) + " (" +
                                (((player.ObjectDesc.MaxHpRegen - player.Stats[5]))) + ") to max vitality");
                player.SendInfo((player.ObjectDesc.MaxMpRegen - player.Stats[6]) + " (" +
                                (((player.ObjectDesc.MaxMpRegen - player.Stats[6]))) + ") to max wisdom");
                player.SendInfo((player.ObjectDesc.MaxDexterity - player.Stats[7]) + " (" +
                                (((player.ObjectDesc.MaxDexterity - player.Stats[7]))) + ") to max dexterity");
				player.SendInfo((player.ObjectDesc.MaxMight - player.Stats[8]) + " (" +
								(((player.ObjectDesc.MaxMight - player.Stats[8]))) + ") to max might");
				player.SendInfo((player.ObjectDesc.MaxLuck - player.Stats[9]) + " (" +
								(((player.ObjectDesc.MaxLuck - player.Stats[9]))) + ") to max luck");
            }
            return true;
        }
    }

    internal class tpall : Command
    {
        public tpall()
            : base("tpall", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            foreach (Client i in player.Manager.Clients.Values)
            {
                i.SendPacket(new GotoPacket
                {
                    ObjectId = i.Player.Id,
                    Position = new Position(player.X, player.Y)
                });
                player.SendInfo("Teleporting Players");
            }
            return true;
        }
    }

    internal class summonall : Command
    {
        public summonall()
            : base("summonall", 5)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Owner is Vault || player.Owner is PetYard)
            {
                player.SendInfo("You cant summon in this world.");
                return false;
            }
            foreach (Client i in player.Manager.Clients.Values)
            {
                i.SendPacket(new ReconnectPacket
                {
                    GameId = player.Owner.Id,
                    Host = "",
                    IsFromArena = false,
                    Key = Empty<byte>.Array,
                    KeyTime = -1,
                    Name = player.Owner.Name,
                    Port = -1
                });
                player.SendInfo("CONNECTING PLAYERS!");
            }
            return true;
        }
    }

    internal class TqCommand : Command
    {
        public TqCommand()
            : base("tq", 4)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Quest == null)
            {
                player.SendError("Player does not have a quest!");
                return false;
            }
            player.Move(player.Quest.X + 0.5f, player.Quest.Y + 0.5f);
            if (player.Pet != null)
                player.Pet.Move(player.Quest.X + 0.5f, player.Quest.Y + 0.5f);
            player.UpdateCount++;
            player.Owner.BroadcastPacket(new GotoPacket
            {
                ObjectId = player.Id,
                Position = new Position
                {
                    X = player.Quest.X,
                    Y = player.Quest.Y
                }
            }, null);
            player.SendInfo("Success!");
            return true;
        }
    }

    //class GodMode : ICommand
    //{
    //    public string Command { get { return "god"; } }
    //    public int RequiredRank { get { return 3; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        if (player.HasConditionEffect(ConditionEffects.Invincible))
    //        {
    //            player.ApplyConditionEffect(new ConditionEffect()
    //            {
    //                Effect = ConditionEffectIndex.Invincible,
    //                DurationMS = 0
    //            });
    //            player.SendInfo("Godmode Off");
    //        }
    //        else
    //        {
    //            player.ApplyConditionEffect(new ConditionEffect()
    //            {
    //                Effect = ConditionEffectIndex.Invincible,
    //                DurationMS = -1
    //            });
    //            player.SendInfo("Godmode On");
    //        }
    //    }
    //}
    //class StarCommand : ICommand
    //{
    //    public string Command { get { return "stars"; } }
    //    public int RequiredRank { get { return 2; } }

    //    protected override bool Process(Player player, RealmTime time, string[] args)
    //    {
    //        try
    //        {
    //            if (args.Length == 0)
    //            {
    //                player.SendHelp("Use /stars <ammount>");
    //            }
    //            else if (args.Length == 1)
    //            {
    //                player.Client.Player.Stars = int.Parse(args[0]);
    //                player.UpdateCount++;
    //                player.SendInfo("Success!");
    //            }
    //        }
    //        catch
    //        {
    //            player.SendError("Error!");
    //        }
    //    }
    //}

    internal class LevelCommand : Command
    {
        public LevelCommand()
            : base("level", 6)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    player.SendHelp("Use /level <ammount>");
                    return false;
                }
                if (args.Length == 1)
                {
                    player.Client.Character.Level = int.Parse(args[0]);
                    player.Client.Player.Level = int.Parse(args[0]);
                    player.UpdateCount++;
                    player.SendInfo("Success!");
                }
            }
            catch
            {
                player.SendError("Error!");
                return false;
            }
            return true;
        }
    }

    internal class SetpieceCommand : Command
    {
        public SetpieceCommand()
            : base("setpiece", 7)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.Owner.Name == "Nexus")
            {
                player.SendError("You aren't able add a setpiece in this area.");
                return false;
            }

            ISetPiece piece = (ISetPiece)Activator.CreateInstance(Type.GetType(
                "wServer.realm.setpieces." + args[0], true, true));
            piece.RenderSetPiece(player.Owner, new IntPoint((int)player.X + 1, (int)player.Y + 1));
            return true;
        }
    }

    internal class ArenaCommand : Command
    {
        public ArenaCommand()
            : base("sarena", 5)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            Entity entity = Entity.Resolve(player.Manager, 0x47a9);
            World we = player.Manager.GetWorld(player.Owner.Id); //can't use Owner here, as it goes out of scope
            int TimeoutTime = player.Manager.GameData.Portals[0x47a9].TimeoutTime;
            string DungName = player.Manager.GameData.Portals[0x47a9].DungeonName;

            entity.Move(player.X, player.Y);
            we.EnterWorld(entity);

            ARGB c = new ARGB(0xFF00FF);

            TextPacket packet = new TextPacket
            {
                BubbleTime = 0,
                Stars = -1,
                Name = "",
                Text = "A spawn arena has been opened by " + player.Name
            };
            player.Owner.BroadcastPacket(packet, null);
            we.Timers.Add(new WorldTimer(TimeoutTime * 1000,
                (world, t) => //default portal close time * 1000
                {
                    try
                    {
                        we.LeaveWorld(entity);
                    }
                    catch (Exception ex)
                    //couldn't remove portal, Owner became null. Should be fixed with RealmManager implementation
                    {
                        log.ErrorFormat("Couldn't despawn portal.\n{0}", ex);
                    }
                }));
            return true;
        }
    }

    internal class ListCommands : Command
    {
        public ListCommands() : base("commands", permLevel: 1)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            Dictionary<string, Command> cmds = new Dictionary<string, Command>();
            Type t = typeof(Command);
            foreach (Type i in t.Assembly.GetTypes())
                if (t.IsAssignableFrom(i) && i != t)
                {
                    Command instance = (Command)Activator.CreateInstance(i);
                    cmds.Add(instance.CommandName, instance);
                }
            StringBuilder sb = new StringBuilder("");
            Command[] copy = cmds.Values.ToArray();
            for (int i = 0; i < copy.Length; i++)
            {
                if (i != 0)
                    sb.Append(", ");
                sb.Append(copy[i].CommandName);
            }

            player.SendInfo(sb.ToString());
            return true;
        }
    }
}