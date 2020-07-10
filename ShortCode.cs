using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

public static class ShortCode
{
    private static readonly char[] upperChar = {
            '2', '3', '4', '5', '6', '7', '8', '9',
            'A', 'B', 'C', 'D', 'E', 'F', 'G',
            'H', 'J', 'K', 'L', 'M', 'N',
            'U', 'V', 'W', 'X', 'Y', 'Z',
            'P', 'Q', 'R', 'S', 'T'
        };
    private static readonly char[] lowerChar = {
            'p', 'q', 'r', 's', 't',
            'h', 'j', 'k', 'l', 'm', 'n',
            'u', 'v', 'w', 'x', 'y', 'z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g',
            '2', '3', '4', '5', '6', '7', '8', '9'
        };
    private static readonly StringBuilder builder = new StringBuilder();
    private static readonly MD5 md5Hasher = MD5.Create();
    private static readonly Random random = new Random(Guid.NewGuid().GetHashCode());

    /// <summary>
    /// 获取字符串的MD5校验码
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static string GetMd5Str(string str)
    {
        // Convert the input string to a byte array and compute the hash.
        byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(str));
        return BitConverter.ToString(data).Replace("-", string.Empty);
    }

    /// <summary>
    /// 从区间内获取一个随机整数
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    private static int GetRandomInt(int min, int max)
    {
        return random.Next(min, max);
    }

    /// <summary>
    /// 生成4个不同的6位短码
    /// </summary>
    /// <param name="md5"></param>
    /// <returns></returns>
    public static List<string> GenerateShortCode(string md5)
    {
        List<string> result = new List<string>();

        long now = DateTime.Now.Ticks;
        string orgStr = GetMd5Str($"{md5}{now}").ToLower();
        // 将md5字符串分割为4个子串
        for (int i = 0; i < 32; i += 8)
        {
            // 取对应子串
            string subStr = orgStr.Substring(i, 8);
            // 将子串转为长整型（32bit）
            if (ulong.TryParse(subStr, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out ulong orgNum))
            {
                // 取随机数（6bit），用来决定6位短码各字符的源集合
                int randomInt = GetRandomInt(0, 1 << 6);
                // 因为要生成6位短码，所以32bit中使用每5bit生成1位短码，共使用30bit
                for (int j = 0; j < 30; j += 5)
                {
                    // 获取对应位置5bit数值作为索引
                    ulong index = (orgNum >> j) & 0x1f;
                    // 从源集合中取出对应索引的字符
                    builder.Append((randomInt & (1 << builder.Length)) == 0 ? upperChar[index] : lowerChar[index]);
                }
                result.Add(builder.ToString());
                builder.Clear();
            }
        }

        return result;
    }
}
