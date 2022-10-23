using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    sealed class InitEncounters : IEcsInitSystem
    {
        readonly EcsFilterInject<Inc<EnemyUnitComponent, DisabledEncounterTag, EncounterComponent>, Exc<EmptyEntityAfterDeadComponent>> _enemyUnitsFilter = default;

        readonly EcsPoolInject<DisabledEncounterTag> _disabledEncounterPool = default;
        readonly EcsPoolInject<EncounterComponent> _encounterPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;

        private int _defaultActiveEncounterOnLevel = 1;
        private string Point;

        public void Init (EcsSystems systems)
        {
            var world = systems.GetWorld();

            var allEncounters = GameObject.FindGameObjectsWithTag(nameof(Point));

            foreach (var encounter in allEncounters)
            {
                var encounterEntity = world.NewEntity();

                ref var encounterComponent = ref _encounterPool.Value.Add(encounterEntity);
                encounterComponent.Number = int.Parse(encounter.gameObject.name);
                encounterComponent.EnemyUnitsEntitys = new List<int>();

                ref var viewComponent = ref _viewPool.Value.Add(encounterEntity);
                viewComponent.GameObject = encounter;

                if (encounterComponent.Number > _defaultActiveEncounterOnLevel)
                {
                    _disabledEncounterPool.Value.Add(encounterEntity);
                }

                foreach (var enemyUnit in _enemyUnitsFilter.Value)
                {
                    ref var enemyUnitEncounter = ref _enemyUnitsFilter.Pools.Inc3.Get(enemyUnit);

                    if (enemyUnitEncounter.Number == encounterComponent.Number)
                    {
                        encounterComponent.EnemyUnitsEntitys.Add(enemyUnit);
                    }
                }
            }
        }
    }
}