using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using System;

namespace Client
{
    sealed class LevelingSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _gameState = default;
        readonly EcsFilterInject<Inc<LevelingEvent>> _levelingFilter = default;
        readonly EcsFilterInject<Inc<EnemyUnitComponent, UnitLevelComponent>> _enemyUnitsFilter = default;
        private int _startLevelingPoint;
        private int _levelingPointPerUnit;
        private int _LevelingValue;
        private int _enemyMultiply;
        private float _jointLevelingValue;

        public void Run (EcsSystems systems)
        {
            if (_levelingFilter.Value.GetEntitiesCount() < 1)
            {
                return;
            }

            _startLevelingPoint = 6;
            _jointLevelingValue = _startLevelingPoint + ((_gameState.Value.Saves.LVL - 1) * 2);
            _levelingPointPerUnit = Mathf.CeilToInt(_jointLevelingValue / _enemyUnitsFilter.Value.GetEntitiesCount());

            for (int i = 0; _levelingPointPerUnit >= Mathf.Pow(2, i); i++)
            {
                _LevelingValue = i;
            }

            var world = systems.GetWorld();

            foreach (var enemyEntity in _enemyUnitsFilter.Value)
            {

                ref var coinRewardComponent = ref world.GetPool<CoinRewardComponent>().Get(enemyEntity);
                ref var healthPointComponent = ref world.GetPool<HealthPointComponent>().Get(enemyEntity);
                ref var viewComponent = ref world.GetPool<ViewComponent>().Get(enemyEntity);
                ref var cooldownComponent = ref world.GetPool<CooldownComponent>().Get(enemyEntity);
                ref var doDamageComponent = ref world.GetPool<DoDamageComponent>().Get(enemyEntity);
                ref var unitLevelComponent = ref world.GetPool<UnitLevelComponent>().Get(enemyEntity);

                unitLevelComponent.Value += _LevelingValue;
                _enemyMultiply = Convert.ToInt32(unitLevelComponent.Value - 1);

                viewComponent.Transform.localScale *= (1 + (_enemyMultiply * 0.05f));

                coinRewardComponent.FullValue = (ulong)((coinRewardComponent.FullValue * Mathf.Pow(2, _enemyMultiply)) * Mathf.Pow(0.65f, _enemyMultiply));
                coinRewardComponent.DamageReward = (ulong)Mathf.RoundToInt(coinRewardComponent.FullValue * 0.5f);
                coinRewardComponent.KillReward = (ulong)Mathf.RoundToInt(coinRewardComponent.FullValue * 0.5f);

                healthPointComponent.MaxHealthPoint *= (1 + (_enemyMultiply * 1f));
                healthPointComponent.CurrentHealthPoint = healthPointComponent.MaxHealthPoint;

                viewComponent.HealthBarMB.SetMaxHealth(healthPointComponent.MaxHealthPoint);
                viewComponent.HealthBarMB.SetHealth(healthPointComponent.CurrentHealthPoint);

                cooldownComponent.MaxValue *= Mathf.Pow(0.97f, _enemyMultiply);

                doDamageComponent.Value *= (1 + (_enemyMultiply * 1f));
            }

            foreach (var levelingEntity in _levelingFilter.Value)
            {
                world.DelEntity(levelingEntity);
            }
        }
    }
}