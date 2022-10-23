using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class WinRotationSystem : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _state = default;

        readonly EcsFilterInject<Inc<WinEvent>> _winFilter = default;
        readonly EcsFilterInject<Inc<LoseEvent>> _loseFilter = default;

        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<CameraComponent> _cameraPool = default;

        private EcsFilter _winUnitsFilter;

        private bool _levelEnd = false;
        private float unitRotationSpeed = 1f;

        public void Run (EcsSystems systems)
        {
            if (!_levelEnd)
            {
                foreach (var entity in _winFilter.Value)
                {
                    _levelEnd = true;
                    _winUnitsFilter = systems.GetWorld().Filter<FrendlyUnitComponent>().Inc<ClosestTargetComponent>().Inc<ViewComponent>().Exc<EmptyEntityAfterDeadComponent>().End();
                }
                foreach (var entity in _loseFilter.Value)
                {
                    _levelEnd = true;
                    _winUnitsFilter = systems.GetWorld().Filter<EnemyUnitComponent>().Inc<ViewComponent>().Exc<EmptyEntityAfterDeadComponent>().End();
                }
                return;
            }

            foreach (var unitEntity in _winUnitsFilter)
            {
                ref var unitViewComponent = ref _viewPool.Value.Get(unitEntity);
                ref var cameraComponent = ref _cameraPool.Value.Get(_state.Value.EntityCamera);

                var direction = new Vector2(cameraComponent.CameraTransform.position.z, cameraComponent.CameraTransform.position.x) - new Vector2(unitViewComponent.Transform.position.z, unitViewComponent.Transform.position.x);
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                unitViewComponent.Transform.rotation = Quaternion.Slerp(unitViewComponent.Transform.rotation, Quaternion.Euler(0, angle, 0), unitRotationSpeed * Time.deltaTime);
                // Debug.Log(angle);

                // unitViewComponent.Transform.LookAt(cameraComponent.CameraTransform.position, Vector3.left);
            }
        }
    }
}