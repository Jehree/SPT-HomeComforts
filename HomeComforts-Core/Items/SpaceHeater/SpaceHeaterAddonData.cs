using Newtonsoft.Json;

internal class SpaceHeaterAddonData
{
    public bool HeaterEnabled = false;

    [JsonIgnore]
    private static SpaceHeaterAddonData _enabledData = new(true);
    [JsonIgnore]
    private static SpaceHeaterAddonData _disabledData = new(false);

    public SpaceHeaterAddonData() { }

    public SpaceHeaterAddonData(bool enabled)
    {
        HeaterEnabled = enabled;
    }

    public static SpaceHeaterAddonData CreateData(bool enabled)
    {
        if (enabled)
        {
            return _enabledData;
        }
        else
        {
            return _disabledData;
        }
    }
}