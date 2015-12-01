using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;


namespace LockingGameHub
{
    public class collectable
    {
        public int X;
        public int Y;
    }

    public static class HubObject
    {
        public static List<collectable> Collectables = new List<collectable>()
            {
            new collectable { X=200,Y = 200 },
            new collectable {X= 250, Y = 300 },
            new collectable {X= 100, Y = 100 },
            new collectable {X= 400, Y = 400 },
            new collectable {X= 150, Y = 150 },
            };

        public static void reset()
        {
            Collectables = new List<collectable>()
            {
            new collectable { X=200,Y = 200 },
            new collectable {X= 250, Y = 300 },
            new collectable {X= 100, Y = 100 },
            new collectable {X= 400, Y = 400 },
            new collectable {X= 150, Y = 150 },
            };
        }
    }


public class exclusiveGameHub : Hub
    {
        public override Task OnConnected()
        {
            foreach (var item in HubObject.Collectables)
            {
                Clients.Caller.createOpponentCollectable(item.X, item.Y);
            }
            return base.OnConnected();
        }

        public void moveTowards()
        {
            Random r = new Random();
            int x = r.Next(640);
            int y = r.Next(480);
            Clients.All.moveTo(x, y);
        }

        public void remove(int x, int y)
        {
            Clients.Others.removed(x, y);
            var removed = HubObject.Collectables.Find(c => c.X == x && c.Y == y);
            if (removed != null)
                HubObject.Collectables.Remove(removed);

            if (HubObject.Collectables.Count == 0)
            {
                Clients.All.end();
                HubObject.reset();
            }

        }
    }
}