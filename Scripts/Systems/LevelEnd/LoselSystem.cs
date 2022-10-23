using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client {
    sealed class LoseSystem : IEcsRunSystem 
    {
        readonly EcsWorldInject _world = default;
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<LoseEvent>> _filter = default;
        readonly EcsFilterInject<Inc<ViewComponent, EnemyUnitComponent, ClosestTargetComponent>, Exc<EmptyEntityAfterDeadComponent>> _enemyUnitsFilter = default;
        readonly EcsPoolInject<InterfaceComponent> _interface = default;
        readonly EcsPoolInject<VibrationEvent> _vibrationPool = default;
        readonly EcsPoolInject<TutorialComponent> _tutorialPool = default;
        private bool _oneTime = false;

        public void Run (EcsSystems systems) {
            foreach (var entity in _filter.Value)
            {
                if (!_oneTime)
                {
                    ref var intComp = ref _interface.Value.Get(_state.Value.EntityInterface);
                    intComp.CanvasController.StartWait(2.4f,true, false);
                    intComp.CanvasController.BeforeStartPanel(false);
                    intComp.CanvasController.GamePanel(false);
                    if (_state.Value.Saves.TutorialState == 1)
                    {
                        ref var tutorialComp = ref _tutorialPool.Value.Get(_state.Value.EntityInterface);
                        tutorialComp.TutorialState = TutorialComponent.TutorialStates.Exit;
                    }
                    ref var vibrationComp = ref _vibrationPool.Value.Add(_world.Value.NewEntity());
                    vibrationComp.Vibration = VibrationEvent.VibrationType.Success;

                    foreach (var unitEntity in _enemyUnitsFilter.Value)
                    {
                        ref var viewComponent = ref _enemyUnitsFilter.Pools.Inc1.Get(unitEntity);
                        if (viewComponent.Animator)
                        {
                            viewComponent.Animator.SetBool("Win", true);
                        }
                    }

                    _state.Value.Saves.SavePlayerUnits(_state.Value.SavedPlayerUnits);
                    _state.Value.Saves.SaveCoin(_state.Value.Coins);
                    
                    _oneTime = true;
                }

            }
        }
    }
}