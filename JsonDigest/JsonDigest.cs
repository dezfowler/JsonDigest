using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace JsonDigest
{
    public class JsonDigest
    {
        private static readonly JsonLoadSettings JsonLoadSettings = new JsonLoadSettings
        {
            CommentHandling = CommentHandling.Ignore,
            LineInfoHandling = LineInfoHandling.Ignore
        };

        public Action<JToken> NormalizationStrategy { get; set; }

        public Func<HashAlgorithm> HashAlgorithmFactory { get; set; }

        public string Generate(string jsonContent)
        {
            string normalizedJson;

            JToken token;
            using (var reader = new StringReader(jsonContent))
            using (var jsonReader = new JsonTextReader(reader))
            {
                // Read dates as strings
                jsonReader.DateParseHandling = DateParseHandling.None;

                // Read numbers as decimals
                jsonReader.FloatParseHandling = FloatParseHandling.Decimal;

                token = JToken.ReadFrom(jsonReader, JsonLoadSettings);
            }
            
            var normalizeAction = NormalizationStrategy ?? Normalize;
            normalizeAction(token);

            using (var writer = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                jsonWriter.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                jsonWriter.Formatting = Newtonsoft.Json.Formatting.None;

                token.WriteTo(jsonWriter);
                jsonWriter.Flush();
                writer.Flush();

                normalizedJson = writer.ToString();
            }

            var hashAlgorithmFactory = HashAlgorithmFactory ?? MD5.Create;
            using (HashAlgorithm hashAlgorithm = hashAlgorithmFactory())
            {
                byte[] retVal = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(normalizedJson));
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        private static void Normalize(JToken token)
        {
            if (token.Type == JTokenType.Array)
            {
                var arr = (JArray)token;
                foreach (var item in arr)
                {
                    Normalize(item);
                }
            }

            if (token.Type != JTokenType.Object)
            {
                return;
            }

            var obj = (JObject)token;

            var reordered = obj.Properties().OrderBy(p => p.Name).ToList();

            foreach (JProperty t in reordered)
            {
                Normalize(t.Value);
            }

            obj.ReplaceAll(reordered);
        }
    }
}
