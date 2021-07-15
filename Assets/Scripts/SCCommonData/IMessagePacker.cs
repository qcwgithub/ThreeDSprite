using System;
using System.Text;

namespace Data
{
    public struct UnpackResult
    {
        public bool success;

        public int totalLength;

        public int seq;
        public int code;

        // 1 of 2
        public int typeLength; // ---
        public MessageCode messageCode;
        
        public object msg;

        // 对方是否要求回复
        public bool requireResponse;
    }

    public interface IMessagePacker
    {
        bool IsCompeteMessage(byte[] buffer, int offset, int count);
        UnpackResult Unpack(byte[] buffer, int offset, int count);
        byte[] Pack(int msgTypeOrECode, object msg, int seq, bool requireResponse);
    }
}