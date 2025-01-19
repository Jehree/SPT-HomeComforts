using Comfort.Common;
using EFT;
using HomeComforts.Items.SpaceHeater;
using LeaveItThere.Components;
using System;
using UnityEngine;

namespace HomeComforts.Components
{
    internal class HCSession : MonoBehaviour
    {
        public SafehouseSession SafehouseSession { get; private set; } = new();
        public SpaceHeaterSession SpaceHeaterSession { get; private set; } = new();
        public NeedsRateReductions NeedsRateReductions { get; private set; } = new();
        public SafehouseExfil CustomSafehouseExfil;

        public GameWorld GameWorld { get; private set; }

        private Player _player = null;
        public Player Player
        {
            get
            {
                if (_player == null)
                {
                    _player = GameWorld.MainPlayer;
                }
                return _player;
            }
        }

        private GamePlayerOwner _gamePlayerOwner = null;
        public GamePlayerOwner GamePlayerOwner
        {
            get
            {
                if (_gamePlayerOwner == null)
                {
                    _gamePlayerOwner = Player.gameObject.GetComponent<GamePlayerOwner>();
                }
                return _gamePlayerOwner;
            }
        }
        private HCSession() { }
        private static HCSession _instance = null;
        public static HCSession Instance
        {
            get
            {
                if (!Singleton<GameWorld>.Instantiated)
                {
                    throw new Exception("Tried to get HomeComfortsSession when game world was not instantiated!");
                }
                if (_instance == null)
                {
                    _instance = Singleton<GameWorld>.Instance.gameObject.GetOrAddComponent<HCSession>();
                }
                return _instance;
            }
        }


        private void Awake()
        {
            GameWorld = Singleton<GameWorld>.Instance;
        }

        public static void OnRaidEnd(LocalRaidSettings settings, object results, object lostInsuredItems, object transferItems, string exitName)
        {
            SafehouseSession.OnRaidEnd(settings, results, lostInsuredItems, transferItems, exitName);
        }

        public static void OnLastPlacedItemSpawned(FakeItem fakeItem)
        {
            SafehouseSession.OnLastPlacedItemSpawned(fakeItem);
        }
    }
}
