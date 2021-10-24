using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DataBase : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public List<Block> blocks = new List<Block>();
    public int lengthListBlock;
    void Start(){lengthListBlock=blocks.Count;}

}
[System.Serializable]

public class Item
{
  public int id;
  public string name;
  public Sprite img;
  public int stak; 
}

[System.Serializable]
public class Block
{
  public int idItemBlock;
  public GameObject prefabBlock;
  public float sizeX;
  public float sizeY;
  public float sizeZ;
}
