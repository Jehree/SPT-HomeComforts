using BepInEx.Configuration;
using Comfort.Common;
using EFT;
using System.Collections.Generic;

namespace HomeComforts.Helpers
{
    public class Settings
    {
        public static ConfigEntry<float> ExfilSizeMultiplier;

        public static ConfigEntry<bool> AlwaysInfilAtSafehouse;
        public static ConfigEntry<bool> ScavsCanUseSafehouse;

        public static ConfigEntry<float> SpaceHeaterAOESizeMultiplier;
        public static ConfigEntry<float> SpaceHeaterHydrationBuff;
        public static ConfigEntry<float> SpaceHeaterEnergyBuff;

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

            AlwaysInfilAtSafehouse = config.Bind(
                "1: Safehouse",
                "Always Infil at Safehouse",
                false,
                "true = always infil at the last enabled safehouse you exfil'd at. false = only infil at a safehouse if you exfil'd at it in the last raid you played on that map."
            );
            ScavsCanUseSafehouse = config.Bind(
                "1: Safehouse",
                "Player Scavs can use Safehouse Marker Radio",
                false,
                "If safehouses can be used while on a scav raid."
            );

            SpaceHeaterAOESizeMultiplier = config.Bind(
                "2: Space Heater",
                "Space Heater AOE Size Multiplier",
                14f,
                "Size multiplier for Space Heater area of affect zone. Requires raid restart to fully take affect."
            );
            SpaceHeaterHydrationBuff = config.Bind(
                "2: Space Heater",
                "Hydration Buff",
                3.5f,
                "Hydration buff (per minute) while near a space heater."
            );
            SpaceHeaterEnergyBuff = config.Bind(
                "2: Space Heater",
                "Energy Buff",
                3.5f,
                "Energy buff (per minute) while near a space heater."
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
