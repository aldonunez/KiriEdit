using Microsoft.Win32;
using KiriFT;
using System;
using System.IO;

namespace KiriEdit.Font
{
    internal static class FontFinder
    {
        public static FontFamilyCollection FindFonts()
        {
            using (var finder = new Finder())
            {
                finder.Find();
                return finder.FontFamilyCollection;
            }
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
                        string path = fontsKey.GetValue(valName) as string;

                        if (path == null)
                            continue;

                        if (!Path.IsPathRooted(path))
                        {
                            string fontsPath = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

                            path = Path.Combine(fontsPath, path);
                        }

                        ProcessFontFile(path);
                    }
                }
            }

            private void ProcessFontFile(string path)
            {
                int faceCount = 1;

                for (int i = 0; i < faceCount; i++)
                {
                    Face face = null;
                    try
                    {
                        face = _library.OpenFace(path, i, OpenParams.IgnoreTypographicFamily);
                        faceCount = face.FaceCount;

                        if ((face.Flags & FaceFlags.Scalable) == FaceFlags.Scalable)
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
                FontFamily family;

                var fontStyle = ProcessStyleName(face);

                if (!_nameToFamily.TryGetValue(face.FamilyName, out family))
                {
                    family = new FontFamily(face.FamilyName);
                    _nameToFamily.Add(face.FamilyName, family);
                }

                FontFace fontFace = new FontFace(family, fontStyle, path, face.FaceIndex);

                family.AddFace(fontStyle, fontFace);
            }

            private FontStyle ProcessStyleName(Face face)
            {
                FontStyle fontStyle = FontStyle.Regular;
                string styleName = face.StyleName;

                // I'm not sure of the pattern. So, be cautious.

                if (styleName.IndexOf("Fat", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    fontStyle |= FontStyle.Bold;
                }
                else if (styleName.IndexOf("Heavy", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    fontStyle |= FontStyle.Bold;
                }
                else if (styleName.IndexOf("Thick", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    fontStyle |= FontStyle.Bold;
                }
                else if (styleName.IndexOf("Bold", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    fontStyle |= FontStyle.Bold;
                }
                else if (styleName.IndexOf("Black", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    fontStyle |= FontStyle.Bold;
                }

                if (styleName.IndexOf("Oblique", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    fontStyle |= FontStyle.Italic;
                }
                else if (styleName.IndexOf("Italic", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    fontStyle |= FontStyle.Italic;
                }

                return fontStyle;
            }
        }
    }
}
