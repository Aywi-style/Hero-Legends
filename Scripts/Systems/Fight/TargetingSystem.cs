using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class TargetingSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<FrendlyUnitComponent, ClosestTargetComponent>, Exc<EmptyEntityAfterDeadComponent>> _frendlyUnitsFilter = default;
        readonly EcsFilterInject<Inc<EnemyUnitComponent>, Exc<EmptyEntityAfterDeadComponent>> _enemyTargetsFilter = default;

        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<ClosestTargetComponent> _closestTargetPool = default;
        readonly EcsPoolInject<EmptyEntityAfterDeadComponent> _emptyEntityAfterDeadPool = default;
        
        public void Run (EcsSystems systems)
        {
            if (_frendlyUnitsFilter.Value.GetEntitiesCount() == 0)
            {
                return;
            }

            foreach (int frendlyUnitEntity in _frendlyUnitsFilter.Value)
            {
                ref var viewComponent = ref _viewPool.Value.Get(frendlyUnitEntity);
                ref var closestTargetComponent = ref _closestTargetPool.Value.Get(frendlyUnitEntity);

                if (_emptyEntityAfterDeadPool.Value.Has(closestTargetComponent.Entity) || closestTargetComponent.Entity != 0)
                {
                    closestTargetComponent.Entity = 0;
                    closestTargetComponent.Target = null;
                }

                if (_enemyTargetsFilter.Value.GetEntitiesCount() == 0)
                {
                    closestTargetComponent.Entity = 0;
                    closestTargetComponent.Target = null;
                    //viewComponent.Animator.CrossFadeInFixedTime("Idle", 0.1f, -1, 0f, 0f);
                    viewComponent.Animator.SetTrigger("UnitIdle");
                    //todo переделать на бул
                    continue;
                }

                float distance = Mathf.Infinity;

                foreach (int enemyEntity in _enemyTargetsFilter.Value)
                {
                    ref ViewComponent enemyViewComponent = ref _viewPool.Value.Get(enemyEntity);

                    Vector3 distanceBetweenEntitys = enemyViewComponent.Transform.position - viewComponent.Transform.position;
                    float curentDistance = distanceBetweenEntitys.sqrMagnitude;
                    if (curentDistance < distance)
                    {
                        distance = curentDistance;
                        closestTargetComponent.Entity = enemyEntity;
                        closestTargetComponent.Target = enemyViewComponent.GameObject;
                    }
                }
            }
        }
    }
}