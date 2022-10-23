using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class DistanceToTargetSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<ClosestTargetComponent, ViewComponent>, Exc<DieComponent>> _entitysFilter = default;

        readonly EcsPoolInject<ClosestTargetComponent> _targetablePool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        public void Run(EcsSystems systems)
        {
            foreach (var entity in _entitysFilter.Value)
            {
                ref var targetableComponent = ref _targetablePool.Value.Get(entity);
                if (!targetableComponent.Target)
                {
                    continue;
                }

                ref var viewComponent = ref _viewPool.Value.Get(entity);
                targetableComponent.Distance = Mathf.Sqrt((targetableComponent.Target.transform.position - viewComponent.GameObject.transform.position).sqrMagnitude);
            }
        }
    }
}