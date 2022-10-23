using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class InitLevelRaward : IEcsInitSystem
    {
        readonly EcsSharedInject<GameState> _state = default;

        public void Init (EcsSystems systems)
        {
            var world = systems.GetWorld();
            var LevelRawardEntity = world.NewEntity();

            _state.Value.EntityLevelReward = LevelRawardEntity;

            ref var levelRawardComponent = ref world.GetPool<LevelRawardComponent>().Add(LevelRawardEntity);
            levelRawardComponent.Value = 0;
        }
    }
}