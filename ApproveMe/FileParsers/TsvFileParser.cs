namespace ApproveMe.FileParsers;

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

public class TsvFileParser : IFileParser
{
    public IEnumerable<IDictionary<string, object>> Parse(Stream fileStream, IEnumerable<int> selectedRows)
    {
        var selectedRowSet = new HashSet<int>(selectedRows);
        var records = new List<IDictionary<string, object>>();

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = "\t"
        };
        
        using var reader = new StreamReader(fileStream);
        using var csv = new CsvReader(reader, config);

        // Read header row
        csv.Read();
        csv.ReadHeader();
        var rowIndex = 0;
        while (csv.Read())
        {
            if (selectedRowSet.Contains(rowIndex))
            {
                // GetRecord<dynamic>() returns an ExpandoObject which implements IDictionary<string, object>.
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
