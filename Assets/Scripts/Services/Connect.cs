using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Services {
    public class Connect : IConnect
    {
        public IEnumerator postRequest(string serverUrl, string json, Action onSuccess, Action onError) {
            Debug.Log($"send event to json: {json}");
            UnityWebRequest www = UnityWebRequest.Put(serverUrl, json);
            www.method = "POST";
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError) {
                onError.Invoke();
                www.Dispose();
            }
            else {
                onSuccess.Invoke();
                www.Dispose();
            }
        }
    }
}