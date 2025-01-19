using BepInEx.Configuration;
using Comfort.Common;
using EFT;
using System.Collections.Generic;

namespace HomeComforts.Helpers
{
    public class Settings
    {
        public const float DefaultHydrationRate = -1.976f;
        public const float DefaultEnergyRate = -2.432f;

        public static ConfigEntry<float> HydrationFullBuff;
        public static ConfigEntry<float> EnergyFullBuff;

        public static ConfigEntry<float> ExfilSizeMultiplier;

        public static ConfigEntry<bool> AlwaysInfilAtSafehouse;

        public static ConfigEntry<float> HydrationDrainReduction;
        public static ConfigEntry<float> EnergyDrainReduction;
        public static ConfigEntry<float> SpaceHeaterAOESizeMultiplier;

        public static ConfigEntry<int> CustomsSafehouseLimit;
        public static ConfigEntry<int> FactorySafehouseLimit;
        public static ConfigEntry<int> InterchangeSafehouseLimit;
        public static ConfigEntry<int> LabSafehouseLimit;
        public static ConfigEntry<int> LighthouseSafehouseLimit;
        public static ConfigEntry<int> ReserveSafehouseLimit;
        public static ConfigEntry<int> GroundZeroSafehouseLimit;
        public static ConfigEntry<int> ShorelineSafehouseLimit;
        public static ConfigEntry<int> StreetsSafehouseLimit;
        public static ConfigEntry<int> WoodsSafehouseLimit;
        private const string _safehouseLimitSectionName = "9: Number of Safehouses per Map";
        private const string _safehouseLimitSectionDescription = "Maximum number of safehouses allowed to be placed on a map. It is HIGHLY recommended to leave this number set to something quite small to avoid balance issues.";
        private static Dictionary<string, ConfigEntry<int>> _safehouseLimitLookup = [];
        public static int ThisMapSafehouseLimit
        {
            get
            {
                return _safehouseLimitLookup[Singleton<GameWorld>.Instance.LocationId.ToLower()].Value;
            }
        }

        public static void Init(ConfigFile config)
        {
            ExfilSizeMultiplier = config.Bind(
                "0: Advanced",
                "Exfil Area Size Multiplier",
                8f,
                new ConfigDescription("Size of exfil trigger.", null, new ConfigurationManagerAttributes { IsAdvanced = true })
            );
            HydrationFullBuff = config.Bind(
                "0: Advanced",
                "Hydration Full Buff",
                1.976f,
                new ConfigDescription("Default will bring hydration to a drain of 0 when set to 100%.", null, new ConfigurationManagerAttributes { IsAdvanced = true })
            );
            EnergyFullBuff = config.Bind(
                "0: Advanced",
                "Energy Full Buff",
                2.432f,
                new ConfigDescription("Default will bring energy to a drain of 0 when set to 100%.", null, new ConfigurationManagerAttributes { IsAdvanced = true })
            );

            AlwaysInfilAtSafehouse = config.Bind(
                "1: Safehouse",
                "Always Infil at Safehouse",
                false,
                "true = always infil at the last enabled safehouse you exfil'd at. false = only infil at a safehouse if you exfil'd at it in the last raid you played on that map."
            );

            HydrationDrainReduction = config.Bind(
                "2: Space Heater",
                "Safehouse Hydration Drain Reduction",
                1f,
                new ConfigDescription("100% = no hydration draion, 0% = full hydration drain. Requires raid restart to take affect.", new AcceptableValueRange<float>(0, 1))
            );
            EnergyDrainReduction = config.Bind(
                "2: Space Heater",
                "Safehouse Energy Drain Reduction",
                1f,
                new ConfigDescription("100% = no energy draion, 0% = full energy drain. Requires raid restart to take affect.", new AcceptableValueRange<float>(0, 1))
            );
            SpaceHeaterAOESizeMultiplier = config.Bind(
                "2: Space Heater",
                "Space Heater AOE Size Multiplier",
                12f,
                "Size multiplier for Space Heater area of affect zone. Requires raid restart to fully take affect."
            );

            CustomsSafehouseLimit = config.Bind(
                _safehouseLimitSectionName,
                "Customs",
                1,
                _safehouseLimitSectionDescription
            );
            FactorySafehouseLimit = config.Bind(
                _safehouseLimitSectionName,
                "Factory",
                1,
                _safehouseLimitSectionDescription
            );
            InterchangeSafehouseLimit = config.Bind(
                _safehouseLimitSectionName,
                "Interchange",
                1,
                _safehouseLimitSectionDescription
            );
            LabSafehouseLimit = config.Bind(
                _safehouseLimitSectionName,
                "Lab",
                1,
                _safehouseLimitSectionDescription
            );
            LighthouseSafehouseLimit = config.Bind(
                _safehouseLimitSectionName,
                "Lighthouse",
                1,
                _safehouseLimitSectionDescription
            );
            ReserveSafehouseLimit = config.Bind(
                _safehouseLimitSectionName,
                "Reserve",
                1,
                _safehouseLimitSectionDescription
            );
            GroundZeroSafehouseLimit = config.Bind(
                _safehouseLimitSectionName,
                "Ground Zero",
                1,
                _safehouseLimitSectionDescription
            );
            ShorelineSafehouseLimit = config.Bind(
                _safehouseLimitSectionName,
                "Shoreline",
                1,
                _safehouseLimitSectionDescription
            );
            StreetsSafehouseLimit = config.Bind(
                _safehouseLimitSectionName,
                "Streets",
                1,
                _safehouseLimitSectionDescription
            );
            WoodsSafehouseLimit = config.Bind(
                _safehouseLimitSectionName,
                "Woods",
                1,
                _safehouseLimitSectionDescription
            );

            _safehouseLimitLookup.Add("bigmap", CustomsSafehouseLimit);
            _safehouseLimitLookup.Add("factory4_day", FactorySafehouseLimit);
            _safehouseLimitLookup.Add("factory4_night", FactorySafehouseLimit);
            _safehouseLimitLookup.Add("interchange", InterchangeSafehouseLimit);
            _safehouseLimitLookup.Add("laboratory", LabSafehouseLimit);
            _safehouseLimitLookup.Add("lighthouse", LighthouseSafehouseLimit);
            _safehouseLimitLookup.Add("rezervbase", ReserveSafehouseLimit);
            _safehouseLimitLookup.Add("sandbox", GroundZeroSafehouseLimit);
            _safehouseLimitLookup.Add("sandbox_high", GroundZeroSafehouseLimit);
            _safehouseLimitLookup.Add("shoreline", ShorelineSafehouseLimit);
            _safehouseLimitLookup.Add("tarkovstreets", StreetsSafehouseLimit);
            _safehouseLimitLookup.Add("woods", WoodsSafehouseLimit);
        }
    }
}
