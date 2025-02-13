namespace ApproveMe.Models.Transactions;

public class Sample
{
    public Guid Id { get; set; }
    public List<Transaction> Transactions { get; set; }
    public List<string> Fields { get; set; } // Fields to show from the data object
    public object Data { get; set; } // One defined table with coloumn names: csv file, SQL table etc
}