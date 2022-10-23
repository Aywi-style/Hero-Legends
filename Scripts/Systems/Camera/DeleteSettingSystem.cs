using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class DeleteSettingSystem : MonoBehaviour, IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<DeletableSetting, ViewComponent>> _deletableSettingFilter = default;
        readonly EcsFilterInject<Inc<CameraComponent>> _cameraFilter = default;

        private Vector3 cameraPosition;

        private int visibleDistance = 130;
        private int deletableDistance = 30;

        public void Run (EcsSystems systems)
        {
            foreach (var cameraEntity in _cameraFilter.Value)
            {
                ref var cameraComponent = ref _cameraFilter.Pools.Inc1.Get(cameraEntity);
                cameraPosition = cameraComponent.CameraTransform.position;
            }

            foreach (var deletableEntity in _deletableSettingFilter.Value)
            {
                ref var viewComponent = ref _deletableSettingFilter.Pools.Inc2.Get(deletableEntity);
                ref var deletableObject = ref viewComponent.GameObject;
                Vector3 deletableSettingPosition = viewComponent.Transform.position;

                if (cameraPosition.z - deletableDistance > deletableSettingPosition.z)
                {
                    deletableObject.SetActive(false);
                }
                else
                {
                    Vector3 cameraDistance = cameraPosition - deletableSettingPosition;
                    float curentDistance = cameraDistance.sqrMagnitude;
                    
                    if (curentDistance > visibleDistance * visibleDistance)
                    {
                        deletableObject.SetActive(false);
                    }
                    else
                    {
                        deletableObject.SetActive(true);
                    }
                }
            }
        }
    }
}