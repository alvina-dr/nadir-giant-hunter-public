using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meshtrail : MonoBehaviour
{
    public float activeTime = 2f;
    [Header("Mesh Related")]
    public float meshRefreshRate = 0.1f;
    public Transform positionToSpawn;
    public float meshDestroyDelay;
    private bool isTrailActive;

    [Header("Shader Related")]
    public SkinnedMeshRenderer[] skinnedMeshRenderers;
    public List<Material> materials;

    public void ShowTrail()
    {
        if (!isTrailActive)
        {
            StartCoroutine(ActivateTrail(activeTime));
        }
    }
    IEnumerator ActivateTrail (float timeActive)
    {
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate;

            if (skinnedMeshRenderers == null)
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            for (int i = 0; i < skinnedMeshRenderers.Length; i++)
            {
                GameObject gObj = new GameObject();
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();

                mr.SetMaterials(materials);

                MeshFilter mf = gObj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);

                mf.mesh = mesh;

                Destroy(gObj, meshDestroyDelay);   
            }
            yield return new WaitForSeconds (meshRefreshRate); 
        }
        isTrailActive = false;
    }
}