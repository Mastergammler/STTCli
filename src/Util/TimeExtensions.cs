
using System.Globalization;

public static class Time
{
    public static DateTime Now() => DateTime.UtcNow;
    public static DateTime Today() => DateTime.UtcNow.Date;

    public static string TillNow(this DateTime time)
    {
        var span = Now() - time;
        return span.Hours();
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