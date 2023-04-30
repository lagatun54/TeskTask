using System;
using System.Collections;

namespace Services {
    public interface IConnect
    {
        public IEnumerator postRequest(string serverUrl, string json, Action onSuccess, Action onError);
    }
}