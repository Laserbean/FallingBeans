

public enum Matter
{
    Solid,
    Powder,
    Liquid,
    Gas, 
    Other,
    None,
    Bedrock
}

public class Constants
{
    public const float GRAVITY = 0.5f;  //pixels per frame
    public const int CHUNK_SIZE = 16;
    public const float PIXEL_SCALE = 1f;
    public const float PERIOD = 0.05f;
    public const int seed = 123;


    // public const float PERIOD = 0.5f;
}

public enum e_name {
    //Solids
    Bedrock, Stone, 

    //Powders,
    Sand,
    //Liquids
    Water, Oil,

    //Gas,
    Smoke, 

    //Other
    Nothing
    
}