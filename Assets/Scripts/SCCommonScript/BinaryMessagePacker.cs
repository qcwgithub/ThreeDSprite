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

            int length = this.ReadInt(buffer, offset);
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
        
        protected void WriteInt(byte[] buffer, int offset, int value_)
        {
            uint value = (uint)value_;
            buffer[offset + 0] = (byte)(value);
            buffer[offset + 1] = (byte)(value >> 8);
            buffer[offset + 2] = (byte)(value >> 16);
            buffer[offset + 3] = (byte)(value >> 24);
        }
        protected int ReadInt(byte[] buffer, int offset)
        {
            uint value = 0;
            value += buffer[offset];
            value += (((uint)buffer[offset + 1]) << 8);
            value += (((uint)buffer[offset + 2]) << 16);
            value += (((uint)buffer[offset + 3]) << 24);
            return (int)value;
        }
        protected void WriteShort(byte[] buffer, int offset, short value)
        {
            buffer[offset + 0] = (byte)(value << 8 >> 8);
            buffer[offset + 1] = (byte)(value << 0 >> 8);
        }
        protected short ReadShort(byte[] buffer, int offset)
        {
            ushort value = 0;
            value += buffer[offset];
            value += (ushort)(((ushort)buffer[offset + 1]) << 8);
            return (short)value;
        }
        public byte[] Pack(int msgTypeOrECode, object msg, int seq, bool requireResponse)
        {
            var messageCode = TypeToMessageCodeCache.getMessageCode(msg);
            var bytes = this.PackBody(messageCode, msg);
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
            offset += sizeof(short);

            // 1 = require response?
            buffer[offset] = (requireResponse ? (byte)1 : (byte)0);
            offset++;

            Array.Copy(bytes, 0, buffer, offset, bytes.Length);
            offset += bytes.Length;

            return buffer;
        }

        public UnpackResult Unpack(byte[] buffer, int offset, int count)
        {
            var r = new UnpackResult();

            int startOffset = offset;
            
            // 4 = total length
            r.totalLength = this.ReadInt(buffer, offset);
            offset += sizeof(int);
            count -= sizeof(int);

            // 4 = seq
            r.seq = this.ReadInt(buffer, offset);
            offset += sizeof(int);
            count -= sizeof(int);

            // 4 = ECode/MsgType
            r.code = this.ReadInt(buffer, offset);
            offset += sizeof(int);
            count -= sizeof(int);

            // 2 = MessageCode
            r.typeLength = 0;
            r.messageCode = (MessageCode) this.ReadShort(buffer, offset);
            offset += sizeof(short);
            count -= sizeof(short);

            // 1 = require response
            r.requireResponse = buffer[offset] == 1;
            offset++;
            count--;

            r.msg = this.UnpackBody(r.messageCode, buffer, offset, count);
            offset = startOffset + r.totalLength;

            r.success = true;
            return r;
        }
    }
}