using System.Collections.Generic;
using UnityEngine;

public class ProceduralMeshService : MonoBehaviour
{
    private static ProceduralMeshService _instance;
    public static ProceduralMeshService Instance => _instance;

    private Dictionary<System.Type, object> generators = new Dictionary<System.Type, object>();

    private void Awake()
    {
        _instance = this;

        // Можно регистрировать через инспектор или автоматически
    }

    public void RegisterGenerator<TTopology, TShape>(ProceduralPartGenerator<TTopology, TShape> generator)
        where TTopology : BaseMeshTopologySO
        where TShape : BaseModelShape
    {
        var key = typeof(ProceduralPartGenerator<TTopology, TShape>);
        generators[key] = generator;
    }

    public Mesh GetMesh<TTopology, TShape>(TTopology topology, TShape shape)
        where TTopology : BaseMeshTopologySO
        where TShape : BaseModelShape
    {
        var generatorType = typeof(ProceduralPartGenerator<TTopology, TShape>);

        if (generators.TryGetValue(generatorType, out object genObj) &&
            genObj is ProceduralPartGenerator<TTopology, TShape> generator)
        {
            // Здесь можно добавить кэширование по комбинации topology + shape параметров
            Mesh baseMesh = generator.CreateNewMesh(topology);
            Mesh finalMesh = Instantiate(baseMesh);
            generator.ApplyShape(finalMesh, topology, shape);
            return finalMesh;
        }

        Debug.LogError($"No generator registered for {typeof(TTopology)} / {typeof(TShape)}");
        return null;
    }
}