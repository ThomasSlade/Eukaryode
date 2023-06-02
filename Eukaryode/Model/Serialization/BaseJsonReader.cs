using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;

namespace Eukaryode.Serialization
{
	/// <summary> The base class for json content readers. Use this to define standard deserialisation settings. </summary>
	public abstract class BaseJsonReader<T> : JsonContentTypeReader<T>
	{
		protected override T Read(ContentReader reader, T existingInstance)
		{
			string json = reader.ReadString();

			using (StringReader stringReader = new StringReader(json))
			using (JsonTextReader jsonReader = new JsonTextReader(stringReader))
			{
				JsonSerializer serializer = new JsonSerializer();
				serializer.Converters.Add(new StringEnumConverter());
				serializer.Converters.Add(new ColorConverter());
				return serializer.Deserialize<T>(jsonReader);
			}
		}
	}
}
