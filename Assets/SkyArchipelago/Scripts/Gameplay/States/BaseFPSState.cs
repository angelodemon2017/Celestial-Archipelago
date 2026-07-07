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

    private EntityViewMB LocalPlayerView;
    private FPSCommonModel _fPSCommonModel;

    public override string GetHint => _playerInteractionService.GetHint;

    protected abstract LayerMask RaycastLayer { get; }

    protected BaseFPSState(
        DiContainer container,
        PlayerConfig playerConfig,
        FPSCommonModel fPSCommonModel,
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
        _cameraService.AttachTo(LocalPlayerView.transform, Vector3.up);
        InputProviderUpdated?.Invoke();
    }

    private void CheckPlayerController()
    {
        if (LocalPlayerView != null)
            return;

        var players = _entityRuntimeService.GetModelsByType<PlayerModel>();
        foreach (var p in players)
        {
            if (p.PlayerId == _fPSCommonModel.PlayerName)
            {
                _fPSCommonModel.SetPlModel(p);
                return;
            }
        }

        var sps = _entityRuntimeService.GetModelsByType<SpawnPointModel>();
        var spawnPoint = sps[0].SpawnPoint;
        var playerData = new PlayerData(_fPSCommonModel.PlayerName);
        playerData.position = spawnPoint;
        playerData.rotation = sps[0].Rotation;
        _fPSCommonModel.SetPlModel((PlayerModel)playerData.CreateModel());
        _entityRuntimeService.AddModel(_fPSCommonModel.LocalPlayerModel);

        LocalPlayerView = _entityViewsFactory.Spawn(_fPSCommonModel.LocalPlayerModel);
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
        Vector3 moveDirection = LocalPlayerView.transform.right * _fPSCommonModel.CurrentMoveInput.x +
            LocalPlayerView.transform.forward * _fPSCommonModel.CurrentMoveInput.y;
        Vector3 targetVelocity = moveDirection.normalized * _fPSCommonModel.LocalPlayerModel.MoveSpeed;
        targetVelocity.y = LocalPlayerView.RB.linearVelocity.y;

        LocalPlayerView.RB.linearVelocity = targetVelocity;
    }

    public override void ProcessLook(Vector2 lookInput)
    {
        if (!LocalPlayerView)
            return;

        float mouseX = lookInput.x * _playerConfig.mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * _playerConfig.mouseSensitivity * Time.deltaTime;

        _fPSCommonModel.XRotation -= mouseY;
        _fPSCommonModel.XRotation = Mathf.Clamp(_fPSCommonModel.XRotation, -90f, 90f);

        _cameraService.ProcessLookVertical(mouseY);

        LocalPlayerView.transform.Rotate(Vector3.up * mouseX);
    }

    public override void ProcessJump(bool jumpPressed)
    {
        if (jumpPressed)
        {
            if (_fPSCommonModel.LocalPlayerModel.IsGrounded)
            {
                LocalPlayerView.RB.AddForce(Vector3.up * _fPSCommonModel.JumpForce, ForceMode.Impulse);
            }
        }
    }
}