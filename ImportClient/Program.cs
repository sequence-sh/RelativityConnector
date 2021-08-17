using Grpc.Core;
using ReductechImport;

namespace ImportClient
{

public class Program
{
    

    //public static Request CreateRequest
    //{
    //    get
    //    {
    //        var dataSource = new DataTable();

    //        dataSource.Columns.AddRange(
    //            new[]
    //            {
    //                new DataColumn("control number",            typeof(string)),
    //                new DataColumn("File Path",                 typeof(string)),
    //                new DataColumn("Folder Path",               typeof(string)),
    //                new DataColumn("Title",                     typeof(string)),
    //                new DataColumn("Document Extension",        typeof(string)),
    //                new DataColumn("Custodian - Single Choice", typeof(string)),
    //            }
    //        );

    //        dataSource.Rows.Add(
    //            "REL-4444444",
    //            @"C:\Users\wainw\Documents\Take On Me.pdf",
    //            "Songs",
    //            "Take On Me",
    //            ".pdf",
    //            "Mark W"
    //        );

    //        return new Request()
    //        {
    //            RelativityUsername  = "relativity.admin@relativity.com",
    //            RelativityPassword  = "Test1234!",
    //            RelativityWebAPIUrl = "http://relativitydevvm/relativitywebapi",
    //            Data                = dataSource
    //        };
    //    }
    //}

    const int Port = 30051;

    public static void Main(string[] args)
    {
        var server = new Server
        {
            Services = { Reductech_Import.BindService(new ReductechImportImplementation()) },
            Ports    = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
        };

        server.Start();
        server.ShutdownAsync().Wait();
    }
}

}
