using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Automobile.Mobile.Framework.Data;

namespace Automobile.Registrar.UnitTests
{
    public class JsonProvider : IJsonProvider
    {
        private readonly DataContractJsonSerializer _json;

        public JsonProvider()
        {
            _json = new DataContractJsonSerializer(typeof(DeviceInfo));
        }

        public T Deserialize<T>(string str)
        {
            var stream = new MemoryStream();
            stream.Write(Encoding.ASCII.GetBytes(str), 0, Encoding.ASCII.GetByteCount(str));
            stream.Position = 0;
            return (T) _json.ReadObject(stream);
        }

        public string Serialize(object obj)
        {
            var stream = new MemoryStream();
            _json.WriteObject(stream, obj);
            stream.Position = 0;
            return new StreamReader(stream).ReadToEnd();
        }
    }
}