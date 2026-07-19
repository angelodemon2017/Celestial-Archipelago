using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class HitSource : MonoBehaviour, IPoolable<HitSourceInitModel>
{
    [SerializeField] private List<Collider> colliders;

    private SignalBus _signalBus;
    private HitsCoordinatorService _hitsCoordinatorService;
    private UIMBFactory<HitSourceInitModel, HitSource> _hitSourceFactory;

    private bool _inited = false;
    private EntityModel _entityOwner;
    private ItemModel _itemReason;
    private HashSet<GameObject> _hits = new();
    private HashSet<EntityModel> _wasHits = new();
    private Coroutine _corOfDestroyer;

    private void OnValidate()
    {
        gameObject.layer = 8;
        colliders.Clear();
        var cols = GetComponents<Collider>();
        foreach (var col in cols)
        {
            col.isTrigger = true;
            colliders.Add(col);
        }
    }

    [Inject]
    public void Construct(
        SignalBus signalBus,
        HitsCoordinatorService hitsCoordinatorService,
        UIMBFactory<HitSourceInitModel, HitSource> hitSourceFactory)
    {
        _signalBus = signalBus;
        _hitsCoordinatorService = hitsCoordinatorService;
        _hitSourceFactory = hitSourceFactory;
    }

    public void OnSpawned(HitSourceInitModel p1)
    {
        Init(p1.Owner, p1.ItemOfOwner);
    }

    public void Init(EntityModel owner, ItemModel itemOfOwner)
    {
        _entityOwner = owner;
        _itemReason = itemOfOwner;
        _inited = true;
        _corOfDestroyer = StartCoroutine(WaitAndCallRout(1f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_inited)
            return;

        if (_hits.Add(other.gameObject))
        {
            if (_hitsCoordinatorService.TryGetEntityByGOOfTrigger(other.gameObject, out var targetEntity) &&
                _wasHits.Add(targetEntity))
            {
                _signalBus.Fire(new InteractContext(
                    _entityOwner,
                    _itemReason,
                    EModeInteract.LCM,
                    targetEntity));
            }
        }
    }

    private IEnumerator WaitAndCallRout(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _hitSourceFactory.Despawn(this);
    }

    public void OnDespawned()
    {
        Deativate();
    }

    private void Deativate()
    {
        StopCoroutine(_corOfDestroyer);
        _corOfDestroyer = null;
        _hits.Clear();
        _wasHits.Clear();
        _inited = false;
    }
}

public struct HitSourceInitModel
{
    public EntityModel Owner;
    public ItemModel ItemOfOwner;

    public HitSourceInitModel(EntityModel owner, ItemModel itemOfOwner)
    {
        Owner = owner;
        ItemOfOwner = itemOfOwner;
    }
}