// Hubs/ClaimHub.cs
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace CMCS_Web.Hubs
{
    // Minimal hub to support live updates (can be extended later).
    public class ClaimHub : Hub
    {
        // Example: server can call Clients.All.SendAsync("ClaimUpdated", claimId);
        public async Task NotifyClaimUpdated(string claimId)
        {
            await Clients.All.SendAsync("ClaimUpdated", claimId);
        }

        // Example: server can call Clients.User(userId).SendAsync("PrivateMessage", msg);
        public async Task SendMessageToAll(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
