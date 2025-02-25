namespace ApproveMe.FileParsers;

public interface IFileParser
{
    /// <summary>
    /// Parses the file and returns a collection of dictionaries for the selected rows.
    /// </summary>
    /// <param name="fileStream">The file stream containing the data.</param>
    /// <param name="selectedRows">Zero-based indexes of the rows (excluding header) to parse.</param>
    /// <returns>An enumerable of dictionaries with column names as keys.</returns>
    IEnumerable<IDictionary<string, object>> Parse(Stream fileStream, IEnumerable<int> selectedRows);
}