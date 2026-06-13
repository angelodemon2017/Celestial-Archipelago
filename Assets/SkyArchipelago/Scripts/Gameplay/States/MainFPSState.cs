using UnityEngine;
using Zenject;

public class MainFPSState : StateWithWindow<GameplayControllerView>
{
    private readonly DiContainer _container;
    private readonly SignalBus _signalBus;
    private readonly PlayerConfig _playerConfig;

    private readonly PointsRepository _pointsRepository;
    private readonly GameplayStateService _gameplayStateService;
    private readonly RaycastService _raycastService;
    private readonly CameraService _cameraService;
    private readonly PlayerInteractionService _playerInteractionService;

    private PlayerController _localPlayerController;

    private float _xRotation = 0f;

    public override bool CursorIsAvailable => false;
    public override string GetHint => _playerInteractionService.GetHint;

    public MainFPSState(
        DiContainer container,
        SignalBus signalBus,
        PlayerConfig playerConfig,
        UIViewCoordinator uIViewCoordinator,
        PointsRepository pointsRepository,
        GameplayStateService gameplayStateService,
        RaycastService raycastService,
        CameraService cameraService,
        PlayerInteractionService playerInteractionService) :
        base (uIViewCoordinator)
    {
        _container = container;
        _signalBus = signalBus;
        _playerConfig = playerConfig;
        _pointsRepository = pointsRepository;
        _gameplayStateService = gameplayStateService;
        _raycastService = raycastService;
        _cameraService = cameraService;
        _playerInteractionService = playerInteractionService;
    }

    private void StartCharacterPlay()
    {
        CheckPlayerController();
        SetPlayerController(_localPlayerController);
    }

    private void SetPlayerController(PlayerController playerController)
    {
        _localPlayerController = playerController;
        _cameraService.AttachTo(_localPlayerController.transform, Vector3.up);
        InputProviderUpdated?.Invoke();
    }

    private void CheckPlayerController()
    {
        if (_localPlayerController != null)
            return;

        var spawnPoint = _pointsRepository.GetPoint();

        _localPlayerController = GameObject.Instantiate(_playerConfig.PlayerControllerPrefab,
            spawnPoint, Quaternion.identity, null);

        _container.Inject(_localPlayerController);
    }

    public override void StateOn()
    {
        base.StateOn();
        _playerInteractionService.HintUpdated += HintUpdate;
        _raycastService.SetCurrentLayerMask(_playerConfig.FPSLayer);

        StartCharacterPlay();
    }

    public override void StateOff()
    {
        _localPlayerController.ProcessMovement(Vector2.zero);
        _playerInteractionService.HintUpdated -= HintUpdate;
    }

    public override void ProcessMovement(Vector2 moveInput)
    {
        _localPlayerController.ProcessMovement(moveInput);
    }

    public override void ProcessLook(Vector2 lookInput)
    {
        if (!_localPlayerController)
            return;

        float mouseX = lookInput.x * _playerConfig.mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * _playerConfig.mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        _cameraService.ProcessLookVertical(mouseY);

        _localPlayerController.transform.Rotate(Vector3.up * mouseX);
    }

    public override void ProcessJump(bool jumpPressed)
    {
        if (jumpPressed)
            _localPlayerController.ProcessJump();
    }

    public override void ProcessInteract(bool interacted)
    {
        if (interacted)
            _playerInteractionService.TryInteract();
    }

    public override void ProcessTab(bool interact)
    {
        if(interact)
            _gameplayStateService.SetState<ManagerMenuState>();
    }
}