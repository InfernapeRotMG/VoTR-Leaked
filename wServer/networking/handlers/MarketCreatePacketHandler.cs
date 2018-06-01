using db;
using System.Collections.Generic;
using wServer.networking.cliPackets;
using wServer.networking.svrPackets;
using wServer.realm;
using wServer.realm.entities.player;

namespace wServer.networking.handlers
{
    internal class MarketCreatePacketHandler : PacketHandlerBase<MarketCreatePacket>
    {
        public override PacketID ID
        {
            get { return PacketID.MARKETCREATE; }
        }

        protected override void HandlePacket(Client client, MarketCreatePacket packet)
        {
            client.Manager.Logic.AddPendingAction(t => Handle(client.Player, t, packet.IncludedSlots,
                packet.RequestItems, packet.RequestDatas));
        }

        private void Handle(Player player, RealmTime time, int[] slots, int[] items, ItemData[] datas)
        {
            if (player.Owner == null)
                return;
            Manager.Logic.AddPendingAction(t =>
            {
                if (slots.Length == 0 || items.Length == 0)
                {
                    player.Client.SendPacket(new MarketTradeResultPacket
                    {
                        Success = false,
                        ResultMessage = "No items were put up for trade"
                    });
                    return;
                }

                List<int> slotList = new List<int>(slots);
                List<ushort> requestItemList = new List<ushort>();
                foreach (var i in items)
                    requestItemList.Add((ushort)i);
                List<ItemData> offerDatas = new List<ItemData>();
                List<ushort> offerItems = new List<ushort>();
                foreach (var i in slotList)
                {
                    offerItems.Add(player.Inventory[i].ObjectType);
                    offerDatas.Add(null);
                    player.Inventory[i] = null;
                }

                player.UpdateCount++;

                Offer offer = new Offer();
                offer.Status = 0;
                offer.OfferItems = offerItems.ToArray();
                offer.OfferData = offerDatas.ToArray();
                offer.RequestItems = requestItemList.ToArray();
                offer.RequestData = datas;

                using (var db = new Database())
                    db.AddOffer(int.Parse(player.AccountId), offer);

                player.Client.SendPacket(new MarketTradeResultPacket
                {
                    Success = true,
                    ResultMessage = "Successfully added trade"
                });
            });
        }
    }
}