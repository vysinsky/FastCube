using System;
using System.Linq;
using FastCube.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace FastCube.Behaviours
{
    public class UIControllerBehaviour : MonoBehaviour
    {
        private Label _scoreCounterLabel;
        private EntityManager _em;
        private NativeArray<Entity> _playerEntities;

        private void OnEnable()
        {
            var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
            _scoreCounterLabel = rootVisualElement.Q<Label>("score-counter");
            _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void OnDestroy()
        {
            try
            {
                _playerEntities.Dispose();
            }
            catch (Exception)
            {
                // Nothing
            }
        }

        private void Update()
        {
            var playerQuery = _em.CreateEntityQuery(typeof(PlayerTag));
            _playerEntities = playerQuery.ToEntityArray(Allocator.TempJob);

            if (_playerEntities.Length <= 0)
            {
                _playerEntities.Dispose();
                return;
            }

            var player = _playerEntities.First();

            if (player != Entity.Null)
            {
                var score = _em.GetComponentData<Score>(player);
                _scoreCounterLabel.text = $"{score.Value}";
            }

            _playerEntities.Dispose();
        }
    }
}
