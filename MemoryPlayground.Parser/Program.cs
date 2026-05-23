var result = new List<Data>();

foreach (var line in File.ReadAllLines("test.csv"))
{
    ReadOnlySpan<char> span = line;
    
    TryReadColumn(ref span, out var idSpan);
    TryReadColumn(ref span, out var nameSpan);
    TryReadColumn(ref span, out var countrySpan);
    TryReadColumn(ref span, out var amountSpan);

    result.Add(new Data(int.Parse(idSpan), nameSpan.ToString(), countrySpan.ToString(), decimal.Parse(amountSpan)));
}

static bool TryReadColumn(ref ReadOnlySpan<char> span, out ReadOnlySpan<char> column)
{
    if (span.IsEmpty)
    {
        column = default;
        return false;
    }

    int commaIndex = span.IndexOf(',');

    if (commaIndex == -1)
    {
        column = span;
        span = ReadOnlySpan<char>.Empty;
        return true;
    }

    column = span[..commaIndex];
    span = span[(commaIndex + 1)..];

    return true;
}

record Data(long Id, string Name, string Country, decimal Amount);



