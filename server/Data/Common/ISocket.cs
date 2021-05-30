using System;
using System.Threading.Tasks;
using Data;

public interface ISocket
{
    void send(MsgType type, object msg, Action<ECode, string> cb);
    Task<MyResponse> sendAsync(MsgType type, object msg);

    bool isConnected();
    int getId();
    void close();

    void bindPlayer(PMPlayerInfo player, int clientTimestamp);
    void unbindPlayer(PMPlayerInfo player);
    int getClientTimestamp();
    PMPlayerInfo getPlayer();
}