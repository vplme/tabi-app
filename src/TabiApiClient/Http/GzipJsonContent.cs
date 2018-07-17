using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TabiApiClient.Http
{
    public class GzipJsonContent : HttpContent
    {
        private readonly JsonSerializer _serializer = new JsonSerializer();
        private readonly object _value;

        public GzipJsonContent(object value)
        {
            _value = value;
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
            Headers.Add("Content-Encoding", "gzip");
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var gzip = new GZipStream(stream, CompressionMode.Compress, true))
                using (var writer = new StreamWriter(gzip, new UTF8Encoding(false), 1024, true))
                {
                    _serializer.Serialize(writer, _value);
                }
            });
        }

        protected override bool TryComputeLength(out long length)
        {
            length = -1;
            return false;
        }
    }
}
