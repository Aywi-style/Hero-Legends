using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class WinCheckSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<EnemyUnitComponent>, Exc<EmptyEntityAfterDeadComponent>> _enemyUnitsFilter = default;
        readonly EcsFilterInject<Inc<ViewComponent>> _filter = default;
        readonly EcsWorldInject _world;
        readonly EcsPoolInject<WinEvent> _winPool = default;
        public void Run (EcsSystems systems)
        {
            if (_enemyUnitsFilter.Value.GetEntitiesCount() > 0)
            {
                return;
            }

            _winPool.Value.Add(_world.Value.NewEntity());

            /*foreach(var entity in _filter.Value)
            {
                ref var viewComponent = ref _filter.Pools.Inc1.Get(entity);
                if(viewComponent.Animator != null)
                {
                    viewComponent.Animator.SetBool("Run", false);
                }
            }*/
        }
    }
}