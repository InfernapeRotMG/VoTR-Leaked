#region

using db;
using System;
using System.Text;

#endregion

namespace server.guild
{
    internal class listMembers : RequestHandler
    {
        protected override void HandleRequest()
        {
            using (Database db = new Database())
            {
                Account acc = db.Verify(Query["guid"], Query["password"], Program.GameData);
                byte[] status = new byte[0];
                if (CheckAccount(acc, db, false))
                {
                    try
                    {
                        status =
                            Encoding.UTF8.GetBytes(db.HttpGetGuildMembers(Convert.ToInt32(Query["num"]),
                                Convert.ToInt32(Query["offset"]), acc));
                    }
                    catch
                    {
                        status = Encoding.UTF8.GetBytes("<Error>Guild member error</Error>");
                    }
                }
                Context.Response.OutputStream.Write(status, 0, status.Length);
                Context.Response.Close();
            }
        }
    }
}