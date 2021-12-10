using System.Linq;
using Reductech.EDR.Connectors.Relativity.Managers;
using Xunit;
using Xunit.Abstractions;

namespace Reductech.EDR.Connectors.Relativity.Tests;

public class CodeGeneration
{
    public CodeGeneration(ITestOutputHelper testOutputHelper)
    {
        TestOutputHelper = testOutputHelper;
    }
    public ITestOutputHelper TestOutputHelper { get; set; }

    public static IEnumerable<object[]> ManagerGenerators =>
        CodeGenerator.ManagerGenerators.Select(x => new object[] { x.Type.Name });

    [Theory]
    [MemberData(nameof(ManagerGenerators))]
    public void GenerateCode(string typeName)
    {
        var managerGenerator = CodeGenerator.ManagerGenerators.Single(x => x.Type.Name == typeName);

        var text = CodeGenerator.Generate(managerGenerator);

        TestOutputHelper.WriteLine(text);
    }
}
