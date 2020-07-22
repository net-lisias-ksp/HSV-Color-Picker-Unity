using UnityEngine;
using System;

public static class HSVUtil
{
    public static HsvColor ConvertRgbToHsv(Color color) =>
        ConvertRgbToHsv((int)(color.r * 255), (int)(color.g * 255), (int)(color.b * 255));

    //Converts an RGB color to an HSV color.
    private static HsvColor ConvertRgbToHsv(double r, double b, double g)
    {
        var min = Math.Min(Math.Min(r, g), b);
        var v = Math.Max(Math.Max(r, g), b);
        var delta = v - min;
        var s = v.Equals(0) ? 0 : delta / v;
        double h = 0;
        if(s.Equals(0))
            h = 360;
        else
        {
            if(r.Equals(v))
                h = (g - b) / delta;
            else if(g.Equals(v))
                h = 2 + (b - r) / delta;
            else if(b.Equals(v))
                h = 4 + (r - g) / delta;

            h *= 60;
            if(h <= 0.0)
                h += 360;
        }
        return new HsvColor { H = 360 - h, S = s, V = v / 255 };
    }

    // Converts an HSV color to an RGB color.
    public static Color ConvertHsvToRgb(double h, double s, double v, float alpha)
    {
        double r, g, b;
        if(s.Equals(0))
        {
            r = v;
            g = v;
            b = v;
        }
        else
        {
            if(h.Equals(360))
                h = 0;
            else
                h /= 60;
            var i = (int)h;
            var f = h - i;
            var p = v * (1.0 - s);
            var q = v * (1.0 - s * f);
            var t = v * (1.0 - s * (1.0f - f));
            switch(i)
            {
                case 0:
                    r = v;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = v;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = v;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = v;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = v;
                    break;
                default:
                    r = v;
                    g = p;
                    b = q;
                    break;
            }
        }
        return new Color((float)r, (float)g, (float)b, alpha);
    }
}
