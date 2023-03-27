using UnityEngine;

//Based upon Natty Creations "Dynamic Footsteps!: For Terrains and Imported Meshes (Unity3D)", modified by Jan Dzyr for Midnight Harvest and use of FMOD

public class TerrainTextureFinder : MonoBehaviour
{
    private Vector3 positionToCheck;
    private string fmod_surfaceName;

    private Terrain t_Terrain;
    private string t_SurfaceLayer;
    private float[] cellTextureMix;

    public string CheckLayers(Vector3 newPosition)
    {
        RaycastHit hit;
        positionToCheck = newPosition;

        if (Physics.Raycast(positionToCheck, Vector3.down, out hit, 3))
        {
            if (hit.transform.GetComponent<Terrain>() != null)              //For walking on regular TERRAIN
            {
                t_Terrain = hit.transform.GetComponent<Terrain>();

                if (t_SurfaceLayer != GetLayerName())
                {
                    t_SurfaceLayer = GetLayerName();

                    ChangeFootstepSurface();
                    return fmod_surfaceName;
                }
            }
            if (hit.transform.GetComponent<SurfaceType>() != null)          //For walking on other OBJECTS (leaders, stairs, crates etc) that would have speciffied surface type. This checks for the SurfaceType script containing string "soundMaterial" for what I used Scriptable Object
            {
                return hit.transform.GetComponent<SurfaceType>().surfaceType.soundMaterial;
            }
        }

        ChangeFootstepSurface();                                            //For situation when script for some reason won't find any terrain or texture, it will use last terrain surface or the default one
        return fmod_surfaceName;
    }

    private string GetLayerName()
    {
        int t_layerIndex = 0;
        float strongestTexture = 0;

        Vector3 t_Pos = t_Terrain.transform.position;
        TerrainData t_Data = t_Terrain.terrainData;

        int mapX = Mathf.RoundToInt((positionToCheck.x - t_Pos.x) / t_Data.size.x * t_Data.alphamapWidth);
        int mapZ = Mathf.RoundToInt((positionToCheck.z - t_Pos.z) / t_Data.size.z * t_Data.alphamapHeight);

        float[,,] splatMapData = t_Data.GetAlphamaps(mapX, mapZ, 1, 1);
        cellTextureMix = new float[splatMapData.GetUpperBound(2) + 1];

        for (int i = 0; i < cellTextureMix.Length; i++)
        {
            cellTextureMix[i] = splatMapData[0, 0, i];

            if (cellTextureMix[i] > strongestTexture)
            {
                t_layerIndex = i;
                strongestTexture = cellTextureMix[i];
            }
        }

        return t_Terrain.terrainData.terrainLayers[t_layerIndex].name;
    }

    private void ChangeFootstepSurface()    //This function will 'translate' the names of textures used by graphic team, into names we actually will use further as parameter names or values depending on need
    {
        //Debug.Log(t_SurfaceLayer);

        switch (t_SurfaceLayer)             //Cases depends on the names you use for textures and parameters
        {
            case "T_Dirt":  { fmod_surfaceName = "Dirt";    break; }
            case "T_Grass": { fmod_surfaceName = "Grass";   break; }
            case "T_Sand":  { fmod_surfaceName = "Dirt";    break; }
            default:        { fmod_surfaceName = "Default"; break; }
        }
    }
}