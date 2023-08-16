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

    }
}