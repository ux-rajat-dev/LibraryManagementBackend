using Microsoft.AspNetCore.SignalR;

namespace LibraryManagement
{
    public class NotificationHub : Hub
    {
        // Method to send updates to all connected clients
        public async Task SendBookUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveBookUpdate", message);
        }
    }
}
