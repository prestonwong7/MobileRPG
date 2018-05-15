using System.Runtime.Serialization;
using UnityEngine;

sealed class Color32Surrogate : ISerializationSurrogate {

	// Method called to serialize a Color32 object
	public void GetObjectData(System.Object obj,
		SerializationInfo info, StreamingContext context) {

		Color32 color32 = (Color32) obj;
		info.AddValue("r", color32.r);
		info.AddValue("g", color32.g);
		info.AddValue("b", color32.b);
		info.AddValue("a", color32.a);
	}

	// Method called to deserialize a Color32 object
	public System.Object SetObjectData(System.Object obj,
		SerializationInfo info, StreamingContext context,
		ISurrogateSelector selector) {

		Color32 color32 = (Color32) obj;
		color32.r = (byte)info.GetValue("r", typeof(byte));
		color32.g = (byte)info.GetValue("g", typeof(byte));
		color32.b = (byte)info.GetValue("b", typeof(byte));
		color32.a = (byte)info.GetValue("a", typeof(byte));
		obj = color32;
		return obj;
	}
}