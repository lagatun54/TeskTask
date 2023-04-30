using System.Collections;
using UnityEngine;

namespace Services {
    public class EventService : MonoBehaviour
    {
        private readonly Events _eventsData = new Events();

        const string Key = "event";
        
        private string _serverUrl;
        private string _eventsDataJson;
        private int _cooldownBeforeSendInSeconds = 10;
        
        private IConnect _serverConnect;

        private WaitForSeconds _cooldownBeforeSend;
        private IEnumerator _sendingAfterCooldown;

        
        //start send event if not start send events
        private void StartSendingIfNotStarted() 
        {
            if (_sendingAfterCooldown == null) 
            {
                _sendingAfterCooldown = SendEventsAfterCooldown();
                StartCoroutine(_sendingAfterCooldown);
            }
        }
        
        private IEnumerator SendEventsAfterCooldown() 
        {
            yield return _cooldownBeforeSend;

            string sendingEvents = (string) _eventsDataJson.Clone();
            IEnumerator post = _serverConnect.postRequest(serverUrl, _eventsDataJson,
                () => RemoveSuccessfullySendedEvents(sendingEvents), StartSendingIfNotStarted);
            StartCoroutine(post);
            _sendingAfterCooldown = null;
        }

        private void RemoveSuccessfullySendedEvents(string jsonSendedEvents) 
        {
            Events sendedEventsData = JsonUtility.FromJson<Events>(jsonSendedEvents);
            foreach (var sendedEvent in sendedEventsData.events) 
            {
                _eventsData.events.Remove(sendedEvent);
            }

            UpdateJsonOfEventsData();
        }

        private void LoadUnsentEvents() 
        {
            string unsentEventsJson = PlayerPrefs.GetString(Key);
            if (string.IsNullOrEmpty(unsentEventsJson)) return;
            var eventsData = JsonUtility.FromJson<Events>(unsentEventsJson);
            _eventsData.events.AddRange(eventsData.events);
            _eventsDataJson = unsentEventsJson;
        }

        private void UpdateJsonOfEventsData()
        {
            _eventsDataJson = JsonUtility.ToJson(_eventsData);
        }

        //init server address 
        public void Init(IConnect serverConnect)
        {
            _serverConnect = serverConnect;
            LoadUnsentEvents();
            if (_eventsData.events.Count > 0) StartSendingIfNotStarted();
        }
        
        //URL for analytics event messages
        public string serverUrl 
        {
            get => _serverUrl;
            set => _serverUrl = value;
        }

        // The minimum time to resend the queue from the accumulated data
        public int CooldownBeforeSendInSeconds 
        {
            get => _cooldownBeforeSendInSeconds;
            set 
            {
                _cooldownBeforeSendInSeconds = value;
                _cooldownBeforeSend = new WaitForSeconds(_cooldownBeforeSendInSeconds);
            }
        }

        //this service
        public void TrackEvent(string type, string data)
        {
            _eventsData.events.Add(new Event(type, data));
            UpdateJsonOfEventsData();

            PlayerPrefs.SetString(Key, _eventsDataJson);
            StartSendingIfNotStarted();

            Debug.Log($"track event: {type}:{data}");
        }
        
    }
}

