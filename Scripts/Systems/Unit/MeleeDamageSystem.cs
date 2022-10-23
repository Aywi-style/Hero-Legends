using Leopotam.EcsLite;
using UnityEngine;
using Leopotam.EcsLite.Di;

namespace Client {
    sealed class MeleeDamageSystem : IEcsRunSystem {
        readonly EcsWorldInject _world;
        readonly EcsFilterInject<Inc<MeleeAttackEvent>> _meleeAttackFilter = default;
        readonly EcsPoolInject<TakeDamageEvent> _takeDamagePool = default;

        public void Run (EcsSystems systems) {
            foreach(var entity in _meleeAttackFilter.Value)
            {
                ref var meleeAttackCompomponent = ref _meleeAttackFilter.Pools.Inc1.Get(entity);

                ref var takeDamageComponent = ref _takeDamagePool.Value.Add(_world.Value.NewEntity());
                takeDamageComponent.Damage = meleeAttackCompomponent.Damage;
                takeDamageComponent.Entity = meleeAttackCompomponent.TargetEntity;
                takeDamageComponent.HolderEntity = meleeAttackCompomponent.HolderEntity;
            }
        }
    }
}