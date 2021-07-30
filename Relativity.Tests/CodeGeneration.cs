using Xunit;

namespace Reductech.EDR.Connectors.Relativity.Tests
{
    [AutoTheory.UseTestOutputHelper]
    public partial class CodeGeneration
    {
        [Fact]
        public void GenerateCode()
        {
            var text = CodeGenerator.Generate();

            TestOutputHelper.WriteLine(text);
        }

    }
}
