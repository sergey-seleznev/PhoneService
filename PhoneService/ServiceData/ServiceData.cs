namespace Proekspert.PhoneServiceTask
{
    public class ServiceData
    {
        public int Index { get; }
        public ServiceDataContent State { get; }

        public ServiceData(int index, ServiceDataContent state)
        {
            Index = index;
            State = state;
        }

    }
}
