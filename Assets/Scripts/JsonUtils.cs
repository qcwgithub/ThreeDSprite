using System;
using System.Collections.Generic;
using System.Collections;


public class JsonUtils
{
	public static string ToJson(object obj)
	{
		return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
	}

	public static T FromJson<T>(string json)
	{
		return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
	}
}
