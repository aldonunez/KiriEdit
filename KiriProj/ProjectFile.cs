/*
   Copyright 2020 Aldo J. Nunez

   Licensed under the Apache License, Version 2.0.
   See the LICENSE.txt file for details.
*/

using System.Text.Json.Serialization;

namespace KiriProj
{
    public class ProjectFile
    {
        public string FontPath { get; set; }
        public int FaceIndex { get; set; }
        public string FontFamily { get; set; }
        public string FontName { get; set; }
        public int FontStyle { get; set; }
        public string CharactersFolderPath { get; set; }

        // Runtime properties.

        [JsonIgnore]
        public string Path { get; set; }
        [JsonIgnore]
        public bool IsDirty { get; set; }
    }

    public class ProjectSpec
    {
        public string ProjectName;
        public string ProjectLocation;
        public string FontPath;
        public int FaceIndex;
    }
}
