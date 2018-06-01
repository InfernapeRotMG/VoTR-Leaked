using System.IO;

namespace server.credits
{
    internal class kabamadd : RequestHandler
    {
        protected override void HandleRequest()
        {
            using (StreamWriter wtr = new StreamWriter(Context.Response.OutputStream))
            {
                string s = File.ReadAllText("game/saved_resource.htm");
                wtr.Write(s);
            }
        }
    }
}