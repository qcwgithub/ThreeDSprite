using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public interface btIHasPosition
    {
        Vector3 prePos { get; set; }
        Vector3 pos { get; set; }
    }
}