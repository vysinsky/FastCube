using Unity.Entities;
using UnityEngine;

namespace FastCube
{
    public class GameManager: MonoBehaviour
    {

        public static void PauseGame()
        {
            Debug.Log("PAUSE");
            Time.timeScale = 0;
        }

        public static void ResumeGame()
        {
            Time.timeScale = 1;
        }

    }
}
