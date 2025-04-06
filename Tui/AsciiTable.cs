using System.Text;

public class AsciiTable
{
    public const string NO_DATA = "-";

    private int _curColumIndex = 0;
    private IDictionary<int, string> _columns = new Dictionary<int, string>();
    private TableRow[] _rows = Array.Empty<TableRow>();

    // minimum padding before and after each table column thingy
    public int TextPadding { set; get; } = 1;

    public void AddColumns(params string[] names)
    {
        foreach (string s in names) _columns.Add(_curColumIndex++, s);
    }

    public void AddData(IEnumerable<object[]> rowData)
    {
        _rows = rowData.Select((d, i) => new TableRow { RowIndex = i, ColumnData = d }).ToArray();
    }

    public void Print(int indent = 0)
    {
        TableRow header = new TableRow { RowIndex = -2, ColumnData = _columns.Values.ToArray() };

        // column index will reflect the amount of columns in use, since it's assinged on add
        int columnCount = _curColumIndex;
        int[] columnWidths = new int[columnCount];
        for (int ci = 0; ci < columnWidths.Length; ci++)
        {
            string name = _columns[ci];
            int maxDataLength = _rows.Max(r => r.ColumnData[ci]?.ToString().Length ?? 1);
            int columnMax = Math.Max(name.Length, maxDataLength);

            columnWidths[ci] = columnMax;
        }

        TableRow stylingRow = new TableRow
        {
            RowIndex = -1,
            ColumnData = columnWidths.Select(w => "".PadLeft(w, '-')).ToArray(),
            IsStylingRow = true
        };
        TableRow[] tableHeader = [stylingRow, header, stylingRow];

        StringBuilder output = new StringBuilder();
        output.AppendLine();
        string indetation = "".PadLeft(indent);
        string textPadding = "".PadLeft(TextPadding);
        string stylingPadding = "".PadLeft(TextPadding, '-');
        foreach (var row in tableHeader.Concat(_rows).Append(stylingRow))
        {
            string separator = row.IsStylingRow ? "+" : "|";
            string padding = row.IsStylingRow ? stylingPadding : textPadding;
            output.Append(indetation);

            for (int i = 0; i < columnWidths.Length; i++)
            {
                output.Append(separator);
                output.Append(padding);
                //TODO: pad left or right handling
                output.Append(row.ColumnData[i]?.ToString().PadLeft(columnWidths[i]) ?? NO_DATA.PadLeft(columnWidths[i]));
                output.Append(padding);
            }

            output.AppendLine(separator);
        }

        Console.WriteLine(output);
    }
}

public class TableRow
{
    public int RowIndex { get; init; }
    // column count = array count & should be the same!
    public object[] ColumnData { get; init; }
    public bool IsStylingRow { get; set; } = false;
}
