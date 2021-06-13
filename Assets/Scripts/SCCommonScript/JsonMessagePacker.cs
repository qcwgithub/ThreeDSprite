using Data;
using System.Text;
using System;
using System.Collections.Generic;

namespace Script
{
    public abstract class JsonMessagePacker : MessageHeaderPacker
    {
        protected abstract JsonUtils JSON { get; }
        protected abstract Dictionary<string, Type> name2Type { get; }
        protected abstract void onError(string msg);

        public override byte[] Pack(int msgTypeOrECode, object _msg, int seq, bool requireResponse)
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