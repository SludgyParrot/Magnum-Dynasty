public static class MathsExentions
{
    public static int ToMilliseconds(this float value)
        => (int) (value * 1000);

    public static float ToSeconds(this int value)
        => (float) (value / 1000);
}
