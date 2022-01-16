using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkColliderScript : MonoBehaviour
{
    [SerializeField] PolygonCollider2D solid;
    [SerializeField] PolygonCollider2D liquid;
    public void Test() {
        Debug.LogError("nice");

    }
    Vector2Int position;

    public void SetSolidPath(List<List<Vector2>> fish2, Vector2Int chunkpos) {

        position = chunkpos;
        solid.pathCount = fish2.Count;
        // if (fish2.Count == 0) {
        //     this.gameObject.SetActive(false); 
        // }
        for (int i = 0; i < fish2.Count; i++)
        {
            Vector2[] points = fish2[i].ToArray();
            solid.SetPath(i, points);
        }
    }

    public void SetLiquidPath(List<List<Vector2>> fish2, Vector2Int chunkpos) {
        position = chunkpos; 
        liquid.pathCount = fish2.Count;
        // if (fish2.Count == 0) {
        //     this.gameObject.SetActive(false); 
        // }
        for (int i = 0; i < fish2.Count; i++)
        {
            Vector2[] points = fish2[i].ToArray();
            liquid.SetPath(i, points);
        }
    }

    // public void EventHandler(Vector2Int fish) {
    //     if (position == fish) {
    //         this.gameObject.SetActive(false); 
    //     }
    // }
}
