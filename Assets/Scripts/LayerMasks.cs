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


    public int arrowsOnlyLayer   { private set; get; }   // The layer mask to check Raycast against arrows layer only
    public int arrowsLayerNumber { private set; get; }   // The layer code of the arrows layer
    public int blocksLayerNumber { private set; get; }   // The layer code of the blocks layer
    public int wallsLayerNumber  { private set; get; }   // The layer code of the walls layer 
    public int blockLayerOnly    { private set; get; }   // The layer mask to check Raycast against blocks only
    public int wallLayerOnly     { private set; get; }   // The layer mask to check Raycast against walls only
    public int wallAndBlockLayer { private set; get; }   // The layer mask to check Raycast against both walls and blocks only


    public static LayerMasks instance { private set; get; }


    void Awake()
    {
        instance = this;

        blocksLayerNumber = blocksLayer;
        wallsLayerNumber  = wallsLayer;
        blockLayerOnly    = 1 << blocksLayer;
        wallLayerOnly     = 1 << wallsLayer;
        wallAndBlockLayer = blockLayerOnly | wallLayerOnly;

    }


}
