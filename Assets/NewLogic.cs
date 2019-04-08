using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewLogic : MonoBehaviour {
	public Sprite []sprite;
	public GameObject Togglimage;
	public	bool playerMove=false;
	public GameObject [] ButtonWall; 
	public GameObject [] TopWall;
//	public GameObject [] TopWall;
	int i=0;
	int j=0;
	int Counter = 0;
	int Counter2 = 0;
	MoveWall moveWall;
	public Text text;
	PlayerController pc;
	public Text remaingWalls;
	public Text losePanelText;
	public GameObject losePanel;
	Transform moveTranfrom;
	Board.Move placement;

	// Use this for initialization
	void Start () {
		moveWall = GameObject.Find ("Controller").GetComponent<MoveWall> ();
		pc = moveWall.gameObject.GetComponent<PlayerController> ();
	}

	// Update is called once per frame
	void Update () {
		if (text.text == "Player1") {
			remaingWalls.text = ( 10-Counter).ToString ();
		} else if (text.text == "Player2") {
			remaingWalls.text = (10-Counter2).ToString ();
		}
	}

	public void CanMove(){
		playerMove=!playerMove;
		Togglimage.gameObject.GetComponent<Image> ().sprite = sprite [1];

		if (playerMove == true) {
			

			if (text.text == "Player1") {
				for (i = Counter; i < ButtonWall.Length; i++) {
					if (i == Counter) {
						Counter++;

						ButtonWall [i].gameObject.SetActive (true);
						moveWall.MoveFinalWall (ButtonWall [i].gameObject.transform);
						i = Counter;
					} else {
						ButtonWall [i].gameObject.SetActive (false);

					}

				}

			} else  if(text.text == "Player2"){
				for (j = Counter2; j < TopWall.Length; j++) {
					if (j == Counter2) {
						Counter2++;

						TopWall [j].gameObject.SetActive (true);
					moveWall.MoveFinalWall (TopWall [j].gameObject.transform);
						j = Counter2;
					} else {
						TopWall [j].gameObject.SetActive (false);

					}

				}
			

			}

		}
		   else if(playerMove==false)
	     	{
		    	Togglimage.gameObject.GetComponent<Image> ().sprite = sprite [0];

		//	UtilityServices.instance.RunDelayedCommand(1f , ()=> {MoveWall.isWallMoving = false; });    
		    }
	}
	public void LoseButton(){
		if (text.text == "Player1") {
			losePanel.SetActive (true);
			losePanelText.text = "Player 1 Lose";
		}
		if (text.text == "Player2") {
			losePanel.SetActive (true);
			losePanelText.text="Player 2 Lose";
		}
	}
}//class
