using System;
using System.Threading.Tasks;

public interface ISocket
{
    void send(MsgType type, object msg, Action<ECode, string> cb);
    Task<MyResponse> sendAsync(MsgType type, object msg);

    void setCustomMessageListener(Action<MsgType, string, Action<ECode, string>> reply);
    void removeCustomMessageListener();

    bool isConnected();
    int getId();
    void close();

    void bindPlayer(PMPlayerInfo player, int clientTimestamp);
    void unbindPlayer(PMPlayerInfo player);
    int getClientTimestamp();
    PMPlayerInfo getPlayer();
}