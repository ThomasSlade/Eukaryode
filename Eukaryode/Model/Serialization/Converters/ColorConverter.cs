﻿using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Amino;

namespace Eukaryode.Serialization
{
	/// <summary> Allows the serialisation of a colour to its hexcode. </summary>
	public class ColorConverter : JsonConverter<Color>
	{
		public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			JToken token = JToken.ReadFrom(reader);

			Color fromHex = System.Drawing.ColorTranslator.FromHtml(token.Value<string>()).ToXNACol();

			return fromHex;
		}

		public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
		{
			System.Drawing.Color drawingCol = value.ToDrawingCol();
			writer.WriteValue(System.Drawing.ColorTranslator.ToHtml(drawingCol));
		}
	}
}
