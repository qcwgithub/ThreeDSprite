namespace Data
{
    // 为什么要有 shape
    // 1 用于生成碰撞体，进而用于触发事件、行走、障碍
    // 2 用于计算 sprite 的 pivot，因此也影响他的 position。如果只是这个目的，生成完 prefab 后可以丢弃 shape
    public enum btShape
    {
        cube,
        xy,
        xz,
    }
}