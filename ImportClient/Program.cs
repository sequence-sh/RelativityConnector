using System;
using Grpc.Core;
using ReductechRelativityImport;

namespace ImportClient
{

public class Program
{
    const int Port = 30051;

    public static void Main(string[] args)
    {
        var server = new Server
        {
            Services = { Reductech_Relativity_Import.BindService(new ReductechImportImplementation()) },
            Ports    = { new ServerPort("localhost", Port, ServerCredentials.Insecure) }
        };

        server.Start();

        Console.ReadLine();
    }
}

}
