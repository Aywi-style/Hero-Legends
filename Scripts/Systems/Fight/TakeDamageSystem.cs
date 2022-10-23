using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class TakeDamageSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsWorldInject _world = default;
        readonly EcsFilterInject<Inc<TakeDamageEvent>> _takeDamageFilter = default;
        readonly EcsPoolInject<HealthPointComponent> _healthPointPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;

        readonly EcsPoolInject<CoinRewardComponent> _coinRewardpool = default;
        readonly EcsPoolInject<CoinsChangerEvent> _coinChangerPool = default;
        public void Run (EcsSystems systems)
        {
            foreach(var entity in _takeDamageFilter.Value)
            {
                ref var takeDamageComponent = ref _takeDamageFilter.Pools.Inc1.Get(entity);

                if (!_healthPointPool.Value.Has(takeDamageComponent.Entity))
                {
                    _takeDamageFilter.Pools.Inc1.Del(entity);
                    continue;
                }

                ref var healthPointComponent = ref _healthPointPool.Value.Get(takeDamageComponent.Entity);

                if (takeDamageComponent.Damage > healthPointComponent.CurrentHealthPoint)
                {
                    takeDamageComponent.Damage = healthPointComponent.CurrentHealthPoint;
                }

                healthPointComponent.CurrentHealthPoint -= takeDamageComponent.Damage;

                if (_viewPool.Value.Has(takeDamageComponent.Entity))
                {
                    ref var viewComponent = ref _viewPool.Value.Get(takeDamageComponent.Entity);
                    viewComponent.HealthBarMB.UpdateHealthBar(healthPointComponent.CurrentHealthPoint);
                    var pos = viewComponent.GameObject.transform.position;
                    
                    viewComponent.HitParticle.Play();
                }
                //
               
                //
                if (_coinRewardpool.Value.Has(takeDamageComponent.Entity))
                {
                    ref var coinEventComponent = ref _coinChangerPool.Value.Add(_world.Value.NewEntity());
                    ref var coinRewardComponent = ref _coinRewardpool.Value.Get(takeDamageComponent.Entity);
                    coinEventComponent.Damage = Mathf.RoundToInt(takeDamageComponent.Damage);
                    coinEventComponent.DamageReward = coinRewardComponent.DamageReward;
                    coinEventComponent.MaxHealthPoint = healthPointComponent.MaxHealthPoint;
                    coinEventComponent.Entity = takeDamageComponent.Entity;
                }
                //_takeDamageFilter.Pools.Inc1.Del(entity);
            }
        }
    }
}