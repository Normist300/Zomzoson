using Barmetler.RoadSystem;
using UnityEngine;

public class GenerateRoadMesh : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Берём все компоненты RoadMeshGenerator на ячейке и генерируем меш
        RoadMeshGenerator[] roadMeshGenerators = gameObject.GetComponentsInChildren<RoadMeshGenerator>();
        foreach (var roadMeshGenerator in roadMeshGenerators)
        {
            roadMeshGenerator.GenerateRoadMesh();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
