using UnityEngine;
using System;
using System.Text;
using System.Collections;

public class GameCore : MonoBehavior
{
	Board board;
	
	bool AIGame;
	Board::Move LastMove;
	
	public GameCore(bool AIOption) {
		AIGame = AIOption;
		//will need to initialize modules
	}
	
	public bool ValidateMove(string move) {
		return board.ValidateMove(move);
	}
	
	public bool ProcessMove(string move) {
		bool result = false;
		
		LastMove = Board.ConvertStringToMove(move);
		
		if(board.MakeMove(move)) {
			result = true;
		}
		else {
			if(!AIGame) {
				network.SendMove(move);
			}
			
			string newMove = GetNewMove();
			
			if(Board.MakeMove(newMove)) {
				result = true;
			}
		}
		
		return result;
	}
	
	private string GetNewMove() {
		Board::Move move;
		
		if(AIGame) {
			getAIMove(LastMove);
		}
		else {
			network.GetMove();
		}
	}
}

