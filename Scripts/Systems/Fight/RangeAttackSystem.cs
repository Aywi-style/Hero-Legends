using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class RangeAttackSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<InFightComponent, RangeUnitTag>, Exc<EmptyEntityAfterDeadComponent, DisabledEncounterTag, RayRangeAttack>> _filter = default;

        readonly EcsPoolInject<CooldownComponent> _cooldownPool = default;
        readonly EcsPoolInject<DoDamageComponent> _doDamagePool = default;
        readonly EcsPoolInject<ClosestTargetComponent> _closestTargetPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<ReloadingComponent> _reloadingPool = default;
        public void Run(EcsSystems systems)
        {
            var state = systems.GetShared<GameState>();

            foreach (int entity in _filter.Value)
            {
                ref var cooldownComponent = ref _cooldownPool.Value.Get(entity);
                ref var doDamageComponent = ref _doDamagePool.Value.Get(entity);
                ref var closestTargetComponent = ref _closestTargetPool.Value.Get(entity);
                ref var viewComponent = ref _viewPool.Value.Get(entity);

                /*ref var targetHealthPointComponent = ref _healthPointPool.Value.Get(closestTargetComponent.Entity);
                ref var targeCurrenttHealthPoint = ref targetHealthPointComponent.CurrentHealthPoint;*/

                if (cooldownComponent.CurrentValue == 0)
                {
                    cooldownComponent.CurrentValue = cooldownComponent.MaxValue;
                    viewComponent.Transform.LookAt(closestTargetComponent.Target.transform);
                    _reloadingPool.Value.Add(entity);
                    viewComponent.Animator.SetBool("Idle", false);
                    viewComponent.Animator.CrossFadeInFixedTime("RangeAttack", 0.1f, -1, 0f, 0f);
                    viewComponent.Animator.SetBool("Run", false);
                    viewComponent.AttackMB.InitArrow(closestTargetComponent.Target, closestTargetComponent.Entity, viewComponent.Transform, doDamageComponent.Value, entity);
                }
                else
                {
                    viewComponent.Animator.SetBool("Idle", true);
                }
            }
        }
    }
}