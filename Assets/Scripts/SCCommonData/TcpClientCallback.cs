using System;
using System.Net.Sockets;
using System.IO;

namespace Data
{
    public interface ITcpClientCallback
    {
        int nextSocketId { get; }
        int nextMsgSeq { get; }
        IMessagePacker messagePacker { get; }
        void logError(TcpClientData data, string str);
        void logInfo(TcpClientData data, string str);

        void onConnectComplete(TcpClientData data, SocketAsyncEventArgs e, SocketError socketError);
        void onCloseComplete(TcpClientData data);

        void dispatch(TcpClientData data, MsgType msgType, object msg, Action<ECode, object> cb);
    }
}