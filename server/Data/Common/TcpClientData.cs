using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace Data
{
    public class TcpClientBasicData
    {
        Socket _socket;
        IPEndPoint ipEndPointForConnect;
        CancellationTokenSource _cancellationTaskSource;
        CancellationToken _cancellationToken;
        SocketAsyncEventArgs _innArgs;
        SocketAsyncEventArgs _outArgs;

        public TcpClientBasicData()
        {
            _cancellationTaskSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTaskSource.Token;
            _innArgs = new SocketAsyncEventArgs();
            _outArgs = new SocketAsyncEventArgs();
            _innArgs.Completed += _onComplete;
            _outArgs.Completed += _onComplete;
        }

        void _onComplete(object sender, SocketAsyncEventArgs e)
        {
            if (this.onComplete != null)
            {
                this.onComplete(sender, e);
            }
        }

        //-----------
        public event Action<object, SocketAsyncEventArgs> onComplete;
    }
}