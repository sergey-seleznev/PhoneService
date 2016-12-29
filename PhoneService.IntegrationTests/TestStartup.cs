using Microsoft.AspNetCore.Hosting;
using Moq;
using Proekspert.PhoneServiceTask.Host;
using System;

namespace Proekspert.PhoneServiceTask.IntegrationTests
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TestStartup : Startup
    {
        public TestStartup(IHostingEnvironment env) : base(env) { }

        public static string Number => NumberPadded.Trim();
        private static string NumberPadded = "0551234567          ";

        private ILegacyDataProvider CreateLegacyDataProviderMock()
        {
            int index = 0;
            string data = $"A{NumberPadded}JE 20111023160008001200E";
            
            var legacyDataProvider = new Mock<ILegacyDataProvider>();

            legacyDataProvider
                .Setup(m => m.ReadDataAsync())
                .ReturnsAsync(new LegacyDataContent(index, data));

            return legacyDataProvider.Object;
        }
        
        protected override PhoneService CreatePhoneServiceInstance(IServiceProvider sp)
        {
            return new PhoneService(
                legacyDataProvider: CreateLegacyDataProviderMock(), 
                languageProvider: null, 
                updateIntervalMSec: 1000);
        }
    }
}
