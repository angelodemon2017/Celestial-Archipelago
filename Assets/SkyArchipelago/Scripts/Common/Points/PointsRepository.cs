using System.Collections.Generic;
using UnityEngine;

public class PointsRepository
{
    private Queue<Vector3> _points = new();

    public PointsRepository()
    {

    }

    public void RegisterPoint(Vector3 newPoint)
    {
        _points.Enqueue(newPoint);
    }

    public Vector3 GetPoint()
    {
        return _points.Dequeue();
    }
}