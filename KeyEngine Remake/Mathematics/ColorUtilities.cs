namespace KeyEngine.Mathematics
{
    public static class ColorUtilities
    {
        public static float ToFloat(byte component)
        {
            float value = (float)(component / 255f);
            return value;
        }

        public static byte ToByte(float component)
        {
            int value = (int)(component * 255f);
            return (byte)Mathf.Clamp(value, 0, 255);
        }
    }
}
