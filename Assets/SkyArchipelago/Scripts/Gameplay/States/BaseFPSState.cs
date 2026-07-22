using UnityEngine;
using Zenject;

public abstract class BaseFPSState<T> : StateWithWindow<T>
    where T : UIWindowBase
{
    protected readonly DiContainer _container;
    protected readonly PlayerConfig _playerConfig;

    private readonly RaycastService _raycastService;
    private readonly CameraService _cameraService;
    private readonly EntityRuntimeService _entityRuntimeService;
    protected readonly PlayerInteractionService _playerInteractionService;
    private readonly EntityViewsFactory _entityViewsFactory;
    private readonly GameplayLocalFPSModel _gameplayLocalFPSModel;

    private FPSCommonModel _fPSCommonModel;

    public override string GetHint => _playerInteractionService.GetHint;

    protected abstract LayerMask RaycastLayer { get; }

    protected BaseFPSState(
        DiContainer container,
        PlayerConfig playerConfig,
        FPSCommonModel fPSCommonModel,
        GameplayLocalFPSModel gameplayLocalFPSModel,
        RaycastService raycastService,
        CameraService cameraService,
        EntityRuntimeService entityRuntimeService,
        PlayerInteractionService playerInteractionService,
        EntityViewsFactory entityViewsFactory,
        UIViewCoordinator uIViewCoordinator)
        : base(uIViewCoordinator)
    {
        _container = container;
        _playerConfig = playerConfig;
        _fPSCommonModel = fPSCommonModel;
        _gameplayLocalFPSModel = gameplayLocalFPSModel;
        _raycastService = raycastService;
        _cameraService = cameraService;
        _entityRuntimeService = entityRuntimeService;
        _playerInteractionService = playerInteractionService;
        _entityViewsFactory = entityViewsFactory;
    }

    public override void StateOn()
    {
        base.StateOn();
        _playerInteractionService.HintUpdated += HintUpdate;
        _raycastService.SetCurrentLayerMask(RaycastLayer);

        StartCharacterPlay();
    }

    private void StartCharacterPlay()
    {
        CheckPlayerController();
        SetPlayerController();
    }

    private void SetPlayerController()
    {
        _cameraService.AttachTo(_gameplayLocalFPSModel.LocalPlayerView.transform, Vector3.up);
        InputProviderUpdated?.Invoke();
    }

    private void CheckPlayerController()
    {
        if (_gameplayLocalFPSModel.LocalPlayerView != null)
            return;

        var players = _entityRuntimeService.GetModelsByEType(EEntityType.Player);
        foreach (var p in players)
        {
            if (p is PlayerModel pm &&
                pm.PlayerId == _fPSCommonModel.PlayerName)
            {
                _fPSCommonModel.SetPlModel(pm);
                return;
            }
        }

        var sps = _entityRuntimeService.GetModelsByEType(EEntityType.SpawnPoint);
        var spawnPoint = (sps[0] is SpawnPointModel spm) ? spm.SpawnPoint : Vector3.zero;
        var playerData = EntityDataMap.CreateData(EEntityType.Player);
        playerData.InitConfig(_playerConfig.PlayerEntityConfig);
        if (playerData is PlayerData pd)
            pd.PlayerName = _fPSCommonModel.PlayerName;
        playerData.position = spawnPoint;
        playerData.rotation = sps[0].Rotation;
        _fPSCommonModel.SetPlModel((PlayerModel)playerData.CreateModel());
        _entityRuntimeService.AddModel(_fPSCommonModel.LocalPlayerModel);

        _gameplayLocalFPSModel.LocalPlayerView = _entityViewsFactory.Spawn(_fPSCommonModel.LocalPlayerModel);
    }

    public override void StateOff()
    {
        _fPSCommonModel.CurrentMoveInput = Vector2.zero;
        _playerInteractionService.HintUpdated -= HintUpdate;
    }

    public override void ProcessMovement(Vector2 moveInput)
    {
        _fPSCommonModel.CurrentMoveInput = moveInput;
    }

    public override void StateFixedRun()
    {
        Vector3 moveDirection = _gameplayLocalFPSModel.LocalPlayerView.transform.right * _fPSCommonModel.CurrentMoveInput.x +
            _gameplayLocalFPSModel.LocalPlayerView.transform.forward * _fPSCommonModel.CurrentMoveInput.y;
        Vector3 targetVelocity = moveDirection.normalized * _fPSCommonModel.LocalPlayerModel.MoveSpeed;
        targetVelocity.y = _gameplayLocalFPSModel.LocalPlayerView.RB.linearVelocity.y;

        _gameplayLocalFPSModel.LocalPlayerView.RB.linearVelocity = targetVelocity;
    }

    public override void ProcessLook(Vector2 lookInput)
    {
        if (!_gameplayLocalFPSModel.LocalPlayerView)
            return;

        float mouseX = lookInput.x * _playerConfig.mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * _playerConfig.mouseSensitivity * Time.deltaTime;

        _fPSCommonModel.XRotation -= mouseY;
        _fPSCommonModel.XRotation = Mathf.Clamp(_fPSCommonModel.XRotation, -90f, 90f);

        _cameraService.ProcessLookVertical(mouseY);

        _gameplayLocalFPSModel.LocalPlayerView.transform.Rotate(Vector3.up * mouseX);
    }

    public override void ProcessJump(bool jumpPressed)
    {
        if (jumpPressed)
        {
            if (_fPSCommonModel.LocalPlayerModel.IsGrounded)
            {
                _gameplayLocalFPSModel.LocalPlayerView.RB.AddForce(Vector3.up * _fPSCommonModel.JumpForce, ForceMode.Impulse);
            }
        }
    }
}