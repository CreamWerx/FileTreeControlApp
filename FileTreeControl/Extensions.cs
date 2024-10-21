using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTreeControl;
public static class Extensions
{
    public static string ToSizeString(this long length)
    {
        long KB = 1024, MB = KB * 1024, GB = MB * 1024;
        double size = length;
        string suffix = "B";

        if (length >= GB)
        {
            size = Round(length, GB);
            suffix = "GB";
        }
        else if (length >= MB)
        {
            size = Round(length, MB);
            suffix = "MB";
        }
        else if (length >= KB)
        {
            size = Round(length, KB);
            suffix = "KB";
        }

        return $"{size} {suffix}";
    }

    public static double Round(long length, long divisor)
    {
        return Math.Round((double)length / divisor, 2);
    }
}
