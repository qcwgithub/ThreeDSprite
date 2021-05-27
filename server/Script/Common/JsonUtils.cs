
public class JsonUtils : IScript
{
    public Server server { get; set; }

    public string stringify(object obj)
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
    }

    public T parse<T>(string json)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
    }
}