using System.Threading.Tasks;
using Xunit;

namespace Proekspert.PhoneServiceTask.Tests
{
    public class LegacyDataReaderTests
    {
        [Fact(DisplayName ="Construction")]
        public void ConstructionTest()
        {
            // ReSharper disable once UnusedVariable
            ILegacyDataProvider reader = new WebLegacyDataProvider("file_{0}.txt", 12, 12);
        }

        [Fact(DisplayName = "Passing null as URI format")]
        public async void NullUriTest()
        {
            ILegacyDataProvider reader = new WebLegacyDataProvider(null, 1, 1);

            var result = await reader.ReadDataAsync();

            Assert.Equal(null, result.Data);
        }

        [Fact(DisplayName = "Correct index looping")]
        public async void IndexLoopTest()
        {
            ILegacyDataProvider reader = new WebLegacyDataProvider(null, 1, 3);

            // 1
            LegacyDataContent result = await reader.ReadDataAsync();
            Assert.Equal(1, result.Index);

            // 2
            result = await reader.ReadDataAsync();
            Assert.Equal(2, result.Index);

            // 3
            result = await reader.ReadDataAsync();
            Assert.Equal(3, result.Index);

            // 1
            result = await reader.ReadDataAsync();
            Assert.Equal(1, result.Index);
        }
        
        [Fact(DisplayName = "Web resource reading")]
        public async void SuccessfullWebResourceReadTest()
        {
            string format = "http://example.com/";
            
            ILegacyDataProvider reader = new WebLegacyDataProvider(format, 1, 1);
            LegacyDataContent result = await reader.ReadDataAsync();

            // verify resource's html string length
            Assert.Equal(1270, result.Data.Length);
        }

        [Fact(DisplayName = "Missing web resource reading")]
        public async void MissingWebResourceReadTest()
        {
            string format = "http://not-existing-uri.com";

            ILegacyDataProvider reader = new WebLegacyDataProvider(format, 1, 1);
            LegacyDataContent result = await reader.ReadDataAsync();

            await Task.Delay(300);

            Assert.Null(result.Data);
        }

    }
}
