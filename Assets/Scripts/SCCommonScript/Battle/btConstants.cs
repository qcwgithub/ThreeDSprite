using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class btConstants
{
    public const float sqrt2 = 1.414213562373095f;
    public static readonly Quaternion sprite_rotation = Quaternion.Euler(45f, 0f, 0f);
    public const float pixels_per_unit = 100f;
    public const SpriteSortPoint sprite_sort_point = SpriteSortPoint.Pivot;
    public static readonly Vector2 sprite_pivot_cube = new Vector2(0.5f, 0f);
    public static readonly Vector2 sprite_pivot_xy = new Vector2(0.5f, 0f);
    public static readonly Vector2 sprite_pivot_xz = new Vector2(0.5f, 1f);
}
