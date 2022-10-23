using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class MoveToTargetSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<FrendlyUnitComponent, SpeedComponent>, Exc<InFightComponent, EmptyEntityAfterDeadComponent>> _friendlyUnitsFilter = default;
        readonly EcsFilterInject<Inc<EnemyUnitComponent, SpeedComponent>, Exc<InFightComponent, EmptyEntityAfterDeadComponent, DisabledEncounterTag>> _enemyUnitsFilter = default;

        readonly EcsFilterInject<Inc<FrendlyUnitComponent, ClosestTargetComponent>, Exc<EmptyEntityAfterDeadComponent>> _liveFriendlyUnitsFilter = default;
        readonly EcsFilterInject<Inc<EnemyUnitComponent, ClosestTargetComponent>, Exc<EmptyEntityAfterDeadComponent>> _liveEnemyUnitsFilter = default;

        readonly EcsPoolInject<RangeUnitTag> _rangeUnitPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<ClosestTargetComponent> _closestTargetPool = default;

        public void Run (EcsSystems systems)
        {
            if (_liveEnemyUnitsFilter.Value.GetEntitiesCount() != 0)
            {

                foreach (int friendlyUnitEntity in _friendlyUnitsFilter.Value)
                {
                    ref ViewComponent viewComponent = ref _viewPool.Value.Get(friendlyUnitEntity);
                    ref SpeedComponent speedComponent = ref _friendlyUnitsFilter.Pools.Inc2.Get(friendlyUnitEntity);
                    ref ClosestTargetComponent closestTargetComponent = ref _closestTargetPool.Value.Get(friendlyUnitEntity);

                    if (_rangeUnitPool.Value.Has(friendlyUnitEntity))
                    {
                        Vector3 tempTargetPosition = new Vector3(viewComponent.Transform.position.x, viewComponent.Transform.position.y, closestTargetComponent.Target.transform.position.z);
                        viewComponent.Transform.LookAt(tempTargetPosition);
                        viewComponent.Transform.position = Vector3.MoveTowards  (viewComponent.Transform.position,
                                                                                tempTargetPosition,
                                                                                speedComponent.Speed * Time.deltaTime);
                    }
                    else
                    {
                        viewComponent.Transform.LookAt(closestTargetComponent.Target.transform.position);
                        viewComponent.Transform.position = Vector3.MoveTowards  (viewComponent.Transform.position,
                                                                                closestTargetComponent.Target.transform.position,
                                                                                speedComponent.Speed * Time.deltaTime);
                    }
                    
                    viewComponent.Animator.SetBool("Run", true);
                }
            }

            if (_liveFriendlyUnitsFilter.Value.GetEntitiesCount() != 0)
            {

                foreach (int enemyUnitEntity in _enemyUnitsFilter.Value)
                {
                    ref ViewComponent viewComponent = ref _viewPool.Value.Get(enemyUnitEntity);
                    ref SpeedComponent speedComponent = ref _enemyUnitsFilter.Pools.Inc2.Get(enemyUnitEntity);
                    ref ClosestTargetComponent closestTargetComponent = ref _closestTargetPool.Value.Get(enemyUnitEntity);

                    viewComponent.Transform.LookAt(closestTargetComponent.Target.transform.position);
                    viewComponent.Transform.position = Vector3.MoveTowards  (viewComponent.Transform.position,
                                                                            closestTargetComponent.Target.transform.position,
                                                                            speedComponent.Speed * Time.deltaTime);
                    
                    viewComponent.Animator.SetBool("Run", true);

                }
            }
        }
    }
}