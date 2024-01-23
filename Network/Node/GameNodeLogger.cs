using System.Text;
using Microsoft.IdentityModel.Tokens;
using Network.Packets;
using Serilog;
using Exception = System.Exception;

namespace Network.Node;

public class GameNodeLogger(IGameNode node) : IGameNode
{
    public void OnDisconnect(Author author)
    {
        Log.Information(
            "[{connectionId}] connect ip:{ip}",
            author.ConnectionId, 
            author.Socket.RemoteEndPoint?.ToString());
        
        try
        {
            node.OnDisconnect(author);
        }
        catch (Exception e)
        {
            Log.Error(e, 
                "[{connectionId}] on disconnect id:{Id}",
                author.ConnectionId,
                author.UserId);
        }
    }

    public void OnConnect(Author author)
    {
        try
        {
            node.OnConnect(author);
        }
        catch (Exception e)
        {
            Log.Error(e, 
                "[{connectionId}] on connect id:{Id}",
                author.ConnectionId,
                author.UserId);
        }
    }

    public void OnMessage(Author author, ushort actionType, byte[] body)
    {
        try
        {
            node.OnMessage(author, actionType, body);
        }
        catch (SecurityTokenMalformedException)
        {
            Log.Information(
                "[{connectionId}] auth fail id:{id} type:{type} request:{body}",
                author.ConnectionId, 
                author.UserId,
                actionType,
                Encoding.Default.GetString(body));
        }
        catch (Exception e)
        {
            Log.Error(
                e,
                "[{connectionId}] action fail id:{id} type:{type} request:{body}",
                author.ConnectionId,
                author.UserId,
                actionType,
                Encoding.Default.GetString(body));
        }
    }
}