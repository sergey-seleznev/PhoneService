using System;

namespace Proekspert.PhoneServiceTask
{
    public interface IPhoneService
    {
        ServiceData GetCurrentData();

        event EventHandler DataUpdated;
    }
}
