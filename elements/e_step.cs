using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

public static class e_step {
    public static int TryWakeCell(element_s thisp) {
        if (thisp.IsFreeFalling >0) { 
            return thisp.IsFreeFalling;
        }
        return _random.NextFloat(0f, 1f) >= thisp.inertialResistance ? 2: 0;
        // this.IsFreeFalling = true;
        // return thisp;
    }
   
    private static Unity.Mathematics.Random _random = new Unity.Mathematics.Random(Constants.seed);
    
    public static Vector3Int PowderStep(element_s powder)
    {
        /*
                Calculates the destination to swap or whatever of powder element. 
                Returns a vector where x and y are the coordinates and z is the behaviour. 
                Currently z = 0  : swap
                        z = 1  : do nothing/ignore
                        z = -1 : delete/replace with nothing element. 
                      
        */
        // // // Vector3 newpos = powder._position + powder.speed;

        
        // Vector2Int curspeed = new Vector2Int((int) (Mathf.Abs(powder.speed.x) >= 1 ? powder.speed.x: Mathf.Round(_random.NextFloat(0f, powder.speed.x))),
        //                     (int) (Mathf.Abs(powder.speed.y) >= 1 ? powder.speed.y: Mathf.Round(_random.NextFloat(0f, powder.speed.y))));

        // Vector2Int end = powder.position + curspeed; 
        
        // Vector2Int candidate;
        // powder.color = new Color32(200, 200, 50, 255);

        // if (Chunks.GetCell((powder.position + new Vector2Int(0, -1))).matter == Matter.None) {  //can go down
        //     // powder.color = new Color32(255, 50, 50, 255);//red
        //     candidate = (powder.position + new Vector2Int(0, -1));
        //     powder.speed = new Vector2(powder.speed.x, powder.speed.y - Constants.GRAVITY);
        //     for (int i = -2; i >= curspeed.y; i--) {
        //         if (Chunks.GetCell(powder.position + new Vector2Int(0, i)).matter == Matter.None) {
        //         } else { 
        //             break;
        //         }
        //         candidate = (powder.position + new Vector2Int(0, i));

        //     }
        //     // powder.color = new Color32(10, 244, 200, 255);
        //     if(powder.speed.y != 0f && Chunks.GetCell(candidate - new Vector2Int(0, 1)).matter != Matter.None) {
        //         float absY = Mathf.Max(Mathf.Abs(powder.speed.y) * powder.airResistance, 3);
        //         if (Mathf.Abs(powder.speed.x) < 0.01f) {
        //             powder.speed = new Vector2(Mathf.Sign(_random.NextFloat(-1f, 1f))*absY, 0f);
        //         } else {
        //             powder.speed = new Vector2(Mathf.Sign(powder.speed.x)*absY, 0f);
        //         }
        //     }
        //     if (Chunks.GetCell(candidate).matter != Matter.None) {
        //         Debug.LogError("error");
        //     }
            
        //     Chunks.SetCell(powder);
        //     return (Vector3Int) candidate;
        // } else {
        //         // powder.color = new Color32(40, 205, 255, 255); //turquoise maybe
        //     candidate = (powder.position);
        //     if (Mathf.Abs(curspeed.x) > 0) {
        //         powder.speed =new Vector2 ( Chunks.GetCell(powder.position-new Vector2Int(0, 1)).friction * powder.friction * powder.speed.x, 0f);
        //         if (Mathf.Abs(powder.speed.x) < 0.01) {
        //             powder.IsFreeFalling -=1;
        //         }
        //         // powder.color = new Color32(40, 255, 50, 255); //green
        //         for (int i = 1; i < Mathf.Abs(curspeed.x); i++) {
        //             if (Chunks.GetCell(powder.position + new Vector2Int((int)Mathf.Sign(curspeed.x)*i,0)).matter == Matter.None) {
        //             } else { 
        //                 powder.speed = Vector2.zero; 
        //                 if (Chunks.GetCell(powder.position + new Vector2Int((int)Mathf.Sign(curspeed.x)*i,0)).matter != powder.matter) {
        //                             //powder thing doesn't work.
        //                 }
        //                 break;
        //             }
        //             candidate = (powder.position + new Vector2Int((int)Mathf.Sign(curspeed.x)*i,0));

        //         }
        //         // powder.color = new Color32(40, 30, 50, 255); 
                
        //         if (candidate== powder.position) {
        //              //pass
        //         } else {
        //             if (Chunks.GetCell(candidate).matter != Matter.None) {
        //                 Debug.LogError("error");
        //             }
        //             Chunks.SetCell(powder);
                    
        //             return (Vector3Int) candidate;
        //         }
        //     } 
        //     // powder.color = new Color32(10, 244, 200, 255);
            
        // }

        // if (powder.IsFreeFalling == 0) {
        //     Chunks.SetCell(powder);

        //     return Vector3Int.one;
        // }
        
        // if (_random.NextFloat(0f, 1f) <= powder.inertialResistance) {
        //     powder.IsFreeFalling = 0;
        //     Chunks.SetCell(powder);
        //     return Vector3Int.one;
        // }

        // powder.IsFreeFalling -=1; 
        powder.IsFreeFalling = 0;
        Chunks.SetCell(powder);
        return Vector3Int.one;

        //if under is nothing, add gravity

    }



}
