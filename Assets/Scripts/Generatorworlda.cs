using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Generatorworlda : MonoBehaviour
{
    public UIScript uis;
    [SerializeField] private GameObject worldBuild;
    [SerializeField] private GameObject blockBuild;
    [SerializeField] private Terrain terrainPrefab;
    public bool createdWorld=false;
    private Terrain terrain;
    public TerrainController tc;

    public int seed;
    public long[] seeds;
    public long[] modingSeed;

    private int planetCount = 9;
    private int curentPlanet = 4;

    private int radiusChunk = 6;
    private int chunkwidth = 10;

    [SerializeField]private int widthChunk;
    [SerializeField]private int heightChunk;
    [SerializeField]private int depthChunk;
    [SerializeField]private float scale;

    public void Generate()
    {
      modingSeed = new long[planetCount];
      seeds = new long[planetCount];
      GenerateSeeds();

      tc.seed = (int)seeds[curentPlanet];

      tc.InitialLoad();
      createdWorld=true;
      uis.player.transform.position += new Vector3(0,400,0);
    }

    private void GenerateSeeds()
    {

      for(int i = 0;i<planetCount;i++)
      {
        if(modingSeed[i] != 0){seeds[i] = modingSeed[i];}
        if(seeds[i] != 0 || seeds[i] != null)
        {
          switch(i)
          {
            case 0:
            seeds[i]=seed + 273123456;
            break;
            case 1:
            seeds[i]=seed + 912876543;
            break;
            case 2:
            seeds[i]=seed + 345696145;
            break;
            case 3:
            seeds[i]=seed + 843755345;
            break;
            case 4:
            seeds[i]=seed + 932437237;
            break;
            case 5:
            seeds[i]=seed + 216542389;
            break;
            case 6:
            seeds[i]=seed + 234237823;
            break;
            case 7:
            seeds[i]=seed + 987654235;
            break;
            case 8:
            seeds[i]=seed + 189642343;
            break;
            case 9:
            seeds[i]=seed + 238904374;
            break;
          }
        }
      }
    }


    private void ReloadChunks()
    {
      var a = uis.player.transform.position;

    }
    private void Update()
    {
      ReloadChunks();
    }
}
