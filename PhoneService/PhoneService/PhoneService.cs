using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Proekspert.PhoneServiceTask
{
    public class PhoneService : IPhoneService, IDisposable
    {
        private readonly ILegacyDataProvider _legacyDataProvider;
        private readonly IDictionary<string, string> _languageProvider;
        private readonly int _updateIntervalMs;
        private readonly CancellationTokenSource _cancel;

        public PhoneService(ILegacyDataProvider legacyDataProvider, IDictionary<string, string> languageProvider,
            int updateIntervalMSec)
        {
            _legacyDataProvider = legacyDataProvider;
            _languageProvider = languageProvider;
            _updateIntervalMs = updateIntervalMSec;

            // start cancellable update thread
            _cancel = new CancellationTokenSource();
            Task.Run(() => UpdateThreadFunc(_cancel.Token), _cancel.Token);
        }

        public void Dispose()
        {
            _cancel.Cancel(false);
        }

        #region Update loop

        private ServiceData _data;

        private async void UpdateThreadFunc(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                ServiceData newData = await UpdateData();

                // update data enforcing thread safety
                lock (this)
                {
                    _data = newData;
                }

                DataUpdated?.Invoke(this, new EventArgs());

                try
                {
                    await Task.Delay(_updateIntervalMs, ct);
                }
                catch
                {
                    // skip thread cancellation exception
                }
                
            }
        }
        private async Task<ServiceData> UpdateData()
        {
            if (_legacyDataProvider == null) return null;

            LegacyDataContent legacyData = await _legacyDataProvider.ReadDataAsync();

            // content
            ServiceDataContent content = new ServiceDataContent(_languageProvider);
            bool isContentValid = content.DeserializePositionedString(legacyData.Data);
            if (!isContentValid) content = null;

            // container
            return new ServiceData(legacyData.Index, content);
        }

        #endregion

        public event EventHandler DataUpdated;
        public ServiceData GetCurrentData()
        {
            // protect against simultaneous read-write access
            lock (this)
            {
                return _data;
            }
        }

        
    }
}
