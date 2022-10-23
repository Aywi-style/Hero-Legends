using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class LoseCheckSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<FrendlyUnitComponent, ClosestTargetComponent>, Exc<EmptyEntityAfterDeadComponent>> _enemyUnitsFilter = default;
        readonly EcsFilterInject<Inc<ViewComponent>> _filter = default;
        readonly EcsWorldInject _world;
        readonly EcsPoolInject<LoseEvent> _losePool = default;
        public void Run (EcsSystems systems)
        {
            if (_enemyUnitsFilter.Value.GetEntitiesCount() > 0)
            {
                return;
            }

            _losePool.Value.Add(_world.Value.NewEntity());

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