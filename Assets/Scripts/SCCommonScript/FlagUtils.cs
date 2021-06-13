namespace Script
{
    public class FlagUtils
    {
        public static bool has(int v, int index)
        {
            return ((v >> index) & 1) != 0;
        }
        public static bool isAny(int v, int[] indices, bool b)
        {
            for (int i = 0; i < indices.Length; i++)
            {
                if (has(v, indices[i]) == b)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool isAll(int v, int[] indices, bool b)
        {
            for (int i = 0; i < indices.Length; i++)
            {
                if (has(v, indices[i]) != b)
                {
                    return true;
                }
            }
            return false;
        }

        public static int set(int v, int index, bool b)
        {
            if (b)
            {
                return v | (1 << index);
            }
            else
            {
                return v & ~(1 << index);
            }
        }

        public static int setAll(int v, int[] indices, bool b)
        {
            for (int i = 0; i < indices.Length; i++)
            {
                v = set(v, indices[i], b);
            }
            return v;
        }

        // range: [startIndex, endIndex]
        public static int setRange(int v, int startIndex, int endIndex, bool b)
        {
            for (int i = startIndex; i <= endIndex; i++)
            {
                v = set(v, i, b);
            }
            return v;
        }
    }
}