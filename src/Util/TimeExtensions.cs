
using System.Globalization;

public static class Time
{
    public static readonly DateTime TIME_TEMPLATE_LOCAL = new DateTime(1, 1, 1, 23, 0, 0);

    public static DateTime Now() => DateTime.UtcNow;
    public static DateTime Today() => DateTime.UtcNow.Date;

    public static string TillNow(this DateTime time)
    {
        var span = Now() - time;
        return span.Hours();
    }

    /// <summary>
    ///  Will apply the defined default time if the time is null
    /// </summary
    public static DateTime WithTime(this DateTime refDate)
    {
        if (refDate.Hour == 0 && refDate.Minute == 0 && refDate.Second == 0)
        {
            return refDate.AddTime(TIME_TEMPLATE_LOCAL).ToUniversalTime();
        }
        return refDate;
    }

    public static DateTime AddTime(this DateTime refDate, DateTime template)
    {
        return new DateTime(refDate.Year, refDate.Month, refDate.Day, template.Hour, template.Minute, template.Second);
    }

    public static Func<DateTime, TimePeriod> SingleDay = d => new(d, d.Midnight());

    /// <summary>
    ///     End of day ( = 0:00 of the next day)
    ///     Midnight as seen from the reference date on
    ///     0:01 would be referred to as 'morning'
    /// </summary
    public static DateTime Midnight(this DateTime refDate) => refDate.Date.AddDays(1);

    /// <summary>
    ///     Start of week 
    /// </summary
    /// FIXME: Year is passed in, which means it can be wrong for CW01 in the new year
    public static DateTime Sow(this DateTime refDate)
    {
        var week = ISOWeek.GetWeekOfYear(refDate);
        return ISOWeek.ToDateTime(refDate.Year, week, DayOfWeek.Monday);
    }

    /// <summary>
    ///     End of week 
    /// </summary
    public static DateTime Eow(this DateTime refDate)
    {
        var week = ISOWeek.GetWeekOfYear(refDate);
        return ISOWeek.ToDateTime(refDate.Year, week, DayOfWeek.Sunday);
    }

    /// <summary>
    ///     Start of month 
    /// </summary
    public static DateTime Som(this DateTime refDate) => new DateTime(refDate.Year, refDate.Month, 1);

    /// <summary>
    ///     End of month
    /// </summary
    public static DateTime Eom(this DateTime refDate)
    {
        int lastDay = DateTime.DaysInMonth(refDate.Year, refDate.Month);
        return new DateTime(refDate.Year, refDate.Month, lastDay);
    }

    /// <summary>
    ///     Start of year 
    /// </summary
    public static DateTime Soy(this DateTime refDate) => new DateTime(refDate.Year, 1, 1);


    /// <summary>
    ///     End of year
    /// </summary
    public static DateTime Eoy(this DateTime refDate) => new DateTime(refDate.Year, 12, 31);


    /// <summary>
    ///     Start of quarter
    /// </summary
    public static DateTime Soq(this DateTime refDate)
    {
        var week = ISOWeek.GetWeekOfYear(refDate);
        int quarter = week / 13;
        int quarterStartCw = 13 * quarter + 1;
        return ISOWeek.ToDateTime(refDate.Year, quarterStartCw, DayOfWeek.Monday);
    }

    /// <summary>
    ///  Returns the date based on a weekday input
    ///  A week offset can be specified to get the day of the next (offset 1) or previous (offset -1) week
    /// </summary
    public static DateTime Weekday(this DateTime refDate, DayOfWeek weekday, int weekOffset = 0)
    {
        var week = ISOWeek.GetWeekOfYear(refDate.AddDays(weekOffset * 7));
        return ISOWeek.ToDateTime(refDate.Year, week, weekday);
    }

    /// <summary>
    ///     Returns the start date of the quarter of that year
    /// </summary
    public static DateTime QuarterStart(int year, int quarter)
    {
        if (quarter < 1 || quarter > 4) throw new ArgumentException($"Quarter has to be between 1-4, but was {quarter}");

        var quarterStartCw = (quarter - 1) * 13 + 1;
        return ISOWeek.ToDateTime(year, quarterStartCw, DayOfWeek.Monday);
    }

    public static (int year, int quarter) Quarter(this DateTime refDate)
    {
        var currentCw = ISOWeek.GetWeekOfYear(refDate);

        if (currentCw == 1 && refDate.Month == 12) return (refDate.Year + 1, 1);
        if (currentCw >= 52 && refDate.Month == 1) return (refDate.Year - 1, 4);

        return currentCw switch
        {
            >= 1 and <= 13 => (refDate.Year, 1),
            >= 14 and <= 26 => (refDate.Year, 2),
            >= 27 and <= 39 => (refDate.Year, 3),
            >= 40 => (refDate.Year, 4),
            _ => throw new ArgumentException($"Invalid calendar week: {currentCw} for date {refDate}")
        };
    }

    // TEST: is the quarter calculation correct?
    public static (int year, int quarter) PastQuarter(this DateTime refDate, int quartersAgo)
    {
        var quarter = refDate.Quarter();

        var years = quartersAgo / 4;
        var rest = quartersAgo % 4; // 0-3

        var year = quarter.year - years;
        int newQuarter = quarter.quarter - rest;

        if (newQuarter <= 0)
        {
            newQuarter = 4 + newQuarter;
            year--;
        }

        return (year, newQuarter);
    }

    /// <summary>
    ///     End of quarter
    /// </summary
    public static DateTime Eoq(this DateTime refDate)
    {
        var week = ISOWeek.GetWeekOfYear(refDate);
        int quarter = week / 13;
        int quarterStartCw = 13 * quarter + 1;

        int year = refDate.Year;
        // case CW01 in previous year
        if (week == 1 && refDate.Month == 12) year++;

        int quarterEndCw;

        if (quarter == 4)
        {
            quarterEndCw = ISOWeek.GetWeeksInYear(refDate.Year);
        }
        else
        {
            // first cw would be already 13 + 1 (eg CW1 + 13 = CW14)
            quarterEndCw = quarterStartCw + 13 - 1;
        }

        return ISOWeek.ToDateTime(year, quarterEndCw, DayOfWeek.Sunday);
    }
}