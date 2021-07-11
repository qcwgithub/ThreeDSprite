using System.Collections;
using System.Collections.Generic;

namespace Data
{
    public interface IBinarySerializable
    {
        MessageCode code { get; }
    }
}