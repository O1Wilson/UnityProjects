using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance; // Singleton instance

    public float LevelWidth;
    public float LevelHeight;

    // New boundary values
    public float MinX = -0.64f;
    public float MaxX = 0.64f;
    public float MinY = -1.92f;
    public float MaxY = 1.92f;

    void Awake()
    {
        Instance = this; // Set the singleton instance to this instance of the LevelManager
        // Initialize other level properties if needed
    }
}