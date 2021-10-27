using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainController : MonoBehaviour {

    [SerializeField]
    private Generatorworlda gw;
    [SerializeField]
    private Terrain terrainPrefab = null;
    [SerializeField]
    private GameObject terrainTilePrefab = null;
    [SerializeField]
    private Vector3 terrainSize = new Vector3(20, 1, 20);
    public Vector3 TerrainSize { get { return terrainSize; } }
    [SerializeField]
    private Gradient gradient;
    [SerializeField]
    private float noiseScale = 3, cellSize = 1;
    [SerializeField]
    private int radiusToRender = 5;
    [SerializeField]
    private Transform[] gameTransforms;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private Transform water;
    public Transform Water { get { return water; } }
    [SerializeField]
    public int seed;
    [SerializeField]
    private GameObject[] placeableObjects;
    public GameObject[] PlaceableObjects { get { return placeableObjects; } }
    [SerializeField]
    private Vector3[] placeableObjectSizes;//the sizes of placeableObjects, in matching order
    public Vector3[] PlaceableObjectSizes { get { return placeableObjectSizes; } }
    [SerializeField]
    private int minObjectsPerTile = 0, maxObjectsPerTile = 20;
    public int MinObjectsPerTile { get { return minObjectsPerTile; } }
    public int MaxObjectsPerTile { get { return maxObjectsPerTile; } }
    [SerializeField]
    private float destroyDistance = 1000;
    [SerializeField]
    private bool usePerlinNoise = true;
    [SerializeField]
    private Texture2D noise;
    public static float[][] noisePixels;

    private Vector2 startOffset;

    private Dictionary<Vector2, GameObject> terrainTiles = new Dictionary<Vector2, GameObject>();

    private Vector2[] previousCenterTiles;
    private List<GameObject> previousTileObjects = new List<GameObject>();
    public Transform Level { get; set; }//позиция уровня
    private Vector2 noiseRange;

    [SerializeField]
    private GameObject wb;
    [SerializeField]
    private float mn;

    private void Awake() {
        if (noise)//существует ли текстура шума
            noisePixels = GetGrayScalePixels(noise);//получаем лист пикселей
        GenerateMesh.UsePerlinNoise = usePerlinNoise;//говорим другому скрпту что ми используем шум
        noiseRange = usePerlinNoise ? Vector2.one * 256 : new Vector2(noisePixels.Length, noisePixels[0].Length);//делаем диапазон шума
    }

    private void Start() {
        //InitialLoad();//вызываем метод инициализацию загрузки
    }

    public void InitialLoad() {//метод инициализацию загрузки
        DestroyTerrain();//метод уничтожения тераин

        Level = new GameObject("Level").transform;//создаем позицию уровня
        water.parent = Level;// воде устанавливаем поицию уровня
        playerTransform.parent = Level;// игроку устанавлваем позицию уровня
        foreach (Transform t in gameTransforms)
            t.parent = Level;//устанавливаем позицию уровня

        float waterSideLength = radiusToRender * 2 + 1;//длина бококвой стороны воды
        water.localScale = new Vector3(terrainSize.x / 10 * waterSideLength, 1, terrainSize.z / 10 * waterSideLength);//устанавливаем размер воды

        Random.InitState(seed);//устанавливаем зерно для рандома
        //choose a random place on perlin noise
        startOffset = new Vector2(Random.Range(0f, noiseRange.x), Random.Range(0f, noiseRange.y));//создаем стартовое смещение
        RandomizeInitState();//востанавливаем фунцию рандома
    }

    private void Update() {
      if(gw.createdWorld)
      {
        //water.SetActive(true);
        //save the tile the player is on
        Vector2 playerTile = TileFromPosition(playerTransform.localPosition);//получаем позицию игрока в 2д
        //save the tiles of all tracked objects in gameTransforms (including the player)
        List<Vector2> centerTiles = new List<Vector2>();//лист позиций центральных плиток
        centerTiles.Add(playerTile);//добавляем позицию игрока
        foreach (Transform t in gameTransforms)
            centerTiles.Add(TileFromPosition(t.localPosition));//добавляем позиции игровых обектов

        //если плитки еще нет или он должна измениться
        if (previousCenterTiles == null || HaveTilesChanged(centerTiles)) {
            List<GameObject> tileObjects = new List<GameObject>();//обекти плиток
            //activate new tiles
            foreach (Vector2 tile in centerTiles) {
                bool isPlayerTile = tile == playerTile;
                int radius = isPlayerTile ? radiusToRender : 1;//создаем радиус
                for (int i = -radius; i <= radius; i++)
                    for (int j = -radius; j <= radius; j++)
                        ActivateOrCreateTile((int)tile.x + i, (int)tile.y + j, tileObjects);//активируем или создаем плитку
                if (isPlayerTile)
                    water.localPosition = new Vector3(tile.x * terrainSize.x, water.localPosition.y + 100, tile.y * terrainSize.z);//изменяем размер воды
            }
            //deactivate old tiles
            foreach (GameObject g in previousTileObjects)
                if (!tileObjects.Contains(g))
                    g.SetActive(false);// выключаем плитку

            //уничтожаем плитки если они далеко
            List<Vector2> keysToRemove = new List<Vector2>();//can't remove item when inside a foreach loop
            foreach (KeyValuePair<Vector2, GameObject> kv in terrainTiles) {
                if (Vector3.Distance(playerTransform.position, kv.Value.transform.position) > destroyDistance && !kv.Value.activeSelf) {//если плитка за радиусом прогрузки
                    keysToRemove.Add(kv.Key);
                    Destroy(kv.Value);
                }
            }
            foreach (Vector2 key in keysToRemove)
                terrainTiles.Remove(key);//уничтожаем не нужние ключи

            previousTileObjects = new List<GameObject>(tileObjects);//обновляем список не прогружоный чанков
        }

        previousCenterTiles = centerTiles.ToArray();
      }
      else{
      //  water.SetActive(false);
      }
    }


    //Helper methods below

    private void ActivateOrCreateTile(int xIndex, int yIndex, List<GameObject> tileObjects) {
        if (!terrainTiles.ContainsKey(new Vector2(xIndex, yIndex))) {//если тайла нет
            tileObjects.Add(CreateTile(xIndex, yIndex));//создаем тайл
        } else {//если есть
            GameObject t = terrainTiles[new Vector2(xIndex, yIndex)];
            tileObjects.Add(t);
            if (!t.activeSelf)//если не активен
                t.SetActive(true);//активируем тайл
        }
    }

    private GameObject CreateTile(int xIndex, int yIndex) {//создаем тайл
      Random.InitState((int)(seed + (long)xIndex * 100 + yIndex));//so it doesn't form a (noticable) pattern of similar tiles

        //TerrainData td = terrainPrefab.terrainData;
        //td = GenerateTerrain(td,xIndex,yIndex);

        GameObject terrain = Instantiate(
            terrainTilePrefab,//Terrain.CreateTerrainGameObject(td),
            Vector3.zero,
            Quaternion.identity,
            Level
        );
        //had to move outside of instantiate because it's a local position
        terrain.transform.localPosition = new Vector3(terrainSize.x * xIndex, terrainSize.y, terrainSize.z * yIndex);//устанавливаем позицию тайла
        terrain.name = TrimEnd(terrain.name, "(Clone)") + " [" + xIndex + " , " + yIndex + "]";//переименовуем

        terrainTiles.Add(new Vector2(xIndex, yIndex), terrain);//добавляем в список

        GenerateMesh gm = terrain.GetComponent<GenerateMesh>();//создаем меш
        gm.TerrainSize = terrainSize;//устанавливаем размер територии
        gm.Gradient = gradient;//устанавливаем градент
        gm.NoiseScale = noiseScale;//устанавливаем размер шума
        gm.CellSize = cellSize;
        gm.NoiseOffset = NoiseOffset(xIndex, yIndex);//устанавливаем шумовое смещения
        gm.Generate();//создаем меш

        PlaceObjects po = gm.GetComponent<PlaceObjects>();
        po.worldBuild = wb;
        po.TerrainController = this;
        po.Place();
        RandomizeInitState();

        return terrain;
    }






    private Vector2 NoiseOffset(int xIndex, int yIndex) {//шумовое смещение
        Vector2 noiseOffset = new Vector2(
            (xIndex * noiseScale + startOffset.x) % noiseRange.x,
            (yIndex * noiseScale + startOffset.y) % noiseRange.y
        );
        //account for negatives (ex. -1 % 256 = -1, needs to loop around to 255)
        if (noiseOffset.x < 0)
            noiseOffset = new Vector2(noiseOffset.x + noiseRange.x, noiseOffset.y);
        if (noiseOffset.y < 0)
            noiseOffset = new Vector2(noiseOffset.x, noiseOffset.y + noiseRange.y);
        return noiseOffset;
    }

    private Vector2 TileFromPosition(Vector3 position) {
        return new Vector2(Mathf.FloorToInt(position.x / terrainSize.x + .5f), Mathf.FloorToInt(position.z / terrainSize.z + .5f));
    }

    private void RandomizeInitState() {
        Random.InitState((int)System.DateTime.UtcNow.Ticks);//casting a long to an int "loops" it (like modulo)
    }

    private bool HaveTilesChanged(List<Vector2> centerTiles) {
        if (previousCenterTiles.Length != centerTiles.Count)
            return true;
        for (int i = 0; i < previousCenterTiles.Length; i++)
            if (previousCenterTiles[i] != centerTiles[i])
                return true;
        return false;
    }

    public void DestroyTerrain() {//метод уничтожения територии
        water.parent = null;//убираем значения воды
        playerTransform.parent = null;//убираем значения игрока
        foreach (Transform t in gameTransforms)
            t.parent = Level;//устанавливаем позицю уровня
        Destroy(Level);//уничтожаем уровень
        terrainTiles.Clear();//очищаем
    }

    private static string TrimEnd(string str, string end) {
        if (str.EndsWith(end))
            return str.Substring(0, str.LastIndexOf(end));
        return str;
    }

    public static float[][] GetGrayScalePixels(Texture2D texture2D) {
        List<float> grayscale = texture2D.GetPixels().Select(c => c.grayscale).ToList();

        List<List<float>> grayscale2d = new List<List<float>>();
        for (int i = 0; i < grayscale.Count; i += texture2D.width)
            grayscale2d.Add(grayscale.GetRange(i, texture2D.width));

        return grayscale2d.Select(a => a.ToArray()).ToArray();
    }

}
