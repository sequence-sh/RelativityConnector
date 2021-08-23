using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Core.Utils;
using kCura.Relativity.ImportAPI;
using ReductechRelativityImport;

namespace ImportClient
{

public static class FieldValueHelper
{
    public static object GetValue(this ImportObject.Types.FieldValue fieldValue)
    {
        switch (fieldValue.TestOneofCase)
        {
            
            case ImportObject.Types.FieldValue.TestOneofOneofCase.StringValue: return fieldValue.StringValue;
            case ImportObject.Types.FieldValue.TestOneofOneofCase.IntValue:    return fieldValue.IntValue;
            case ImportObject.Types.FieldValue.TestOneofOneofCase.DoubleValue: return fieldValue.DoubleValue;
            case ImportObject.Types.FieldValue.TestOneofOneofCase.BoolValue:   return fieldValue.BoolValue;
            case ImportObject.Types.FieldValue.TestOneofOneofCase.DateValue:   return fieldValue.DateValue;
            default: return DBNull.Value;
        }
    }
}

class ReductechImportImplementation : Reductech_Relativity_Import .Reductech_Relativity_ImportBase
{
    private StartImportCommand _command = null;

    /// <inheritdoc />
    public override async Task<StartImportReply> StartImport(StartImportCommand request, ServerCallContext context)
    {
        await Task.CompletedTask;

        if (_command is null)
        {
            _command = request;

            return new StartImportReply() { Success = true, Message  = "Success" };
            
        }
        return new StartImportReply() { Success = false, Message = "Command was already set" };
        
    }

    /// <inheritdoc />
    public override async Task<ImportDataReply> ImportData(IAsyncStreamReader<ImportObject> requestStream, ServerCallContext context)
    {
        Debugger.Launch();

        if (_command is null)
            return new ImportDataReply() { Success = false, Message = "Import was not started"};

        var importApi = new ImportAPI(
            _command.RelativityUsername,
            _command.RelativityPassword,
            _command.RelativityWebAPIUrl
        );

        var job = importApi.NewNativeDocumentImportJob();

        JobHelpers.SetSettings(job.Settings, _command);
        JobHelpers.SetJobMessages(job);

        var dataTable = new DataTable();
        dataTable.Columns.AddRange(
            _command.DataFields.Select(x=> new DataColumn(x.Name, Map(x.DataType))).ToArray()
            );

        var streamList = await requestStream.ToListAsync();

        foreach (var row in streamList)
        {
            var values = row.Values.Select(x => x.GetValue()).ToArray();


            dataTable.Rows.Add(values);
        }

        //var dataReader = new AsyncDataReader(
        //    _command.DataFields.Select(x => x.Name).ToArray(),
        //    _command.DataFields.Select(x => Map(x.DataType)).ToArray(),
        //    requestStream
        //);

        //job.SourceData.SourceData = dataReader;
        job.SourceData.SourceData = dataTable.CreateDataReader();

        

        // Wait for the job to complete.
        try
        {
            job.Execute();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }


        return new ImportDataReply() { Success = true, Message = "Success" };
    }

    static Type Map(StartImportCommand.Types.DataField.Types.DataType dataType)
    {
        switch (dataType)
        {
            case StartImportCommand.Types.DataField.Types.DataType.String: return typeof(string);
            case StartImportCommand.Types.DataField.Types.DataType.Int:    return typeof(int);
            case StartImportCommand.Types.DataField.Types.DataType.Double: return typeof(double);
            case StartImportCommand.Types.DataField.Types.DataType.Bool:   return typeof(bool);
            case StartImportCommand.Types.DataField.Types.DataType.Date:   return typeof(DateTime);
            default:                                                       throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null);
        }
    }
}

}
