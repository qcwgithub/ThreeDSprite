using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MessagePack;
using Data;

namespace Script
{
    public partial class BinaryMessagePacker : IMessagePacker
    {
        private static BinaryMessagePacker instance;
        public static BinaryMessagePacker Instance
        {
            get
            {
                if (instance == null) instance = new BinaryMessagePacker();
                return instance;
            }
        }

        public bool IsCompeteMessage(byte[] buffer, int offset, int count)
        {
            // 4 = totalLength
            // 4 = seq
            // 4 = ECode/MsgType
            // 2 = MessageCode
            // 1 = require response?
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
        int GetHeaderLength()
        {
            return 3 * sizeof(int) + sizeof(short) + 1;
        }
        
        protected void WriteInt(byte[] buffer, int offset, int value)
        {
            buffer[offset + 0] = (byte)(value << 24 >> 24);
            buffer[offset + 1] = (byte)(value << 16 >> 24);
            buffer[offset + 2] = (byte)(value << 8 >> 24);
            buffer[offset + 3] = (byte)(value << 0 >> 24);
        }
        protected void WriteShort(byte[] buffer, int offset, short value)
        {
            buffer[offset + 0] = (byte)(value << 8 >> 8);
            buffer[offset + 1] = (byte)(value << 0 >> 8);
        }
        public byte[] Pack<T>(int msgTypeOrECode, T msg, int seq, bool requireResponse)
        {
            var messageCode = TypeToMessageCodeCache<T>.messageCode;
            var bytes = MessagePackSerializer.Serialize<T>(msg);
            int totalLength = this.GetHeaderLength() + sizeof(int) + bytes.Length;

            var buffer = new byte[totalLength];

            int offset = 0;
            
            // 4 = totalLength
            this.WriteInt(buffer, offset, totalLength);
            offset += sizeof(int);

            
            // 4 = seq
            this.WriteInt(buffer, offset, seq);
            offset += sizeof(int);

            // 4 = ECode/MsgType
            this.WriteInt(buffer, offset, msgTypeOrECode);
            offset += sizeof(int);

            // 2 = MessageCode
            this.WriteShort(buffer, offset, (short)messageCode);
            offset += 2;

            // 1 = require response?
            buffer[offset] = (requireResponse ? (byte)1 : (byte)0);
            offset++;

            Array.Copy(bytes, 0, buffer, offset, bytes.Length);
            offset += bytes.Length;

            return buffer;
        }

        public UnpackResult Unpack(byte[] buffer, ref int offset, int count)
        {
            var r = new UnpackResult();
            
            // 4 = total length
            r.totalLength = BitConverter.ToInt32(buffer, offset);
            offset += sizeof(int);
            count -= sizeof(int);

            // 4 = seq
            r.seq = BitConverter.ToInt32(buffer, offset);
            offset += sizeof(int);
            count -= sizeof(int);

            // 4 = ECode/MsgType
            r.code = BitConverter.ToInt32(buffer, offset);
            offset += sizeof(int);
            count -= sizeof(int);

            // 2 = MessageCode
            r.typeLength = 0;
            r.messageCode = (MessageCode) BitConverter.ToInt16(buffer, offset);
            offset += sizeof(int);
            count -= sizeof(int);

            // 1 = require response
            r.requireResponse = buffer[offset] == 1;
            offset++;
            count--;

            r.success = true;
            return r;
        }
    }
}