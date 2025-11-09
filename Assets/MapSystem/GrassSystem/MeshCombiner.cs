using UnityEngine;

public static class MeshCombiner
{
    public static Mesh CombineAllMeshes(GameObject fbxRoot)
    {
        MeshFilter[] filters = fbxRoot.GetComponentsInChildren<MeshFilter>();

        CombineInstance[] combine = new CombineInstance[filters.Length];

        for (int i = 0; i < filters.Length; i++)
        {
            combine[i].mesh = filters[i].sharedMesh;
            combine[i].transform = filters[i].transform.localToWorldMatrix;
        }

        Mesh combined = new Mesh();
        combined.name = fbxRoot.name + "_Combined";
        combined.CombineMeshes(combine, true, true);

        return combined;
    }
}
