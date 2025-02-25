using ApproveMe.FileParsers;
using ApproveMe.Models.Transactions;
using Microsoft.AspNetCore.Mvc;

namespace ApproveMe.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] List<int> selectedRows)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is empty");
        }

        var parser = GetParser(file.FileName);

        await using var stream = file.OpenReadStream();
        var records = parser.Parse(stream, selectedRows);

        // Map each record to a Transaction or other domain model as needed
        var transactions = records.Select(record => new Transaction
        {
            Id = Guid.NewGuid(),
            Content = string.Join(", ", record.Values.Select(v => v?.ToString())),
            Status = TransactionStatus.Pending
        }).ToList();

        // TODO: Create a DataBatch, add transactions, and save to your PostgreSQL DB

        return Ok("File processed successfully");
    }
    
    private IFileParser GetParser(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".json" => new JsonFileParser(),
            ".tsv"  => new TsvFileParser(),
            ".sql"  => new SqlColumnsFileParser(),
            ".csv"  => new CsvFileParser(),
            _       => throw new NotSupportedException("File type not supported"),
        };
    }
}
