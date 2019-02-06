using UnityEngine;
using System;
using System.Text;
using System.Collections;


public class Board
{
	//move class
	public class Move{
		public byte Row;
		public byte Column;
		public sbyte Value;
		
		public static bool operator <(Move lValue, Move rValue){
			return ((lValue.Row < rValue.Row) ||
				(lValue.Column < rValue.Column));
		}
	}
	
	//member variables
	private sbyte[][] GameBoard;
	const byte BoardSize = 7;
	
	private Move[] PlayerPositions;
	private byte Current;
	
	
	//constructor
	public Board() {
		GameBoard = new sbyte[BoardSize][BoardSize];
	}
	
	//public functions
	
	public Move ConvertStringToMove(string moveSent) {
		Move move;
		
		move.Row = 10 - Int32.Parse(moveSent[1]);
		move.Column = (byte)moveSent[0] - 140;
		
		if(moveSent.Length == 2) {
			move.Value = 0;
		}
		else {
			if(moveSent[2] == 'h') {
				move.Value = 1;
			}
			else if(moveSent[2] == 'v') {
				move.Value = -1;
			}
			else {
				//error code
				move.Row = 0;
				move.Column = 0;
				move.Value = 0;
			}
		}
	}
	
	public string ConvertMoveToString(Move move) {
		
	}
	
	public bool ValidateMove(string moveString) {
		bool valid = false;
		
		Move moveSent = ConvertStringToMove(moveString);
		
		//reference as if moving from smaller point to larger point
		Move move;
		if(moveSent < PlayerPositions[Current]) {
			move = moveSent;
		}
		else {
			move = PlayerPositions[Current];
		}
		
		if(Adjacent(moveSent, PlayerPositions[Current])){
			//if moving up or down
			if(moveSent.Row != PlayerPositions[Current].Row){
				if(GameBoard[move.Row][move.Column - 1] <= 0 &&
					GameBoard[move.Row][move.Column] <= 0) {
					valid = true;	
				}
			}
			//if moving left or right
			else {
				if(GameBoard[move.Row - 1][move.Column] >= 0 &&
					GameBoard[move.Row][move.Column] >= 0) {
					valid = true;	
				}
			}
		}
		
		return valid;
	}
	
	public void MakeMove(string moveSent) {
		Move move = ConvertStringToMove(moveSent);
		
		//a value of 0 means it is a pawn move
		if(move.Value == 0) {
			SetNewPlayerPosition(move);
		}
		else{
			SetNewWall(move);
		}
		
		//flip current player
		Current = Math.Abs(Current - 1);
	}
	
	//overloaded to deal with either representation of a move
	public void MakeMove(Move move) {
		if(move.Value == 0) {
			SetNewPlayerPosition(move);
		}
		else{
			SetNewWall(move);
		}
		
		Current = Math.Abs(Current - 1);
	}
	
	//private functions
	
	private bool Adjacent(Move to, Move start) {
		return (Math.Abs(to.Row - start.row) == 1 ||
				Math.Abs(to.Column - start.Column) == 1);
	}
	
	private void SetNewPlayerPosition(Move move) {
		PlayerPositions[Current] = move;
	}

	private void SetNewWall(Move move){
		GameBoard[move.Row][move.Column] = move.Value;
	}
}