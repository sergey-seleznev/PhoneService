using System.Threading.Tasks;

namespace Proekspert.PhoneServiceTask
{
    public interface ILegacyDataProvider
    {
        Task<LegacyDataContent> ReadDataAsync();
    }
}
