namespace Zaharuddin.Models;

/// <summary>
/// DateTime extension
/// </summary>
public static class DateTimeExtension
{
    /// <summary>
    /// Quarter
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static int Quarter(this DateTime dateTime)
    {
        return Convert.ToInt16((dateTime.Month - 1)/3) + 1;
    }

    /// <summary>
    /// Week of year
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static int WeekOfYear(this DateTime dateTime)
    {
        CultureInfo ci = CultureInfo.CreateSpecificCulture(CurrentLanguage);
        CalendarWeekRule cwr = ci.DateTimeFormat.CalendarWeekRule;
        DayOfWeek dow = ci.DateTimeFormat.FirstDayOfWeek;
        return ci.Calendar.GetWeekOfYear(dateTime, cwr, dow);
    }
}
