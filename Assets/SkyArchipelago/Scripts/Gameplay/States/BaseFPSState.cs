using UnityEngine;
using Zenject;

public abstract class BaseFPSState<T> : StateWithWindow<T>
    where T : UIWindowBase
{
    protected readonly DiContainer _container;
    protected readonly PlayerConfig _playerConfig;

    private readonly PointsRepository _pointsRepository;
    private readonly RaycastService _raycastService;
    private readonly CameraService _cameraService;
    protected readonly PlayerInteractionService _playerInteractionService;

    private FPSCommonModel _fPSCommonModel;

    public override string GetHint => _playerInteractionService.GetHint;

    protected abstract LayerMask RaycastLayer { get; }

    protected BaseFPSState(
        DiContainer container,
        PlayerConfig playerConfig,
        FPSCommonModel fPSCommonModel,
        PointsRepository pointsRepository,
        RaycastService raycastService,
        CameraService cameraService,
        PlayerInteractionService playerInteractionService,
        UIViewCoordinator uIViewCoordinator)
        : base(uIViewCoordinator)
    {
        _container = container;
        _playerConfig = playerConfig;
        _fPSCommonModel = fPSCommonModel;
        _pointsRepository = pointsRepository;
        _raycastService = raycastService;
        _cameraService = cameraService;
        _playerInteractionService = playerInteractionService;
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
        SetPlayerController(_fPSCommonModel.LocalPlayerController);
    }

    private void SetPlayerController(PlayerController playerController)
    {
        _fPSCommonModel.LocalPlayerController = playerController;
        _cameraService.AttachTo(_fPSCommonModel.LocalPlayerController.transform, Vector3.up);
        InputProviderUpdated?.Invoke();
    }

    private void CheckPlayerController()
    {
        if (_fPSCommonModel.LocalPlayerController != null)
            return;

        var spawnPoint = _pointsRepository.GetPoint();

        _fPSCommonModel.LocalPlayerController = GameObject.Instantiate(_playerConfig.PlayerControllerPrefab,
            spawnPoint, Quaternion.identity, null);

        _container.Inject(_fPSCommonModel.LocalPlayerController);
    }

    public override void StateOff()
    {
        _fPSCommonModel.LocalPlayerController.ProcessMovement(Vector2.zero);
        _playerInteractionService.HintUpdated -= HintUpdate;
    }

    public override void ProcessMovement(Vector2 moveInput)
    {
        _fPSCommonModel.LocalPlayerController.ProcessMovement(moveInput);
    }

    public override void ProcessLook(Vector2 lookInput)
    {
        if (!_fPSCommonModel.LocalPlayerController)
            return;

        float mouseX = lookInput.x * _playerConfig.mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * _playerConfig.mouseSensitivity * Time.deltaTime;

        _fPSCommonModel.XRotation -= mouseY;
        _fPSCommonModel.XRotation = Mathf.Clamp(_fPSCommonModel.XRotation, -90f, 90f);

        _cameraService.ProcessLookVertical(mouseY);

        _fPSCommonModel.LocalPlayerController.transform.Rotate(Vector3.up * mouseX);
    }

    public override void ProcessJump(bool jumpPressed)
    {
        if (jumpPressed)
            _fPSCommonModel.LocalPlayerController.ProcessJump();
    }
}