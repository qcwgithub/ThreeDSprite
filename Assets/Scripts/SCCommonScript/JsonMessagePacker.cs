using Data;
using System.Text;
using System;
using System.Collections.Generic;

namespace Script
{

    public abstract class JsonMessageHeaderPacker : IMessagePacker
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
        public abstract byte[] Pack<T>(int msgTypeOrECode, T msg, int seq, bool requireResponse);
    }

    public abstract class JsonMessagePacker : JsonMessageHeaderPacker
    {
        protected abstract JsonUtils JSON { get; }
        protected abstract Dictionary<string, Type> name2Type { get; }
        protected abstract void onError(string msg);

        public override byte[] Pack<T>(int msgTypeOrECode,  T _msg, int seq, bool requireResponse)
        {
            string typeStr = null;
            int typeLength = 0;

            string msgStr = null;
            int msgLength = 0;
            // SerializeType serializeType = SerializeType.NULL;
            if (_msg == null)
            {
                typeStr = "null";
                typeLength = Encoding.UTF8.GetByteCount(typeStr);
            }
            else
            {
                var type = _msg.GetType();

                typeStr = type.Name;
                typeLength = Encoding.UTF8.GetByteCount(typeStr);

                msgStr = this.JSON.stringify(_msg);
                msgLength = Encoding.UTF8.GetByteCount(msgStr);
            }

            int totalLength = this.GetHeaderLength() + typeLength + msgLength;

            var buffer = new byte[totalLength];
            //this.server.logger.InfoFormat("Pack seq={0} msgTypeOrECode={1} totalLength={2}", seq, msgTypeOrECode, buffer.Length);

            int offset = 0;
            this.PackHeader(buffer, ref offset, totalLength, seq, msgTypeOrECode, typeLength, requireResponse);

            if (typeStr != null)
            {
                Encoding.UTF8.GetBytes(typeStr, 0, typeStr.Length, buffer, offset);
                offset += typeLength;
            }

            if (msgStr != null)
            {
                Encoding.UTF8.GetBytes(msgStr, 0, msgStr.Length, buffer, offset);
                offset += msgLength;
            }

            return buffer;
        }

        public override UnpackResult Unpack(byte[] buffer, ref int offset, int count)
        {
            var r = new UnpackResult();
            base.UnpackHeader(buffer, ref offset, ref count, ref r);
            if (!r.success)
            {
                return r;
            }
            //this.server.logger.InfoFormat("Unpack seq={0} msgTypeOrECode={1} totalLength={2}", r.seq, r.code, r.totalLength);

            var typeStr = Encoding.UTF8.GetString(buffer, offset, r.typeLength);
            offset += r.typeLength;
            count -= r.typeLength;

            var msgStr = Encoding.UTF8.GetString(buffer, offset, count);
            offset += r.typeLength;
            count -= r.typeLength;

            // 传出去
            offset = r.totalLength;

            r.msg = this.Deserialize(typeStr, msgStr);
            r.success = true;
            return r;
        }

        protected object Deserialize(string typeStr, string msgStr)
        {
            if (typeStr == "null")
            {
                return null;
            }

            var JSON = this.JSON;
            try
            {
                Type type;
                if (!this.name2Type.TryGetValue(typeStr, out type))
                {
                    type = Type.GetType(typeStr);
                    this.name2Type.Add(typeStr, type);
                }

                if (type == null)
                {
                    this.onError(string.Format("type==null, typeStr={0}, msgStr={1}", typeStr, msgStr));
                }

                object obj = JSON.parse(msgStr, type);
                return obj;
            }
            catch (Exception ex)
            {
                this.onError(string.Format("{0} {1}\n{2}", typeStr, msgStr, ex));
            }
            return null;
        }
    }
}