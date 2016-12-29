using System;
using System.Collections.Generic;
using Xunit;

namespace Proekspert.PhoneServiceTask.Tests
{
    public class ServiceDataContentTests
    {
        private IDictionary<string, string> GetLanguageProvider()
        {
            return new Dictionary<string, string>
            {
                ["E"] = "Estonian",
                ["I"] = "English"
            };
        }

        #region Construction

        [Fact(DisplayName = "Construction")]
        public void ConstructionTest()
        {
            var languages = GetLanguageProvider();

            // ReSharper disable once UnusedVariable
            var content = new ServiceDataContent(languages);
        }

        [Fact(DisplayName = "Construction without language provider")]
        public void ConstructionWithoutLanguageProviderTest()
        {
            // ReSharper disable once UnusedVariable
            var content = new ServiceDataContent(languageProvider: null);
        }

        #endregion

        // Data elements:

        #region A: State

        [Fact(DisplayName = "A: active state")]
        public void StateActiveTest()
        {
            string legacyDataString = @"A0551234567          JE 20111023160008001200E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.True(content.IsActive);
        }

        [Fact(DisplayName = "A: passive state")]
        public void StatePassiveTest()
        {
            string legacyDataString = @"P0502234569          EE 201201012359        E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.False(content.IsActive);
        }

        #endregion

        #region B: Number

        [Fact(DisplayName = "B: full number")]
        public void FullNumberTest()
        {
            string legacyDataString = @"A05512345671983194312JE 20111023160008001200E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.Equal("05512345671983194312", content.Number);
        }

        [Fact(DisplayName = "B: short number")]
        public void ShortNumberTest()
        {
            string legacyDataString = @"A0551234567          JE 20111023160008001200E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.Equal("0551234567", content.Number);
        }

