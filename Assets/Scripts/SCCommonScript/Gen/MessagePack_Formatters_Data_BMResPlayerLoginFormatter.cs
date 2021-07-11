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

    public sealed class BMResPlayerLoginFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Data.BMResPlayerLogin>
    {

        public void Serialize(ref MessagePackWriter writer, global::Data.BMResPlayerLogin value, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }

            IFormatterResolver formatterResolver = options.Resolver;
            writer.WriteArrayHeader(2);
            formatterResolver.GetFormatterWithVerify<global::Data.BMBattleInfo>().Serialize(ref writer, value.battle, options);
            formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.Dictionary<int, global::Data.MCharacter>>().Serialize(ref writer, value.characterDict, options);
        }

        public global::Data.BMResPlayerLogin Deserialize(ref MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return null;
            }

            options.Security.DepthStep(ref reader);
            IFormatterResolver formatterResolver = options.Resolver;
            var length = reader.ReadArrayHeader();
            var __battle__ = default(global::Data.BMBattleInfo);
            var __characterDict__ = default(global::System.Collections.Generic.Dictionary<int, global::Data.MCharacter>);

            for (int i = 0; i < length; i++)
            {
                switch (i)
                {
                    case 0:
                        __battle__ = formatterResolver.GetFormatterWithVerify<global::Data.BMBattleInfo>().Deserialize(ref reader, options);
                        break;
                    case 1:
                        __characterDict__ = formatterResolver.GetFormatterWithVerify<global::System.Collections.Generic.Dictionary<int, global::Data.MCharacter>>().Deserialize(ref reader, options);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            var ____result = new global::Data.BMResPlayerLogin();
            ____result.battle = __battle__;
            ____result.characterDict = __characterDict__;
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
