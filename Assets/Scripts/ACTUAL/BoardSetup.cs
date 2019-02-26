using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSetup : MonoBehaviour
{

    public GameObject placementArrowsPrefab;                      // Reference to the placement arrows prefab
    [SerializeField]
    private GameObject grid;                                      // References the grid gameobject
    public List<string> gridArray { private set; get; }           // The list representation of the grid array
    public List<Transform> placementArrows { private set; get; }  // The list of placement arrows

    public static BoardSetup instance { private set; get; }



    // Start is called before the first frame update
    void Start()
    {

        instance = this;

        gridArray = new List<string>();
        placementArrows = new List<Transform>();

        for (int a = 0; a < grid.transform.childCount; a++)
        {
            Transform block = grid.transform.GetChild(a);
            gridArray.Add(block.name);


            if((a+1) % 9 == 0) { continue; }   // don't add placement arrows for the last block in a row
            if(a >= 72) { continue; }          // don't add placement arrows for the last row

            Transform placementArrow = (Instantiate(placementArrowsPrefab) as GameObject).transform;
            placementArrow.name += (a + 1);
            placementArrows.Add(placementArrow);


            placementArrow.parent = block;
            float height = placementArrow.localPosition.y;
            placementArrow.localPosition = new Vector3(0,height,0);

            Transform box = placementArrow.GetChild(2);

            Collider[] neighbours = Physics.OverlapBox(box.position , box.localScale / 2 , Quaternion.identity , LayerMasks.instance.blockLayerOnly);

            Vector3 neighboursCenter = Vector3.zero;

            try
            {
                for (int b = 0; b < 4; b++)
                {
                    neighboursCenter += neighbours[b].transform.position;
                }
            }

            catch
            {
                if((a+1) % 9 != 0)
                {
                    Debug.LogWarning("The grid neighbours have been improperly resized.Please resize the arrow prefabs cube child to overlap all 4 neighbours.");
                }
            }


            neighboursCenter /= 4f;

            placementArrow.GetChild(0).GetComponent<PlacementObject>().neighboursCenterPoint = neighboursCenter;
            placementArrow.GetChild(1).GetComponent<PlacementObject>().neighboursCenterPoint = neighboursCenter;

            placementArrow.gameObject.SetActive(false);
        }


    }



    public void EnableDisablePlacementArrows(bool enable)
    {

        for(int a = 0; a < placementArrows.Count; a++ )
        {
            placementArrows[a].gameObject.SetActive(enable);
        }

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
