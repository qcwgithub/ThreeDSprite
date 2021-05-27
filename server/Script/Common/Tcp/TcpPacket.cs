using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Model
{
    public enum ParserState
    {
        PacketSize,
        PacketBody
    }

    public abstract class NetPacket
    {
        public const int PacketSizeLength2 = 2;
        public const int PacketSizeLength4 = 4;
        public const int MinPacketSize = 2;
        public const int OpcodeIndex = 0;
        public const int MessageIndex = 2;
    }

    public class TcpPacket : NetPacket
    {
        private readonly RingBuffer buffer;
        private int packetSize;
        private ParserState state;
        public MemoryStream memoryStream;
        private bool isOK;
        public readonly int packetSizeLength;

        public TcpPacket(int packetSizeLength, RingBuffer buffer, MemoryStream memoryStream)
        {
            this.packetSizeLength = packetSizeLength;
            this.buffer = buffer;
            this.memoryStream = memoryStream;
        }

        public bool Parse()
        {
            if (this.isOK)
            {
                return true;
            }

            bool finish = false;
            while (!finish)
            {
                switch (this.state)
                {
                    case ParserState.PacketSize:
                        if (this.buffer.Length < this.packetSizeLength)
                        {
                            finish = true;
                        }
                        else
                        {
                            this.buffer.Read(this.memoryStream.GetBuffer(), 0, this.packetSizeLength);

                            switch (this.packetSizeLength)
                            {
                                case PacketSizeLength4:
                                    this.packetSize = BitConverter.ToInt32(this.memoryStream.GetBuffer(), 0);
                                    if (this.packetSize > ushort.MaxValue * 16 || this.packetSize < MinPacketSize)
                                    {
                                        throw new Exception($"recv packet size error, 可能是外网探测端口: {this.packetSize}");
                                    }
                                    break;
                                case PacketSizeLength2:
                                    this.packetSize = BitConverter.ToUInt16(this.memoryStream.GetBuffer(), 0);
                                    if (this.packetSize > ushort.MaxValue || this.packetSize < MinPacketSize)
                                    {
                                        throw new Exception($"recv packet size error:, 可能是外网探测端口: {this.packetSize}");
                                    }
                                    break;
                                default:
                                    throw new Exception("packet size byte count must be 2 or 4!");
                            }
                            this.state = ParserState.PacketBody;
                        }
                        break;
                    case ParserState.PacketBody:
                        if (this.buffer.Length < this.packetSize)
                        {
                            finish = true;
                        }
                        else
                        {
                            this.memoryStream.Seek(0, SeekOrigin.Begin);
                            this.memoryStream.SetLength(this.packetSize);
                            byte[] bytes = this.memoryStream.GetBuffer();
                            this.buffer.Read(bytes, 0, this.packetSize);
                            this.isOK = true;
                            this.state = ParserState.PacketSize;
                            finish = true;
                        }
                        break;
                }
            }
            return this.isOK;
        }

        public MemoryStream GetPacket()
        {
            this.isOK = false;
            return this.memoryStream;
        }
    }
}
