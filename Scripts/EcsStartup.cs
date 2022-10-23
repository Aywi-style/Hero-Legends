using Leopotam.EcsLite;
using Leopotam.EcsLite.ExtendedSystems;
using UnityEngine;
using UnityEngine.SceneManagement;
using Leopotam.EcsLite.Di;
using System.Text.RegularExpressions;
using Advertising;


namespace Client
{
    sealed class EcsStartup : MonoBehaviour
    {
        EcsSystems _commonSystems, _beforeSystems, _playSystems, _delhereSystems;
        EcsWorld _world = null;
        GameState _state = null;
        [SerializeField] private BoardConfig _boardConfig;
        [SerializeField] private UnitStorage _unitStorage;
        [SerializeField] private UltimateStorage _ultimateStorage;
        [SerializeField] private InterfaceConfig _interfaceConfig;
        [SerializeField] private SoundsConfig _soundsConfig;
        [SerializeField] private EnemyUnitStorage _enemyUnitStorage;
        [SerializeField] private EffectsStorage _effectsStorage;
        [SerializeField] private LevelConfig _levelConfig;
        [SerializeField] private RewardStorage _rewardStorage;
        public string[] units;
        public string[] originalUnits;

        void Start()
        {
            _world = new EcsWorld();
            IAdvertising ads = new DeputyAdvertising();

            _state = new GameState(_world, _boardConfig, _unitStorage, _ultimateStorage, 
                _interfaceConfig, _enemyUnitStorage, _soundsConfig, _effectsStorage, _levelConfig, ads, _rewardStorage);

            _commonSystems = new EcsSystems(_world, _state);
            _beforeSystems = new EcsSystems(_world, _state);
            _playSystems = new EcsSystems(_world, _state);
            _delhereSystems = new EcsSystems(_world, _state);

            _commonSystems
            //.Add(new InitBoard())
            .Add(new InitInput())
            .Add(new InitLevel())
            .Add(new InitInterface())
            .Add(new InitBoard())
            .Add(new InitEnemyUnit())
            .Add(new InitEncounters())
            .Add(new InitUltimateSystem())
            .Add(new InitCamera())
            .Add(new InitSounds())
            .Add(new InitLevelRaward())
            .Add(new InitHand())
            .Add(new MoneyInit())
            /*.Add(new InitDeletableSetting())*/

            .Add(new EnableBeforeStartLevel())
            .Add(new EnablePlayLevel())
            .Add(new FillingUnitComponentSystem())
            .Add(new ClearComponentSystem())
            .Add(new VibrationSystem())
            
            .Add(new TutorialSystem())
            .Add(new HandMoveSystem())
            .Add(new OpenSundukSystem())

            .Add(new PlayInterstitialSystem())
            .Add(new AdOfferSystem())
            .Add(new MultiplyCoinsByADS())
            .Add(new AddKeysByADS())
            .Add(new AddHeroByADS())
            ;

            _beforeSystems
                .Add(new InputSystem())
                .Add(new GetUnitFromBoard())
                .Add(new DragUnitSystem())
                .Add(new CheckDroppedUnit())
                .Add(new CreateNextUnitSystem())
                .Add(new RelocateUnit())
                .Add(new UnitSpawnSystem()) //в новую группу
                .Add(new CameraBezierSystem())

                .Add(new LevelingSystem())

            ;

            _playSystems
                .Add(new WinCheckSystem())
                .Add(new LoseCheckSystem())
                .Add(new WinSystem())
                .Add(new LoseSystem())
                .Add(new WinRotationSystem())

                .Add(new UnitSpawnInFightSystem())
                .Add(new CenterFightPointSystem())
                .Add(new SetNullParenSystem())
                
                .Add(new CameraControllSystem())
                .Add(new MoveBoardSystem())

                .Add(new TargetingSystem())
                .Add(new EnemyTargetingSystem())
                .Add(new DistanceToTargetSystem())
                .Add(new MoveToTargetSystem())
                .Add(new JounFightSystem())
                .Add(new RangeDistanceBuffSystem())

                .Add(new MeleeAttackSystem())
                .Add(new MeleeDamageSystem())

                .Add(new RangeAttackSystem())
                .Add(new RangeDamageSystem())

                .Add(new RayAttackSystem())
                //.Add(new RayDamageSystem())

                .Add(new EncounterSwitcher())
                
                .Add(new CooldownSystem())
                /*.Add(new UnitAfterDyingSystem())*/

                .Add(new ArrowWorkingSystem())

                .Add(new MeleeUltimateSystem())
                .Add(new MeleeUltimateEffect())
                .Add(new RangeUltimateSystem())

                .Add(new TakeDamageSystem())
                .Add(new HitSoundsSystem())
                
                .Add(new DeathCheckSystem())
                .Add(new SpawnKeySystem())
                .Add(new MoveKeySystem())
                .Add(new AddCoinsSystem())
                .Add(new AddedCoinMoveSystem())


                .Add(new CooldownUltimateRangeSystem())
                .Add(new CooldownUltimateMeleeSystem())
                /*.Add(new DeleteSettingSystem())*/
                .Add(new RemoveVFXSystem())
                .Add(new HealthbarToCameraSystem())
                .Add(new RotateSpinSystem())
                //.Add(new DeleteSettingSystem())

            ;

            _delhereSystems
            // .DelHere<TouchEvent>()
            .DelHere<DroppedUnitEvent>()
            .DelHere<CreateNextUnitEvent>()
            .DelHere<WinEvent>()
            .DelHere<LoseEvent>()
            .DelHere<RelocateUnitEvent>()
            .DelHere<UnitsSpawnEventComponent>()
            .DelHere<UltimateEvent>()
            .DelHere<CameraBezierComponent>()
            //.DelHere<TeleportBoardEvent>()
            .DelHere<NullParentEvent>()
            .DelHere<RangeAttackEvent>()
            .DelHere<MeleeAttackEvent>()
            .DelHere<TakeDamageEvent>()
            .DelHere<CoinsChangerEvent>()
            .DelHere<OpenSundukEvent>()
            //.DelHere<LevelingEvent>()
            /*.DelHere<EnableBeforeStartLevelEvent>()
            .DelHere<EnablePlayLevelEvent>()*/
            ;

#if UNITY_EDITOR
            _commonSystems.Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem());
#endif

            _commonSystems.Inject();
            _beforeSystems.Inject();
            _playSystems.Inject();
            _delhereSystems.Inject();

            _commonSystems.Init();
            _beforeSystems.Init();
            _playSystems.Init();
            _delhereSystems.Init();
        }

        void Update()
        {
            units = _state.SavedPlayerUnits;
            originalUnits = _state.PlayerUnits;
            _commonSystems?.Run();
            if (_state.BeforeGroup) _beforeSystems?.Run();
            if (_state.PlayGroup) _playSystems?.Run();
            _delhereSystems?.Run();

        }

        void OnDestroy()
        {
            _commonSystems.Destroy();
            _beforeSystems.Destroy();
            _playSystems.Destroy();
            _delhereSystems.Destroy();

            _world.Destroy();
            _commonSystems = null;
            _beforeSystems = null;
            _playSystems = null;
            _delhereSystems = null;
        }
    }
}
