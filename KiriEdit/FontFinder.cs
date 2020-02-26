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

                // TEST:
                if (face.FamilyName.StartsWith("Lucida"))
                    System.Diagnostics.Debug.WriteLine(string.Format("[{0}] :: [{1}]", face.FamilyName, styleName));

                // I'm not sure of the pattern. So, be cautious.

                styleName = styleName.Trim();

                if (styleName.EndsWith("Regular", StringComparison.OrdinalIgnoreCase))
                {
                    if (styleName.Length == 7)
                    {
                        styleName = "";
                        styleName = styleName.Trim();
                        return (fontStyle, styleName);
                    }
                    else if (styleName[styleName.Length - 8] == ' ')
                    {
                        styleName = styleName.Substring(0, styleName.Length - 8);
                        styleName = styleName.Trim();
                        return (fontStyle, styleName);
                    }
                }

                if (styleName.EndsWith("Bold", StringComparison.OrdinalIgnoreCase))
                {
                    if (styleName.Length == 4)
                    {
                        fontStyle |= FontStyle.Bold;
                        styleName = "";
                    }
                    else if (styleName[styleName.Length - 5] == ' ')
                    {
                        fontStyle |= FontStyle.Bold;
                        styleName = styleName.Substring(0, styleName.Length - 5);
                    }
                }

                if (styleName.EndsWith("Italic", StringComparison.OrdinalIgnoreCase))
                {
                    if (styleName.Length == 6)
                    {
                        fontStyle |= FontStyle.Italic;
                        styleName = "";
                    }
                    else if (styleName[styleName.Length - 7] == ' ')
                    {
                        fontStyle |= FontStyle.Italic;
                        styleName = styleName.Substring(0, styleName.Length - 7);
                    }
                }

                if (styleName.EndsWith("Bold", StringComparison.OrdinalIgnoreCase))
                {
                    if (styleName.Length == 4)
                    {
                        fontStyle |= FontStyle.Bold;
                        styleName = "";
                    }
                    else if (styleName[styleName.Length - 5] == ' ')
                    {
                        fontStyle |= FontStyle.Bold;
                        styleName = styleName.Substring(0, styleName.Length - 5);
                    }
                }

                styleName = styleName.Trim();

                return (fontStyle, styleName);
            }
        }
    }
}
