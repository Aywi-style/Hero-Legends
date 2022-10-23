using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client
{
    sealed class EncounterSwitcher : IEcsRunSystem
    {
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<EnemyUnitComponent, EncounterComponent>, Exc<DisabledEncounterTag, EmptyEntityAfterDeadComponent>> _enabledEnemyUnitsFilter = default;
        readonly EcsFilterInject<Inc<FrendlyUnitComponent, InFightComponent, ClosestTargetComponent>, Exc<EmptyEntityAfterDeadComponent>> _friendlyUnitsInFightFilter = default;
        readonly EcsFilterInject<Inc<EnemyUnitComponent, EncounterComponent, DisabledEncounterTag>, Exc<EmptyEntityAfterDeadComponent>> _disabledEnemyUnitsFilter = default;
        readonly EcsFilterInject<Inc<EncounterComponent, DisabledEncounterTag>, Exc<EnemyUnitComponent>> _disabledEncounetFilter = default;
        readonly EcsFilterInject<Inc<EncounterComponent>, Exc<EnemyUnitComponent, DisabledEncounterTag>> _enabledEncounetFilter = default;

        readonly EcsPoolInject<EncounterComponent> _encounterPool = default;
        readonly EcsPoolInject<DisabledEncounterTag> _disabledEncounterPool = default;
        readonly EcsPoolInject<ViewComponent> _viewPool = default;
        readonly EcsPoolInject<OutOfRangeComponent> _outOfRangePool = default;
        readonly EcsPoolInject<InterfaceComponent> _interface = default;
        readonly EcsPoolInject<TutorialComponent> _tutorialPool = default;

        private int _enabledEncounterNumber = int.MinValue;
        private int _nextEncounterNumber = int.MinValue;
        private int _enabledEncounterEntity = int.MinValue;
        private bool firstEncounter = true;
        private int encounter = 0;

        public void Run (EcsSystems systems)
        {
            ref var intComp = ref _interface.Value.Get(_state.Value.EntityInterface);
            if (_enabledEnemyUnitsFilter.Value.GetEntitiesCount() > 0)
            {
                //todo включаю ульту
                
                if(_outOfRangePool.Value.Has(_state.Value.EntityMeleeUltimate))
                {
                    if (firstEncounter)
                    {
                        var isMelee = _state.Value.UnitsInArrayMelee();
                        var isRange = _state.Value.UnitsInArrayRange();
                        intComp.CanvasController.TurnOffUltimateMelee(isMelee);
                        intComp.CanvasController.TurnOffUltimateRange(isRange);
                        firstEncounter = false;
                    }
                    
                    _outOfRangePool.Value.Del(_state.Value.EntityMeleeUltimate);
                    _outOfRangePool.Value.Del(_state.Value.EntityRangeUltimate);
                    intComp.CanvasController.EncounterCheck(true);
                    var isMelee2 = _state.Value.UnitsInArrayMelee();
                    var isRange2 = _state.Value.UnitsInArrayRange();
                    intComp.CanvasController.TurnOffUltimateMelee(isMelee2);
                    intComp.CanvasController.TurnOffUltimateRange(isRange2);

                    if(_state.Value.Saves.TutorialState == 1)
                    {
                        ref var tutorComp = ref _tutorialPool.Value.Get(_state.Value.EntityInterface);
                        if(tutorComp.TutorialState == TutorialComponent.TutorialStates.MinusFour)
                        {
                            tutorComp.TutorialState = TutorialComponent.TutorialStates.Four;
                        }
                    }
                }
                return;
            }
            //todo выключаю ульту
            if(!_outOfRangePool.Value.Has(_state.Value.EntityMeleeUltimate))
            {
                intComp.CanvasController.ChangePointSprite(encounter);
                encounter++;
                _outOfRangePool.Value.Add(_state.Value.EntityMeleeUltimate);
                _outOfRangePool.Value.Add(_state.Value.EntityRangeUltimate);
                intComp.CanvasController.EncounterCheck(false);
                intComp.CanvasController.TurnOffUltimateRange(false);
                intComp.CanvasController.TurnOffUltimateMelee(false);
            }

            if (_friendlyUnitsInFightFilter.Value.GetEntitiesCount() == 0)
            {
                return;
            }

            if (_disabledEnemyUnitsFilter.Value.GetEntitiesCount() == 0)
            {
                return;
            }
            
            foreach (var encounter in _enabledEncounetFilter.Value)
            {
                ref var encounterComponent = ref _enabledEncounetFilter.Pools.Inc1.Get(encounter);
                if (encounterComponent.Number > _enabledEncounterNumber)
                {
                    _enabledEncounterNumber = encounterComponent.Number;
                    _nextEncounterNumber = encounterComponent.Number + 1;
                    _enabledEncounterEntity = encounter;
                }
            }

            ref var enabledEncounterComponent = ref _encounterPool.Value.Get(_enabledEncounterEntity);
            foreach (var enemyUnits in enabledEncounterComponent.EnemyUnitsEntitys)
            {
                _disabledEncounterPool.Value.Del(enemyUnits);
            }

            foreach (var encounter in _disabledEncounetFilter.Value)
            {
                ref var encounterComponent = ref _enabledEncounetFilter.Pools.Inc1.Get(encounter);
                if (encounterComponent.Number == _nextEncounterNumber)
                {
                    ref var viewComponent = ref _viewPool.Value.Get(encounter);
                    _disabledEncounterPool.Value.Del(encounter);
                }
            }
        }
    }
}