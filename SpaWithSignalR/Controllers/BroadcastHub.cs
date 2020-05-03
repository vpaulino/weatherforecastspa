using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Frontend.Controllers
{
    public class BroadcastHub : Hub<IHubClient>
    {
    }
}
