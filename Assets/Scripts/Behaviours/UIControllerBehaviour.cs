using System;
using System.Linq;
using FastCube.Components;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace FastCube.Behaviours
{
    public class UIControllerBehaviour : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;
        private EntityManager _em;

        private NativeArray<Entity> _playerEntities;

        private void Start()
        {
            _em = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        private void OnDestroy()
        {
            try
            {
                _playerEntities.Dispose();
            }
            catch (Exception e)
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
                scoreText.SetText($"{score.Value}");
            }

            _playerEntities.Dispose();
        }
    }
}
