using System.Text.RegularExpressions;

namespace KPClassLibrary
{
    public static class KPValidations
    {
        public static string KPCapitalize(string data)
        {

            string result;
            if (string.IsNullOrEmpty(data))
            {
                return "";
            }
            else
            {
                result = data.Trim().ToLower();

                if (result.Length == 1)
                    result = char.ToUpper(result[0]).ToString();
                else
                    result = char.ToUpper(result[0]) + result.Substring(1);
            }
            return result;
        }
        public static string KPExtractDigits(string data)
        {
            string result = "";
            if (!string.IsNullOrEmpty(data))
            {
                for (int i = 0; i < data.Length; i++)
                {
                    if (Char.IsDigit(data[i]))
                        result += data[i];
                }
            }

            return result;
        }

        public static bool KPPostalCodeValidation(string data)
        {
            if (!string.IsNullOrWhiteSpace(data))
            {
                Regex pattern = new Regex(@"^[A-Za-z]\d[A-Za-z] ?\d[A-Za-z]\d$", RegexOptions.IgnoreCase);
                if (pattern.IsMatch(data.ToString()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;


        }
        public static string KPPostalCodeFormat(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                if (KPPostalCodeValidation(data))
                {
                    if (data.Contains(" "))
                    {
                        return data;
                    }
                    else
                    {
                        data = data.Substring(0, 3) + " " + data.Substring(3, 3);
                        data = data.ToUpper();
                    }
                }
            }
            return data;
        }

        public static bool KPZipCodeValidation(string data)
        {
            if (!string.IsNullOrWhiteSpace(data))
            {
                data = KPExtractDigits(data);
                if (data.Length == 5)
                {
                    return true;
                }
                else if (data.Length == 9)
                {
                    data = data.Substring(0, 5) + "-" + data.Substring(5, 4);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
