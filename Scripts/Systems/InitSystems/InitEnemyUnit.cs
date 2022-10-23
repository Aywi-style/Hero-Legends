using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Client
{
    sealed class InitEnemyUnit : IEcsInitSystem
    {
        readonly EcsSharedInject<GameState> _state;
        private int _startRewardValue = 120;
        private string EnemyMeleeUnit;
        private string EnemyRangeUnit;
        readonly EcsPoolInject<KeyHolderComponent> _keyHolderPool = default;

        public void Init(EcsSystems systems)
        {
            List<GameObject[]> allEnemyUnits = new List<GameObject[]>();

            var allEnemyMeleeUnits = GameObject.FindGameObjectsWithTag(nameof(EnemyMeleeUnit));
            var allEnemyRangeUnits = GameObject.FindGameObjectsWithTag(nameof(EnemyRangeUnit));

            allEnemyUnits.Add(allEnemyMeleeUnits);
            allEnemyUnits.Add(allEnemyRangeUnits);
            var enemyCount = allEnemyMeleeUnits.Length + allEnemyRangeUnits.Length;
            var random = Random.Range(0, enemyCount + 1);
            var counter = 1;
            var world = systems.GetWorld();
            //Debug.Log("random " + random);
            foreach (var enemyType in allEnemyUnits)
            {
                foreach (var enemyUnit in enemyType)
                {
                    var enemyEntity = world.NewEntity();
                    if(counter == random && _state.Value.KeysCount < 3)//random)
                    {
                        _keyHolderPool.Value.Add(enemyEntity);
                    }
                    counter++;
                    world.GetPool<ClosestTargetComponent>().Add(enemyEntity);
                    world.GetPool<DisabledEncounterTag>().Add(enemyEntity);
                    ref var unitLevelComponent = ref world.GetPool<UnitLevelComponent>().Add(enemyEntity);
                    ref var coinRewardComponent = ref world.GetPool<CoinRewardComponent>().Add(enemyEntity);
                    ref var healthPointComponent = ref world.GetPool<HealthPointComponent>().Add(enemyEntity);
                    ref var enemyUnitComponent = ref world.GetPool<EnemyUnitComponent>().Add(enemyEntity);
                    ref var viewComponent = ref world.GetPool<ViewComponent>().Add(enemyEntity);
                    ref var encounterComponent = ref world.GetPool<EncounterComponent>().Add(enemyEntity);
                    ref var speedComponent = ref world.GetPool<SpeedComponent>().Add(enemyEntity);
                    ref var distanceForFightComponent = ref world.GetPool<DistanceForFightComponent>().Add(enemyEntity);
                    ref var cooldownComponent = ref world.GetPool<CooldownComponent>().Add(enemyEntity);
                    ref var doDamageComponent = ref world.GetPool<DoDamageComponent>().Add(enemyEntity);
                    ref var ragdollComponent = ref world.GetPool<RagdollComponent>().Add(enemyEntity);

                    viewComponent.GameObject = enemyUnit;
                    viewComponent.Transform = enemyUnit.transform;
                    viewComponent.Collider = enemyUnit.GetComponent<Collider>();
                    viewComponent.Animator = enemyUnit.GetComponent<Animator>();
                    viewComponent.AttackMB = enemyUnit.GetComponent<AttackMB>();
                    viewComponent.AttackMB.Init(world);
                    viewComponent.HealthBarMB = enemyUnit.GetComponent<HealthBarMB>();
                    viewComponent.AudioSource = enemyUnit.GetComponents<AudioSource>();
                    viewComponent.ExplosionParticleSystem = viewComponent.Transform.GetChild(2).gameObject;
                    viewComponent.RangeUltimateParticleSystem = new GameObject[10];
                    for (int i = 0; i < viewComponent.RangeUltimateParticleSystem.Length;i++)
                    {
                        viewComponent.RangeUltimateParticleSystem[i] = viewComponent.Transform.GetChild(3).transform.GetChild(i).gameObject;
                    }
                    viewComponent.HitParticle = viewComponent.Transform.GetChild(4).GetComponent<ParticleSystem>();

                    ragdollComponent.AllRigidbodys = new List<Rigidbody>();
                    ragdollComponent.AllColliders = new List<Collider>();
                    ragdollComponent.AllRigidbodys.AddRange(viewComponent.GameObject.GetComponentsInChildren<Rigidbody>());
                    ragdollComponent.AllColliders.AddRange(viewComponent.GameObject.GetComponentsInChildren<Collider>());

                    unitLevelComponent.Value = int.Parse(Regex.Match(viewComponent.GameObject.name, @"\d+").Value);
                    encounterComponent.Number = int.Parse(viewComponent.Transform.parent.name);

                    speedComponent.Speed = _state.Value.EnemyUnitStorage.GetSpeedByID(enemyUnit.gameObject.tag);

                    coinRewardComponent.FullValue = (ulong)_startRewardValue;
                    coinRewardComponent.DamageReward = (ulong)Mathf.RoundToInt(coinRewardComponent.FullValue * 0.5f);
                    coinRewardComponent.KillReward = (ulong)Mathf.RoundToInt(coinRewardComponent.FullValue * 0.5f);

                    healthPointComponent.MaxHealthPoint = _state.Value.EnemyUnitStorage.GetHealthByID(enemyUnit.gameObject.tag);
                    healthPointComponent.CurrentHealthPoint = healthPointComponent.MaxHealthPoint;

                    viewComponent.HealthBarMB.SetMaxHealth(healthPointComponent.MaxHealthPoint);
                    viewComponent.HealthBarMB.SetHealth(healthPointComponent.CurrentHealthPoint);
                    viewComponent.HealthBarMB.Init(world, _state.Value);

                    distanceForFightComponent.Distance = _state.Value.EnemyUnitStorage.GetDistanceByID(enemyUnit.gameObject.tag);

                    cooldownComponent.MaxValue = _state.Value.EnemyUnitStorage.GetCoolDownByID(enemyUnit.gameObject.tag);
                    cooldownComponent.CurrentValue = 0f;

                    doDamageComponent.Value = _state.Value.EnemyUnitStorage.GetDamageByID(enemyUnit.gameObject.tag);

                    if (unitLevelComponent.Value < 2)
                    {
                        enemyUnitComponent.VenomLevel = 1 - 1;
                        Debug.Log("����� 1 ���");
                    }
                    else if (unitLevelComponent.Value < 4)
                    {
                        enemyUnitComponent.VenomLevel = 2 - 1;
                        Debug.Log("����� 2 ���");
                    }
                    else
                    {
                        enemyUnitComponent.VenomLevel = 3 - 1;
                        Debug.Log("����� 3 ���");
                    }

                    switch (enemyUnit.gameObject.tag)
                    {
                        case nameof(EnemyMeleeUnit):
                        {
                            world.GetPool<MeleeUnitComponent>().Add(enemyEntity);
                            break;
                        }
                        case nameof(EnemyRangeUnit):
                        {
                            world.GetPool<RangeUnitTag>().Add(enemyEntity);
                            world.GetPool<RayRangeAttack>().Add(enemyEntity);
                            viewComponent.RayParticle = viewComponent.AttackMB.GetRayParticle().GetComponent<ParticleSystem>();
                            viewComponent.RayParticleChildren = viewComponent.AttackMB.GetRayParticle().GetComponentsInChildren<ParticleSystem>();

                            break;
                        }
                    }
                }
            }
        }
    }
}