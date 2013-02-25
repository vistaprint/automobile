using Automobile.Mobile.Framework.Data;
using Newtonsoft.Json;

namespace Automobile.Mobile.iOS.Automation
{
	public class JsonProvider : IJsonProvider
	{
		public T Deserialize<T>(string str)
		{
			return JsonConvert.DeserializeObject<T>(str);
		}

		public string Serialize(object obj)
		{
			return JsonConvert.SerializeObject(obj);
		}

	}
}

