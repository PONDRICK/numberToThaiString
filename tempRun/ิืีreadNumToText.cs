using System;
using System.Text;

class ThaiNumberReader
{
    static readonly string[] Ones = { "ศูนย์", "หนึ่ง", "สอง", "สาม", "สี่", "ห้า", "หก", "เจ็ด", "แปด", "เก้า" };
    static readonly string[] Places = { "", "สิบ", "ร้อย", "พัน", "หมื่น", "แสน", "ล้าน" };

    public static string ReadNumber(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return "ศูนย์";

        if (!decimal.TryParse(input, out decimal number))
            return "❌ รูปแบบตัวเลขไม่ถูกต้อง";

        if (number == 0)
            return "ศูนย์";

        bool isNegative = number < 0;
        number = Math.Abs(number);

        string[] parts = number.ToString("0.################").Split('.'); // ไม่ปัดเศษ
        string integerPart = parts[0];
        string decimalPart = parts.Length > 1 ? parts[1] : "";

        string result = (isNegative ? "ลบ" : "") + ReadIntegerPart(integerPart);

        if (!string.IsNullOrEmpty(decimalPart))
        {
            result += "จุด";
            foreach (char digit in decimalPart)
            {
                result += Ones[digit - '0'];
            }
        }

        return result;
    }

    private static string ReadIntegerPart(string numStr)
    {
        if (numStr == "0") return "ศูนย์";

        StringBuilder result = new StringBuilder();
        int len = numStr.Length;
        int group = 0;

        while (len > 0)
        {
            int groupLen = Math.Min(6, len);
            string segment = numStr.Substring(len - groupLen, groupLen);
            string read = ReadGroup(segment);

            if (group > 0 && !string.IsNullOrEmpty(read))
                result.Insert(0, "ล้าน");

            result.Insert(0, read);
            len -= groupLen;
            group++;
        }

        return result.ToString();
    }

    private static string ReadGroup(string segment)
    {
        StringBuilder result = new StringBuilder();
        int len = segment.Length;

        for (int i = 0; i < len; i++)
        {
            int digit = segment[i] - '0';
            int pos = len - i - 1;

            if (digit == 0) continue;

            if (pos == 0) // หน่วย
            {
                if (digit == 1 && len > 1)
                    result.Append("เอ็ด");
                else
                    result.Append(Ones[digit]);
            }
            else if (pos == 1) // สิบ
            {
                if (digit == 1)
                    result.Append("สิบ");
                else if (digit == 2)
                    result.Append("ยี่สิบ");
                else
                    result.Append(Ones[digit] + "สิบ");
            }
            else
            {
                result.Append(Ones[digit] + Places[pos]);
            }
        }

        return result.ToString();
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        while (true)
        {
            Console.Write("กรุณาใส่ตัวเลข (หรือพิมพ์ 'exit' เพื่อออก): ");
            string input = Console.ReadLine();

            if (input?.ToLower() == "exit")
                break;

            string result = ReadNumber(input);
            Console.WriteLine($"→ {result}\n");
        }
    }
}
