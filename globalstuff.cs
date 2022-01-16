

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
    public const float GRAVITY = 0.0f;  //pixels per frame
    public const int CHUNK_SIZE = 32;
    public const float PIXEL_SCALE = 0.125f;
    public const float PERIOD = 0.1f;
    public const int seed = 123;
    public const int RENDER_DISTANCE = 24;


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