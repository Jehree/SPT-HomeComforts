using BepInEx.Configuration;

namespace HomeComforts.Helpers
{
    public class Settings
    {
        public static ConfigEntry<float> ExfilSizeMultiplier;

        public static void Init(ConfigFile config)
        {
            ExfilSizeMultiplier = config.Bind(
                "0: Debug",
                "Exfil Area Size Multiplier",
                3f,
                new ConfigDescription("Size of exfil trigger.", null, new ConfigurationManagerAttributes { IsAdvanced = true })
            );
        }
    }
}
