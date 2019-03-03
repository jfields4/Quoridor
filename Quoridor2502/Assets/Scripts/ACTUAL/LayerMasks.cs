using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerMasks : MonoBehaviour
{


    [Tooltip("Set the layer in which blocks are placed")]
    [SerializeField]
    [Range(0, 31)]
    private int blocksLayer;                              
    [Tooltip("Set the layer in which walls are placed")]
    [SerializeField]
    [Range(0, 31)]
    private int wallsLayer;
    [Tooltip("Set the layer in which placement arrows are placed")]
    [SerializeField]
    [Range(0, 31)]
    private int arrowsLayer;
    [Tooltip("Set the layer in which walls that are placed by a player will be placed.")]
    [SerializeField]
    [Range(0, 31)]
    private int placedWallsLayer;
    [Tooltip("Set the layer in which the players are placed")]
    [SerializeField]
    [Range(0, 31)]
    private int playersLayer;


    public int arrowsLayerNumber      { private set; get; }   // The layer code of the arrow layer
    public int blocksLayerNumber      { private set; get; }   // The layer code of the block layer
    public int wallsLayerNumber       { private set; get; }   // The layer code of the wall layer 
    public int placedWallsLayerNumber { private set; get; }   // The layer code in which the walls that are placed by the player will be placed
    public int playersLayerNumber     { private set; get; }   // The layer code of the players layer


    public int arrowsOnlyLayer   { private set; get; }   // The layer mask to check Raycast against arrows layer only
    public int blockLayerOnly    { private set; get; }   // The layer mask to check Raycast against blocks only
    public int wallLayerOnly     { private set; get; }   // The layer mask to check Raycast against walls only
    public int placedWallsOnly   { private set; get; }   // The layer mask to check Raycast against already placed walls
    public int playersLayerOnly  { private set; get; }   // The layer mask to check Raycast against the players only


    public static LayerMasks instance { private set; get; }


    void Awake()
    {
        instance = this;

        arrowsLayerNumber      = arrowsLayer;
        blocksLayerNumber      = blocksLayer;
        wallsLayerNumber       = wallsLayer;
        placedWallsLayerNumber = placedWallsLayer;
        playersLayerNumber     = playersLayer;

        blockLayerOnly    = 1 << blocksLayer;
        wallLayerOnly     = 1 << wallsLayer;
        placedWallsOnly   = 1 << placedWallsLayer;
        playersLayerOnly  = 1 << playersLayer;

    }


}
