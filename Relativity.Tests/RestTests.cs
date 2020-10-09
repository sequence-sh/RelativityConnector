using Reductech.Utilities.Testing;
using Xunit;

namespace Reductech.Connectors.Relativity.Tests
{

    public class Tests
    {
        //[Fact] //Disabled for ci
        public void TestMain()
        {
            var settings = new RelativitySettings
            {
                RelativityUsername = "relativity.admin@relativity.com",
                RelativityPassword = "Test1234!",
                Url = "http://172.20.0.11/"
            };

            var r = Rest.Main(settings);

            r.ShouldBeSuccessful();
        }
    }

}
