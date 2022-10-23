using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class ArrowWorkingSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<ArrowComponent, ClosestTargetComponent, ViewComponent, DoDamageComponent>> _arrowFilter = default;
        readonly EcsWorldInject _world = default;
        readonly EcsPoolInject<TakeDamageEvent> _takeDamagePool = default;

        private int arrowSpeed = 20;
        private Vector3 arrowRotateOffset = new Vector3(-90, 0, 0);
        private Vector3 arrowPositionOffset = new Vector3(0, 1, 0);
        public void Run (EcsSystems systems)
        {
            foreach (int entity in _arrowFilter.Value)
            {
                ref var arrowComponent = ref _arrowFilter.Pools.Inc1.Get(entity);
                ref var viewComponent = ref _arrowFilter.Pools.Inc3.Get(entity);
                ref var doDamageComponent = ref _arrowFilter.Pools.Inc4.Get(entity);
                ref var closestTargetComponent = ref _arrowFilter.Pools.Inc2.Get(entity);
                var targetPosition = closestTargetComponent.Target.transform.position;

                viewComponent.Transform.LookAt(targetPosition + arrowPositionOffset);
                viewComponent.Transform.Rotate(arrowRotateOffset);
                viewComponent.Transform.position = Vector3.MoveTowards(viewComponent.Transform.position, targetPosition + arrowPositionOffset, Time.deltaTime * arrowSpeed);

                Vector3 diff = targetPosition - viewComponent.Transform.position;
                float distanceToTarget = diff.sqrMagnitude;
                if (distanceToTarget <= 3)
                {
                    ref var takeDamageComponent = ref _takeDamagePool.Value.Add(_world.Value.NewEntity());
                    takeDamageComponent.Damage = doDamageComponent.Value;
                    takeDamageComponent.Entity = closestTargetComponent.Entity;
                    takeDamageComponent.HolderEntity = arrowComponent.HolderEntity;

                    viewComponent.Transform.SetParent(arrowComponent.Quiver);
                    viewComponent.GameObject.SetActive(false);
                    systems.GetWorld().DelEntity(entity);
                }
            }
        }
    }
}