using Leopotam.EcsLite;
using UnityEngine;

namespace Client
{
    sealed class InitPlayerUnit : IEcsInitSystem
    {
        private readonly string FrendlyUnit;
        public void Init (EcsSystems systems)
        {
             var allFrendlyUnits = GameObject.FindGameObjectsWithTag(nameof(FrendlyUnit));

             var world = systems.GetWorld();

             foreach (var frendlyUnit in allFrendlyUnits)
             {
                var entity = world.NewEntity();

                world.GetPool<FrendlyUnitComponent>().Add(entity);

                world.GetPool<ClosestTargetComponent>().Add(entity);

                ref var speedComponent = ref world.GetPool<SpeedComponent>().Add(entity);
                speedComponent.Speed = 20.0f;

                ref var viewComponent = ref world.GetPool<ViewComponent>().Add(entity);
                viewComponent.GameObject = frendlyUnit;
                viewComponent.Transform = frendlyUnit.transform;
                viewComponent.HealthBarMB = frendlyUnit.GetComponent<HealthBarMB>();
                viewComponent.HealthBarMB.SetMaxHealth(150);
                viewComponent.HealthBarMB.SetHealth(150);
                //viewComponent.HealthBarMB.Init(world);

                ref var distanceForFightComponent = ref world.GetPool<DistanceForFightComponent>().Add(entity);
                distanceForFightComponent.Distance = 5f;

                ref var cooldownComponent = ref world.GetPool<CooldownComponent>().Add(entity);
                cooldownComponent.MaxValue = 5f;
                cooldownComponent.CurrentValue = 0f;

                ref var doDamageComponent = ref world.GetPool<DoDamageComponent>().Add(entity);
                doDamageComponent.Value = 20f;
             }
        }
    }
}