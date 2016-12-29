namespace Proekspert.PhoneServiceTask
{
    public class LegacyDataContent
    {
        public int Index { get; }
        public string Data { get; }

        public LegacyDataContent(int index, string data)
        {
            Index = index;
            Data = data;
        }
    }
}
