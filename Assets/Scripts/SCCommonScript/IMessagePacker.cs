using System;
using System.Text;
using Data;

namespace Script
{
    public struct UnpackResult
    {
        public bool success;

        public int totalLength;

        public int seq;
        public int code;
        public int typeLength;
        public object msg;

        // 对方是否要求回复
        public bool requireResponse;
    }

    public interface IMessagePacker
    {
        bool IsCompeteMessage(byte[] buffer, int offset, int count);
        UnpackResult Unpack(byte[] buffer, ref int offset, int count);
        byte[] Pack(int msgTypeOrECode, object msg, int seq, bool requireResponse);
    }

    public abstract class MessageHeaderPacker : IMessagePacker
    {
        public bool IsCompeteMessage(byte[] buffer, int offset, int count)
        {
            if (count < GetHeaderLength())
            {
                return false;
            }
            int length = BitConverter.ToInt32(buffer, offset);
            if (count < length)
            {
                return false;
            }
            return true;
        }
        protected void UnpackHeader(byte[] buffer, ref int offset, ref int count, ref UnpackResult r)
        {
            // 4 = total length
            r.totalLength = BitConverter.ToInt32(buffer, offset);
            offset += sizeof(int);
            count -= sizeof(int);

            // 4 = seq
            r.seq = BitConverter.ToInt32(buffer, offset);
            offset += sizeof(int);
            count -= sizeof(int);

            // 4 = code
            r.code = BitConverter.ToInt32(buffer, offset);
            offset += sizeof(int);
            count -= sizeof(int);

            // 4 = serialize type
            r.typeLength = BitConverter.ToInt32(buffer, offset);
            offset += sizeof(int);
            count -= sizeof(int);

            // 1 = require response
            r.requireResponse = buffer[offset] == 1;
            offset++;
            count--;

            r.success = true;
        }
        protected int GetHeaderLength()
        {
            // total length 4
            // requireResponse 1
            // seq 4
            // msgType or eCode 4
            // SerializeType 4
            return 1 + sizeof(int) * 4;
        }
        protected void WriteInt(byte[] buffer, int offset, int value)
        {
            buffer[offset + 0] = (byte)(value << 24 >> 24);
            buffer[offset + 1] = (byte)(value << 16 >> 24);
            buffer[offset + 2] = (byte)(value << 8 >> 24);
            buffer[offset + 3] = (byte)(value << 0 >> 24);
        }
        protected void PackHeader(byte[] buffer, ref int offset,
            int totalLength, int seq, int code, 
            int typeLength, bool requireResponse)
        {
            // 4 = total length
            //BitConverter.TryWriteBytes(new Span<byte>(buffer, offset, sizeof(int)), totalLength);
            this.WriteInt(buffer, offset, totalLength);
            offset += sizeof(int);

            // 4 = seq
            // BitConverter.TryWriteBytes(new Span<byte>(buffer, offset, sizeof(int)), seq);
            this.WriteInt(buffer, offset, seq);
            offset += sizeof(int);

            // 4 = code
            // BitConverter.TryWriteBytes(new Span<byte>(buffer, offset, sizeof(int)), code);
            this.WriteInt(buffer, offset, code);
            offset += sizeof(int);

            // 4 = serialize type
            // BitConverter.TryWriteBytes(new Span<byte>(buffer, offset, sizeof(int)), typeLength);
            this.WriteInt(buffer, offset, typeLength);
            offset += sizeof(int);

            // 1 = require response
            buffer[offset] = (requireResponse ? (byte)1 : (byte)0);
            offset++;
        }

        public abstract UnpackResult Unpack(byte[] buffer, ref int offset, int count);
        public abstract byte[] Pack(int msgTypeOrECode, object msg, int seq, bool requireResponse);
    }
}