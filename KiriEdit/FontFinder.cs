using Microsoft.Win32;
using SharpFont;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KiriEdit.Font
{
    internal static class FontFinder
    {
        public static async Task<FontFamilyCollection> FindFontsAsync()
        {
            var result = await Task.Run(() =>
            {
                using (var finder = new Finder())
                {
                    finder.Find();
                    return finder.FontFamilyCollection;
                }
            });

            return result;
        }

        private class Finder : IDisposable
        {
            private const string FontsKeyPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts";

            private Library _library;
            private FontFamilyCollection _nameToFamily = new FontFamilyCollection();

            public FontFamilyCollection FontFamilyCollection { get => _nameToFamily; }

            public Finder()
            {
                _library = new Library();
            }

            public void Dispose()
            {
                Dispose(true);
                // No finalizer to suppress.
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_library != null)
                    {
                        _library.Dispose();
                        _library = null;
                    }
                }
            }

            public void Find()
            {
                using (var fontsKey = Registry.LocalMachine.OpenSubKey(FontsKeyPath))
                {
                    foreach (string valName in fontsKey.GetValueNames())
                    {
                        string val = fontsKey.GetValue(valName) as string;

                        if (val == null)
                            continue;

                        string path = val;
                        string fontsPath = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

                        path = Path.Combine(fontsPath, path);

                        ProcessFontFile(path);
                    }
                }
            }

            private void ProcessFontFile(string path)
            {
                int faceCount = 0;

                try
                {
                    // Look up a face with a negative index to find out how many faces there are.
                    var dummyFace = new Face(_library, path, -1);
                    faceCount = dummyFace.FaceCount;
                    dummyFace.Dispose();
                }
                catch
                {
                    // Can't handle the file. So there's nothing to do.
                    return;
                }

                for (int i = 0; i < faceCount; i++)
                {
                    Face face = null;
                    try
                    {
                        face = new Face(_library, path, i);
                        if (face.GetX11FontFormat() == "TrueType")
                            ProcessFace(path, face);
                    }
                    catch
                    {
                        // Move on to the next face.
                    }
                    finally
                    {
                        if (face != null)
                            face.Dispose();
                    }
                }
            }

            private void ProcessFace(string path, Face face)
            {
                var (fontStyle, styleName) = ProcessStyleName(face);
                string gdipFamilyName;

                if (styleName.Length == 0)
                    gdipFamilyName = face.FamilyName;
                else
                    gdipFamilyName = face.FamilyName + " " + styleName;

                FontFamily family;

                if (!_nameToFamily.TryGetValue(gdipFamilyName, out family))
                {
                    family = new FontFamily(gdipFamilyName);
                    _nameToFamily.Add(gdipFamilyName, family);
                }

                FontFace fontFace = new FontFace(family, fontStyle, path, face.FaceIndex);

                family.AddFace(fontStyle, fontFace);
            }

            private (FontStyle fontStyle, string styleName) ProcessStyleName(Face face)
            {
                FontStyle fontStyle = FontStyle.Regular;
                string styleName = face.StyleName;

                if (face.FamilyName.Contains("Arial"))
                    System.Diagnostics.Debug.WriteLine(string.Format("[{0}] :: [{1}]", face.FamilyName, face.StyleName));

                // I'm not sure of the pattern. So, be cautious.

                styleName = styleName.Trim();

                bool oneWord = styleName.IndexOf(' ') < 0;

                if (styleName.IndexOf("Demibold", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    styleName = styleName.Replace("Demibold", "");
                    fontStyle |= FontStyle.Bold;
                }

                if (styleName.IndexOf("Bold", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    styleName = styleName.Replace("Bold", "");
                    fontStyle |= FontStyle.Bold;
                }

                if (styleName.IndexOf("Oblique", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    styleName = styleName.Replace("Oblique", "");
                    fontStyle |= FontStyle.Italic;
                }

                if (styleName.IndexOf("Italic", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    styleName = styleName.Replace("Italic", "");
                    fontStyle |= FontStyle.Italic;
                }

                if (styleName.IndexOf("Regular", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    styleName = styleName.Replace("Regular", "");
                }

                if (styleName.IndexOf("Roman", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    styleName = styleName.Replace("Roman", "");
                }

                if (styleName.IndexOf("Black", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    fontStyle |= FontStyle.Bold;
                    // But leave the word in the name.
                }

                styleName = styleName.Trim();

                // If we started and ended with one word, then treat it as a regular style.
                if (oneWord && styleName.Length > 0 && styleName != "Black")
                    styleName = "";

                return (fontStyle, styleName);
            }
        }
    }
}
