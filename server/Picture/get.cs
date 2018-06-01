#region

using System.IO;
using System.Text;

#endregion

namespace server.picture
{
    internal class get : RequestHandler
    {
        private readonly byte[] buff = new byte[0x10000];

        protected override void HandleRequest()
        {
            //warning: maybe has hidden url injection HA
            string id = Query["id"];

            if (!id.StartsWith("draw:") && !id.StartsWith("file:"))
            {
                string path = Path.GetFullPath("texture/" + id + ".png");
                if (!File.Exists(path))
                {
                    Program.Logger.Warn($"RemoteTexture not found: {id}");
                    byte[] status = Encoding.UTF8.GetBytes("<Error>Invalid ID.</Error>");
                    Context.Response.OutputStream.Write(status, 0, status.Length);
                    return;
                }
                using (FileStream i = File.OpenRead(path))
                {
                    int c;
                    while ((c = i.Read(buff, 0, buff.Length)) > 0)
                        Context.Response.OutputStream.Write(buff, 0, c);
                }
            }
        }
    }
}