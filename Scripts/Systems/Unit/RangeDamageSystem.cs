using Leopotam.EcsLite;
using UnityEngine;
using Leopotam.EcsLite.Di;

namespace Client {
    sealed class RangeDamageSystem : IEcsRunSystem {
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<RangeAttackEvent>> _rangeAttackFilter = default;
        readonly EcsPoolInject<ClosestTargetComponent> _closestTargetPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<ArrowComponent> _arrowPool = default;
        readonly EcsPoolInject<DoDamageComponent> _doDamagePool = default;
        readonly EcsPoolInject<FrendlyUnitComponent> _frendlyPool = default;
        readonly EcsPoolInject<EnemyUnitComponent> _enemyPool = default;


        public void Run (EcsSystems systems) {
            
            foreach(var entity in _rangeAttackFilter.Value)
            {
                ref var rangeAttackEvent = ref _rangeAttackFilter.Pools.Inc1.Get(entity);
                GameObject _arrowPrefab = _state.Value.EnemyUnitStorage.GetBulletByID(0);
                GameObject arrowGameObject = null;

                if (_enemyPool.Value.Has(rangeAttackEvent.HolderEntity))
                {
                    ref var enemyUnitComponent = ref _enemyPool.Value.Get(rangeAttackEvent.HolderEntity);
                    _arrowPrefab = _state.Value.EnemyUnitStorage.GetBulletByID(enemyUnitComponent.VenomLevel);
                    Debug.Log("Запускаем " + _arrowPrefab);
                }

                if (_frendlyPool.Value.Has(rangeAttackEvent.HolderEntity))
                {
                    ref var unitComp = ref _frendlyPool.Value.Get(rangeAttackEvent.HolderEntity);
                    _arrowPrefab = _state.Value.UnitStorage.GetArrowObjectByID(unitComp.UnitID);
                    //Debug.Log(unitComp.UnitID);
                }

                if (rangeAttackEvent.TransformFirePoint.childCount < 1)
                {
                    arrowGameObject = GameObject.Instantiate(_arrowPrefab);
                }
                else
                {
                    arrowGameObject = rangeAttackEvent.TransformFirePoint.GetChild(0).gameObject;
                    arrowGameObject.transform.SetParent(null);
                }

                arrowGameObject.SetActive(true);

                ref var arrowClosestTargetComponent = ref _closestTargetPool.Value.Add(entity);
                arrowClosestTargetComponent.Entity = rangeAttackEvent.Entity;
                arrowClosestTargetComponent.Target = rangeAttackEvent.Target;

                ref var arrowViewComponent = ref _viewPool.Value.Add(entity);
                arrowViewComponent.GameObject = arrowGameObject;
                arrowViewComponent.Transform = arrowGameObject.transform;
                
                arrowGameObject.transform.position = rangeAttackEvent.TransformFirePoint.position;
                arrowGameObject.transform.rotation = rangeAttackEvent.Transform.rotation;

                ref var arrowDoDamgeComponent = ref _doDamagePool.Value.Add(entity);
                arrowDoDamgeComponent.Value = rangeAttackEvent.Damage;

                ref var arrowComponent = ref _arrowPool.Value.Add(entity);
                arrowComponent.HolderEntity = rangeAttackEvent.HolderEntity;
                arrowComponent.Quiver = rangeAttackEvent.TransformFirePoint;
            }
        }
    }
}