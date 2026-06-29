using Zenject;
using System;
using UnityEngine;

public abstract class EntityModelProto
{
    public int Id { get; protected set; }
    public Vector3 Position { get; protected set; }
    public Quaternion Rotation { get; protected set; }

    protected readonly SignalBus _signalBus;

    [Inject]
    protected EntityModelProto(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    public virtual void Initialize(EntityData data)
    {
        Id = data.Id;
        Position = data.position;
        Rotation = data.rotation;
    }

    public virtual EntityData ToData()
    {
        var data = CreateDataInstance();
        data.Id = Id;
        data.position = Position;
        data.rotation = Rotation;
        return data;
    }

    protected abstract EntityData CreateDataInstance();

    public event Action<EntityModelProto> OnDestroyed;
    public event Action OnPositionChanged;

    public virtual void SetPosition(Vector3 newPos)
    {
        Position = newPos;
        OnPositionChanged?.Invoke();
    }

    public virtual void Destroy()
    {
        OnDestroyed?.Invoke(this);
    }
}