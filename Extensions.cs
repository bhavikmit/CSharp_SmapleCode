using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Colten.Common
{
    public static class Extensions
    {
        /// <summary>
        /// Adds clone to all objects.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.Object.</returns>
        public static object CloneObject(this object obj)
        {
            if (obj == null) return null;
            Type ObjType = obj.GetType();
            System.Reflection.MethodInfo inst = ObjType.GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (inst != null)
            {
                return inst.Invoke(obj, null);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Casts an object to string or empty string
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.String.</returns>
        public static string AsString(this object obj)
        {
            if (obj != null)
                return obj.ToString();
            else
                return string.Empty;
        }

        /// <summary>
        /// Casts an object as integer value or 0
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.Int32.</returns>
        public static int AsInt(this object obj)
        {
            int retVal = 0;
            if (obj != null && obj is int)
                return (int)obj;
            if (obj is bool)
                return ((bool)obj) ? 1 : 0;
            if (obj != null && obj is double)
                return (int)Math.Truncate((double)obj);
            if (obj != null)
            {
                if (!int.TryParse(obj.ToString(), out retVal))
                    retVal = 0;
            }
            return retVal;
        }

        /// <summary>
        /// Ases the int.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.Int32.</returns>
        public static int AsInt(this int? obj)
        {
            if (obj.HasValue)
                return obj.Value;
            else
                return 0;
        }

        /// <summary>
        /// Ases the int.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="defaultVal">The default value.</param>
        /// <returns>System.Int32.</returns>
        public static int AsInt(this object obj, int defaultVal)
        {
            int retVal = 0;
            if (obj != null && obj is int)
                return (int)obj;
            if (obj is bool)
                return ((bool)obj) ? 1 : 0;
            if (obj != null && obj is double)
                return (int)Math.Truncate((double)obj);
            if (obj != null)
            {
                if (!int.TryParse(obj.ToString(), out retVal))
                    retVal = defaultVal;
            }
            return retVal;
        }

        #region Int64(long)

        /// <summary>
        /// for long(Int64)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>return Int64</returns>
        public static Int64 AsInt64(this object obj)
        {
            Int64 retVal = 0;
            if (obj != null && obj is Int64)
                return (Int64)obj;
            if (obj is bool)
                return ((bool)obj) ? 1 : 0;
            if (obj != null && obj is double)
                return (Int64)Math.Truncate((double)obj);
            if (obj != null)
            {
                if (!Int64.TryParse(obj.ToString(), out retVal))
                    retVal = 0;
            }
            return retVal;
        }

        /// <summary>
        /// For Int64(long)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>return System.Int64</returns>
        public static Int64 AsInt64(this Int64? obj)
        {
            if (obj.HasValue)
                return obj.Value;
            else
                return 0;
        }

        /// <summary>
        /// For Int64(long)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="defaultVal"></param>
        /// <returns>return System.Int64</returns>
        public static Int64 AsInt64(this object obj, Int64 defaultVal)
        {
            Int64 retVal = 0;
            if (obj != null && obj is Int64)
                return (Int64)obj;
            if (obj is bool)
                return ((bool)obj) ? 1 : 0;
            if (obj != null && obj is double)
                return (Int64)Math.Truncate((double)obj);
            if (obj != null)
            {
                if (!Int64.TryParse(obj.ToString(), out retVal))
                    retVal = defaultVal;
            }
            return retVal;
        }

        #endregion

        /// <summary>
        /// Casts and object to a date or returns datetime.minvalue
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>DateTime.</returns>
        public static DateTime AsDate(this object obj)
        {
            return obj.AsDate(DateTime.MinValue);
        }

        /// <summary>
        /// Safes the hash.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.Int32.</returns>
        public static int SafeHash(this object obj)
        {
            if (obj == null)
                return 0;
            else
                return obj.GetHashCode();
        }

        /// <summary>
        /// Safes the hash.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.Int32.</returns>
        public static int SafeHash(this string obj)
        {
            return ((object)obj).SafeHash();
        }

        /// <summary>
        /// Ases the date.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="defaultVal">The default value.</param>
        /// <returns>DateTime.</returns>
        public static DateTime AsDate(this object obj, DateTime defaultVal)
        {
            DateTime retVal = defaultVal;
            if (obj is DateTime)
                retVal = (DateTime)obj;
            if (obj is DateTime? && ((DateTime?)obj).HasValue)
                retVal = ((DateTime?)obj).Value;
            if (obj is string)
            {
                if (!DateTime.TryParse(obj as string, out retVal))
                    retVal = defaultVal;
            }
            return retVal;
        }

        /// <summary>
        /// Ases the bool.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool AsBool(this object obj)
        {
            bool retVal = false;
            if (obj is bool)
                retVal = (bool)obj;
            if (obj is string)
            {
                string strObj = obj.AsString().ToLower();
                strObj = strObj.Replace("on", "1");
                strObj = strObj.Replace("off", "0");
                strObj = strObj.Replace("true", "1");
                strObj = strObj.Replace("false", "0");

                if (strObj.AsString().IsNumber())
                    retVal = (strObj.AsInt() != 0);
                else if (!bool.TryParse(obj as string, out retVal))
                    retVal = false;
            }
            return retVal;
        }

        /// <summary>
        /// Ases the double.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.Double.</returns>
        public static double AsDouble(this object obj)
        {
            double retVal = 0.0;
            if (obj != null && obj is double)
                retVal = (double)obj;
            if (obj != null)
            {
                if (!double.TryParse(obj.ToString(), out retVal))
                    retVal = 0.0;
            }
            return retVal;
        }

        /// <summary>
        /// Dates the only.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>DateTime.</returns>
        public static DateTime DateOnly(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }

        /// <summary>
        /// returns the sunday week of
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>DateTime.</returns>
        public static DateTime LastSunday(this DateTime date)
        {
            int diff = date.DayOfWeek - DayOfWeek.Sunday;
            if (diff < 0)
            { diff += 7; }
            return date.AddDays(-1 * diff).Date;
        }

        public static DateTime LastMonday(this DateTime date)
        {
            int diff = date.DayOfWeek - DayOfWeek.Monday;
            if (diff < 0)
            { diff += 7; }
            return date.AddDays(-1 * diff).Date;
        }

        public static DateTime LastSaturday(this DateTime date)
        {
            int diff = date.DayOfWeek - DayOfWeek.Saturday;
            if (diff < 0)
            { diff += 7; }
            return date.AddDays(-1 * diff).Date;
        }

        public static DateTime LastWednesday(this DateTime date)
        {
            int diff = date.DayOfWeek - DayOfWeek.Wednesday;
            if (diff < 0)
            { diff += 7; }
            return date.AddDays(-1 * diff).Date;
        }

        public static DateTime GetLastWeekStart(this DateTime date, int intWeekStartDay)

        {
            DateTime Weekstart = DateTime.Now;

            switch (intWeekStartDay)
            {
                case 1:

                    Weekstart = LastSunday(date);
                    break;

                case 2:

                    Weekstart = LastMonday(date);
                    break;

                case 0:

                    Weekstart = LastSaturday(date);
                    break;

                case 4:

                    Weekstart = LastWednesday(date);
                    break;



            }

            return Weekstart;
        }

        /// <summary>
        /// Determines whether is week start [the specified int week start day].
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="intWeekStartDay">The int week start day.</param>
        /// <returns><c>true</c> if [is week start] [the specified int week start day]; otherwise, <c>false</c>.</returns>
        public static bool IsWeekStart(this DateTime date, int intWeekStartDay)
        {
            bool weekstart = false;

            switch (intWeekStartDay)
            {
                case 1:

                    if (date.DayOfWeek == DayOfWeek.Sunday) { weekstart = true; }
                    break;

                case 2:

                    if (date.DayOfWeek == DayOfWeek.Monday) { weekstart = true; }
                    break;

                case 0:

                    if (date.DayOfWeek == DayOfWeek.Saturday) { weekstart = true; }
                    break;

                case 4:

                    if (date.DayOfWeek == DayOfWeek.Wednesday) { weekstart = true; }
                    break;
            }

            return weekstart;
        }

        public static DateTime GetNextWeekStart(this DateTime date, int intWeekStartDay)
        {
            return GetLastWeekStart(date, intWeekStartDay).AddDays(7);
        }

        /// <summary>
        /// To the bool.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="throwError">if set to <c>true</c> [throw error].</param>
        /// <param name="defaultVal">if set to <c>true</c> [default value].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <exception cref="System.ArgumentException">String was null or not bool.</exception>
        public static bool ToBool(this string str, bool throwError = false, bool defaultVal = false)
        {
            bool retVal;
            str = str.Equals("yes", StringComparison.CurrentCultureIgnoreCase) ? "true" : str;
            str = str.Equals("no", StringComparison.CurrentCultureIgnoreCase) ? "false" : str;

            if (str != null && bool.TryParse(str, out retVal))
                return retVal;
            else
            {
                if (throwError)
                    throw new ArgumentException("String was null or not bool.");
                else
                    return defaultVal;
            }
        }

        /// <summary>
        /// Converts the string representation of a number to an integer.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="throwError">if set to <c>true</c> [throw error].</param>
        /// <param name="defaultVal">The default value.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.ArgumentException">String was null or not numeric.</exception>
        public static int ToInt(this string str, bool throwError = false, int defaultVal = 0)
        {
            int retVal;
            if (str != null && int.TryParse(str, out retVal))
                return retVal;
            else
            {
                if (throwError)
                    throw new ArgumentException("String was null or not numeric.");
                else
                    return defaultVal;
            }
        }

        public static Int64 ToInt64(this string str, bool throwError = false, int defaultVal = 0)
        {
            Int64 retVal;
            if (str != null && Int64.TryParse(str, out retVal))
                return retVal;
            else
            {
                if (throwError)
                    throw new ArgumentException("String was null or not numeric.");
                else
                    return defaultVal;
            }
        }

        /// <summary>
        /// Converts the string representation of a number to an integer.
        /// </summary>
        /// <param name="boolVal">if set to <c>true</c> [bool value].</param>
        /// <returns>System.Int32.</returns>
        public static int ToInt(this bool boolVal)
        {
            return boolVal ? 1 : 0;
        }

        /// <summary>
        /// To the double.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="throwError">if set to <c>true</c> [throw error].</param>
        /// <param name="defaultVal">The default value.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArgumentException">String was null or not numeric.</exception>
        public static double ToDouble(this string str, bool throwError = false, double defaultVal = 0)
        {
            double retVal;
            if (str != null && double.TryParse(str, out retVal))
                return retVal;
            else
            {
                if (throwError)
                    throw new ArgumentException("String was null or not numeric.");
                else
                    return defaultVal;
            }
        }

        /// <summary>
        /// To the date time.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="throwError">if set to <c>true</c> [throw error].</param>
        /// <returns>DateTime.</returns>
        /// <exception cref="System.ArgumentException">String was null or not a DateTime.</exception>
        public static DateTime ToDateTime(this string str, bool throwError = false)
        {
            DateTime retVal;
            if (str != null && DateTime.TryParse(str, out retVal))
                return retVal;
            else
            {
                if (throwError)
                    throw new ArgumentException("String was null or not a DateTime.");
                else
                    return DateTime.MinValue;
            }
        }

        /// <summary>
        /// To the date time.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="defaultVal">The default value.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToDateTime(this string str, DateTime defaultVal)
        {
            DateTime retVal;
            if (str != null && DateTime.TryParse(str, out retVal))
                return retVal;
            else
            {
                return defaultVal;
            }
        }

        /// <summary>
        /// returns a date mindate + the time
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="throwError">if set to <c>true</c> [throw error].</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToTime(this string str, bool throwError = false)
        {
            if (str.IsEmpty())
                return DateTime.MinValue;

            string temp = Formatter.NumbersOnly(str);
            if (temp.Length != 4)
                return DateTime.MinValue;

            return DateTime.MinValue.AddHours(temp.Substring(0, 2).ToDouble()).AddSeconds(temp.Substring(2, 2).ToDouble());
        }

        /// <summary>
        /// To the database date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="NoTimePart">if set to <c>true</c> [no time part].</param>
        /// <returns>System.Nullable&lt;DateTime&gt;.</returns>
        public static DateTime? ToDbDate(this DateTime date, bool NoTimePart = false)
        {
            if (date == DateTime.MinValue)
                return null;

            int yr = date.Year;
            if (yr < 1900 || yr > 2100)
                yr = DateTime.Now.Year;
            if (NoTimePart)
                return new DateTime(yr, date.Month, date.Day);
            else
                return new DateTime(yr, date.Month, date.Day, date.Hour, date.Minute, date.Second);
        }

        /// <summary>
        /// 0/0/0000
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>System.String.</returns>
        public static string ToSWDate(this DateTime date)
        {
            return date.ToString("d"); // date.Month.ToString() + '/' + date.Day.ToString() + '/' + date.Year.ToString();
        }

        /// <summary>
        /// To the sw date and time.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>System.String.</returns>
        public static string ToSWDateAndTime(this DateTime date)
        {
            return date.ToString("MM/dd/yyyy HH:mm");
        }

        /// <summary>
        /// Day 0/0/0000
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>System.String.</returns>
        public static string ToSWLongDisplayDate(this DateTime date)
        {
            return date.ToString("ddd M/d/yyyy"); // date.Month.ToString() + '/' + date.Day.ToString() + '/' + date.Year.ToString();
        }

        /// <summary>
        /// hour:minute
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>System.String.</returns>
        public static string ToSWTime(this DateTime date)
        {
            return date.ToString("HH:mm"); // date.Month.ToString() + '/' + date.Day.ToString() + '/' + date.Year.ToString();
        }

        /// <summary>
        /// used for SW date 00/00/0000 columns that contain leading 0's
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>System.String.</returns>
        public static string ToDateString(this DateTime date)
        {
            return date.ToString("MM/dd/yyyy");
        }

        /// <summary>
        /// To the date only.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ToDateOnly(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }

        // test a string for valid email.
        /// <summary>
        /// Determines whether [is valid e mail address] [the specified email address].
        /// </summary>
        /// <param name="emailAddress">The email address.</param>
        /// <returns><c>true</c> if [is valid e mail address] [the specified email address]; otherwise, <c>false</c>.</returns>
        public static bool IsValidEMailAddress(this string emailAddress)
        {
            return Regex.IsMatch(emailAddress, "^([\\w-]+\\.)*?[\\w-]+@[\\w-]+\\.([\\w-]+\\.)*?[\\w]+$");
        }

        /// <summary>
        /// replaces all instances of the combination. example usage for removing all double spaces "  ", to single space a string.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="oldChar">The old character.</param>
        /// <param name="newChar">The new character.</param>
        /// <returns>System.String.</returns>
        public static string ReplaceAll(this string source, string oldChar, string newChar)
        {
            string retVal = source;
            int len = 0;
            if (retVal != null && len > 0)
            {
                do
                {
                    len = retVal.Length;
                    retVal = retVal.Replace(oldChar, newChar);
                } while (retVal.Length != len || retVal.Length == 0);
            }
            return retVal;
        }

        /// <summary>
        /// Determines whether the specified source is empty.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns><c>true</c> if the specified source is empty; otherwise, <c>false</c>.</returns>
        public static bool IsEmpty(this string source)
        {
            return source == null ? true : source.Trim().Length == 0;
        }

        /// <summary>
        /// Determines whether the specified source is number.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns><c>true</c> if the specified source is number; otherwise, <c>false</c>.</returns>
        public static bool IsNumber(this string source)
        {
            int retVal;
            if (source != null && int.TryParse(source, out retVal))
                return true;
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Maximums the length.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="len">The length.</param>
        /// <returns>System.String.</returns>
        public static string MaxLen(this string source, int len)
        {
            if (source == null)
                return source;
            return source.Substring(0, source.Length > len ? len : source.Length);
        }

        /// <summary>
        /// Safes the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>System.String.</returns>
        public static string Safe(this string source)
        {
            return source ?? string.Empty;
        }

        /// <summary>
        /// Databases the ecape.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>System.String.</returns>
        public static string DBEcape(this string source)
        {
            if (source.IsEmpty())
                return "";
            else
                return source.Replace("'", "''").Replace(";", "");
        }

        /// <summary>
        /// Determines whether the specified source is date.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns><c>true</c> if the specified source is date; otherwise, <c>false</c>.</returns>
        public static bool IsDate(this string source)
        {
            DateTime retVal;
            return DateTime.TryParse(source, out retVal);
        }

        /// <summary>
        /// returns the first non zero item
        /// </summary>
        /// <param name="num">The number.</param>
        /// <param name="otherNum">The other number.</param>
        /// <returns>System.Int32.</returns>
        public static int Best(this int num, int otherNum)
        {
            if (num != 0)
                return num;
            else
                return otherNum;
        }

        /// <summary>
        /// returns the first non null or empty string
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="otherStr">The other string.</param>
        /// <returns>System.String.</returns>
        public static string Best(this string str, string otherStr)
        {
            if (!string.IsNullOrEmpty(str))
                return str;
            else
                return otherStr ?? string.Empty;
        }

        /// <summary>
        /// Casts the specified object.
        /// </summary>
        /// <typeparam name="TT">The type of the tt.</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>TT.</returns>
        public static TT Cast<TT>(this object obj)
        {
            return (TT)obj;
        }

        /// <summary>
        /// Casts the list.
        /// </summary>
        /// <typeparam name="TT">The type of the tt.</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>IList&lt;TT&gt;.</returns>
        internal static IList<TT> CastList<TT>(this IEnumerable obj)
        {
            IList<TT> retVal = new List<TT>();
            foreach (var item in obj)
            {
                retVal.Add((TT)item);
            }
            return retVal;
        }

        /// <summary>
        /// Parts the specified index.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="index">The index.</param>
        /// <param name="joinOn">The join on.</param>
        /// <returns>System.String.</returns>
        public static string Part(this string str, int index, char joinOn)
        {
            if (string.IsNullOrEmpty(str))
                return str;
            else
            {
                string[] parts = str.Split(joinOn);
                try
                {
                    return parts[index];
                }
                catch
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Ors if zero.
        /// </summary>
        /// <param name="val1">The val1.</param>
        /// <param name="val2">The val2.</param>
        /// <returns>System.Int32.</returns>
        public static int OrIfZero(this int val1, int val2)
        {
            if (val1 == 0)
                return val2;
            else
                return val1;
        }

        /// <summary>
        /// To the string with null zero.
        /// </summary>
        /// <param name="val1">The val1.</param>
        /// <returns>System.String.</returns>
        public static string ToStringWithNullZero(this int val1)
        {
            if (val1 != 0)
                return val1.ToString();
            else
                return "null";
        }

        /// <summary>
        /// To the delimited string.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="delimeter">The delimeter.</param>
        /// <returns>System.String.</returns>
        public static string ToDelimitedString(this IList<string> list, string delimeter = ";")
        {
            string retVal = "";
            if (list != null)
            {
                foreach (string item in list)
                    retVal += retVal.IsEmpty() ? item : delimeter + item;
            }

            return retVal;
        }

        /// <summary>
        /// To the delimited string.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="delimeter">The delimeter.</param>
        /// <returns>System.String.</returns>
        public static string ToDelimitedString(this IList<int> list, string delimeter = ";")
        {
            string retVal = "";
            if (list != null)
            {
                foreach (int item in list)
                    retVal += retVal.IsEmpty() ? item.ToString() : delimeter + item.ToString();
            }

            return retVal;
        }

        /// <summary>
        /// To the list of string.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="delimeter">The delimeter.</param>
        /// <returns>List&lt;System.String&gt;.</returns>
        public static List<string> ToListOfString(this string list, char delimeter = ';')
        {
            if (list == null)
                return new List<string>();

            List<string> retVal = list.Split(delimeter).ToList();
            return retVal;
        }

        /// <summary>
        /// Negitives the specified i.
        /// </summary>
        /// <param name="i">The i.</param>
        /// <returns>System.Int32.</returns>
        internal static int Negitive(this int i)
        {
            int retVal = Math.Abs(i) * -1;
            return retVal;
        }

        /// <summary>
        /// Smallests the specified b.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns>System.Int32.</returns>
        internal static int Smallest(this int a, int b)
        {
            if (a > b)
                return b;
            else
                return a;
        }

        /// <summary>
        /// Insures the range.
        /// </summary>
        /// <param name="thisDuration">Duration of the this.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <returns>System.Double.</returns>
        public static double InsureRange(this double thisDuration, double minValue, double maxValue)
        {
            if (thisDuration >= minValue && thisDuration <= maxValue)
                return thisDuration;

            if (thisDuration < minValue)
                return minValue;

            if (thisDuration > maxValue)
                return maxValue;

            return 0;  // should not be here.
        }

        public static string CheckSingleQuote(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return str.Replace("'", "''");
            }
            return string.Empty;
        }


        public static string FromBase64String(this string value, bool throwException = true)
        {
            try
            {
                byte[] decodedBytes = System.Convert.FromBase64String(value);
                string decoded = System.Text.Encoding.UTF8.GetString(decodedBytes);

                return decoded;
            }
            catch (Exception ex)
            {
                if (throwException)
                    throw new Exception(ex.Message, ex);
                else
                    return value;
            }
        }

        public static string ToBase64String(this string value)
        {
            byte[] bytes = System.Text.ASCIIEncoding.UTF8.GetBytes(value);
            string encoded = System.Convert.ToBase64String(bytes);

            return encoded;
        }
        public static bool IsInRange(this int checkVal, int value1, int value2)
        {
            // First check to see if the passed in values are in order. If so, then check to see if checkVal is between them
            if (value1 <= value2)
                return checkVal >= value1 && checkVal <= value2;

            // Otherwise invert them and check the checkVal to see if it is between them
            return checkVal >= value2 && checkVal <= value1;
        }

        public static string ToDecimal(this decimal value)
        {
            return value.ToString("F"); // date.Month.ToString() + '/' + date.Day.ToString() + '/' + date.Year.ToString();
        }
        public static void AddRange<T, S>(this Dictionary<T, S> source, Dictionary<T, S> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("Collection is null");
            }

            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key) && item.Value != null)
                {
                    source.Add(item.Key, item.Value);
                }
                else
                {
                    // handle duplicate key issue here
                }
            }
        }

        public static string ToDateStringWithoutDash(this DateTime date)
        {
            return date.ToString("MMddyyyy");
        }

        /// <summary>
        /// Casts an object to string or empty string
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>System.String.</returns>
        public static string ToDateStringWithFormat(this DateTime date)
        {
            return date != null ? date.ToString("MM/dd/yyy").Replace("-", "/") : "";
        }

        public static DateTime ToDateTimeFromString(this string date)
        {
            if (!string.IsNullOrEmpty(date))
            {
                return DateTime.ParseExact(date,Constant.DateFormat_MMDDYYYY, null);
            }
            else
            {
                return DateTime.ParseExact(DateTime.MinValue.AsString(), Constant.DateFormat_MMDDYYYY, null);
            }
        }
    }
}
