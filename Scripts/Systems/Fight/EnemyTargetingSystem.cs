using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class EnemyTargetingSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<EnemyUnitComponent>, Exc<EmptyEntityAfterDeadComponent, InFightComponent, DisabledEncounterTag>> _enemyUnitsFilter = default;
        readonly EcsFilterInject<Inc<FrendlyUnitComponent, ClosestTargetComponent>, Exc<EmptyEntityAfterDeadComponent>> _frendlyTargetsFilter = default;

        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<ClosestTargetComponent> _closestTargetPool = default;
        readonly EcsPoolInject<EmptyEntityAfterDeadComponent> _emptyEntityAfterDeadPool = default;

        public void Run (EcsSystems systems)
        {
            if (_enemyUnitsFilter.Value.GetEntitiesCount() == 0)
            {
                return;
            }

            foreach (int enemyUnitEntity in _enemyUnitsFilter.Value)
            {
                ref ViewComponent viewComponent = ref _viewPool.Value.Get(enemyUnitEntity);
                ref ClosestTargetComponent closestTargetComponent = ref _closestTargetPool.Value.Get(enemyUnitEntity);

                if (closestTargetComponent.Entity != 0)
                {
                    if (_emptyEntityAfterDeadPool.Value.Has(closestTargetComponent.Entity))
                    {
                        closestTargetComponent.Entity = 0;
                        closestTargetComponent.Target = null;
                    }
                }

                if (_frendlyTargetsFilter.Value.GetEntitiesCount() == 0)
                {
                    closestTargetComponent.Entity = 0;
                    closestTargetComponent.Target = null;
                    //viewComponent.Animator.CrossFadeInFixedTime("Idle", 0.1f, -1, 0f, 0f);
                    viewComponent.Animator.SetTrigger("UnitIdle");
                    //todo переделать на бул
                    continue;
                }
                
                float distance = Mathf.Infinity;

                foreach (int enemyEntity in _frendlyTargetsFilter.Value)
                {
                    ref ViewComponent enemyViewComponent = ref _viewPool.Value.Get(enemyEntity);

                    ref var enemyObject = ref enemyViewComponent.GameObject;
                    ref var enemyTransform = ref enemyViewComponent.Transform;

                    Vector3 distanceBetweenEntitys = enemyTransform.position - viewComponent.Transform.position;
                    float curentDistance = distanceBetweenEntitys.sqrMagnitude;
                    if (curentDistance < distance)
                    {
                        distance = curentDistance;
                        closestTargetComponent.Entity = enemyEntity;
                        closestTargetComponent.Target = enemyObject;
                    }
                }
            }
        }
    }
}