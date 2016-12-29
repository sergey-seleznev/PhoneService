using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Proekspert.PhoneServiceTask
{
    public class ServiceDataContent : IPositionedStringDeserializable
    {
        #region Data fields (properties)

        public bool IsActive { get; private set; }
        public string Number { get; private set; }
        public string Language { get; private set; }
        public DateTime? End { get; private set; }

        public bool IsXlActive { get; private set; }
        public string XlLanguage { get; private set; }
        public TimeSpan? XlActivationTime { get; private set; }
        public TimeSpan? XlEndTime { get; private set; }

        public bool IsOverrideListUsed { get; private set; }
        public IList<OverrideListItem> OverrideList { get; private set; }

        #endregion
        
        #region Constructor
        
        public ServiceDataContent(IDictionary<string, string> languageProvider)
        {
            _languageProvider = languageProvider;
        }
        private readonly IDictionary<string, string> _languageProvider;

        #endregion


        #region IPositionedStringDeserializable

        private static string _regexPattern;
        private static string RegexPattern
        {
            get
            {
                if (_regexPattern == null)
                {
                    var assembly = typeof(ServiceDataContent).GetTypeInfo().Assembly;
                    var resourceStream = assembly.GetManifestResourceStream("PhoneService." + 
                        "ServiceData.ServiceDataContent.ServiceDataContent.regex");
                        
                    using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
                    {
                        _regexPattern = reader.ReadToEnd();
                    }
                }
                
                return _regexPattern;
            }

        }

        private string GetLanguageName(string code)
        {
            string name = code;

            if (_languageProvider != null && 
                _languageProvider.ContainsKey(code))
                    name = _languageProvider[code];

            if (string.IsNullOrWhiteSpace(name))
                name = null;

            return name;
        }

        private TimeSpan? GetTime(string str)
        {
            TimeSpan ts;

            return TimeSpan.TryParseExact(str, "hhmm", 
                CultureInfo.InvariantCulture, out ts) ?
                    ts : (TimeSpan?)null;
        }

        private DateTime? GetDate(string str)
        {
            DateTime dt;

            return DateTime.TryParseExact(str, "yyyyMMdd",
                CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt) ?
                    dt : (DateTime?)null;
        }

        // needs refactoring
        public bool DeserializePositionedString(String str)
        {
            if (string.IsNullOrWhiteSpace(str)) return false;

            Match match = Regex.Match(str, RegexPattern,
                    RegexOptions.IgnorePatternWhitespace);
            if (!match.Success) return false;
            
            // helpers
            var GroupValue = new Func<string, string>(g => match.Groups[g].Value);
            var GroupCaptures = new Func<string, CaptureCollection>(g => match.Groups[g].Captures);
            
            IsActive = GroupValue("A") == "A";

            Number = GroupValue("B").Trim();
            if (string.IsNullOrWhiteSpace(Number)) return false;

            Language = GetLanguageName(GroupValue("D"));
            if (IsActive && Language == null) return false;

            End = GetDate(GroupValue("F")) + 
                  GetTime(GroupValue("G"));
            if (IsActive && End == null) return false;

            IsXlActive = GroupValue("C") == "J";
            if (IsXlActive)
            {
                XlLanguage = GetLanguageName(GroupValue("E")) ?? Language;
                XlActivationTime = GetTime(GroupValue("H"));
                XlEndTime = GetTime(GroupValue("I"));
                
                if (XlActivationTime == null ||
                    XlEndTime == null) return false;
            }
            else
            {
                XlLanguage = null;
                XlActivationTime = null;
                XlEndTime = null;
            }

            IsOverrideListUsed = GroupValue("J") == "K";
            if (IsOverrideListUsed)
            {
                OverrideList = new List<OverrideListItem>();

                CaptureCollection phoneCaptures = GroupCaptures("K");
                CaptureCollection nameCaptures = GroupCaptures("L");

                int count = Math.Min(phoneCaptures.Count, nameCaptures.Count);

                for (int i = 0; i < count; i++)
                {
                    string phone = phoneCaptures[i].Value.Trim();
                    string name = nameCaptures[i].Value.Trim();

                    if (!string.IsNullOrWhiteSpace(phone) || 
                        !string.IsNullOrWhiteSpace(name))
                    {
                        OverrideList.Add(new OverrideListItem(phone, name));
                    }
                }
            }
            else
            {
                OverrideList = null;
            }

            return true;
        }

        #endregion
        
    }
}
