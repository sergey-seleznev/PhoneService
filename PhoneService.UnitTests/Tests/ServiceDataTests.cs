using Xunit;

namespace Proekspert.PhoneServiceTask.Tests
{
    public class ServiceDataTests
    {
        [Fact(DisplayName = "Construction")]
        public void ContainerConstructionTest()
        {
            int index = 329;
            ServiceDataContent content = new ServiceDataContent(null);

            ServiceData serviceData = new ServiceData(index, content);

            Assert.Equal(index, serviceData.Index);
            Assert.Equal(content, serviceData.State);
        }
    }
}
