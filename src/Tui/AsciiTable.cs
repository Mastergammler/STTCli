using System.Text;

public class AsciiTable
{
    public const string NO_DATA = "-";

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
            _columns.Add(curIndex, new TableColumn
            {
                ColumnName = tuple.Item1,
                ColumnIndex = curIndex,
                RightAligned = tuple.Item2
            });
        }
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
            string name = _columns[ci].ColumnName;
            int maxDataLength = displayRows.Any() ? displayRows.Max(r => r.ColumnData[ci]?.ToString().Length ?? 1) : 0;
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

            for (int i = 0; i < _columns.Count; i++)
            {
                output.Append(separator);
                output.Append(padding);
                output.Append(FormatColumnData(row.ColumnData[i], _columns[i]));
                output.Append(padding);
            }

            output.AppendLine(separator);
        }

        Console.WriteLine(output);

        //filter should be applied every time again
        _filterValue = null;
        _filterColumnIdx = 0;
    }

    private string FormatColumnData(object data, TableColumn columnInfo)
    {
        string text = data?.ToString() ?? NO_DATA;
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

public class TableColumn
{
    public int ColumnIndex { get; init; }
    public string ColumnName { get; init; }
    public bool RightAligned { get; set; } = true;
    public int MaxWidth { get; set; } = 0;
    public int MinWidth { get; set; }
}