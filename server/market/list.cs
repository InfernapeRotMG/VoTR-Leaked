using db;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace server.market
{
    internal class list : RequestHandler
    {
        protected override void HandleRequest()
        {
            OfferList list = new OfferList();

            using (Database db = new Database())
            {
                MySqlCommand cmd = db.CreateQuery();
                Account acc = db.Verify(Query["guid"], Query["password"], Program.GameData);

                cmd.CommandText = "SELECT * FROM market WHERE status=0 ORDER BY id DESC";
                if (acc != null && Query["filter"] == "mine")
                {
                    cmd.CommandText = "SELECT * FROM market WHERE accId=@accId ORDER BY id DESC";
                    cmd.Parameters.AddWithValue("@accId", int.Parse(acc.AccountId));
                }

                ushort[] offerSearch = new ushort[0];
                ItemData[] offerSearchD = new ItemData[0];
                if (Query["offerItems"] != null && Query["offerItems"] != "")
                {
                    offerSearch = Utils.FromCommaSepString16z(Query["offerItems"]);
                    offerSearchD = new ItemData[offerSearch.Length];
                    if (Query["offerData"] != "")
                        offerSearchD = ItemDataList.CreateData(Query["offerData"]);
                }

                ushort[] reqSearch = new ushort[0];
                ItemData[] reqSearchD = new ItemData[0];
                if (Query["requestItems"] != null && Query["requestItems"] != "")
                {
                    reqSearch = Utils.FromCommaSepString16z(Query["requestItems"]);
                    reqSearchD = new ItemData[reqSearch.Length];
                    if (Query["requestData"] != "")
                        reqSearchD = ItemDataList.CreateData(Query["requestData"]);
                }
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                    if (rdr.HasRows)
                    {
                        while (rdr.Read())
                        {
                            if (offerSearch.Length > 0)
                            {
                                List<ushort> offerItems = new List<ushort>(Utils.FromCommaSepString16z(rdr.GetString("offerItems")));
                                ItemData[] offerData = ItemDataList.CreateData(rdr.GetString("offerData"));
                                bool success = false;
                                for (int i = 0; i < offerSearch.Length; i++)
                                {
                                    int res = -1;
                                    if ((res = offerItems.IndexOf(offerSearch[i])) == -1)
                                        continue;
                                    success = true;
                                    break;
                                }
                                if (!success)
                                    continue;
                            }

                            if (reqSearch.Length > 0)
                            {
                                List<ushort> reqItems = new List<ushort>(Utils.FromCommaSepString16z(rdr.GetString("requestItems")));
                                ItemData[] reqData = ItemDataList.CreateData(rdr.GetString("requestData"));
                                bool success = false;
                                for (int i = 0; i < reqSearch.Length; i++)
                                {
                                    int res = -1;
                                    if ((res = reqItems.IndexOf(reqSearch[i])) == -1)
                                        continue;
                                    success = true;
                                    break;
                                }
                                if (!success)
                                    continue;
                            }

                            list.Offers.Add(new Offer
                            {
                                Id = rdr.GetInt32("id"),
                                AccId = rdr.GetInt32("accId"),

                                Mine = acc != null ? rdr.GetInt32("accId") == int.Parse(acc.AccountId) : false,
                                Status = rdr.GetInt32("status"),

                                _OfferItems = rdr.GetString("offerItems"),
                                _OfferData = rdr.GetString("offerData"),

                                _RequestItems = rdr.GetString("requestItems"),
                                _RequestData = rdr.GetString("requestData")
                            });
                        }
                    }
            }
            if (Query["filter"] != "mine" && Query["filter"] != "searched")
                if (list.Offers.Count > 50)
                    list.Offers.RemoveRange(50, list.Offers.Count - 50);

            XmlSerializer serializer = new XmlSerializer(list.GetType(), new XmlRootAttribute("Offers") { Namespace = "" });
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Encoding = Encoding.UTF8;
            xws.Indent = true;
            XmlWriter xtw = XmlWriter.Create(Context.Response.OutputStream, xws);
            serializer.Serialize(xtw, list, list.Namespaces);
        }
    }
}