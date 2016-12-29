using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Proekspert.PhoneServiceTask.Tests
{
    public class PhoneServiceTests
    {
        private ILegacyDataProvider GetLegacyDataProviderMock(bool validData = true)
        {
            int index = 0;
            string legacyData = validData ?
                "A0551234567          JE 20111023160008001200E" : 
                "invalid-data";
            
            var legacyDataProvider = new Mock<ILegacyDataProvider>();

            legacyDataProvider
                .Setup(m => m.ReadDataAsync())
                .ReturnsAsync(new LegacyDataContent(index, legacyData));

            return legacyDataProvider.Object;
        }

        [Fact(DisplayName = "Update iteration")]
        public async void SuccessfulUpdateIterationTest()
        {
            var legacyDataProvider = GetLegacyDataProviderMock();

            using (var phoneService = new PhoneService(legacyDataProvider, null, 0))
            {
                await Task.Delay(1000);
                Assert.NotNull(phoneService.GetCurrentData());
            }
        }

        [Fact(DisplayName = "Update iteration with listener")]
        public async void SuccessfulUpdateIterationTestWithAListenerTest()
        {
            var sync = new SemaphoreSlim(0, 1);
            
            var legacyDataProvider = GetLegacyDataProviderMock();
            var phoneService = new PhoneService(legacyDataProvider, null, 0);

            // add DataUpdated handler
            phoneService.DataUpdated += (sender, args) =>
            {
                if (sync.CurrentCount == 0)
                    sync.Release();
            };

            // wait for DataUpdated event
            await sync.WaitAsync();

            ServiceData serviceData = phoneService.GetCurrentData();

            Assert.NotNull(serviceData);
        }

        [Fact(DisplayName = "Invalid data update iteration")]
        public async void UnsuccessfulUpdateIterationTest()
        {
            var sync = new SemaphoreSlim(0, 1);
            
            var legacyDataProvider = GetLegacyDataProviderMock(validData: false);
            
            using (var phoneService = new PhoneService(legacyDataProvider, null, 0))
            {
                // add DataUpdated handler
                phoneService.DataUpdated += (sender, args) =>
                {
                    if (sync.CurrentCount == 0)
                        sync.Release();
                };

                await sync.WaitAsync();

                ServiceData data = phoneService.GetCurrentData();
                Assert.NotNull(data);
                Assert.Null(data.State);
            }    
        }

        [Fact(DisplayName = "Data provider is not specified")]
        public async void NoDataProviderTest()
        {
            using (var phoneService = new PhoneService(null, null, 0))
            {
                await Task.Delay(300);

                Assert.Null(phoneService.GetCurrentData());
            }
        }

        [Fact(DisplayName = "A cycle of update iterations")]
        public async void SuccessfulUpdateCycleTest()
        {
            var sync = new SemaphoreSlim(0, 1);
            
            var legacyDataProvider = GetLegacyDataProviderMock();
            var phoneService = new PhoneService(legacyDataProvider, null, 10);

            // add DataUpdated handler
            int count = 0;
            phoneService.DataUpdated += (sender, args) =>
            {
                Interlocked.Increment(ref count);
                if (count == 10 && sync.CurrentCount == 0)
                    sync.Release();
            };

            // wait for DataUpdated event
            await sync.WaitAsync();
        }

    }
}
