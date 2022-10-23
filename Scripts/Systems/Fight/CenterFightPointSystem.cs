using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CenterFightPointSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<InFightComponent, ViewComponent>, Exc<EmptyEntityAfterDeadComponent>> _unitsInFightFilter = default;
        readonly EcsFilterInject<Inc<CenterFightPointComponent>> _centerFightPointFilter = default;

        private Vector3 _positionBewteenUnits;
        private Vector3 _actualPosition;
        private Vector3 _highestPosition;
        private Vector3 _lowestPosition;
        private float _divisionCount;

        public void Run (EcsSystems systems)
        {
            if (_centerFightPointFilter.Value.GetEntitiesCount() == 0)
            {
                var centerFightPointEntity = systems.GetWorld().NewEntity();
                ref var centerFightPointComponent = ref _centerFightPointFilter.Pools.Inc1.Add(centerFightPointEntity);
                centerFightPointComponent.Point = Vector3.zero;
            }

            if (_unitsInFightFilter.Value.GetEntitiesCount() == 0)
            {
                return;
            }

            _highestPosition = Vector3.negativeInfinity;
            _lowestPosition = Vector3.positiveInfinity;

            foreach (var unitEntity in _unitsInFightFilter.Value)
            {
                ref var viewComponent = ref _unitsInFightFilter.Pools.Inc2.Get(unitEntity);
                _highestPosition = Vector3.Max(_highestPosition, viewComponent.Transform.position);
                _lowestPosition = Vector3.Min(_lowestPosition, viewComponent.Transform.position);
                _positionBewteenUnits += viewComponent.Transform.position;
                _divisionCount++;
            }

            _actualPosition = Vector3.Lerp(_lowestPosition, _highestPosition, 0.5f);

            foreach (var centerFightPointEntity in _centerFightPointFilter.Value)
            {
                ref var centerFightPointComponent = ref _centerFightPointFilter.Pools.Inc1.Get(centerFightPointEntity);
                centerFightPointComponent.Point = _actualPosition;
                _positionBewteenUnits = Vector3.zero;
                _divisionCount = 0;
            }
        }
    }
}