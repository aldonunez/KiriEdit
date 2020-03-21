using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using TryFreetype;
using TryFreetype.Model;

namespace KiriEdit
{
    public class FigureDocument
    {
        public class Shape
        {
            public bool Enabled { get; set; }

            // The outer contour is first.
            public int[] Contours { get; set; }
        }

        [JsonConverter(typeof(FigureConverter))]
        public Figure Figure { get; set; }

        public Shape[] Shapes { get; set; }
    }

    internal class FigureConverter : JsonConverter<Figure>
    {
        public override Figure Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string s = reader.GetString();

            using (var stringReader = new StringReader(s))
            {
                return FigureDeserialzer.Deserialize(stringReader);
            }
        }

        public override void Write(Utf8JsonWriter writer, Figure value, JsonSerializerOptions options)
        {
            using (var stringWriter = new StringWriter())
            {
                FigureSerializer.Serialize(value, stringWriter);

                writer.WriteStringValue(stringWriter.ToString());
            }
        }
    }
}
