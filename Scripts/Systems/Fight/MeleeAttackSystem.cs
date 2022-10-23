using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class MeleeAttackSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<InFightComponent, MeleeUnitComponent>, Exc<EmptyEntityAfterDeadComponent, DisabledEncounterTag>> _filter = default;

        readonly EcsPoolInject<CooldownComponent> _cooldownPool = default;
        readonly EcsPoolInject<DoDamageComponent> _doDamagePool = default;
        readonly EcsPoolInject<ClosestTargetComponent> _closestTargetPool = default;
        readonly EcsPoolInject<ReloadingComponent> _reloadingPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        //readonly EcsFilterInject<Inc<ViewComponent, FrendlyUnitComponent, ClosestTargetComponent>, Exc<EmptyEntityAfterDeadComponent>> _animFilter = default;
        public void Run (EcsSystems systems)
        {
            foreach (int entity in _filter.Value)
            {
                ref var cooldownComponent = ref _cooldownPool.Value.Get(entity);
                ref var doDamageComponent = ref _doDamagePool.Value.Get(entity);
                ref var closestTargetComponent = ref _closestTargetPool.Value.Get(entity);
                ref var viewComp = ref _viewPool.Value.Get(entity);

                if (cooldownComponent.CurrentValue == 0)
                {
                    /*ref HealthPointComponent targetHealthPointComponent = ref _healthPointPool.Value.Get(targetEntity);
                    ref var targeCurrentHealthPoint = ref targetHealthPointComponent.CurrentHealthPoint;
                    targeCurrentHealthPoint -= giveDamage;*/
                    viewComp.Transform.LookAt(closestTargetComponent.Target.transform);
                    cooldownComponent.CurrentValue = cooldownComponent.MaxValue;
                    _reloadingPool.Value.Add(entity);
                    viewComp.Animator.SetBool("Idle", false);
                    viewComp.Animator.CrossFadeInFixedTime("MeleeAttack", 0.1f, -1, 0f, 0f);
                    viewComp.Animator.SetBool("Run", false);
                    viewComp.AttackMB.InitMeleeStrike(closestTargetComponent.Entity, doDamageComponent.Value, closestTargetComponent.Target, entity);
                    
                    //viewComp.HealthBarMB.UpdateHealthBar(doDamageComponent.Value);
                    //viewComp.HealthBarMB.CameraFollow();
                }
                else
                {
                    viewComp.Animator.SetBool("Idle", true);
                }
            }
        }
    }
}