/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using KiriFT;

namespace KiriEdit
{
    public class FontInfo
    {
        private struct LocalizedSfntName
        {
            public string String;
            public int LangId;
        }

        public string FamilyName { get; private set; }
        public string StyleName { get; private set; }

        public string Copyright { get; private set; }
        public string License { get; private set; }
        public string LicenseUrl { get; private set; }
        public string Version { get; private set; }

        public FontInfo(Face face)
        {
            FamilyName = face.FamilyName;
            StyleName = face.StyleName;

            ExtractStrings(face);
        }

        private void ExtractStrings(Face face)
        {
            uint count = face.GetSfntNameCount();
            var localizedCopyright = new LocalizedSfntName();
            var localizedLicense = new LocalizedSfntName();
            var localizedLicenseUrl = new LocalizedSfntName();
            var localizedVersion = new LocalizedSfntName();

            for (uint i = 0; i < count; i++)
            {
                SfntName sfntName = face.GetSfntName(i);

                switch (sfntName.NameId)
                {
                    case 0:
                        SetLocalizedSfntName(sfntName.String, sfntName.LanguageId, ref localizedCopyright);
                        break;

                    case 5:
                        SetLocalizedSfntName(sfntName.String, sfntName.LanguageId, ref localizedVersion);
                        break;

                    case 13:
                        SetLocalizedSfntName(sfntName.String, sfntName.LanguageId, ref localizedLicense);
                        break;

                    case 14:
                        SetLocalizedSfntName(sfntName.String, sfntName.LanguageId, ref localizedLicenseUrl);
                        break;
                }
            }

            Copyright = localizedCopyright.String;
            License = localizedLicense.String;
            LicenseUrl = localizedLicenseUrl.String;
            Version = localizedVersion.String;
        }

        private static void SetLocalizedSfntName(string s, int langId, ref LocalizedSfntName sfntName)
        {
            int curLangId = System.Globalization.CultureInfo.CurrentCulture.LCID & 0xFFFF;
            int curPrimLangId = curLangId & 0x03FF;

            // If the stored string is not set
            // Or the incoming LANGID = current LANGID
            // Or the stored LANGID <> current LANGID
            //      And the incoming primary LANGID = current primary LANGID
            // Or the stored primary LANGID <> current primary LANGID
            //      And the incoming primary LANGID = English

            if (sfntName.String == null
                || langId == curLangId
                || ((sfntName.LangId != curLangId) && (langId & 0x03FF) == curPrimLangId)
                || ((sfntName.LangId & 0x03FF) != curPrimLangId && (langId & 0x03FF) == 9)
                )
            {
                sfntName.String = s;
                sfntName.LangId = langId;
            }
        }
    }
}
