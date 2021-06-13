namespace Data
{
    public enum MaterialType
    {
        None,
        Money,
        Reinforced,
        Advanced
    }

    public enum CastleDirection
    {
        /**自己 */
        LeftToRight = 1,
        /**敌对方 */
        RightToLeft = -1
    }
}