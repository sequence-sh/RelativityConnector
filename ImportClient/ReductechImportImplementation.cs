using System;
using System.Data;
using System.Threading.Tasks;
using Grpc.Core;
using kCura.Relativity.ImportAPI;
using ReductechImport;

namespace ImportClient
{

class ReductechImportImplementation : Reductech_Import.Reductech_ImportBase
{
    /// <inheritdoc />
    public override async Task<ImportReply> Import(ImportRequest request, ServerCallContext context)
    {
        var importApi = new ImportAPI(
            request.RelativityUsername,
            request.RelativityPassword,
            request.RelativityWebAPIUrl
        );

        var job = importApi.NewNativeDocumentImportJob();

        var dataTable = new DataTable();
        job.SourceData.SourceData = dataTable.CreateDataReader();

        JobHelpers.SetSettings(job.Settings, request);
        JobHelpers.SetJobMessages(job);

        // Wait for the job to complete.
        try
        {
            job .Execute();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        Console.ReadLine();
        Console.ReadLine();

        return new ImportReply();
    }
}

}
