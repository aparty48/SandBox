using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour
{
    // Start is called before the first frame update
    public Inventar invntr;
    public int idI;
    public int countI;
    public DataBase itm;
    void OnMouseUp()
    {
      
          invntr.dropItem(itm.items[idI],countI,gameObject);
      
    }
}
