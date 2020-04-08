using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using KiriFig;
using KiriFig.Model;

namespace KiriProj
{
    public class FigureDocument
    {
        [JsonConverter(typeof(FigureConverter))]
        public Figure Figure { get; set; }
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
