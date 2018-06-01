using System;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using wServer.realm;
using wServer.realm.entities.player;

namespace wServer.networking.handlers
{
    internal class CookPacketHandler : PacketHandlerBase<CookPacket>
    {
        public override PacketID ID
        {
            get { return PacketID.COOK; }
        }

        protected override void HandlePacket(Client client, CookPacket packet)
        {
            client.Manager.Logic.AddPendingAction(t => Handle(client.Player, t, packet.Fuel, packet.Slot1, packet.Slot2,
                packet.Slot3, packet.Slot4, packet.Slot5, packet.Slot6, packet.Slot7, packet.Slot8, packet.Slot8));
        }

        private bool Handle(Player player, RealmTime time, ObjectSlot Fuel, ObjectSlot Slot1, ObjectSlot Slot2,
            ObjectSlot Slot3, ObjectSlot Slot4, ObjectSlot Slot5, ObjectSlot Slot6, ObjectSlot Slot7, ObjectSlot Slot8,
            ObjectSlot Slot9)
        {
            //this stuff needs to be changed to XML format

            try
            {
                //bread
                if (Fuel.ObjectType == 21690)
                {
                    if (Slot4.ObjectType == 21691 && Slot5.ObjectType == 21691 && Slot6.ObjectType == 21691)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[21692];
                        player.Inventory[Slot4.SlotId] = null;
                        player.Inventory[Slot5.SlotId] = null;
                        player.Inventory[Slot6.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully cooked bread!"
                        });
                        levelCooking("Bread");
                        return false;
                    }
                    //sliceofpizza
                    if (Slot1.ObjectType == 21692 && Slot4.ObjectType == 21694 && Slot7.ObjectType == 21693)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x54bf];
                        player.Inventory[Slot1.SlotId] = null;
                        player.Inventory[Slot4.SlotId] = null;
                        player.Inventory[Slot7.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully cooked a slice of pizza!"
                        });
                        levelCooking("Slice of Pizza");
                        return false;
                    }
                    //full pizza
                    if (Slot1.ObjectType == 0x54bf && Slot2.ObjectType == 0x54bf && Slot4.ObjectType == 0x54bf &&
                        Slot5.ObjectType == 0x54bf)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x54c1];
                        player.Inventory[Slot1.SlotId] = null;
                        player.Inventory[Slot2.SlotId] = null;
                        player.Inventory[Slot4.SlotId] = null;
                        player.Inventory[Slot5.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully cooked a full pizza!"
                        });
                        levelCooking("Full Pizza");
                        return false;
                    }
                    //bowl of soup
                    if (Slot1.ObjectType == 0x54ce && Slot4.ObjectType == 0x54cd)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x54d4];
                        player.Inventory[Slot1.SlotId] = null;
                        player.Inventory[Slot4.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully cooked a bowl of soup!"
                        });
                        levelCooking("Bowl of Soup");
                        return false;
                    }
                    //grilled cheese sandwich
                    if (Slot1.ObjectType == 21692 && Slot4.ObjectType == 21694 && Slot7.ObjectType == 21692)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x54cc];
                        player.Inventory[Slot1.SlotId] = null;
                        player.Inventory[Slot4.SlotId] = null;
                        player.Inventory[Slot7.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully cooked a Grilled Cheese Sandwich!"
                        });
                        levelCooking("Grilled Cheese Sandwich");
                        return false;
                    }
                    //chocolate cake
                    if (Slot1.ObjectType == 0x54c8 && Slot4.ObjectType == 0x54c9 && Slot7.ObjectType == 0x54ca)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x54d2];
                        player.Inventory[Slot1.SlotId] = null;
                        player.Inventory[Slot4.SlotId] = null;
                        player.Inventory[Slot7.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully baked a chocolate cake!"
                        });
                        levelCooking("Chocolate Cake");
                        return false;
                    }
                    //
                    //spicy pizza
                    if (Slot1.ObjectType == 0x54bf && Slot4.ObjectType == 0x54d1)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x54cf];
                        player.Inventory[Slot1.SlotId] = null;
                        player.Inventory[Slot4.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully cooked a spicy slice of pizza!"
                        });
                        levelCooking("Spicy Slice of Pizza");
                        return false;
                    }
                    //KARAMBWAN
                    if (Slot1.ObjectType == 0x54c5 && Slot4.ObjectType == 0x54c5 && Slot7.ObjectType == 0x54c5 &&
                        Slot5.ObjectType == 0x54c5 && Slot6.ObjectType == 0x54c5)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x54c6];
                        player.Inventory[Slot1.SlotId] = null;
                        player.Inventory[Slot4.SlotId] = null;
                        player.Inventory[Slot7.SlotId] = null;
                        player.Inventory[Slot5.SlotId] = null;
                        player.Inventory[Slot6.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully cooked a karambwan"
                        });
                        levelCooking("Karambwan");
                        return false;
                    }
                    //spicy karambwan
                    if (Slot1.ObjectType == 0x54c6 && Slot4.ObjectType == 0x54d1)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x54c7];
                        player.Inventory[Slot1.SlotId] = null;
                        player.Inventory[Slot4.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully cooked a spicy karambwan"
                        });
                        levelCooking("Spicy Karambwan");
                        return false;
                    }
                    //cookie
                    if (Slot1.ObjectType == 0x54c8 && Slot4.ObjectType == 0x54c8)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x54cb];
                        player.Inventory[Slot1.SlotId] = null;
                        player.Inventory[Slot4.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully baked a cookie"
                        });
                        levelCooking("Cookie");
                        return false;
                    }
                    //spicy soup
                    if (Slot1.ObjectType == 0x54d4 && Slot4.ObjectType == 0x54d1)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x54de];
                        player.Inventory[Slot1.SlotId] = null;
                        player.Inventory[Slot4.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully cooked a Spicy Soup"
                        });
                        levelCooking("Spicy Soup");
                        return false;
                    }
                    //salad
                    if (Slot1.ObjectType == 0x54ce && Slot4.ObjectType == 0x54d6 && Slot7.ObjectType == 0x54d5)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x54da];
                        player.Inventory[Slot1.SlotId] = null;
                        player.Inventory[Slot4.SlotId] = null;
                        player.Inventory[Slot7.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully made a Salad"
                        });
                        levelCooking("Salad");
                        return false;
                    }
                    //super full pizza
                    if (Slot1.ObjectType == 0x54c1 && Slot4.ObjectType == 0x54d6)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x54df];
                        player.Inventory[Slot1.SlotId] = null;
                        player.Inventory[Slot4.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully cooked a Super Full Pizza"
                        });
                        levelCooking("Super Full Pizza");
                        return false;
                    }
                    //cooked fish
                    if (Slot4.ObjectType == 0x54f7)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x54f8];
                        player.Inventory[Slot4.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully made a Cooked Fish"
                        });
                        levelCooking("Cooked Fish");
                        return false;
                    }
                    //cooked chicken
                    if (Slot4.ObjectType == 0x55a1)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x55a2];
                        player.Inventory[Slot4.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully made a Cooked Chicken"
                        });
                        levelCooking("Cooked Chicken");
                        return false;
                    }
                    //spicy cooked fish
                    if (Slot1.ObjectType == 0x54f8 && Slot4.ObjectType == 0x54d1)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x54f9];
                        player.Inventory[Slot1.SlotId] = null;
                        player.Inventory[Slot4.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully cooked a Spicy Cooked Fish"
                        });
                        levelCooking("Spicy Cooked Fish");
                        return false;
                    }
                    //chicken burger
                    if (Slot1.ObjectType == 0x54bc && Slot4.ObjectType == 0x55a2 && Slot7.ObjectType == 0x54bc)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x55a9];
                        player.Inventory[Slot1.SlotId] = null;
                        player.Inventory[Slot4.SlotId] = null;
                        player.Inventory[Slot7.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully cooked a Chicken Burger"
                        });
                        levelCooking("Chicken Burger");
                        return false;
                    }
                    //middle of burger
                    if (Slot1.ObjectType == 0x54d6 && Slot4.ObjectType == 0x54bd && Slot7.ObjectType == 0x54d5)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x54f4];
                        player.Inventory[Slot1.SlotId] = null;
                        player.Inventory[Slot4.SlotId] = null;
                        player.Inventory[Slot7.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully cooked the Middle of Burger"
                        });
                        levelCooking("Middle of Burger");
                        return false;
                    }
                    //slappyburger
                    if (Slot1.ObjectType == 0x54bc && Slot4.ObjectType == 0x54f4 && Slot7.ObjectType == 0x54bc)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x54d7];
                        player.Inventory[Slot1.SlotId] = null;
                        player.Inventory[Slot4.SlotId] = null;
                        player.Inventory[Slot7.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully cooked a Slappy Burger"
                        });
                        levelCooking("Slappy Burger");
                        return false;
                    }
                    //coconutmilk
                    if (Slot1.ObjectType == 0x54fa && Slot4.ObjectType == 0x55a3)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x55a4];
                        player.Inventory[Slot1.SlotId] = null;
                        player.Inventory[Slot4.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully cooked a Coconut Milk"
                        });
                        levelCooking("Coconut Milk");
                        return false;
                    }
                    //ricebowl
                    if (Slot1.ObjectType == 0x54ce && Slot4.ObjectType == 0x55a5)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x55a6];
                        player.Inventory[Slot1.SlotId] = null;
                        player.Inventory[Slot4.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully cooked a Rice Bowl"
                        });
                        levelCooking("Rice Bowl");
                        return false;
                    }
                    //sweetricebowl
                    if (Slot1.ObjectType == 0x55a6 && Slot4.ObjectType == 0x55a7)
                    {
                        player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x55a8];
                        player.Inventory[Slot1.SlotId] = null;
                        player.Inventory[Slot4.SlotId] = null;
                        player.UpdateCount++;
                        Client.SendPacket(new CookResultPacket
                        {
                            Success = true,
                            Message = "You have successfully cooked a Sweet Rice Bowl"
                        });
                        levelCooking("Sweet Rice Bowl");
                        return false;
                    }
                    //dragonspicykarambwan
                    if (Slot1.ObjectType == 0x54c7 && Slot4.ObjectType == 0x44d5)
                    {
                        using (var db = new db.Database())
                        {
                            var cooklevel = db.GetCookingLevel(Client.Account);
                            if (cooklevel >= 10)
                            {
                                player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x55ab];
                                player.Inventory[Slot1.SlotId] = null;
                                player.Inventory[Slot4.SlotId] = null;
                                player.UpdateCount++;
                                Client.SendPacket(new CookResultPacket
                                {
                                    Success = true,
                                    Message = "You have successfully cooked a Drago Spicy Karambwan"
                                });

                                levelCooking("Drago Spicy Karambwan");
                                return false;
                            }
                            else
                            {
                                player.SendError("Your cooking level must be at least 10!");

                                return false;
                            }
                        }
                    }
                    //goldenchocolatebar
                    if (Slot1.ObjectType == 0x54f6 && Slot4.ObjectType == 0x53b9)
                    {
                        using (var db = new db.Database())
                        {
                            var cooklevel = db.GetCookingLevel(Client.Account);
                            if (cooklevel >= 15)
                            {
                                player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x55b1];
                                player.Inventory[Slot1.SlotId] = null;
                                player.Inventory[Slot4.SlotId] = null;
                                player.UpdateCount++;
                                Client.SendPacket(new CookResultPacket
                                {
                                    Success = true,
                                    Message = "You have successfully cooked a Golden Chocolate Bar"
                                });

                                levelCooking("Golden Chocolate Bar");
                                return false;
                            }
                            else
                            {
                                player.SendError("Your cooking level must be at least 15!");

                                return false;
                            }
                        }
                    }
                    //strangefullpizza
                    if (Slot1.ObjectType == 0x54df && Slot4.ObjectType == 0x49a5)
                    {
                        using (var db = new db.Database())
                        {
                            var cooklevel = db.GetCookingLevel(Client.Account);
                            if (cooklevel >= 30)
                            {
                                player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x55b2];
                                player.Inventory[Slot1.SlotId] = null;
                                player.Inventory[Slot4.SlotId] = null;
                                player.UpdateCount++;
                                Client.SendPacket(new CookResultPacket
                                {
                                    Success = true,
                                    Message = "You have successfully cooked a Strange Full Pizza"
                                });

                                levelCooking("Strange Full Pizza");
                                return false;
                            }
                            else
                            {
                                player.SendError("Your cooking level must be at least 30!");

                                return false;
                            }
                        }
                    }
                    //ghostpepper
                    if (Slot1.ObjectType == 0x54d1 && Slot4.ObjectType == 0x55ac)
                    {
                        using (var db = new db.Database())
                        {
                            var cooklevel = db.GetCookingLevel(Client.Account);
                            if (cooklevel >= 65)
                            {
                                player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x55b7];
                                player.Inventory[Slot1.SlotId] = null;
                                player.Inventory[Slot4.SlotId] = null;
                                player.UpdateCount++;
                                Client.SendPacket(new CookResultPacket
                                {
                                    Success = true,
                                    Message = "You have successfully cooked a Ghost Pepper"
                                });

                                levelCooking("Ghost Pepper");
                                return false;
                            }
                            else
                            {
                                player.SendError("Your cooking level must be at least 65!");

                                return false;
                            }
                        }
                    }
                    //ghostburger
                    if (Slot1.ObjectType == 0x54d7 && Slot4.ObjectType == 0x55ac)
                    {
                        using (var db = new db.Database())
                        {
                            var cooklevel = db.GetCookingLevel(Client.Account);
                            if (cooklevel >= 20)
                            {
                                player.Inventory[Fuel.SlotId] = player.Manager.GameData.Items[0x55ad];
                                player.Inventory[Slot1.SlotId] = null;
                                player.Inventory[Slot4.SlotId] = null;
                                player.UpdateCount++;
                                Client.SendPacket(new CookResultPacket
                                {
                                    Success = true,
                                    Message = "You have successfully cooked a Ghost Burger"
                                });

                                levelCooking("Ghost Burger");
                                return false;
                            }
                            else
                            {
                                player.SendError("Your cooking level must be at least 20!");

                                return false;
                            }
                        }
                    }
                    else
                    {
                        player.SendError("No recipe found!");
                        return false;
                    }
                }
                else
                {
                    player.SendError("You need to add Fuel to the furnace!");
                    return false;
                }
            }
            catch (Exception e)
            {
                player.SendError("You need to add Fuel!");
                Console.WriteLine("{0} Exception caught.", e);
                return false;
            }
        }

        private void levelCooking(string foodName)
        {
            if (Client.Player.CookingLevel != 120)
            {
                Client.Player.doesLevel(foodName);
            }
        }
    }
}