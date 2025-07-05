public class Math
{
    static double stween(double value, double target, float smoothness)
    {
        return value + (target - value) * smoothness;
    }
}