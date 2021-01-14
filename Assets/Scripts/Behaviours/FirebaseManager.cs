using Firebase;
using Firebase.Analytics;
using UnityEngine;

namespace FastCube.Behaviours
{
    public class FirebaseManager: MonoBehaviour
    {
        public static bool FirebaseAvailable { get; private set; } = false;

        private void Start()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;

                if (dependencyStatus == DependencyStatus.Available)
                {
                    var app = FirebaseApp.DefaultInstance;
                    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                    FirebaseAvailable = true;
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }

                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            });
        }

    }
}
