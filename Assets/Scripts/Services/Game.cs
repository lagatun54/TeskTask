using System;
using System.Collections;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Services {
    public class Game : MonoBehaviour
    {
        [SerializeField] private EventService _eventService;

        private void Awake()
        {
            NativeLeakDetection.Mode = NativeLeakDetectionMode.EnabledWithStackTrace;
        }

        IEnumerator Start()
        {
            //registration server
            var serverConnect = new Connect();
            _eventService.Init(serverConnect);

            //settings server
            _eventService.CooldownBeforeSendInSeconds = 1;
            _eventService.serverUrl = "localhost";
            
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(1, 3));
                _eventService.TrackEvent("levelStart", $"level:{Random.Range(0, 100)}");
            }
        }

        private void readPlayerPrefs()
        {
            Debug.Log($"playerPrefs contains {PlayerPrefs.GetString("eventData")}");
        }

        private void deletePlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}