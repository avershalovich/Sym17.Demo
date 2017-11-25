using Microsoft.AspNet.SignalR;
using Sym17.Web.Models.Contact;

namespace Sym17.Web.Hubs
{
    public class ActiveVisitorHub : Hub
    {
        private static IHubContext _hubContext = GlobalHost.ConnectionManager.GetHubContext<ActiveVisitorHub>();

        public void Hello(ImageProcessingContact contact)
        {
            Clients.All.hello(contact);
        }

        public static void Change(ImageProcessingContact contact)
        {
            _hubContext.Clients.All.hello(contact);
        }
    }
}