using System.Globalization;
using CsvHelper;

namespace ApproveMe.FileParsers;

public class CsvFileParser : IFileParser
{
    public IEnumerable<IDictionary<string, object>> Parse(Stream fileStream, IEnumerable<int> selectedRows)
    {
        // Convert the selection to a hash set for faster lookups.
        var selectedRowSet = new HashSet<int>(selectedRows);
        var records = new List<IDictionary<string, object>>();

        using var reader = new StreamReader(fileStream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        // Read the header row first.
        csv.Read();
        csv.ReadHeader();

        var rowIndex = 0;
        while (csv.Read())
        {
            // Only process the row if it's one of the selected rows.
            if (selectedRowSet.Contains(rowIndex))
            {
                // GetRecord<dynamic>() returns an ExpandoObject,
                // which implements IDictionary<string, object>.
                var record = csv.GetRecord<dynamic>() as IDictionary<string, object>;
                if (record != null)
                {
                    records.Add(record);
                }
            }

            rowIndex++;
        }

        return records;
    }
}