using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace perfAppChk
{
    static class Program
    {


        private readonly static string[] CLEANME = new string[] { "AbC" };

        internal static string ReturnCleanedString(string cleanMe, string replacement)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(cleanMe);

            string s = CLEANME[0];
            int index = sb.ToString().IndexOf(s, StringComparison.OrdinalIgnoreCase);
            int lengthToRemove = s.Length;

            while (index != -1)
            {
                string foundIt = sb.ToString().Substring(index, lengthToRemove);
                string temp = sb.ToString().Replace(foundIt, replacement);

                sb.Remove(0, sb.Length);
                sb.Append(temp);

                index = sb.ToString().IndexOf(s, StringComparison.OrdinalIgnoreCase);
            }

            return sb.ToString();
        }

        public static string ReplaceExPrm(string original,
                    string pattern, string replacement)
        {
            int count, position0, position1;
            count = position0 = position1 = 0;
            string upperString = original.ToUpper();
            string upperPattern = pattern.ToUpper();
            int inc = (original.Length / pattern.Length) *
                      (replacement.Length - pattern.Length);
            char[] chars = new char[original.Length + Math.Max(0, inc)];
            while ((position1 = upperString.IndexOf(upperPattern,
                                              position0)) != -1)
            {
                for (int i = position0; i < position1; ++i)
                    chars[count++] = original[i];
                for (int i = 0; i < replacement.Length; ++i)
                    chars[count++] = replacement[i];
                position0 = position1 + pattern.Length;
            }
            if (position0 == 0) return original;
            for (int i = position0; i < original.Length; ++i)
                chars[count++] = original[i];
            return new string(chars, 0, count);
        }

        static public string ReplaceEx(this string original, string pattern, string replacement, StringComparison comparisonType)
        {
            return ReplaceEx(original, pattern, replacement, comparisonType, -1);
        }

        static public string ReplaceEx(this string original, string pattern, string replacement, StringComparison comparisonType, int stringBuilderInitialSize)
        {
            if (original == null)
            {
                return null;
            }

            if (String.IsNullOrEmpty(pattern))
            {
                return original;
            }


            int posCurrent = 0;
            int lenPattern = pattern.Length;
            int idxNext = original.IndexOf(pattern, comparisonType);
            StringBuilder result = new StringBuilder(stringBuilderInitialSize < 0 ? Math.Min(4096, original.Length) : stringBuilderInitialSize);

            while (idxNext >= 0)
            {
                result.Append(original, posCurrent, idxNext - posCurrent);
                result.Append(replacement);

                posCurrent = idxNext + lenPattern;

                idxNext = original.IndexOf(pattern, posCurrent, comparisonType);
            }

            result.Append(original, posCurrent, original.Length - posCurrent);

            return result.ToString();
        }

        static void Main(string[] args)
        {
            string str1 = null;
            string str2 = null;
            string str3 = str1 + str2 + Environment.NewLine;
            Console.WriteLine(str3);
            Console.WriteLine(str2);
            string C_APDU = null;
            Console.WriteLine("C_APDU: " + (C_APDU != null ? C_APDU : "null"));

            Console.SetBufferSize(80, 500);
            for (int iAll = 0; iAll < 100; iAll++)
            {
                Console.WriteLine("Pomiar:"+iAll);   

            string segment = "ABCabcAbCaBcAbcabCABCAbcaBC";
            string source;
            string pattern = "AbC";
            string destination = "";

            const long count = 1000;
            StringBuilder pressure = new StringBuilder();
            Stopwatch time = new Stopwatch();

            for (int i = 0; i < count; i++)
            {
                pressure.Append(segment);
            }
            source = pressure.ToString();
            GC.Collect();

            //regexp
            time = new Stopwatch();
            time.Start();
            string result = Regex.Replace(source, pattern,
                          destination, RegexOptions.IgnoreCase);
            time.Stop();

            string RegexExample =    "regexp       = " + time.Elapsed.ToString() + "s";
            GC.Collect();

            // ReplaceEx
            time = new Stopwatch();
            time.Start();
            string resultReplaceEx = ReplaceExPrm(source, pattern, destination);
            time.Stop();

            string ReplaceExString = "ReplaceExPrm = " + time.Elapsed.ToString() + "s";
            GC.Collect();

            // Replace
            time = new Stopwatch();
            time.Start();
            string resultReplace = source.Replace(pattern.ToLower(), destination);
            time.Stop();

            string Replace         = "Replace      = " + time.Elapsed.ToString() + "s";
            GC.Collect();

            time = new Stopwatch();
            time.Start();
            string resultMine = ReturnCleanedString(source, "");
            time.Stop();

            string MyVersion       = "My version   = " + time.Elapsed.ToString() + "s";
            GC.Collect();

            StringBuilder XYZ = new StringBuilder(4096);
            XYZ.Append(source);
            XYZ.Replace('a', 'B');


            time = new Stopwatch();
            time.Start();
            string resultEpnerOIC000 = ReplaceEx(source, "Original", "Replacement", StringComparison.OrdinalIgnoreCase);
            time.Stop();

            string EpnerOIC000        = "ReplaceEx OIC= " + time.Elapsed.ToString() + "s  <- (nie brac pod uwage) wszystkie jako pierwsze maja podobnie dlugi czas ";
            GC.Collect();

            time = new Stopwatch();
            time.Start();
            string resultEpnerOIC = ReplaceEx(source, "Original", "Replacement", StringComparison.OrdinalIgnoreCase);
            time.Stop();

            string EpnerOIC = "ReplaceEx OIC= " + time.Elapsed.ToString() + "s";
            GC.Collect();
            time = new Stopwatch();
            time.Start();
            string resultEpnerO = ReplaceEx(source, "Original", "Replacement", StringComparison.Ordinal); //  Ordinal -" porzadkowy", Compare strings using ordinal sort rules
            time.Stop();

            string EpnerO =          "ReplaceEx    = " + time.Elapsed.ToString() + "s";
            GC.Collect();


            time = new Stopwatch();
            time.Start();
            string resultEpnerICIC = ReplaceEx(source, "Original", "Replacement", StringComparison.InvariantCultureIgnoreCase);
            time.Stop();

            string EpnerICIC =       "ReplaceExICIC= " + time.Elapsed.ToString() + "s";
            GC.Collect();

            time = new Stopwatch();
            time.Start();
            string resultEpnerIC = ReplaceEx(source, "Original", "Replacement", StringComparison.InvariantCulture);  // invariant - "niezmienny"
            time.Stop();

            string EpnerIC =         "ReplaceEx IC = " + time.Elapsed.ToString() + "s";
            GC.Collect();


            time = new Stopwatch();
            time.Start();
            string resultEpnerCCIC = ReplaceEx(source, "Original", "Replacement", StringComparison.CurrentCultureIgnoreCase);
            time.Stop();

            string EpnerCCIC =       "ReplaceExCCIC= " + time.Elapsed.ToString() + "s";
            GC.Collect();

            time = new Stopwatch();
            time.Start();
            string resultEpnerCC = ReplaceEx(source, "Original", "Replacement", StringComparison.CurrentCulture);
            time.Stop();

            string EpnerCC =         "ReplaceEx CC = " + time.Elapsed.ToString() + "s";
            GC.Collect();

            byte[] apdu = {
                 0x00, 0xE0, 0x00, 0x00, 0x18, 0x62, 0x16, 0x82,
                 0x01, 0x38, 0x83, 0x02, 0x96, 0x02, 0x84, 0x05,
                 0x01, 0x02, 0x03, 0x04, 0x05, 0x85, 0x01, 0x09,
                 0x8C, 0x03, 0x03, 0x00, 0x00};
            time = new Stopwatch();
            time.Start();
            string bitCnv = BitConverter.ToString(apdu);
            time.Stop();
            string bitCnvMsg = "BitConverter = " + time.Elapsed.ToString() + "s";
            GC.Collect();

            time = new Stopwatch();
            time.Start();
            string bitCnvAndRepl = BitConverter.ToString(apdu).Replace("-", "");
            //responseString = responseString.Replace("-", separator); 
            time.Stop();
            string bitCnvAndReplMsg = "BitCvr & Rpl = " + time.Elapsed.ToString() + "s";
            GC.Collect();

            time = new Stopwatch();
            time.Start();
            string aaaaByteArrayToHexString = ByteArrayToHexString(apdu, true);
            time.Stop();
            string ByteArrayToHexStringMsg = "ByteArrayToHexString = " + time.Elapsed.ToString() + "s";
            GC.Collect();

            StringBuilder myTimeComparison = new StringBuilder();
            myTimeComparison.Append(RegexExample + Environment.NewLine +
                ReplaceExString + Environment.NewLine +
                Replace + Environment.NewLine +
                MyVersion + Environment.NewLine +
                EpnerOIC000 + Environment.NewLine +
                EpnerOIC + Environment.NewLine + 
                EpnerO + Environment.NewLine +
                EpnerICIC + Environment.NewLine +
                EpnerIC + Environment.NewLine +
                EpnerCCIC + Environment.NewLine +
                EpnerCC + Environment.NewLine +
                bitCnvMsg + Environment.NewLine +
                bitCnvAndReplMsg + Environment.NewLine +
                ByteArrayToHexStringMsg + Environment.NewLine);

            Console.WriteLine(myTimeComparison);
            }
            Console.WriteLine(" ReplaceEx jest najszybsze, 2-3x dluzej jest z opcja ignore case (poza tym wszystkie StringComparison... podobnie) ");
            Console.ReadKey();
        }

        /// <summary>
        /// Przekształcenie tablicy bajtów w łańcuch cyfr heksadecymalnych
        /// </summary>
        /// <param name="bytes">Ciąg bajtów</param>
        /// <param name="bTrimButMarkZerosOnRight">Czy usunac zerowe bajty z prawej strony (ale oznacz: ilosc_zerowych_bajtow x(00) ) </param>
        /// <returns>Łańcuch cyfr heksadecymalnych</returns>
        /// <remarks>Wynikowy ciąg cyfr heksadecymalnych jest pozbawiony sepratorów i nie zawiera białych znaków.</remarks>
        /// <exception cref="ArgumentNullException">Wywołano metodę nie podając ciągu bajtów</exception>
        public static String ByteArrayToHexString(
            Byte[] bytes, bool bTrimButMarkZerosOnRight)
        {

            string retStr = ByteArrayToHexString(bytes, true, " ");

            if (bTrimButMarkZerosOnRight)
            {
                int i = retStr.Length - 1;
                while (i > 1)
                {
                    if ((retStr[i] != '0') || (retStr[i - 1] != '0'))
                    {
                        break;
                    }
                    i -= 3;
                }

                if (retStr.Length != i + 1)
                {
                    int zeroAmount = ((retStr.Length - i) / 3);
                    if (zeroAmount > 3)
                    {
                        retStr = retStr.Substring(0, i + 1);
                        retStr += " " + zeroAmount + "x(00)";
                    }
                }
            }

            return retStr;
        }

        /// <summary>
        /// Przekształcenie tablicy bajtów w łańcuch cyfr heksadecymalnych oddzielnych podanym separatorem.
        /// </summary>
        /// <param name="bytes">Ciąg bajtów</param>
        /// <param name="upperCase">Flaga wskazująca czy zwracane cyfry heksadecymalne zawierają wielkie litery</param>
        /// <param name="separator">Separator do umieszczenia pomiędzy parami cyfr heksadecymalnych reprezentującymi pojedyncze bajty</param>
        /// <returns>Łańcuch cyfr heksadecymalnych</returns>
        /// <remarks>
        ///   <para>Wartość <b><i>null</i></b> parametru <paramref name="separator"/> powoduje zwrócenie ciągu cyfr heksadecymalnych bez separatora.</para>
        ///   <para>Znaki separatora kopiowane są do łańcucha wynikowego bez zmian wielkości liter niezależnie od wartości parametru <paramref name="upperCase"/>.
        ///   Parametr ten wpływa jedynie na wielkość liter A-F występujących w heksadecymalnej reprezentacji bajtów z tablicy <paramref name="bytes"/>.</para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Wywołano metodę nie podając ciągu bajtów</exception>
        public static String ByteArrayToHexString(
            Byte[] bytes,
            Boolean upperCase,https://github.com/luk531/test02/branches
            String separator)
        {
            String result;

            // Sprawdzenie czy przekazano parametr
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            // Konwersja tablicy bajtów na łańcuch wartości heksadecymalnych (upperCase) z separatorem "-"
            result = BitConverter.ToString(bytes);

            // Zastąpienie wielkich liter małymi jeżeli tak wskazano w parametrze "upperCase"
            if (!upperCase)
            {
                result = result.ToLower();
            }

            // Jeżeli separator ma wartość "null", to zwrócony łańcuch będzie pozbawiony separatora
            if (separator == null)
            {
                separator = "";
            }

            // Zastąpienie domyślnego separatora "-" wartością wskazaną w parametrze "separator"
            result = result.ReplaceEx("-", separator, StringComparison.Ordinal);

            return result;
        }
    }
}
