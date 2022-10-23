using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class RayAttackSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<InFightComponent, RangeUnitTag, RayRangeAttack>, Exc<EmptyEntityAfterDeadComponent, DisabledEncounterTag>> _RayUnitFilter = default;

        readonly EcsPoolInject<DoDamageComponent> _doDamagePool = default;
        readonly EcsPoolInject<ClosestTargetComponent> _closestTargetPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;

        private int RayLengthOffset = -1;

        public void Run(EcsSystems systems)
        {
            var state = systems.GetShared<GameState>();

            foreach (int rayUnitEntity in _RayUnitFilter.Value)
            {
                ref var doDamageComponent = ref _doDamagePool.Value.Get(rayUnitEntity);
                ref var closestTargetComponent = ref _closestTargetPool.Value.Get(rayUnitEntity);
                ref var viewComponent = ref _viewPool.Value.Get(rayUnitEntity);

                viewComponent.Transform.LookAt(closestTargetComponent.Target.transform);
                viewComponent.Animator.SetBool("Idle", false);
                //viewComponent.Animator.CrossFadeInFixedTime("RayRangeAttack", 0.1f, -1, 0f, 0f);
                viewComponent.Animator.SetBool("Run", false);
                viewComponent.Animator.SetBool("Attack", true);
                viewComponent.AttackMB.InitRayStrike(closestTargetComponent.Entity, doDamageComponent.Value, closestTargetComponent.Target, rayUnitEntity);
                Debug.Log("LUCH DOBRA");

                viewComponent.RayParticle.startLifetime = closestTargetComponent.Distance / viewComponent.RayParticle.startSpeed + RayLengthOffset;
                foreach (var rayParticleChield in viewComponent.RayParticleChildren)
                {
                    rayParticleChield.startLifetime = viewComponent.RayParticle.startLifetime;
                }
            }
        }
    }
}