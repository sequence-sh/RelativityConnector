using System;
using kCura.Relativity.DataReaderClient;
using ReductechRelativityImport;

namespace ImportClient
{

public static class JobHelpers
{
    public static void SetSettings(Settings settings, StartImportCommand command)
    {
        settings.ApplicationName = "Reductech Import";
        settings.CaseArtifactId  = command.WorkspaceArtifactId; // = 1003663;

        settings.SelectedIdentifierFieldName = command.ControlNumberField;
        settings.ArtifactTypeId              = 10;
        //job.Settings.Billable = ;

        settings.CopyFilesToDocumentRepository = true;
        //job.Settings.DisableControlNumberCompatibilityMode = ;
        settings.DisableExtractedTextEncodingCheck = null;
        //job.Settings.DisableExtractedTextFileLocationValidation = false;
        settings.DisableNativeLocationValidation = null;
        settings.DisableNativeValidation         = null;
        settings.DisableUserSecurityCheck        = false;
        //job.Settings.ExtractedTextEncoding = ;
        settings.ExtractedTextFieldContainsFilePath = false;
        settings.FileSizeMapped                     = false;

        settings.NativeFilePathSourceFieldName = command.FilePathField;
        settings.FolderPathSourceFieldName     = command.FolderPathField;
        settings.StartRecordNumber             = 0;
    }

    public static void SetJobMessages(ImportBulkArtifactJob job)
    {
        // This event provides the JobReport object.
        job.OnComplete += report => { Console.WriteLine("The job has completed."); };

        // This event provides an IDictionary object with well-known parameters.
        job.OnError += row =>
        {
            Console.WriteLine(row["Line Number"]);
            Console.WriteLine(row["Identifier"]);
            Console.WriteLine(row["Message"]);
        };

        // This event provides the JobReport object.
        job.OnFatalException += report =>
        {
            Console.WriteLine("The job experienced a fatal exception: " + report.FatalException);
        };

        // This event provides the Status object.
        job.OnMessage += status => { Console.WriteLine("Job message: " + status.Message); };

        // This event provides the FullStatus object.
        job.OnProcessProgress += status =>
        {
            Console.WriteLine("Job start time: " + status.StartTime);
            Console.WriteLine("Job end time: " + status.EndTime);
            Console.WriteLine("Job process ID: " + status.ProcessID);
            Console.WriteLine("Job total records: " + status.TotalRecords);
            Console.WriteLine("Job total records processed: " + status.TotalRecordsProcessed);
            Console.WriteLine("Job total records processed with warnings: " +
                              status.TotalRecordsProcessedWithWarnings);
            Console.WriteLine("Job total records processed with errors: " + status.TotalRecordsProcessedWithErrors);
            Console.WriteLine("Job total records: " + status.TotalRecordsDisplay);
            Console.WriteLine("Job total records processed: " + status.TotalRecordsProcessedDisplay);
            Console.WriteLine("Job status suffix: " + status.StatusSuffixEntries);
        };

        // This event provides the row number.
        job.OnProgress += row => { Console.WriteLine("Job progress line number: " + row); };
    }
}

}
