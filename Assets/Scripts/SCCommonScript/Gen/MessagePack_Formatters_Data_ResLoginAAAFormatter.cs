// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY MPC(MessagePack-CSharp). DO NOT CHANGE IT.
// </auto-generated>

#pragma warning disable 618
#pragma warning disable 612
#pragma warning disable 414
#pragma warning disable 168

#pragma warning disable SA1129 // Do not use default value type constructor
#pragma warning disable SA1200 // Using directives should be placed correctly
#pragma warning disable SA1309 // Field names should not begin with underscore
#pragma warning disable SA1312 // Variable names should begin with lower-case letter
#pragma warning disable SA1403 // File may only contain a single namespace
#pragma warning disable SA1649 // File name should match first type name

namespace MessagePack.Formatters.Data
{
    using System;
    using System.Buffers;
    using MessagePack;

    public sealed class ResLoginAAAFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Data.ResLoginAAA>
    {

        public void Serialize(ref MessagePackWriter writer, global::Data.ResLoginAAA value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(8);
            formatterResolver.GetFormatterWithVerify<string>().Serialize(ref writer, value.channel, options);
            formatterResolver.GetFormatterWithVerify<string>().Serialize(ref writer, value.channelUserId, options);
            writer.Write(value.playerId);
            writer.Write(value.pmId);
            formatterResolver.GetFormatterWithVerify<string>().Serialize(ref writer, value.pmIp, options);
            writer.Write(value.pmPort);
            formatterResolver.GetFormatterWithVerify<string>().Serialize(ref writer, value.pmToken, options);
            writer.Write(value.needUploadProfile);
        }

        public global::Data.ResLoginAAA Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __channel__ = default(string);
            var __channelUserId__ = default(string);
            var __playerId__ = default(int);
            var __pmId__ = default(int);
            var __pmIp__ = default(string);
            var __pmPort__ = default(int);
            var __pmToken__ = default(string);
            var __needUploadProfile__ = default(bool);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __channel__ = formatterResolver.GetFormatterWithVerify<string>().Deserialize(ref reader, options);
                        break;
                    case 1:
                        __channelUserId__ = formatterResolver.GetFormatterWithVerify<string>().Deserialize(ref reader, options);
                        break;
                    case 2:
                        __playerId__ = reader.ReadInt32();
                        break;
                    case 3:
                        __pmId__ = reader.ReadInt32();
                        break;
                    case 4:
                        __pmIp__ = formatterResolver.GetFormatterWithVerify<string>().Deserialize(ref reader, options);
                        break;
                    case 5:
                        __pmPort__ = reader.ReadInt32();
                        break;
                    case 6:
                        __pmToken__ = formatterResolver.GetFormatterWithVerify<string>().Deserialize(ref reader, options);
                        break;
                    case 7:
                        __needUploadProfile__ = reader.ReadBoolean();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Data.ResLoginAAA();
            ____result.channel = __channel__;
            ____result.channelUserId = __channelUserId__;
            ____result.playerId = __playerId__;
            ____result.pmId = __pmId__;
            ____result.pmIp = __pmIp__;
            ____result.pmPort = __pmPort__;
            ____result.pmToken = __pmToken__;
            ____result.needUploadProfile = __needUploadProfile__;
            reader.Depth--;
            return ____result;
        }
    }
}

#pragma warning restore 168
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 612

#pragma warning restore SA1129 // Do not use default value type constructor
#pragma warning restore SA1200 // Using directives should be placed correctly
#pragma warning restore SA1309 // Field names should not begin with underscore
#pragma warning restore SA1312 // Variable names should begin with lower-case letter
#pragma warning restore SA1403 // File may only contain a single namespace
#pragma warning restore SA1649 // File name should match first type name
