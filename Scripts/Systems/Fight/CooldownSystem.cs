using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CooldownSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<CooldownComponent, ReloadingComponent>> _filter = default;
        public void Run(EcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref CooldownComponent cooldownComponent = ref _filter.Pools.Inc1.Get(entity);

                ref var currentCooldown = ref cooldownComponent.CurrentValue;
                ref var maxCooldown = ref cooldownComponent.MaxValue;

                if (currentCooldown > 0)
                {
                    currentCooldown -= Time.deltaTime;
                }
                else if (currentCooldown < 0)
                {
                    currentCooldown = 0;
                    _filter.Pools.Inc2.Del(entity);
                }
            }
        }
    }
}