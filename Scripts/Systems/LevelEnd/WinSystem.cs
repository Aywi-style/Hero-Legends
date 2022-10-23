using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client {
    sealed class WinSystem : IEcsRunSystem {
        readonly EcsWorldInject _world = default;
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<WinEvent>> _winFilter = default;
        readonly EcsFilterInject<Inc<ViewComponent, FrendlyUnitComponent, ClosestTargetComponent>, Exc<EmptyEntityAfterDeadComponent>> _friendlyUnitsFilter = default;
        readonly EcsPoolInject<InterfaceComponent> _interface = default;
        readonly EcsPoolInject<VibrationEvent> _vibrationPool = default;
        readonly EcsPoolInject<TutorialComponent> _tutorialPool = default;
        readonly EcsPoolInject<LevelRawardComponent> _rewardedPool = default;
        private bool _oneTime = false;
        public void Run (EcsSystems systems) {
            foreach (var entity in _winFilter.Value)
            {
                if (!_oneTime)
                {
                    ref var intComp = ref _interface.Value.Get(_state.Value.EntityInterface);
                    //intComp.CanvasController.WinEvent(true);
                    intComp.CanvasController.StartWait(2.4f,true, true);
                    intComp.CanvasController.BeforeStartPanel(false);
                    intComp.CanvasController.GamePanel(false);

                    ref var rewardComp = ref _rewardedPool.Value.Get(_state.Value.EntityLevelReward);
                    intComp.CanvasController.ChangeRewardedText(rewardComp.Value);

                    //_state.Value.Coins += rewardComp.Value;

                    if (_state.Value.Saves.TutorialState == 1)
                    {
                        ref var tutorialComp = ref _tutorialPool.Value.Get(_state.Value.EntityInterface);
                        tutorialComp.TutorialState = TutorialComponent.TutorialStates.Exit;
                    }
                    ref var vibrationComp = ref _vibrationPool.Value.Add(_world.Value.NewEntity());
                    vibrationComp.Vibration = VibrationEvent.VibrationType.Success;

                    foreach (var unitEntity in _friendlyUnitsFilter.Value)
                    {
                        ref var viewComponent = ref _friendlyUnitsFilter.Pools.Inc1.Get(unitEntity);
                        if (viewComponent.Animator)
                        {
                            viewComponent.Animator.SetBool("Win", true);
                        }
                    }
                    _state.Value.Saves.SavePlayerUnits(_state.Value.SavedPlayerUnits);
                    //_state.Value.Saves.SaveCoin(_state.Value.Coins);
                    _oneTime = true;

                }
            }
        }
    }
}