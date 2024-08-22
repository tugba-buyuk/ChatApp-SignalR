using ChatApp.DataSources;
using ChatApp.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Hubs
{
    public class MessageHub : Hub
    {
        public async Task GetNickName(string nickName)
        {
            var client = new Clients
            {
                ConnectionId = Context.ConnectionId,
                NickName = nickName
            };
            ClientSource.Clients.Add(client);
            await Clients.Others.SendAsync("clientJoined", nickName);
            await Clients.All.SendAsync("clientList", ClientSource.Clients);
        }

        public async Task SendMessageAsync(string nickName,string message)
        {
            nickName = nickName.Trim();
            Clients senderClient=ClientSource.Clients.FirstOrDefault(c=>c.ConnectionId == Context.ConnectionId);
            if (nickName == "Tümü")
            {
                await Clients.Others.SendAsync("receiveMessage", message,senderClient.NickName);
            }
            else
            {
                Clients client = ClientSource.Clients.FirstOrDefault(c => c.NickName == nickName);
                await Clients.Client(client.ConnectionId).SendAsync("receiveMessage", message, senderClient.NickName);
            }
            

        }

        public async Task AddGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            Group group= new Group{ GroupName=groupName};
            group.Clients.Add(ClientSource.Clients.FirstOrDefault(c=>c.ConnectionId == Context.ConnectionId));
            GroupSource.Groups.Add(group);
            await Clients.All.SendAsync("groups", GroupSource.Groups);
        }

        public async Task AddClientIntoGroup(IEnumerable<string> groupNames)
        {
            Clients client = ClientSource.Clients.FirstOrDefault(c=>c.ConnectionId== Context.ConnectionId);
            foreach (var group in groupNames)
            {
                Group _group = GroupSource.Groups.FirstOrDefault(g => g.GroupName.Equals(group));
                var result = _group.Clients.Any(c => c.ConnectionId == Context.ConnectionId);
                if (!result)
                {
                    _group.Clients.Add(client);
                    await Groups.AddToGroupAsync(Context.ConnectionId, group);
                }
            }
        }

        public async Task GetClientsForGroup(string groupName)
        {
            Group group=GroupSource.Groups.FirstOrDefault(g=>g.GroupName.Equals(groupName));

            await Clients.Caller.SendAsync("clientList", groupName=="-1" ? ClientSource.Clients : group.Clients);
        }

        public async Task SendMessageToGroup(string groupName,string message)
        {
            var client=ClientSource.Clients.FirstOrDefault(c=>c.ConnectionId == Context.ConnectionId);
            await Clients.Group(groupName).SendAsync("receiveMessage", message, client.NickName);
        }
    }
}
