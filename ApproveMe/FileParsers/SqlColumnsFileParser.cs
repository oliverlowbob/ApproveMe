namespace ApproveMe.FileParsers;

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

public class SqlColumnsFileParser : IFileParser
{
    public IEnumerable<IDictionary<string, object>> Parse(Stream fileStream, IEnumerable<int> selectedRows)
    {
        var selectedRowSet = new HashSet<int>(selectedRows);
        var records = new List<IDictionary<string, object>>();

        using var reader = new StreamReader(fileStream);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "|"
        };
        using var csv = new CsvReader(reader, config);

        // Read header row
        csv.Read();
        csv.ReadHeader();
        var rowIndex = 0;
        while (csv.Read())
        {
            if (selectedRowSet.Contains(rowIndex))
            {
                if (csv.GetRecord<dynamic>() is IDictionary<string, object> record)
                {
                    records.Add(record);
                }
            }
            rowIndex++;
        }

        return records;
    }
}
