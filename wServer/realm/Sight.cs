#region

using db.data;
using System;
using System.Collections.Generic;
using wServer.realm.entities.player;

#endregion

namespace wServer.realm
{
    internal static class Sight
    {
        private static readonly Dictionary<int, IntPoint[]> points = new Dictionary<int, IntPoint[]>();

        public static IntPoint[] Cast(Player player, int difficulty, int radius = 15)
        {
            var ret = new List<IntPoint>();
            var angle = 0;
            while (angle < 360)
            {
                var distance = 0;
                while (distance < radius)
                {
                    var x = (int)(distance * Math.Cos(angle));
                    var y = (int)(distance * Math.Sin(angle));
                    if (x * x + y * y <= radius * radius)
                    {
                        ret.Add(new IntPoint(x, y));
                        ObjectDesc desc;
                        player.Manager.GameData.ObjectDescs.TryGetValue(
                            player.Owner.Map[(int)player.X + x, (int)player.Y + y].ObjType, out desc);
                        if ((desc?.BlocksSight ?? false) && difficulty >= 0)
                            break;
                    }
                    distance++;
                }
                angle++;
            }
            return ret.ToArray();
        }
    }
}