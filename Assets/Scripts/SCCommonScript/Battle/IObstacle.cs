using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IObstacle
{
    bool LimitMove(Vector3 from, ref Vector3 delta);
}