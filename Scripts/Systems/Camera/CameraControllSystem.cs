using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client
{
    sealed class CameraControllSystem : IEcsRunSystem
    {
        readonly EcsFilterInject<Inc<FrendlyUnitComponent, ViewComponent, ClosestTargetComponent>, Exc<EmptyEntityAfterDeadComponent>> _friendlyUnitsFilter = default;
        readonly EcsFilterInject<Inc<InFightComponent>, Exc<EmptyEntityAfterDeadComponent>> _unitsInFightFilter = default;
        readonly EcsFilterInject<Inc<WinEvent>> _winEventFilter = default;
        readonly EcsFilterInject<Inc<LoseEvent>> _loseEventFilter = default;
        readonly EcsFilterInject<Inc<CameraComponent>> _cameraFilter = default;

        readonly EcsFilterInject<Inc<CenterFightPointComponent>> _centerFightPointFilter = default;

        private float _positionZ = float.MinValue;
        //private Vector3 _fightPosition = new Vector3(-9, 3, -6);
        //private Vector3 _runPosition = new Vector3(0, 0, 5);
        private Vector3 _fightPosition = new Vector3(0, 0, 0);
        private Vector3 _runPosition = new Vector3(0, 0, 5);
        private Vector3 _endPosition = new Vector3(11.5f, 2, 13);
        private Vector3 _actualPosition;
        //private Quaternion _fightRotation = Quaternion.Euler(45, 0, 0);
        //private Quaternion _runRotation = Quaternion.Euler(30, -50, 0);
        private Quaternion _fightRotation = Quaternion.Euler(45, 0, 0);
        private Quaternion _runRotation = Quaternion.Euler(45, 0, 0);
        private Quaternion _endRotation = Quaternion.Euler(40, -90, 0);
        private Quaternion _actualRotation;
        private float _fightSpeed = 2f;
        private float _runSpeed = 1f;
        private float _endSpeed = 1f;
        private float _actualSpeed;
        private float _highestZPosition;
        private float _lowestZPosition;
        private int _endCheck;

        public void Run(EcsSystems systems)
        {
            foreach (var win in _winEventFilter.Value)
            {
                _endCheck = + 1;
            }

            foreach (var lose in _loseEventFilter.Value)
            {
                _endCheck = - 1;
            }

            if (_endCheck != 0)
            {
                _highestZPosition = float.MinValue;
                _lowestZPosition = float.MaxValue;
                _actualSpeed = _endSpeed;
                _actualRotation = _endRotation; 
                EcsFilter friendlyUnitsFilter = systems.GetWorld().Filter<FrendlyUnitComponent>().Inc<ViewComponent>().Inc<ClosestTargetComponent>().Exc<EmptyEntityAfterDeadComponent>().End();
                EcsFilter enemyUnitsFilter = systems.GetWorld().Filter<EnemyUnitComponent>().Exc<EmptyEntityAfterDeadComponent>().Exc<DisabledEncounterTag> ().End();
                EcsFilter actualFilter;

                if (_endCheck > 0)
                {
                    actualFilter = friendlyUnitsFilter;
                }
                else
                {
                    actualFilter = enemyUnitsFilter;
                }

                foreach (var unitsEntity in actualFilter)
                {
                    ref var viewComp = ref _friendlyUnitsFilter.Pools.Inc2.Get(unitsEntity);
                    if (viewComp.Transform.position.z > _highestZPosition)
                    {
                        _highestZPosition = viewComp.Transform.position.z;
                    }
                    
                    if (viewComp.Transform.position.z < _lowestZPosition)
                    {
                        _lowestZPosition = viewComp.Transform.position.z;
                    }
                }

                _actualPosition = new Vector3(_endPosition.x, _endPosition.y, _endPosition.z + (_highestZPosition + _lowestZPosition) / 2);
            }
            else
            {
                if (_unitsInFightFilter.Value.GetEntitiesCount() > 0)
                {
                    _actualSpeed = _fightSpeed;
                    foreach (var unitsEntity in _centerFightPointFilter.Value)
                    {
                        ref var centerFightPointComponent = ref _centerFightPointFilter.Pools.Inc1.Get(unitsEntity);
                        _actualPosition = new Vector3(_fightPosition.x, _fightPosition.y, _fightPosition.z + centerFightPointComponent.Point.z);
                    }

                    _actualRotation = _fightRotation;
                }
                else if (_friendlyUnitsFilter.Value.GetEntitiesCount() > 0)
                {
                    _actualSpeed = _runSpeed;
                    foreach (var entity in _friendlyUnitsFilter.Value)
                    {
                        ref var viewComp = ref _friendlyUnitsFilter.Pools.Inc2.Get(entity);
                        if (viewComp.Transform.position.z > _positionZ)
                        {
                            _positionZ = viewComp.Transform.position.z;
                        }
                    }
                    _actualPosition = new Vector3(_runPosition.x, _runPosition.y, _runPosition.z + _positionZ);
                    _actualRotation = _runRotation;
                }
            }

            foreach (var cam in _cameraFilter.Value)
            {
                ref var cameraComp = ref _cameraFilter.Pools.Inc1.Get(cam);
                var pos = Vector3.Lerp(cameraComp.CameraHolderTransform.position, _actualPosition, Time.deltaTime * _actualSpeed);
                cameraComp.CameraHolderTransform.position = pos;
                cameraComp.CameraTransform.rotation = Quaternion.Slerp(cameraComp.CameraTransform.rotation, _actualRotation, Time.deltaTime * _actualSpeed);
                _positionZ = float.MinValue;
            }
        }
    }
}