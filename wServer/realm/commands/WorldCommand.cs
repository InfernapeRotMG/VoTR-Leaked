#region

using db;
using db.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using wServer.realm.entities;
using wServer.realm.entities.player;

#endregion

namespace wServer.realm.commands
{
    internal class TutorialCommand : Command
    {
        public TutorialCommand()
            : base("tutorial")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            player.Client.Reconnect(new ReconnectPacket
            {
                Host = "",
                Port = Program.Settings.GetValue<int>("port"),
                GameId = World.TUT_ID,
                Name = "Tutorial",
                Key = Empty<byte>.Array,
            });
            return true;
        }
    }

    internal class TradeCommand : Command
    {
        public TradeCommand()
            : base("trade")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (String.IsNullOrWhiteSpace(args[0]))
            {
                player.SendInfo("Usage: /trade <player name>");
                return false;
            }
            player.RequestTrade(time, new RequestTradePacket
            {
                Name = args[0]
            });
            return true;
        }
    }

    internal class WhoCommand : Command
    {
        public WhoCommand()
            : base("who")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            StringBuilder sb = new StringBuilder("Players online: ");
            Player[] copy = player.Owner.Players.Values.ToArray();
            for (int i = 0; i < copy.Length; i++)
            {
                if (i != 0)
                    sb.Append(", ");
                sb.Append(copy[i].Name);
            }

            player.SendInfo(sb.ToString());
            return true;
        }
    }

    internal class ServerCommand : Command
    {
        public ServerCommand()
            : base("server")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            player.SendInfo(player.Owner.Name);
            return true;
        }
    }

    internal class PauseCommand : Command
    {
        public PauseCommand()
            : base("pause")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (player.HasConditionEffect(ConditionEffects.Paused))
            {
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Paused,
                    DurationMS = 0
                });
                player.SendInfo("Game resumed.");
            }
            else
            {
                foreach (Enemy i in player.Owner.EnemiesCollision.HitTest(player.X, player.Y, 8).OfType<Enemy>())
                {
                    if (i.ObjectDesc.Enemy)
                    {
                        player.SendInfo("Not safe to pause.");
                        return false;
                    }
                }
                player.ApplyConditionEffect(new ConditionEffect
                {
                    Effect = ConditionEffectIndex.Paused,
                    DurationMS = -1
                });
                player.SendInfo("Game paused.");
            }
            return true;
        }
    }

    internal class TeleportCommand : Command
    {
        public TeleportCommand()
            : base("teleport")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            try
            {
                if (String.Equals(player.Name.ToLower(), args[0].ToLower()))
                {
                    player.SendInfo("You are already at yourself, and always will be!");
                    return false;
                }

                foreach (KeyValuePair<int, Player> i in player.Owner.Players)
                {
                    if (i.Value.Name.ToLower() == args[0].ToLower().Trim())
                    {
                        player.Teleport(time, new TeleportPacket
                        {
                            ObjectId = i.Value.Id
                        });
                        return true;
                    }
                }
                player.SendInfo(string.Format("Cannot teleport, {0} not found!", args[0].Trim()));
            }
            catch
            {
                player.SendHelp("Usage: /teleport <player name>");
            }
            return false;
        }
    }

    internal class CheckELevel : Command
    {
        public CheckELevel()
            : base("checkelevel", 0)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            player.Manager.Database.DoActionAsync(db =>
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "SELECT name FROM accounts WHERE enchantmentLevel=@ele;";
                cmd.Parameters.AddWithValue("@ele", player.EnchantmentLevel);
                if (player.EnchantmentLevel > 0 && player.EnchantmentLevel <= 6)
                {
                    player.SendInfo("Your enchantment level is " + player.EnchantmentLevel + " at Basic Enchanting.");
                }
                else if (player.EnchantmentLevel > 6 && player.EnchantmentLevel <= 12)
                {
                    player.SendInfo("Your enchantment level is " + player.EnchantmentLevel + " at Novice Enchanting.");
                }
                else if (player.EnchantmentLevel > 12 && player.EnchantmentLevel <= 18)
                {
                    player.SendInfo("Your enchantment level is " + player.EnchantmentLevel + " at Advanced Enchanting.");
                }
                else if (player.EnchantmentLevel > 18)
                {
                    player.SendInfo("Your enchantment level is " + player.EnchantmentLevel + " at Mastery Enchanting.");
                }
            });
            return true;
        }
    }

    internal class CheckOAmount : Command
    {
        public CheckOAmount()
            : base("onraneamount", 0)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            player.Manager.Database.DoActionAsync(db =>
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "SELECT onraneCurrency FROM stats WHERE accId=@accId;";
                cmd.Parameters.AddWithValue("@oc", player.Onrane);
                player.SendInfo("The current amount of Onrane you have is " + player.Onrane + ".");
            });
            return true;
        }
    }

    internal class TellCommand : Command
    {
        public TellCommand() : base("tell")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            if (!player.NameChosen)
            {
                player.SendError("Choose a name!");
                return false;
            }
            if (args.Length < 2)
            {
                player.SendError("Usage: /tell <player name> <text>");
                return false;
            }

            string playername = args[0].Trim();
            string msg = string.Join(" ", args, 1, args.Length - 1);

            if (String.Equals(player.Name.ToLower(), playername.ToLower()))
            {
                player.SendInfo("Quit telling yourself!");
                return false;
            }

            if (playername.ToLower() == "muledump")
            {
                if (msg.ToLower() == "private muledump")
                {
                    player.Client.SendPacket(new TextPacket() //echo to self
                    {
                        ObjectId = player.Id,
                        BubbleTime = 10,
                        Stars = player.Stars,
                        StarType = player.StarType,
                        Name = player.Name,
                        Recipient = "Muledump",
                        Text = msg.ToSafeText(),
                        CleanText = ""
                    });

                    player.Manager.Database.DoActionAsync(db =>
                    {
                        var cmd = db.CreateQuery();
                        cmd.CommandText = "UPDATE accounts SET publicMuledump=0 WHERE id=@accId;";
                        cmd.Parameters.AddWithValue("@accId", player.AccountId);
                        cmd.ExecuteNonQuery();
                        player.Client.SendPacket(new TextPacket()
                        {
                            ObjectId = -1,
                            BubbleTime = 10,
                            Stars = 70,
                            Name = "Muledump",
                            Recipient = player.Name,
                            Text = "Your muledump is now hidden, only you can view it now.",
                            CleanText = ""
                        });
                    });
                }
                else if (msg.ToLower() == "public muledump")
                {
                    player.Client.SendPacket(new TextPacket() //echo to self
                    {
                        ObjectId = player.Id,
                        BubbleTime = 10,
                        Stars = player.Stars,
                        StarType = player.StarType,
                        Name = player.Name,
                        Recipient = "Muledump",
                        Text = msg.ToSafeText(),
                        CleanText = ""
                    });
                    player.Manager.Database.DoActionAsync(db =>
                    {
                        var cmd = db.CreateQuery();
                        cmd.CommandText = "UPDATE accounts SET publicMuledump=1 WHERE id=@accId;";
                        cmd.Parameters.AddWithValue("@accId", player.AccountId);
                        cmd.ExecuteNonQuery();

                        player.Client.SendPacket(new TextPacket()
                        {
                            ObjectId = -1,
                            BubbleTime = 10,
                            Stars = 70,
                            Name = "Muledump",
                            Recipient = player.Name,
                            Text = "Your muledump is now public, anyone can view it now.",
                            CleanText = ""
                        });
                    });
                }
                else
                {
                    player.Client.SendPacket(new TextPacket() //echo to self
                    {
                        ObjectId = player.Id,
                        BubbleTime = 10,
                        Stars = player.Stars,
                        StarType = player.StarType,
                        Name = player.Name,
                        Recipient = "Muledump",
                        Text = msg.ToSafeText(),
                        CleanText = ""
                    });

                    player.Client.SendPacket(new TextPacket()
                    {
                        ObjectId = -1,
                        BubbleTime = 10,
                        Stars = 70,
                        Name = "Muledump",
                        Recipient = player.Name,
                        Text = "U WOT M8, 1v1 IN THE GARAGE!!!!111111oneoneoneeleven",
                        CleanText = ""
                    });
                }
                return true;
            }

            foreach (var i in player.Manager.Clients.Values)
            {
                if (i.Account.NameChosen && i.Account.Name.EqualsIgnoreCase(playername))
                {
                    player.Client.SendPacket(new TextPacket() //echo to self
                    {
                        ObjectId = player.Id,
                        BubbleTime = 10,
                        Stars = player.Stars,
                        StarType = player.StarType,
                        Name = player.Name,
                        Recipient = i.Account.Name,
                        Text = msg.ToSafeText(),
                        CleanText = ""
                    });

                    i.SendPacket(new TextPacket() //echo to /tell player
                    {
                        ObjectId = i.Player.Owner.Id == player.Owner.Id ? player.Id : -1,
                        BubbleTime = 10,
                        Stars = player.Stars,
                        StarType = player.StarType,
                        Name = player.Name,
                        Recipient = i.Account.Name,
                        Text = msg.ToSafeText(),
                        CleanText = ""
                    });
                    return true;
                }
            }
            player.SendError(string.Format("{0} not found.", playername));
            return false;
        }
    }

    internal class PetRenameCommand : Command
    {
        public PetRenameCommand() :
            base("rp")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            var p = player.Pet;
            var inv = player.Inventory;
            if (p == null)
            {
                player.SendError("Pet not found");
                return false;
            }
            for (int i = 0; i < inv.Length; i++)
            {
                if (inv[i] == null)
                    continue;
                if (inv[i].ObjectId == "Pet Renamer")
                {
                    inv[i] = null;
                    player.Manager.Database.DoActionAsync(db =>
                    {
                        var cmd = db.CreateQuery();
                        cmd.CommandText = "UPDATE pets SET skinName=@newname WHERE petId=@accId;";
                        cmd.Parameters.AddWithValue("@accId", p.PetId);
                        if (args.Length == 2)
                            cmd.Parameters.AddWithValue("@newname", (args[0] + " " + args[1]));
                        else
                            cmd.Parameters.AddWithValue("@newname", args[0]);
                        cmd.ExecuteNonQuery();
                    });
                    if (args.Length == 1)
                    {
                        TextPacket packet = new TextPacket
                        {
                            BubbleTime = 0,
                            Stars = -1,
                            Name = "",
                            Text = player.Name + " has renamed their pet to " + args[0] + "."
                        };
                        player.Owner.BroadcastPacket(packet, null);
                    }
                    else
                    {
                        TextPacket packet = new TextPacket
                        {
                            BubbleTime = 0,
                            Stars = -1,
                            Name = "",
                            Text = player.Name + " has renamed their pet to " + args[0] + args[1] + "."
                        };
                        player.Owner.BroadcastPacket(packet, null);
                    }
                    player.SendInfo("Pet successfully renamed!");
                    return true;
                }
            };

            player.SendInfo("You need to have a pet renamer in your inventory.");
            return false;
        }
    }

    internal class CheckMarkEnabled : Command
    {
        public CheckMarkEnabled()
            : base("cma", 0)
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            player.Manager.Database.DoActionAsync(db =>
            {
                var cmd = db.CreateQuery();
                cmd.CommandText = "SELECT name FROM accounts WHERE marksEnabled=@mar;";
                cmd.Parameters.AddWithValue("@mar", player.MarksEnabled);
                cmd.CommandText = "SELECT charId FROM characters WHERE markId=@mar2;";
                cmd.Parameters.AddWithValue("@mar2", player.MarkId);
                if (player.MarksEnabled == 1)
                {
                    player.SendInfo("The mark feature has been enabled on your account!");
                    player.SendInfo("The mark id being used on your account is " + player.MarkId);
                }
                else
                {
                    player.SendInfo("You do not have marks enabled on your account.");
                }
            });
            return true;
        }
    }

    internal class EvolveItem : Command
    {
        public EvolveItem() :
            base("evolve")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            Random rand = new Random();
            int Prob = rand.Next(0, 1);
            var inv = player.Inventory;
            bool done = false;
            switch (Prob)
            {
                case 0:
                    for (int d = 0; d < inv.Length; d++)
                    {
                        if (inv[d] == null)
                            continue;
                        if (inv[d].ObjectId == "Exanimation Stone")
                        {
                            for (int i = 0; i < inv.Length; i++)
                            {
                                if (inv[i] == null)
                                    continue;
                                if (inv[i].ObjectId == "Sword of Dark Necromancy")
                                {
                                    inv[i] = player.Manager.GameData.Items[0x47bd];
                                    inv[d] = null;
                                    player.UpdateCount++;
                                    player.SaveToCharacter();
                                    done = true;
                                    return false;
                                }
                            }
                        }
                    }
                    break;
            }
            if (done == false)
            {
                player.SendInfo("You do not have the Exanimation Stone in your inventory.");
            }
            else
            {
                TextPacket packet = new TextPacket
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "",
                    Text = player.Name + " has evolved a Sword of Dark Necromancy."
                };
                player.Owner.BroadcastPacket(packet, null);
            }
            return true;
        }
    }

    internal class ActivateTaskCommand : Command
    {
        public ActivateTaskCommand() :
            base("activatetask")
        {
        }

        protected override bool Process(Player player, RealmTime time, string[] args)
        {
            Random rand = new Random();
            int Prob = rand.Next(0, 1);
            var inv = player.Inventory;
            string taskname = "";
            switch (Prob)
            {
                case 0:
                    for (int d = 0; d < inv.Length; d++)
                    {
                        if (inv[d] == null)
                            continue;
                        if (inv[d].ObjectId == "Sor Crystal")
                        {
                            for (int i = 0; i < inv.Length; i++)
                            {
                                if (inv[i] == null)
                                    continue;
                                if (inv[i].ObjectId == "Task Tablet: Inactive")
                                {
                                    inv[i] = player.Manager.GameData.Items[0x42c8];
                                    inv[d] = null;
                                    player.UpdateCount++;
                                    player.SaveToCharacter();
                                    taskname = "Search for Spirit";
                                    return false;
                                }
                            }
                        }
                    }
                    break;
            }
            if (taskname == "")
            {
                player.SendInfo("You do not have to correct materials to activate a task.");
            }
            else
            {
                TextPacket packet = new TextPacket
                {
                    BubbleTime = 0,
                    Stars = -1,
                    Name = "",
                    Text = player.Name + " has activated the task [" + taskname + "]"
                };
                player.Owner.BroadcastPacket(packet, null);
                player.SendInfo("A task has been activated!");
            }
            return true;
        }
    }
}