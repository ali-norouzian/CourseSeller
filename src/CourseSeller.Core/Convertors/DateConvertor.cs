using System.Globalization;

namespace CourseSeller.Core.Convertors
{
    public static class DateConvertor
    {
        public static string ToShamsi(this DateTime val)
        {
            PersianCalendar pc = new PersianCalendar();

            return $"{pc.GetYear(val):00}/{pc.GetMonth(val):00}/{pc.GetDayOfMonth(val):00}";
        }
        
        public static string ToShamsiWithClock(this DateTime val)
        {
            PersianCalendar pc = new PersianCalendar();

            return $"{pc.GetYear(val):00}/{pc.GetMonth(val):00}/{pc.GetDayOfMonth(val):00} | {val.Hour}:{val.Minute}";
        }

        public static DateTime ShamsiToGregorian(this string date)
        {
            int[] edd = date.Split('/')
                .Select(s => int.Parse(s)).ToArray();
            var result = new DateTime(edd[0],
                edd[1], edd[2], new PersianCalendar());

            return result;
        }
    }
}