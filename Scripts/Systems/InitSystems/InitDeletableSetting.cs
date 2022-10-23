using Leopotam.EcsLite;
using UnityEngine;

namespace Client
{
    sealed class InitDeletableSetting : IEcsInitSystem
    {
        private readonly string DeletableSetting;
        public void Init (EcsSystems systems)
        {
            var allDeletableSettings = GameObject.FindGameObjectsWithTag(nameof(DeletableSetting));

            var world = systems.GetWorld();

            foreach (var deletableSetting in allDeletableSettings)
            {
                var deletableEntity = world.NewEntity();

                world.GetPool<DeletableSetting>().Add(deletableEntity);

                ref var viewComponent = ref world.GetPool<ViewComponent>().Add(deletableEntity);
                viewComponent.GameObject = deletableSetting;
                viewComponent.Transform = deletableSetting.transform;
            }
        }
    }
}