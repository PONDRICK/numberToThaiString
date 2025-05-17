using System;
using System.Text;

class ThaiNumberReader
{
    // ตัวเลขหลักหน่วยในภาษาไทย
    static readonly string[] ThaiDigits = { "ศูนย์", "หนึ่ง", "สอง", "สาม", "สี่", "ห้า", "หก", "เจ็ด", "แปด", "เก้า" };

    // หน่วยของตัวเลขในแต่ละหลัก
    static readonly string[] ThaiPlaces = { "", "สิบ", "ร้อย", "พัน", "หมื่น", "แสน", "ล้าน" };

    /// <summary>
    /// แปลงตัวเลขเป็นข้อความภาษาไทย เช่น 123 -> "หนึ่งร้อยยี่สิบสาม"
    /// </summary>
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

        string[] parts = number.ToString("0.################").Split('.');
        string integerPart = parts[0];
        string decimalPart = parts.Length > 1 ? parts[1] : "";

        // เรียกฟังก์ชันแปลงเลขจำนวนเต็ม
        string result = (isNegative ? "ลบ" : "") + ConvertIntegerToThaiText(integerPart);

        // แปลงจุดทศนิยมถ้ามี
        if (!string.IsNullOrEmpty(decimalPart))
        {
            result += "จุด";
            foreach (char digitChar in decimalPart)
            {
                int digit = digitChar - '0';
                result += ThaiDigits[digit];
            }
        }

        return result;
    }

    /// <summary>
    /// แปลงส่วนจำนวนเต็มเป็นคำอ่านภาษาไทย รองรับตัวเลขหลายหลักและหลักล้านขึ้นไป
    /// </summary>
    private static string ConvertIntegerToThaiText(string numberStr)
    {
        if (numberStr == "0") return "ศูนย์";

        StringBuilder result = new StringBuilder();
        int remainingLength = numberStr.Length;
        int groupCount = 0;

        while (remainingLength > 0)
        {
            int currentGroupLength = Math.Min(6, remainingLength);
            string currentGroup = numberStr.Substring(remainingLength - currentGroupLength, currentGroupLength);

            string groupText = ReadGroupOfDigits(currentGroup);

            if (groupCount > 0 && !string.IsNullOrEmpty(groupText))
                result.Insert(0, "ล้าน");

            result.Insert(0, groupText);
            remainingLength -= currentGroupLength;
            groupCount++;
        }

        return result.ToString();
    }

    /// <summary>
    /// อ่านกลุ่มตัวเลข (สูงสุด 6 หลัก) เช่น "123456" -> "หนึ่งแสนสองหมื่นสามพัน..."
    /// </summary>
    private static string ReadGroupOfDigits(string digits)
    {
        StringBuilder result = new StringBuilder();
        int length = digits.Length;

        for (int i = 0; i < length; i++)
        {
            int digit = digits[i] - '0';
            int position = length - i - 1;

            if (digit == 0)
                continue;

            if (position == 0) // หลักหน่วย
            {
                if (digit == 1 && length > 1)
                    result.Append("เอ็ด");
                else
                    result.Append(ThaiDigits[digit]);
            }
            else if (position == 1) // หลักสิบ
            {
                if (digit == 1)
                    result.Append("สิบ");
                else if (digit == 2)
                    result.Append("ยี่สิบ");
                else
                    result.Append(ThaiDigits[digit] + "สิบ");
            }
            else // หลักร้อย พัน หมื่น แสน
            {
                result.Append(ThaiDigits[digit] + ThaiPlaces[position]);
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// ฟังก์ชันหลักสำหรับรับอินพุตจากผู้ใช้และแสดงผล
    /// </summary>
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        while (true)
        {
            Console.Write("กรุณาใส่ตัวเลข (หรือพิมพ์ 'exit' เพื่อออก): ");
            string input = Console.ReadLine();

            if (input?.Trim().ToLower() == "exit")
                break;

            string thaiText = ReadNumber(input);
            Console.WriteLine($"→ {thaiText}\n");
        }
    }
}
