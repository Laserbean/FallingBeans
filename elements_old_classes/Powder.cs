using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Powder: element 
{
    public float airResistance = 1.3f;
    public Powder(Vector2Int pos, Vector2? speed = null) : base (pos, speed)
    {
        this.color = new Color32(100, 100, 100, 255);
        this.matter = Matter.Solid;
        this.friction = 0.4f;
    }

    public override Vector3Int Step()
    {
        /*
            Calculates the destination to swap or whatever of this element. 
            Returns a vector where x and y are the coordinates and z is the behaviour. 
            Currently z = 0  : swap
                      z = 1  : do nothing/ignore
                      z = -1 : delete/replace with nothing element. 
                      
        */
        // // // Vector3 newpos = this._position + this.speed;
        Vector2Int curspeed = new Vector2Int((int) (Mathf.Abs(this.speed.x) >= 1 ? this.speed.x: Mathf.Round(Random.Range(0f, this.speed.x))),
                            (int) (Mathf.Abs(this.speed.y) >= 1 ? this.speed.y: Mathf.Round(Random.Range(0f, this.speed.y))));

        Vector2Int end = this.position + curspeed; 
        
        Vector2Int candidate;
        // this.color = new Color32(200, 200, 50, 255);

        if (Chunks.GetCell((this.position + new Vector2Int(0, -1))).matter == Matter.None) {  //can go down
            // this.color = new Color32(255, 50, 50, 255);//red
            candidate = (this.position + new Vector2Int(0, -1));
            this.speed = new Vector2(this.speed.x, this.speed.y - Constants.GRAVITY);
            for (int i = -2; i >= curspeed.y; i--) {
                if (Chunks.GetCell(this.position + new Vector2Int(0, i)).matter == Matter.None) {
                } else { 
                    break;
                }
                candidate = (this.position + new Vector2Int(0, i));

            }
            // this.color = new Color32(10, 244, 200, 255);
            if(this.speed.y != 0f && Chunks.GetCell(candidate - new Vector2Int(0, 1)).matter != Matter.None) {
                float absY = Mathf.Min(Mathf.Abs(this.speed.y) * airResistance, 3);
                if (Mathf.Abs(this.speed.x) < 0.01f) {
                    this.speed = new Vector2(Mathf.Sign(Random.Range(-1f, 1f))*absY, 0f);
                } else {
                    this.speed = new Vector2(Mathf.Sign(this.speed.x)*absY, 0f);
                }
            }
            if (Chunks.GetCell(candidate).matter != Matter.None) {
                Debug.LogError("error");
            }
            return (Vector3Int) candidate;
        } else {
                // this.color = new Color32(40, 205, 255, 255); //turquoise maybe
            candidate = (this.position);
            if (Mathf.Abs(curspeed.x) > 0) {
                this.speed =new Vector2 ( Chunks.GetCell(this.position-new Vector2Int(0, 1)).friction * this.friction * this.speed.x, 0f);
                if (Mathf.Abs(this.speed.x) < 0.01) {
                    this.IsFreeFalling -=1;
                }
                // this.color = new Color32(40, 255, 50, 255); //green
                for (int i = 1; i < Mathf.Abs(curspeed.x); i++) {
                    if (Chunks.GetCell(this.position + new Vector2Int((int)Mathf.Sign(curspeed.x)*i,0)).matter == Matter.None) {
                    } else { 
                        this.speed = Vector2.zero; 
                        if (Chunks.GetCell(this.position + new Vector2Int((int)Mathf.Sign(curspeed.x)*i,0)).matter != this.matter) {
                                    //this thing doesn't work.
                        }
                        break;
                    }
                    candidate = (this.position + new Vector2Int((int)Mathf.Sign(curspeed.x)*i,0));

                }
                // this.color = new Color32(40, 30, 50, 255); 
                
                if (candidate== this.position) {
                     //pass
                } else {
                    if (Chunks.GetCell(candidate).matter != Matter.None) {
                        Debug.LogError("error");
                    }
                    return (Vector3Int) candidate;
                }
            } 
            // this.color = new Color32(10, 244, 200, 255);
            
        }
        bool botleft, botright;
        botleft = Chunks.GetCell(this.position + new Vector2Int(-1, -1)).matter == Matter.None;
        botright = Chunks.GetCell(this.position + new Vector2Int(1, -1)).matter == Matter.None;

        if (this.IsFreeFalling == 0) {
            return Vector3Int.one;
        }
        
        if (Random.Range(0f, 1f) <= this.inertialResistance) {
            this.IsFreeFalling = 0;
            return Vector3Int.one;
        }


        if (botleft && botright) {
            if (Random.Range(0f, 1f) > 0.5f) { 
                this.speed = new Vector2(-Constants.GRAVITY*0.3f, this.speed.y);
                return (Vector3Int)(this.position + new Vector2Int(-1, -1));
            } else {
                this.speed = new Vector2(Constants.GRAVITY*0.3f, this.speed.y);
                return (Vector3Int)(this.position + new Vector2Int(1, -1));
            }
        }
        else {
            if (botright) {
                // this.speed = new Vector2(Constants.GRAVITY*0.3f, this.speed.y  -Constants.GRAVITY*0.5f);
                this.speed = new Vector2(Constants.GRAVITY*0.3f, this.speed.y);
                return (Vector3Int)(this.position + new Vector2Int(1, -1));
            }
            if(botleft) {
                // this.speed = new Vector2(-Constants.GRAVITY*0.3f, this.speed.y  -Constants.GRAVITY*0.5f);
                this.speed = new Vector2(-Constants.GRAVITY*0.3f, this.speed.y);
                return (Vector3Int)(this.position + new Vector2Int(-1, -1));
            }
        }

        // if(this.speed.y != 0f && Chunks.GetCell(this.position - new Vector2Int(0, 1)).speed.y == 0f) {
        //     float absY = Mathf.Max(this.speed.y * airResistance, 10);
        //     this.speed = new Vector2(this.speed.x< 0 ? -absY : absY, 0f);
        //     this.color = new Color32(40, 255, 50, 255);
        // }
        // this.speed =new Vector2 ( Chunks.GetCell(this.position - new Vector2Int(0, 1)).friction * this.friction * this.speed.x, 0f);

        this.IsFreeFalling -=1; 
        return Vector3Int.one;

        //if under is nothing, add gravity

    }


}


