using System.Security.Cryptography;
using System.Text;

namespace RepairCircle.Common;

public static class ReferenceCodeGenerator
{
    public static string CreateBorrowReference(DateTime utcNow)
        => $"BRW-{utcNow:yyyyMMdd}-{Convert.ToHexString(RandomNumberGenerator.GetBytes(4))}";

    public static string CreateRepairRequestReference(DateTime utcNow)
        => $"REQ-{utcNow:yyyyMMdd}-{Convert.ToHexString(RandomNumberGenerator.GetBytes(4))}";

    public static bool[] CreateVisualCells(string value, int size = 13)
    {
        if (size < 9)
        {
            size = 9;
        }

        var totalCells = size * size;
        var bits = new bool[totalCells];
        var input = Encoding.UTF8.GetBytes(value);
        var seed = SHA256.HashData(input);

        var offset = 0;
        var current = seed;
        while (offset < totalCells)
        {
            foreach (var b in current)
            {
                for (var bit = 0; bit < 8 && offset < totalCells; bit++)
                {
                    bits[offset++] = ((b >> bit) & 1) == 1;
                }
            }

            current = SHA256.HashData(current);
        }

        ApplyFinder(bits, size, 0, 0);
        ApplyFinder(bits, size, 0, size - 5);
        ApplyFinder(bits, size, size - 5, 0);

        return bits;
    }

    private static void ApplyFinder(bool[] bits, int size, int rowStart, int colStart)
    {
        for (var row = 0; row < 5; row++)
        {
            for (var col = 0; col < 5; col++)
            {
                var isBorder = row == 0 || row == 4 || col == 0 || col == 4;
                var isCenter = row is >= 1 and <= 3 && col is >= 1 and <= 3;
                bits[((rowStart + row) * size) + colStart + col] = isBorder || isCenter;
            }
        }
    }
}
