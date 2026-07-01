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
    private readonly WorldShowerService _worldShowerService;
    protected readonly PlayerInteractionService _playerInteractionService;

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
        WorldShowerService worldShowerService,
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
        _worldShowerService = worldShowerService;
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
        _cameraService.AttachTo(_fPSCommonModel.LocalPlayerView.transform, Vector3.up);
        InputProviderUpdated?.Invoke();
    }

    private void CheckPlayerController()
    {
        if (_fPSCommonModel.LocalPlayerView != null)
            return;

        var players = _entityRuntimeService.GetModelsByType<PlayerModel>();
        foreach (var p in players)
        {
            if (p.PlayerId == _fPSCommonModel.PlayerName)
            {
                _fPSCommonModel.LocalPlayerModel = p;
                return;
            }
        }

        var sps = _entityRuntimeService.GetModelsByType<SpawnPointModel>();
        var spawnPoint = sps[0].SpawnPoint;
        var playerData = new PlayerData(_fPSCommonModel.PlayerName);
        playerData.position = spawnPoint;
        playerData.rotation = sps[0].Rotation;
        _fPSCommonModel.LocalPlayerModel = (PlayerModel)playerData.CreateModel();
        _entityRuntimeService.AddModel(_fPSCommonModel.LocalPlayerModel);

        _fPSCommonModel.LocalPlayerView = _worldShowerService.ShowModel(_fPSCommonModel.LocalPlayerModel);
    }

    public override void StateOff()
    {
        _fPSCommonModel.CurrentMoveInput = Vector2.zero;
        _playerInteractionService.HintUpdated -= HintUpdate;
    }

    public override void ProcessMovement(Vector2 moveInput)
    {
        _fPSCommonModel.CurrentMoveInput = moveInput;
//        _fPSCommonModel.LocalPlayerController.ProcessMovement(moveInput);
    }

    public override void StateFixedRun()
    {
        Vector3 moveDirection = _fPSCommonModel.LocalPlayerView.transform.right * _fPSCommonModel.CurrentMoveInput.x +
            _fPSCommonModel.LocalPlayerView.transform.forward * _fPSCommonModel.CurrentMoveInput.y;
        Vector3 targetVelocity = moveDirection.normalized * _fPSCommonModel.LocalPlayerModel.MoveSpeed;
        targetVelocity.y = _fPSCommonModel.LocalPlayerView.RB.linearVelocity.y;

        _fPSCommonModel.LocalPlayerView.RB.linearVelocity = targetVelocity;
    }

    public override void ProcessLook(Vector2 lookInput)
    {
        if (!_fPSCommonModel.LocalPlayerView)
            return;

        float mouseX = lookInput.x * _playerConfig.mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * _playerConfig.mouseSensitivity * Time.deltaTime;

        _fPSCommonModel.XRotation -= mouseY;
        _fPSCommonModel.XRotation = Mathf.Clamp(_fPSCommonModel.XRotation, -90f, 90f);

        _cameraService.ProcessLookVertical(mouseY);

        _fPSCommonModel.LocalPlayerView.transform.Rotate(Vector3.up * mouseX);
    }

    public override void ProcessJump(bool jumpPressed)
    {
        if (jumpPressed)
        {
            if (_fPSCommonModel.LocalPlayerModel.IsGrounded)
            {
                _fPSCommonModel.LocalPlayerView.RB.AddForce(Vector3.up * _fPSCommonModel.JumpForce, ForceMode.Impulse);
//                _fPSCommonModel.LocalPlayerController.ProcessJump();
            }
        }
    }
}