using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using gamecore = GameCore;
using gameboard = Board;

public class PlacementObject : MonoBehaviour
{


    private PlacementDirection direction;                  // The direction this placement object allows for wall placement.
    internal Vector3 neighboursCenterPoint;                // The center point of the 4 neighbouring blocks of this arrow.This is the point where a wall is placed.
    private MoveWall moveWallScript;                       // Reference to the move wall script
    private Vector3 wallOriginalPosition;                  // The original position of the wall before moving it 
    private Vector3 wallOriginalRotation;                  // The original rotation of the wall before moving it 
    private bool isFirstMouseOver = true;                  // Will be true if it is the first time the mouse gets over this placement object
    private static Material materialHorizontal;            // The material on the horizontal arrow gameObject
    private static Material materialVertical;              // The material on the vertical arrow gameObject
    private static Color originalColorHorizontal;          // The original color of the material on the horizontal arrows
    private static Color originalColorVertical;            // The original color of the material on the vertical arrows
    private bool startFading;                              // Flag indicates whether the system should start fading effects on arrows
    private float fadeSpeed = 0.7f;                        // The time in seconds to reach the target alpha
    private float fadeTo = 0.25f;                          // To what alpha value we fade to
    private static bool routineRunning1;
    private static bool routineRunning2;


    public enum PlacementDirection
    {
        horizontal,
        vertical,
        incorrectlyOriented
    }

    

    // Start is called before the first frame update
    void Awake()
    {


        moveWallScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<MoveWall>();

        if( (int)transform.localEulerAngles.y == 90 && (int)transform.localEulerAngles.x == 0)
        {
            direction = PlacementDirection.vertical;
        }
        else if((int)transform.localEulerAngles.y == 90 && (int)transform.localEulerAngles.x == 90)
        {
            direction = PlacementDirection.horizontal;
        }
        else
        {
            direction = PlacementDirection.incorrectlyOriented;
            Debug.LogWarning($"The placement arrow:  \"{gameObject.name}\" parented to: \"{transform.parent.name}\" is incorrectly oriented.This can cause problems with walls placement");
        }

        if(direction == PlacementDirection.horizontal && materialHorizontal == null)
        {
            materialHorizontal = GetComponent<MeshRenderer>().sharedMaterials[0];
            originalColorHorizontal = materialHorizontal.color;

        }


        if (direction == PlacementDirection.vertical && materialVertical == null)
        {
            materialVertical = GetComponent<MeshRenderer>().sharedMaterials[0];
            originalColorVertical = materialVertical.color;
            //Debug.Log(originalColorVertical + " this is vertical color ");
        }

        //Debug.Log(gameObject.name + "  " + direction.ToString());
    }





    private void OnMouseUp()
    {

        BoardSetup.instance.EnableDisablePlacementArrows(false);
        moveWallScript.selectedWall.GetComponent<BoxCollider>().enabled = true;

        isFirstMouseOver = true;

        PlaceSelectedWall();

        // Change the layer of the wall placed so that it can't be moved again.(Raycast will ignore this wall)
        moveWallScript.selectedWall.gameObject.layer = LayerMasks.instance.placedWallsLayerNumber;
        GameObject.FindGameObjectWithTag("GameController").GetComponent<PlayerController>().ToggleActivePlayer();

    }



    private void OnMouseOver()
    {
        //Debug.Log("Start perchai on  " + transform.parent.name);
        if(isFirstMouseOver)
        {
            isFirstMouseOver = false;
            moveWallScript.selectedWall.GetComponent<BoxCollider>().enabled = false;

            wallOriginalPosition = moveWallScript.selectedWall.position;
            wallOriginalRotation = moveWallScript.selectedWall.eulerAngles;
            PlaceSelectedWall();
        }

    }



    private void OnMouseExit()
    {
        Debug.Log("Stop perchai on  " + transform.parent.name);

        isFirstMouseOver = true;
        moveWallScript.selectedWall.GetComponent<BoxCollider>().enabled = true;
        moveWallScript.selectedWall.position    = wallOriginalPosition;
        moveWallScript.selectedWall.eulerAngles = wallOriginalRotation;
		Debug.Log("Stop perchai on  " + moveWallScript.selectedWall.position);
    }




    private void OnDisable()
    {
        startFading = false;
        StopCoroutine("StartFadingEffect");
        routineRunning1 = false;
        routineRunning2 = false;

        materialVertical.color = originalColorHorizontal;
        materialVertical.color = originalColorVertical;

    }



    private void OnEnable()
    {
        startFading = true;

        UtilityServices.instance.RunDelayedCommand(0.6f, () => 
        {
            if(gameObject.activeInHierarchy == true && !routineRunning1) { routineRunning1 = true; StartCoroutine(StartFadingEffect(materialHorizontal , true)); }
            if(gameObject.activeInHierarchy == true && !routineRunning2) { routineRunning2 = true; StartCoroutine(StartFadingEffect(materialVertical , false)); }

        });
        
    }




    private IEnumerator StartFadingEffect(Material material , bool isHorizontalArrow)
    {


        if(isHorizontalArrow) { }
        else { }
        // Cache the current color of the material, and its initial opacity.
        Color color = material.color;
        float startOpacity = color.a;

        // Track how many seconds we've been fading.
        float t = 0;
        float fadeTo = this.fadeTo;

        while(true)
        {

            yield return new WaitForSeconds(0);

            while (t < fadeSpeed)
            {
                // Step the fade forward one frame.
                t += Time.deltaTime;
                // Turn the time into an interpolation factor between 0 and 1.
                float blend = Mathf.Clamp01(t / fadeSpeed);

                // Blend to the corresponding opacity between start & target.
                color.a = Mathf.Lerp(startOpacity, fadeTo, blend);

                // Apply the resulting color to the material.
                material.color = color;

                // Wait one frame, and repeat.
                yield return null;
            }


            if(fadeTo == this.fadeTo) { fadeTo = 1; }
            else { fadeTo = this.fadeTo; }
            
            color = material.color;
            startOpacity = color.a;
            t = 0;

        }


    }



    private void PlaceSelectedWall()
    {

        Vector3 previousPosition = moveWallScript.selectedWall.position;
        moveWallScript.selectedWall.position = new Vector3(neighboursCenterPoint.x, moveWallScript.selectedWall.position.y, neighboursCenterPoint.z);
        

        if (direction == PlacementDirection.horizontal)
        {
            Vector3 previousAngles = moveWallScript.selectedWall.eulerAngles;
            moveWallScript.selectedWall.eulerAngles = new Vector3(previousAngles.x, 90, previousAngles.z);
			Debug.LogWarning(moveWallScript.selectedWall.position);
		}

        else if (direction == PlacementDirection.vertical)
        {
            Vector3 previousAngles = moveWallScript.selectedWall.eulerAngles;
            moveWallScript.selectedWall.eulerAngles = new Vector3(previousAngles.x, 0, previousAngles.z);
			Debug.LogWarning(moveWallScript.selectedWall.position);
		}

        else if (direction == PlacementDirection.incorrectlyOriented)
        {
            moveWallScript.selectedWall.position = previousPosition;
			Debug.LogWarning(moveWallScript.selectedWall.position);
            Debug.LogWarning($"The placement arrow:  \"{gameObject.name}\" parented to: \"{transform.parent.name}\" is incorrectly oriented.The wall \"{moveWallScript.selectedWall.name}\" won't be moved");
        }    

    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
