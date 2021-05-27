using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Net;

public abstract class TcpClientBasic
{
    Socket _socket;
    IPEndPoint ipEndPointForConnect = null;
    CancellationTokenSource _cancellationTaskSource;
    CancellationToken _cancellationToken;
    SocketAsyncEventArgs _innArgs;
    SocketAsyncEventArgs _outArgs;

    public TcpClientBasic()
    {
        _cancellationTaskSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTaskSource.Token;
        _innArgs = new SocketAsyncEventArgs();
        _outArgs = new SocketAsyncEventArgs();
        _innArgs.Completed += _onComplete;
        _outArgs.Completed += _onComplete;
    }

    protected void _initAcceptSocket(Socket socket)
    {
        _socket = socket;
    }

    protected void _initConnectSocket(string host, int port)
    {
        IPAddress[] addresses = Dns.GetHostAddresses(host);
        foreach (IPAddress address in addresses)
        {
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                this.ipEndPointForConnect = new IPEndPoint(address, port);
                this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this._socket.NoDelay = true;
                break;
            }
        }
    }

    void _onComplete(object sender, SocketAsyncEventArgs e)
    {
        switch (e.LastOperation)
        {
            case SocketAsyncOperation.Connect:
                ET.ThreadSynchronizationContext.Instance.Post(_onConnectComplete, e);
                break;
            case SocketAsyncOperation.Receive:
                ET.ThreadSynchronizationContext.Instance.Post(_onRecvComplete, e);
                break;
            case SocketAsyncOperation.Send:
                ET.ThreadSynchronizationContext.Instance.Post(_onSendComplete, e);
                break;
            case SocketAsyncOperation.Disconnect:
                ET.ThreadSynchronizationContext.Instance.Post(_onDisconnectComplete, e);
                break;
            default:
                throw new Exception($"socket error: {e.LastOperation}");
        }
    }

    void _onConnectComplete(object o)
    {
        SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;

        if (e.SocketError != SocketError.Success)
        {
            this.onError("SocketError." + e.SocketError);
            return;
        }
        e.RemoteEndPoint = null;
        this.onConnectComplete();
    }

    void _onRecvComplete(object o)
    {
        SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;

        if (e.SocketError != SocketError.Success)
        {
            this.onError("SocketError." + e.SocketError);
            return;
        }

        if (e.BytesTransferred == 0)
        {
            this.onError("ErrorCode.ERR_PeerDisconnect");
            return;
        }

        this.onRecvComplete(e.BytesTransferred);
    }

    void _onSendComplete(object o)
    {
        SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;

        if (e.SocketError != SocketError.Success)
        {
            this.onError("SocketError." + e.SocketError);
            return;
        }

        if (e.BytesTransferred == 0)
        {
            this.onError("ErrorCode.ERR_PeerDisconnect");
            return;
        }
        this.onSendComplete();
    }

    void _onDisconnectComplete(object o)
    {
        SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;
        this.onError("SocketError." + e.SocketError);
        this.onDisconnectComplete();
    }

    protected void _connectAsync()
    {
        _outArgs.RemoteEndPoint = this.ipEndPointForConnect;
        bool completed = !this._socket.ConnectAsync(_outArgs);
        if (completed)
        {
            this.onConnectComplete();
        }
    }

    protected void _sendAsync(byte[] buffer, int offset, int count)
    {
        try
        {
            _outArgs.SetBuffer(buffer, 0, buffer.Length);
        }
        catch (Exception e)
        {
            throw new Exception($"socket set buffer error: {buffer.Length}, {offset}, {count}", e);
        }
        bool completed = !this._socket.SendAsync(_outArgs);
        if (completed)
        {
            this.onSendComplete();
        }
    }

    protected void _recvAsync(byte[] buffer, int offset, int count)
    {
        try
        {
            _innArgs.SetBuffer(buffer, offset, count);
        }
        catch (Exception e)
        {
            throw new Exception($"socket set buffer error: {buffer.Length}, {offset}, {count}", e);
        }

        bool completed = !this._socket.ReceiveAsync(_innArgs);
        if (completed)
        {
            _onRecvComplete(_innArgs);
        }
    }

    protected abstract void onError(string e);
    protected abstract void onConnectComplete();
    protected abstract void onRecvComplete(int bytesTransferred);
    protected abstract void onSendComplete();
    protected abstract void onDisconnectComplete();
}