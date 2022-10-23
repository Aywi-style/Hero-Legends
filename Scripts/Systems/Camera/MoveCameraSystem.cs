using Leopotam.EcsLite;
using UnityEngine;
using Leopotam.EcsLite.Di;

namespace Client {
    sealed class MoveCameraSystem : IEcsRunSystem {
        readonly EcsSharedInject<GameState> _state = default;
        readonly EcsFilterInject<Inc<MoveCameraComponent>> _filter = default;
        readonly EcsPoolInject<CameraComponent> _cameraPool = default;
        readonly EcsPoolInject<MoveCameraComponent> _movePool = default;
        readonly EcsPoolInject<TeleportBoardEvent> _teleportBoardPool = default;

        public void Run (EcsSystems systems) {
            
            foreach(var entity in _filter.Value)
            {
                ref var cameraComp = ref _cameraPool.Value.Get(entity);
                ref var moveComp = ref _filter.Pools.Inc1.Get(entity);

                var positionZ = cameraComp.CameraHolderTransform.position.z;
                positionZ += Time.deltaTime * 10;

                cameraComp.CameraHolderTransform.position = new Vector3(
                    cameraComp.CameraHolderTransform.position.x,
                    cameraComp.CameraHolderTransform.position.y,
                    positionZ
                );

                if(positionZ >= moveComp.PositionZ)
                {
                    positionZ = moveComp.PositionZ;
                    cameraComp.CameraHolderTransform.position = new Vector3(
                    cameraComp.CameraHolderTransform.position.x,
                    cameraComp.CameraHolderTransform.position.y,
                    positionZ
                    );

                    ref var teleportBoardComp = ref _teleportBoardPool.Value.Add(_state.Value.EntityBoard);
                    teleportBoardComp.Position = new Vector3(0, 0, positionZ - 13);

                    _movePool.Value.Del(entity);

                }
            }
        }
    }
}