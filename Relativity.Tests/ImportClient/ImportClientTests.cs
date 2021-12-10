using FluentAssertions;
using Grpc.Core;
using Moq;
using ReductechEntityImport;

namespace Reductech.EDR.Connectors.Relativity.Tests.ImportClient;

public class ImportClientTests
{
    [Fact]
    public void StartImportMustWork()
    {
        var startCommand = new StartImportCommand()
        {
            ControlNumberField  = "ControlNumber",
            FilePathField       = "FilePath",
            FolderPathField     = "FolderPath",
            RelativityPassword  = "MyPassword",
            RelativityUsername  = "MyUsername",
            RelativityWebAPIUrl = "MyURl",
            WorkspaceArtifactId = 12345
        };

        var mockRepo = new MockRepository(MockBehavior.Strict);

        var mockInvoker = mockRepo.Create<CallInvoker>();

        mockInvoker.Setup(x => x.BlockingUnaryCall(
                              It.Is<Method<StartImportCommand, StartImportReply>>(
                                  c => c.Name == "StartImport"
                              ),
                              It.IsAny<string>(),
                              It.IsAny<CallOptions>(),
                              startCommand
                          )).Returns(new StartImportReply(){Message = "Test Message", Success = true})
                                  
            ;

        var client =
            new Reductech_Entity_Import.Reductech_Entity_ImportClient(mockInvoker.Object);

        var response = client.StartImport(startCommand);

        response.Message.Should().Be("Test Message");
        response.Success.Should().BeTrue();
    }
        
    [Fact]
    public async Task StartImportAsyncMustWork()
    {
        var startCommand = new StartImportCommand()
        {
            ControlNumberField  = "ControlNumber",
            FilePathField       = "FilePath",
            FolderPathField     = "FolderPath",
            RelativityPassword  = "MyPassword",
            RelativityUsername  = "MyUsername",
            RelativityWebAPIUrl = "MyURl",
            WorkspaceArtifactId = 12345
        };

        var mockRepo = new MockRepository(MockBehavior.Strict);

        var mockInvoker = mockRepo.Create<CallInvoker>();

        mockInvoker.Setup(x => x.AsyncUnaryCall(
                              It.Is<Method<StartImportCommand, StartImportReply>>(
                                  c => c.Name == "StartImport"
                              ),
                              It.IsAny<string>(),
                              It.IsAny<CallOptions>(),
                              startCommand
                          )).Returns(
                new AsyncUnaryCall<StartImportReply>(
                    Task.FromResult(new StartImportReply(){Message = "Test Message", Success = true}),
                    _ => Task.FromResult(new Metadata()),
                    _ =>                         new Status(StatusCode.OK, "DetailString"),
                    _=> new Metadata(),
                    _=>{},
                    "MyState"
                ))
                                  
            ;

        var client =
            new Reductech_Entity_Import.Reductech_Entity_ImportClient(mockInvoker.Object);

        var response = await client.StartImportAsync(startCommand).ResponseAsync;

        response.Message.Should().Be("Test Message");
        response.Success.Should().BeTrue();
    }


    [Fact]
    public async void ImportObjectsMustWork()
    {
        var importObject = new ImportObject(){Values = { new ImportObject.Types.FieldValue()
        {
            StringValue = "Hello World"
        } }};

        var mockRepo = new MockRepository(MockBehavior.Strict);

        var mockInvoker = mockRepo.Create<CallInvoker>();

        mockInvoker.Setup(x => x.AsyncClientStreamingCall(
                              It.Is<Method<ImportObject, ImportDataReply>>(
                                  c => c.Name == nameof(Reductech_Entity_Import.Reductech_Entity_ImportClient.ImportData)
                              ),
                              It.IsAny<string>(),
                              It.IsAny<CallOptions>()
                                      
                          ))
            .Returns(new AsyncClientStreamingCall<ImportObject, ImportDataReply>(
                         new ClientRequestStream<ImportObject,ImportDataReply>(WriteOptions.Default),
                         Task.FromResult(new ImportDataReply(){Message = "Test Message", Success = true}),
                         Task.FromResult(new Metadata()) ,
                         ()=>new Status(StatusCode.OK, "TestDetail"),
                         ()=> new Metadata(),
                         ()=>{}
                     ));

        var client =
            new Reductech_Entity_Import.Reductech_Entity_ImportClient(mockInvoker.Object);


        var call = client.ImportData(new CallOptions());

        await call.RequestStream.WriteAsync(importObject);
        await call.RequestStream.CompleteAsync();

        var response = await  call.ResponseAsync;

        response.Message.Should().Be("Test Message");
        response.Success.Should().BeTrue();
    }

    public class ClientRequestStream<TRequest, TResponse> : 
        IClientStreamWriter<TRequest>,
        IAsyncStreamWriter<TRequest>
    {

        public ClientRequestStream(WriteOptions options)
        {
            WriteOptions = options;
        }

        public Task WriteAsync(TRequest message) => Task.CompletedTask;

        public Task CompleteAsync() => Task.CompletedTask;

        public WriteOptions WriteOptions { get; set; }

        private WriteFlags GetWriteFlags()
        {
            return WriteOptions == null ? (WriteFlags) 0 : WriteOptions.Flags;
        }
    }
}
