using System.Collections.Generic;
using UnityEngine;

public class FogOfWarManager : MonoBehaviour
{
    public GameObject fogQuad;
    public float defaultRevealRadius = 15f;

    private List<GameObject> playerUnits;
    private List<GameObject> playerBuildings;
    private Vector4[] unitPositions;
    private float[] revealRadiuses;

    void Start()
    {
        int maxObjects = 350;
        unitPositions = new Vector4[maxObjects];
        revealRadiuses = new float[maxObjects];
        fogQuad.SetActive(true);
    }

    void Update()
    {
        Material fogMaterial = fogQuad.GetComponent<Renderer>().sharedMaterial;

        playerUnits = SelectionManager.Instance.playerUnits;
        playerBuildings = SelectionManager.Instance.playerBuildings;

        if ((playerUnits != null && playerUnits.Count > 0) || (playerBuildings != null && playerBuildings.Count > 0))
        {
            List<GameObject> visionObjects = new List<GameObject>();
            if (playerUnits != null)
                visionObjects.AddRange(playerUnits);
            if (playerBuildings != null)
                visionObjects.AddRange(playerBuildings);

            int objectCount = Mathf.Min(visionObjects.Count, unitPositions.Length);

            for (int i = 0; i < objectCount; i++)
            {
                GameObject obj = visionObjects[i];

                Vector3 pos = obj.transform.position;
                unitPositions[i] = new Vector4(pos.x, pos.y, pos.z, 1);

                float revealRadius = defaultRevealRadius;
                revealRadiuses[i] = revealRadius;
            }

            fogMaterial.SetInt("_UnitCount", objectCount);
            fogMaterial.SetVectorArray("_UnitPositions", unitPositions);
            fogMaterial.SetFloatArray("_RevealRadiuses", revealRadiuses);

            if (!fogQuad.activeSelf)
                fogQuad.SetActive(true);
        }
        else
        {
            if (fogQuad.activeSelf)
                fogQuad.SetActive(false);
        }
    }
}
