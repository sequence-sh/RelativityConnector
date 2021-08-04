using System.Collections.Generic;
using System.Linq;
using Reductech.EDR.Connectors.Relativity.Managers;
using Xunit;

namespace Reductech.EDR.Connectors.Relativity.Tests
{

[AutoTheory.UseTestOutputHelper]
public partial class CodeGeneration
{
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

}
