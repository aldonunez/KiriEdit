/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using KiriFT;

namespace KiriEdit
{
    internal class FontInfo
    {
        private struct LocalizedSfntName
        {
            public string String;
            public int LangId;
        }

        public string Copyright { get; private set; }

        public FontInfo(Face face)
        {
            ExtractInfo(face);
        }

        private void ExtractInfo(Face face)
        {
            uint count = face.GetSfntNameCount();
            var localizedCopyright = new LocalizedSfntName();

            for (uint i = 0; i < count; i++)
            {
                SfntName sfntName = face.GetSfntName(i);

                if (sfntName.NameId == 0)
                {
                    SetLocalizedSfntName(sfntName.String, sfntName.LanguageId, ref localizedCopyright);
                }
            }

            Copyright = localizedCopyright.String;
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
