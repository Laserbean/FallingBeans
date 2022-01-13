using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FlyParticle: element 
{
    public FlyParticle(Vector2Int pos, Vector2? speed = null, Color32? color = null) : base (pos, speed)
    {
        byte off = (byte) Mathf.Round(Random.Range(50, 150));
        this.color = new Color32(255, 244, off, 255);
    }

    
    public override Vector3Int StepSimple()
    {
        Vector2Int cur = this.position;
        Vector2Int end = this.position + this.speedInt; 

        // Vector2Int end = new Vector2Int(this.position.x + this.speedInt.x, this.position.y + (Mathf.Abs(this.speedInt.y) >=1 ? this.speedInt.y: -1));

        Vector2Int candidate = cur;


        this.speed = new Vector2(this.speed.x, this.speed.y - Constants.GRAVITY);
        
        List<Vector2Int> vlist = Chunks.GetLinearList(cur, end);    

        foreach(Vector2Int pos in vlist) {
            if (pos == cur) {
                continue;
            }
            if (Chunks.GetCell(pos).matter == Matter.None) {
                candidate = pos; 
            } else {
                if (this.speed.x == 0) {
                    return this.StepSimple();

                } else {
                    this.speed = new Vector2(0f, this.speed.y);
                }
                // this.speed = new Vector2(0f, this.speed.y);

                if (candidate == cur) {
                    return Vector3Int.one;
                }
                break;
            }
        }
 

        
        return (Vector3Int) candidate;

        //if under is nothing, add gravity

    }



}


