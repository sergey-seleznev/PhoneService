using System;
using System.Threading.Tasks;

namespace Proekspert.PhoneServiceTask
{
    public abstract class LegacyDataProvider : ILegacyDataProvider
    {
        private int _index;
        private readonly string _uriFormat;
        private readonly int _firstIndex;
        private readonly int _lastIndex;

        protected LegacyDataProvider(string uriFormat, int firstIndex, int lastIndex)
        {
            _uriFormat = uriFormat;

            _index = 
            _firstIndex = Math.Min(firstIndex, lastIndex);
            _lastIndex = Math.Max(firstIndex, lastIndex);
        }

        private int GetAndUpdateRequestIndex()
        {
            int currentIndex;

            // ensure exclusive access
            lock (this)
            {
                // save the value to be returned
                currentIndex = _index;
                
                // increment
                _index++;

                // loop the index
                if (_index > _lastIndex)
                    _index = _firstIndex;
            }

            return currentIndex;
        }
        private string ComposeUri(int index)
        {
            try
            {
                return string.Format(_uriFormat, index);
            }
            catch
            {
                // log invalid format string
                return null;
            }
        }

        protected abstract Task<string> GetDataAsync(string uri);
        
        public async Task<LegacyDataContent> ReadDataAsync()
        {
            int index = GetAndUpdateRequestIndex();
            string uri = ComposeUri(index);
            string data = await GetDataAsync(uri);
            
            return new LegacyDataContent(index, data);
        }
    }
}
