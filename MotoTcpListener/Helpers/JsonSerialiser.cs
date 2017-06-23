using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MotoTcpListener.Lib
{
	public class JsonSerialiser : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return (objectType == typeof(VehicleDataWrapper));
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			VehicleDataWrapper wrapper = (VehicleDataWrapper)value;
			FieldInfo field = typeof(VehicleDataWrapper).GetField("values", BindingFlags.NonPublic | BindingFlags.Instance);
			JObject jo = JObject.FromObject(field.GetValue(wrapper));
			jo.WriteTo(writer);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jo = JObject.Load(reader);
			VehicleDataWrapper wrapper = new VehicleDataWrapper();
			FieldInfo field = typeof(VehicleDataWrapper).GetField("values", BindingFlags.NonPublic | BindingFlags.Instance);
			field.SetValue(wrapper, jo.ToObject(field.FieldType));
			return wrapper;
		}
	}

	[JsonConverter(typeof(JsonSerialiser))]
	class VehicleDataWrapper : IEnumerable
	{
		Dictionary<string, string> values = new Dictionary<string, string>();

		public void Add(string key, string value)
		{
			values.Add(key, value);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return values.GetEnumerator();
		}
	}
}
