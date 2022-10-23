using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class RangeDistanceBuffSystem : IEcsRunSystem
    {        
        readonly EcsFilterInject<Inc<FrendlyUnitComponent, DistanceForFightComponent, RangeUnitTag>, Exc<DistanceBuffedTag>> _unbuffedUnitsFilter = default;
        readonly EcsPoolInject<DistanceBuffedTag> _distanceBuffedPool = default;

        public void Run (EcsSystems systems)
        {
            foreach (var unitEntity in _unbuffedUnitsFilter.Value)
            {
                int buff = 0;
                ref var friendlyUnitComponent = ref _unbuffedUnitsFilter.Pools.Inc1.Get(unitEntity);
                ref var dictanceComponent = ref _unbuffedUnitsFilter.Pools.Inc2.Get(unitEntity);

                if (friendlyUnitComponent.BoundID < 5)
                {
                    buff = 0;
                }
                else if (friendlyUnitComponent.BoundID < 10)
                {
                    buff = 20;
                }
                else
                {
                    buff = 40;
                }

                dictanceComponent.Distance += buff;

                _distanceBuffedPool.Value.Add(unitEntity);
            }
        }
    }
}