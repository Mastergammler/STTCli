using System.Text;

using static Symbols;

public class AsciiTable
{

    private int _curColumIndex = 0;
    private IDictionary<int, TableColumn> _columns = new Dictionary<int, TableColumn>();
    private TableRow[] _rows = Array.Empty<TableRow>();

    //TODO: proper filtering type
    private int _filterColumnIdx;
    private string? _filterValue;

    // minimum padding before and after each table column thingy
    public int TextPadding { set; get; } = 1;

    public void AddColumns(params string[] names)
    {
        AddColumns(names.Select(n => (n, true)).ToArray());
    }

    public void AddColumns(params (string, bool)[] names)
    {
        foreach (var tuple in names)
        {
            int curIndex = _curColumIndex++;
            _columns.Add(curIndex, new TableColumn<string>
            {
                ColumnName = tuple.Item1,
                ColumnIndex = curIndex,
                RightAligned = tuple.Item2
            });
        }
    }

    public void AddColumn<T>(string name,
                             bool rightAligned,
                             Func<T, string> contentFormat,
                             params ColumnStyle<T>[] styles)
    {
        int curIndex = _curColumIndex++;
        _columns.Add(curIndex, new TableColumn<T>
        {
            ColumnName = name,
            ColumnIndex = curIndex,
            RightAligned = rightAligned,
            ContentFormat = contentFormat,
            Styles = styles
        });
    }

    //TODO: how to filter properly, by item types etc?
    public void Filter(string columnName, string value)
    {
        _filterColumnIdx = _columns.Single(kvp => kvp.Value.ColumnName.Equals(columnName)).Key;
        _filterValue = value;
    }

    public void AddData(IEnumerable<object[]> rowData)
    {
        _rows = rowData.Select((d, i) => new TableRow { RowIndex = i, ColumnData = d }).ToArray();
    }

    public void Print(int indent = 0)
    {
        TableRow header = new TableRow
        {
            RowIndex = -2,
            ColumnData = _columns.Values.Select(v => v.ColumnName).ToArray()
        };


        TableRow[] displayRows = _rows;


        if (_filterValue is not null)
        {
            //TODO: Implement fuzzy filtering instead
            displayRows = _rows.Where(r => r.ColumnData[_filterColumnIdx].ToString().Contains(_filterValue, StringComparison.InvariantCultureIgnoreCase))
                               .ToArray();
        }

        for (int ci = 0; ci < _columns.Count; ci++)
        {
            var col = _columns[ci];

            string name = col.ColumnName;
            int maxDataLength = displayRows.Any() ? displayRows.Max(r => col.FormatData(r.ColumnData[ci])?.Length ?? 1) : 0;
            int columnMax = Math.Max(name.Length, maxDataLength);

            _columns[ci].MaxWidth = columnMax;
            _columns[ci].MinWidth = name.Length;
        }

        TableRow stylingRow = new TableRow
        {
            RowIndex = -1,
            ColumnData = _columns.Values.Select(w => "".PadLeft(w.MaxWidth, '-')).ToArray(),
            IsStylingRow = true
        };
        TableRow[] tableHeader = [stylingRow, header, stylingRow];

        //TODO: format table based on available space
        int availableSpace = Console.LargestWindowWidth;

        StringBuilder output = new StringBuilder();
        output.AppendLine();
        string indetation = "".PadLeft(indent);
        string textPadding = "".PadLeft(TextPadding);
        string stylingPadding = "".PadLeft(TextPadding, '-');
        foreach (var row in tableHeader.Concat(displayRows).Append(stylingRow))
        {
            string separator = row.IsStylingRow ? "+" : "|";
            string padding = row.IsStylingRow ? stylingPadding : textPadding;
            output.Append(indetation);

            //TODO: ugly, better handling instead of going thorugh it twice?
            //-> Is it possible?
            ColumnStyle? rowStyle = null;
            for (int i = 0; i < _columns.Count; i++)
            {
                ColumnStyle? colStyle = _columns[i].GetStyle(row.ColumnData[i]);

                if (colStyle is not null && colStyle.IsRowStyle)
                {
                    if (rowStyle is null || rowStyle.Priority < colStyle.Priority) rowStyle = colStyle;
                }
            }

            for (int i = 0; i < _columns.Count; i++)
            {
                var col = _columns[i];

                output.Append(separator);

                var style = col.GetStyle(row.ColumnData[i]);

                if (rowStyle is not null)
                {
                    if (style is null) style = rowStyle;
                    // if the prio is the same, we use the row style to have a consistent visual
                    // if override is desired, the priority should be used!
                    else if (rowStyle.Priority >= style.Priority) style = rowStyle;
                }
                if (style is not null) output.Append(AnsiStyling.Create(style.TextColor, style.BackgroundColor));

                output.Append(padding);
                output.Append(FormatColumnData(row.ColumnData[i], _columns[i]));
                output.Append(padding);

                // reset colors
                output.Append(AnsiStyling.Reset());
            }

            output.Append(separator);
            output.AppendLine();
        }

        Console.WriteLine(output);

        //filter should be applied every time again
        _filterValue = null;
        _filterColumnIdx = 0;
    }

    private string FormatColumnData(object data, TableColumn columnInfo)
    {
        string text = columnInfo.FormatData(data) ?? NO_DATA;
        return columnInfo.RightAligned ? text.PadLeft(columnInfo.MaxWidth) : text.PadRight(columnInfo.MaxWidth);
    }
}

public class TableRow
{
    public int RowIndex { get; init; }
    // column count = array count & should be the same!
    public object[] ColumnData { get; init; }
    public bool IsStylingRow { get; set; } = false;
}

public abstract class TableColumn
{
    public int ColumnIndex { get; init; }
    public string ColumnName { get; init; }
    public bool RightAligned { get; set; } = true;
    public int MaxWidth { get; set; } = 0;
    public int MinWidth { get; set; }

    public abstract ColumnStyle? GetStyle(object data);
    public abstract string? FormatData(object data);
}

public class TableColumn<T> : TableColumn
{
    public static readonly Type DataType = typeof(T);
    public Func<T, string> ContentFormat { get; set; } = t => t.ToString();
    public ColumnStyle<T>[] Styles = [];

    //PERF: i'm going through this then for every row?
    // - but it's not preventable because of different data?
    public override ColumnStyle? GetStyle(object data)
    {
        //TODO: this might hide errors, when 2 conditions apply with the same priority
        // -> Should i have a stronger error handling here?
        if (data is T typedData)
        {
            return Styles.OrderByDescending(s => s.Priority)
                         .FirstOrDefault(s => s.StyleCondition(typedData));
        }
        return null;
    }

    //TODO: refactor - ugly
    public override string? FormatData(object? data)
    {
        if (data is null) return null;
        if (data is T typedData) return ContentFormat(typedData);
        if (data is string str) return str;
        if (data is Int64 i) return i.ToString();
        throw new InvalidCastException($"Expected data of type {DataType.Name} but found {data.GetType().Name}");
    }
}

//TODO: refactor: having a generic structure inside this other generic structure is quite complicated
// -> maybe there is a simpler solution?
public abstract class ColumnStyle
{
    public const byte DefaultTextColor = 15;
    public const byte DefaultBgColor = 0;

    // should this override other row styles or not?
    public byte Priority { get; set; }

    // should this style be applied to the whole row instead of the jsut the 
    public bool IsRowStyle { get; set; }
    public Ansi256Color? TextColor { get; set; }
    public Ansi256Color? BackgroundColor { get; set; }
}

public class ColumnStyle<T> : ColumnStyle
{
    public Func<T, bool> StyleCondition { get; set; } = t => false;
}