/// <summary>
/// Describes a color in terms of
/// Hue, Saturation, and Value (brightness)
/// </summary>
public struct HsvColor
{
    /// <summary>
    /// The Hue, ranges between 0 and 360
    /// </summary>
    public double H;

    /// <summary>
    /// The saturation, ranges between 0 and 1
    /// </summary>
    public double S;

    // The value (brightness), ranges between 0 and 1
    public double V;

    public float normalizedH { get => (float)H / 360f; set => H = (double)value * 360; }

    public float normalizedS { get => (float)S; set => S = value; }

    public float normalizedV { get => (float)V; set => V = value; }

    public HsvColor(double h, double s, double v)
    {
        H = h;
        S = s;
        V = v;
    }

    public override string ToString() => $"{{{H:f2},{S:f2},{V:f2}}}";
}
