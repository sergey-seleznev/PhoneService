using System.Net.Http;
using System.Threading.Tasks;

namespace Proekspert.PhoneServiceTask
{
    public class WebLegacyDataProvider : LegacyDataProvider
    {
        public WebLegacyDataProvider(string uriFormat, int firstIndex, int lastIndex) : 
            base(uriFormat, firstIndex, lastIndex) { }
        
        protected override async Task<string> GetDataAsync(string uri)
        {
            string result = null;

            if (!string.IsNullOrWhiteSpace(uri))
            {
                try
                {
                    var client = new HttpClient();
                    result = await client.GetStringAsync(uri);
                }
                catch
                {
                    // log
                }
            }
            
            return result;
        }
    }
}
