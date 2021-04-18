using System;
using System.Collections.Generic;
using DefaultNamespace.Behaviours;
using FastCube.Components;
using FastCube.LevelGenerationUtils;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = System.Random;

namespace FastCube.Behaviours
{
    [ExecuteInEditMode]
    public class LevelGeneratorBehaviour : MonoBehaviour, IDeclareReferencedPrefabs,
        IConvertGameObjectToEntity
    {
        [Header("Generator settings")] [SerializeField]
        private int seed = 42;

        [SerializeField] private int initialTilesCount = 10;

        [SerializeField] private int minimumTilesCount = 10;

        [SerializeField] private float stepSize = 1.2f;

        [Header("Game Objects")] [SerializeField]
        private GameObject startingGroundPrefab;

        [SerializeField] private GameObject groundPrefab;
        [SerializeField] private GameObject playerPrefab;

        [SerializeField] private GameObject playerTracker;

        private Entity _groundEntityPrototype;

        private Entity _playerEntityPrototype;

        private Entity _startingGroundEntityPrototype;

        public void Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            Generator.Random = new Random(seed);
            PrepareEntityPrototypes(dstManager, conversionSystem);
            GenerateStartingGround(dstManager);
            var lastPosition = GenerateInitialGrounds(dstManager);
            SpawnPlayer(dstManager);

            dstManager.AddComponentData(entity, new LevelGenerationData
            {
                LastPosition = lastPosition,
                GroundEntityPrefab = _groundEntityPrototype,
                CurrentTilesCount = initialTilesCount,
                MinimumTilesCount = minimumTilesCount,
                StepSize = stepSize
            });
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(startingGroundPrefab);
            referencedPrefabs.Add(groundPrefab);
            referencedPrefabs.Add(playerPrefab);
        }

        private void PrepareEntityPrototypes(EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            var blobAssetStore = new BlobAssetStore();
            var conversionSettings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);
            _playerEntityPrototype = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                playerPrefab, conversionSettings);
            _startingGroundEntityPrototype =
                GameObjectConversionUtility.ConvertGameObjectHierarchy(startingGroundPrefab,
                    conversionSettings);
            _groundEntityPrototype =
                GameObjectConversionUtility.ConvertGameObjectHierarchy(groundPrefab,
                    conversionSettings);

#if UNITY_EDITOR
            dstManager.SetName(_playerEntityPrototype, "[Prefab] Player");
            dstManager.SetName(_startingGroundEntityPrototype, "[Prefab] StartingGround");
            dstManager.SetName(_groundEntityPrototype, "[Prefab] Ground");
#endif
        }

        private void GenerateStartingGround(EntityManager dstManager)
        {
            var startingGroundEntity =
                dstManager.Instantiate(_startingGroundEntityPrototype);
#if UNITY_EDITOR
            dstManager.SetName(startingGroundEntity,
                "[GO] StartingGround");
#endif
            dstManager.SetComponentData(startingGroundEntity, new Translation
            {
                Value = float3.zero
            });
        }

        private float3 GenerateInitialGrounds(EntityManager dstManager)
        {
            var lastPosition = new float3(0f, 0f, 0f);
            var groundPrefabEntity = _groundEntityPrototype;
            for (var i = 0; i < initialTilesCount; i++)
            {
                var groundEntity = dstManager.Instantiate(groundPrefabEntity);
#if UNITY_EDITOR
                dstManager.SetName(groundEntity, $"[GO] Ground[{i}]");
#endif
                var nextPosition = Generator.PickNextPosition(stepSize, lastPosition);
                lastPosition = nextPosition;
                dstManager.SetComponentData(groundEntity, new Translation
                {
                    Value = nextPosition
                });
            }

            return lastPosition;
        }

        private void SpawnPlayer(EntityManager dstManager)
        {
            var playerEntity = dstManager.Instantiate(_playerEntityPrototype);
#if UNITY_EDITOR
            dstManager.SetName(playerEntity, "[GO] Player");
#endif
            dstManager.AddComponentData(playerEntity, new Score
            {
                Value = 0
            });
            dstManager.AddBuffer<InputBuffer>(playerEntity);

            var followEntity = playerTracker.GetComponent<FollowEntityBehaviour>();
            followEntity.EntityToFollow = playerEntity;
        }
    }
}
