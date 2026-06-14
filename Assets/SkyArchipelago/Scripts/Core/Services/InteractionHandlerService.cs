using System;
using UnityEngine;
using Zenject;

public class InteractionHandlerService : IInitializable, IDisposable
{
    private readonly PlayerInteractionService _playerInteractionService;
    private readonly GameplayStateService _gameplayStateService;
    private readonly DialogModel _dialogModel;

    [Inject]
    public InteractionHandlerService(
        PlayerInteractionService playerInteractionService,
        GameplayStateService gameplayStateService,
        DialogModel dialogModel)
    {
        _playerInteractionService = playerInteractionService;
        _gameplayStateService = gameplayStateService;
        _dialogModel = dialogModel;
    }

    public void Initialize()
    {
        _playerInteractionService.OnInteracted += Handle;
    }

    public void Handle(InteractionResult result)
    {
        switch (result)
        {
            case PickupResult pickup:
                Debug.Log($"Pickuped:{pickup.Item.name}, q-{pickup.Quantity}");
                GameObject.Destroy(pickup.Go);//TODO return to pool
                break;

            case OpenDialogueResult dialogue:
                _dialogModel.CurrentNpcId = dialogue.NpcId;
                _gameplayStateService.SetState<DialogMenuState>();
                break;

            case OpenUIResult ui:
                break;

            case OpenNoteResult note:
                Debug.Log($"Read note {note.Title}:{note.Text}");
                break;

            default:
                break;
        }
    }

    public void Dispose()
    {
        _playerInteractionService.OnInteracted -= Handle;
    }
}