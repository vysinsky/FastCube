using System;
using Firebase;
using Firebase.Analytics;
using UnityEngine;

namespace FastCube.Behaviours
{
    public class FirebaseInit : MonoBehaviour
    {
        private void Start()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            });
        }
    }
}