        [Fact(DisplayName = "B: empty number")]
        public void EmptyNumberTest()
        {
            string legacyDataString = @"A                    JE 20111023160008001200E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }

        [Fact(DisplayName = "B: non-numeric number")]
        public void NonNumericNumberTest()
        {
            string legacyDataString = @"A+7 1-1-APPLE*101#   JE 20111023160008001200E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.Equal("+7 1-1-APPLE*101#", content.Number);
        }

        #endregion

        #region C: XL-state

        [Fact(DisplayName = "C: active XL-service state")]
        public void XlStateActiveTest()
        {
            string legacyDataString = @"A0551234567          JE 20111023160008001200E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.True(content.IsXlActive);
        }

        [Fact(DisplayName = "C: passive XL-service state")]
        public void XlStatePassiveTest()
        {
            string legacyDataString = @"P0502234569          EE 201201012359        E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.False(content.IsXlActive);
        }

        [Fact(DisplayName = "C: incorrect XL-service state")]
        public void XlStateIncorrectTest()
        {
            string legacyDataString = @"P0502234569          YE 201201012359        E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }

        [Fact(DisplayName = "C: empty XL-service state")]
        public void XlStateEmptyTest()
        {
            string legacyDataString = @"P0502234569           E 201201012359        E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }

        #endregion

        #region D: Service Language

        [Fact(DisplayName = "D: service language without dictionary")]
        public void ServiceLanguageNoDictionaryTest()
        {
            string legacyDataString = @"A0502234569          EE 201201012359        E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.Equal("E", content.Language);
        }

        [Fact(DisplayName = "D: service language not in a dictionary")]
        public void ServiceLanguageNotInADictionaryTest()
        {
            var languages = GetLanguageProvider();

            string legacyDataString = @"A0502234569          ER 201201012359        E";

            var content = new ServiceDataContent(languages);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.Equal("R", content.Language);
        }

        [Fact(DisplayName = "D: service language in a dictionary")]
        public void ServiceLanguageInADictionaryTest()
        {
            var languages = GetLanguageProvider();

            string legacyDataString = @"A0502234569          EI 201201012359        E";

            var content = new ServiceDataContent(languages);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.Equal("English", content.Language);
        }

        [Fact(DisplayName = "D: service language empty (active)")]
        public void ServiceLanguageEmptyIfServiceActiveTest()
        {
            var languages = GetLanguageProvider();

            string legacyDataString = @"A0502234569          E  201201012359        E";

            var content = new ServiceDataContent(languages);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }

        [Fact(DisplayName = "D: service language empty (passive)")]
        public void ServiceLanguageEmptyIfServicePassiveTest()
        {
            var languages = GetLanguageProvider();

            string legacyDataString = @"P0502234569          E  201201012359        E";

            var content = new ServiceDataContent(languages);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.Null(content.Language);
        }

        [Fact(DisplayName = "D: service language invalid")]
        public void ServiceLanguageInvalidTest()
        {
            var languages = GetLanguageProvider();

            string legacyDataString = @"A0502234569          $  201201012359        E";

            var content = new ServiceDataContent(languages);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }

        #endregion

        #region E: XL-service language

        [Fact(DisplayName = "E: XL-service language without dictionary")]
        public void XlServiceLanguageNoDictionaryTest()
        {
            string legacyDataString = @"A0551234567          JEE20111023160008001200E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.Equal("E", content.XlLanguage);
        }

        [Fact(DisplayName = "E: XL-service language not in a dictionary")]
        public void XlServiceLanguageNotInADictionaryTest()
        {
            var languages = GetLanguageProvider();

            string legacyDataString = @"A0551234567          JER20111023160008001200E";

            var content = new ServiceDataContent(languages);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.Equal("R", content.XlLanguage);
        }

        [Fact(DisplayName = "E: XL-service language in a dictionary")]
        public void XlServiceLanguageInADictionaryTest()
        {
            var languages = GetLanguageProvider();

            string legacyDataString = @"A0551234567          JEI20111023160008001200E";

            var content = new ServiceDataContent(languages);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.Equal("English", content.XlLanguage);
        }

        [Fact(DisplayName = "E: XL-service language from language")]
        public void XlServiceLanguageTakenFromLanguageTest()
        {
            var languages = GetLanguageProvider();

            //    service language = 'R'
            // XL-service language = ' '
            string legacyDataString = @"A0551234567          JR 20111023160008001200E";

            var content = new ServiceDataContent(languages);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.Equal("R", content.Language);
            Assert.Equal("R", content.XlLanguage);
        }

        [Fact(DisplayName = "E: XL-service language invalid")]
        public void XlServiceLanguageInvalidTest()
        {
            var languages = GetLanguageProvider();

            string legacyDataString = @"A0551234567          JE@20111023160008001200E";

            var content = new ServiceDataContent(languages);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }

        [Fact(DisplayName = "E: XL-service language empty (active)")]
        public void XlLanguageEmptyIfXlServiceActiveTest()
        {
            var languages = GetLanguageProvider();

            string legacyDataString = @"A0502234569          E  201201012359        E";

            var content = new ServiceDataContent(languages);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }

        [Fact(DisplayName = "E: XL-service language empty (passive)")]
        public void XlLanguageEmptyIfXlServicePassiveTest()
        {
            var languages = GetLanguageProvider();

            string legacyDataString = @"P0502234569          E  201201012359        E";

            var content = new ServiceDataContent(languages);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.Null(content.XlLanguage);
        }
        
        #endregion

        #region F: Service end date
        
        [Fact(DisplayName = "F: service end date")]
        public void ServiceEndDateTest()
        {
            string legacyDataString = @"A0502234569          EE 201201012359        E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.NotNull(content.End);
            Assert.Equal(new DateTime(2012, 01, 01), ((DateTime)content.End).Date);
        }

        [Fact(DisplayName = "F: service end date empty (active)")]
        public void ServiceEndDateEmptyIfServiceActiveTest()
        {
            string legacyDataString = @"A0502234569          EE                     E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }

        [Fact(DisplayName = "F: service end date empty (passive)")]
        public void ServiceEndDateEmptyIfServicePassiveTest()
        {
            string legacyDataString = @"P0502234569          E                      E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.Null(content.End);
        }

        [Fact(DisplayName = "F: service end date invalid")]
        public void ServiceEndDateInvalidTest()
        {
            string legacyDataString = @"A0502234569          EEE2oo2010I2359        E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }

        #endregion

        #region G: Service end time

        [Fact(DisplayName = "G: service end time")]
        public void ServiceEndTimeTest()
        {
            string legacyDataString = @"A0502234569          EE 201201012359        E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.NotNull(content.End);
            Assert.Equal(new TimeSpan(23, 59, 0), ((DateTime)content.End).TimeOfDay);
        }

        [Fact(DisplayName = "G: service end time empty (active)")]
        public void ServiceEndTimeEmptyIfServiceActiveTest()
        {
            string legacyDataString = @"A0502234569          JE                     E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }

        [Fact(DisplayName = "G: service end time empty (passive)")]
        public void ServiceEndTimeEmptyIfServicePassiveTest()
        {
            string legacyDataString = @"P0502234569          E                      E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.Null(content.End);
        }

        [Fact(DisplayName = "G: service end time invalid")]
        public void ServiceEndTimeInvalidTest()
        {
            string legacyDataString = @"A0502234569          JEE200201013169        E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }

        #endregion

        #region H: XL-service activation time

        [Fact(DisplayName = "H: XL-service activation")]
        public void XlServiceActivationTimeTest()
        {
            string legacyDataString = @"A0502234569          JE 20120101235922342357E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.Equal(new TimeSpan(22, 34, 0), content.XlActivationTime);
        }

        [Fact(DisplayName = "H: XL-service activation empty (active)")]
        public void XlServiceActivationTimeEmptyIfXlServiceActiveTest()
        {
            string legacyDataString = @"A0502234569          JE 201201012359    2300E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }

        [Fact(DisplayName = "H: XL-service activation empty (passive)")]
        public void XlServiceActivationTimeEmptyIfXlServiceNotActiveTest()
        {
            string legacyDataString = @"P0502234569          E                  2300E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.Null(content.XlActivationTime);
        }

        [Fact(DisplayName = "H: XL-service activation invalid")]
        public void XlServiceActivationTimeInvalidTest()
        {
            string legacyDataString = @"A0502234569          JEE200201013169IOIO    E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }

        #endregion

        #region I: XL-service end time

        [Fact(DisplayName = "I: XL-service end time")]
        public void XlServiceEndTimeTest()
        {
            string legacyDataString = @"A0502234569          JE 20120101235922342348E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.Equal(new TimeSpan(23, 48, 0), content.XlEndTime);
        }

        [Fact(DisplayName = "I: XL-service end time empty (active)")]
        public void XlServiceEndTimeEmptyIfXlServiceActiveTest()
        {
            string legacyDataString = @"A0502234569          JE 2012010123591200    E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }

        [Fact(DisplayName = "I: XL-service end time empty (passive)")]
        public void XlServiceEndTimeEmptyIfXlServiceNotActiveTest()
        {
            string legacyDataString = @"P0502234569          E                      E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.Null(content.XlEndTime);
        }

        [Fact(DisplayName = "I: XL-service end time invalid")]
        public void XlServiceEndTimeInvalidTest()
        {
            string legacyDataString = @"A0502234569          JEE20020101316912022867E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }

        #endregion

        #region J: Override list use flag

        [Fact(DisplayName = "J: override list flag")]
        public void OverrideListUsedNotUsedTest()
        {
            string legacyDataString = @"A05512345671983194312JE 20111023160008001200E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.False(content.IsOverrideListUsed);
        }

        [Fact(DisplayName = "J: override list flag used")]
        public void OverrideListUsedAndEmptyTest()
        {
            string legacyDataString = @"A05512345671983194312JE 20111023160008001200K";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);
            Assert.True(content.IsOverrideListUsed);
            Assert.NotNull(content.OverrideList);
            Assert.Equal(0, content.OverrideList.Count);
        }

        [Fact(DisplayName = "J: override list flag invalid")]
        public void OverrideListInvalidUsedTest()
        {
            string legacyDataString = @"A05512345671983194312JE 20111023160008001200Y";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }

        #endregion

        #region K,L: Override list phone and names
        
        [Fact(DisplayName = "K,L: override list")]
        public void OverrideListTest()
        {
            int numberLen = 15;
            int nameLen = 20;

            string legacyDataString =
                "A0551234555          " +
                "JII20111111215900001200K" +

                "+7-1-1-APPLE*1#".PadRight(numberLen) +     // [0] 'normal' number
                string.Empty.PadRight(numberLen) +          //     empty item in between
                string.Empty.PadRight(numberLen) +          // [1] empty number for non-empty name
                "0506669999".PadRight(numberLen) +          // [2] duplicate number
                "0506669999".PadRight(numberLen) +          // [3] duplicate number
                string.Empty.PadRight(numberLen * 3) +      //     empty items
                
                "Jaan Juurikas".PadRight(nameLen) +         // [0] duplicate name
                string.Empty.PadRight(nameLen) +            //     empty item in between
                "Peeter".PadRight(nameLen) +                // [1] normal name
                "Jaan Juurikas".PadRight(nameLen) +         // [2] duplicate name
                string.Empty.PadRight(nameLen) +            // [3] empty name for non-empty number
                string.Empty.PadRight(nameLen * 3);         //     empty items

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.True(success);

            Assert.NotNull(content.OverrideList);
            Assert.Equal(4, content.OverrideList.Count);

            OverrideListItem item = content.OverrideList[0];
            Assert.Equal("+7-1-1-APPLE*1#", item.Phone);
            Assert.Equal("Jaan Juurikas", item.Name);

            item = content.OverrideList[1];
            Assert.Equal(string.Empty, item.Phone);
            Assert.Equal("Peeter", item.Name);

            item = content.OverrideList[2];
            Assert.Equal("0506669999", item.Phone);
            Assert.Equal("Jaan Juurikas", item.Name);

            item = content.OverrideList[3];
            Assert.Equal("0506669999", item.Phone);
            Assert.Equal(string.Empty, item.Name);
        }
        
        #endregion


        #region Negative scenarios

        [Fact(DisplayName = "Null data")]
        public void NullDataTest()
        {
            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(null);

            Assert.False(success);
        }

        [Fact(DisplayName = "Empty data")]
        public void EmptyDataTest()
        {
            string legacyDataString = string.Empty;

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }

        [Fact(DisplayName = "Whitespace data")]
        public void WhitespaceDataTest()
        {
            string legacyDataString = "\t\r\n";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }
        
        [Fact(DisplayName = "Unexpected data (beginning)")]
        public void UnexpectedDataInTheBeginningTest()
        {
            string legacyDataString = @">A05512345671983194312JE 20111023160008001200E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }

        [Fact(DisplayName = "Unexpected data (end)")]
        public void UnexpectedDataInTheEndTest()
        {
            string legacyDataString = @"A0551234555          JII20111111215900001200K0552212211     0506669999                                                                                               Rein Ratas                                                                                                                                                      ~~~";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }

        [Fact(DisplayName = "Unexpected data (middle)")]
        public void UnexpectedDataInBetweenTest()
        {
            string legacyDataString = @"A0551-234-56 7198319 4312JE 20111023160008001200E";

            var content = new ServiceDataContent(null);
            bool success = content.DeserializePositionedString(legacyDataString);

            Assert.False(success);
        }

        #endregion

    }
}
