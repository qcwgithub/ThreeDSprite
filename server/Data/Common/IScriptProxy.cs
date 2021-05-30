using System;
using System.Net.Sockets;

namespace Data
{
    public interface IScriptProxy
    {
        void onListenerSocketComplete(SocketAsyncEventArgs e);
        void onAcceptComplete(SocketAsyncEventArgs e);

        void connectorOnConnect(TcpClientData tcpClientData);
        void connectorOnDisconnect(TcpClientData tcpClientData);

        void acceptorOnConnect(TcpClientData tcpClientData, bool isServer);
        void acceptorOnDisconnect(TcpClientData tcpClientData, bool isServer);

        // bool decode(string message,
        //     out int seq,
        //     out MsgType msgType, out ECode ecode,
        //     out string msg);

        // string encodeReply(int seq, ECode e, string replyMsg);
        // string encodeSend(int seq, MsgType msgType, string msg);

        // void send(TcpClientData socket, MsgType type, object msg, Action<ECode, string> cb);
        // void onMessage(TcpClientData socket, bool fromServer, MsgType type, string msg, Action<ECode, string> reply);
        // void _onSendComplete(TcpClientData socket, object o);
        // void _onRecvComplete(TcpClientData socket, object o);
        // void _onConnectComplete(TcpClientData socket, object o);
        // void _onDisconnectComplete(TcpClientData socket, object o);
    }
}