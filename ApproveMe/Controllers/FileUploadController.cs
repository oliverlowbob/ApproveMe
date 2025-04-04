using ApproveMe.FileParsers;
using ApproveMe.Models.Transactions;
using ApproveMe.Repositories;
using Marten;
using Microsoft.AspNetCore.Mvc;

namespace ApproveMe.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileUploadController(IRepository repository) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery] List<int> selectedRows)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is empty");
        }
        
        var parser = GetParser(file.FileName);

        // TODO: Create a DataBatch, add transactions, and save to your PostgreSQL DB
        var dataBatchId = Guid.NewGuid();

        await using var stream = file.OpenReadStream();
        var records = parser.Parse(stream, selectedRows);

        var transactions = records.Select(record => new TransactionAggregate(string.Join(", ", record.Values.Select(v => v?.ToString())), dataBatchId)).ToList();

        foreach (var transaction in transactions)
        { 
            repository.Store(transaction);
        }

        await repository.SaveChanges();

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
