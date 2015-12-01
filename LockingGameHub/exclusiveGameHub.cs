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
        public string Id = string.Empty; 
        public int X;
        public int Y;
    }

    public static class HubObject
    {
        public static List<collectable> Collectables = new List<collectable>()
            {
            new collectable {Id=Guid.NewGuid().ToString(), X=200,Y = 200 },
            new collectable {Id=Guid.NewGuid().ToString(),X= 250, Y = 300 },
            new collectable {Id=Guid.NewGuid().ToString(),X= 100, Y = 100 },
            new collectable {Id=Guid.NewGuid().ToString(),X= 400, Y = 400 },
            new collectable {Id=Guid.NewGuid().ToString(),X= 150, Y = 150 },
            };

        public static void reset()
        {
            Collectables = new List<collectable>()
            {
            new collectable {Id=Guid.NewGuid().ToString(), X=200,Y = 200 },
            new collectable {Id=Guid.NewGuid().ToString(),X= 250, Y = 300 },
            new collectable {Id=Guid.NewGuid().ToString(),X= 100, Y = 100 },
            new collectable {Id=Guid.NewGuid().ToString(),X= 400, Y = 400 },
            new collectable {Id=Guid.NewGuid().ToString(),X= 150, Y = 150 },
            };
        }
    }


public class exclusiveGameHub : Hub
    {
        public override Task OnConnected()
        {
            foreach (var item in HubObject.Collectables)
            {
                Clients.Caller.createOpponentCollectable(item.Id,item.X, item.Y);
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

        public void remove(string id)
        {
            Clients.Others.removed(id);
            var removed = HubObject.Collectables.Find(c => c.Id == id);
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