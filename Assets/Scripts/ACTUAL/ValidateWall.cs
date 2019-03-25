﻿using System.Collections.Generic;
using static Board;

public class WallValidation
{
    //state and node are used to track board state
    public struct State
    {
        public Move position1;
        public Move position2;

        public State(Move first, Move second)
        {
            position1 = new Move(first);
            position2 = new Move(second);
        }
    }
    public struct Node
    {
        public Node(Board currentboard, Move currentmove, State state)
        {
            gameboard = new Board(currentboard);
            mv = new Move(currentmove);
            currentstate = state;
        }

        public State currentstate;
        public readonly Board gameboard;
        public readonly Move mv;
    }

    //this is necessary for the HashSet to not put duplicates in the set
    public class MoveComparer : EqualityComparer<Move>
    {
        public override bool Equals(Move x, Move y)
        {
            return x == y;
        }

        public override int GetHashCode(Move obj)
        {
            int hash = obj.Row ^ obj.Column ^ obj.Value;
            return hash.GetHashCode();
        }
    }

    //this set is used to make sure we don't back track and get infinite recursion
    HashSet<Move> PlacesBeen;

    public WallValidation()
    {
        PlacesBeen = new HashSet<Move>(new MoveComparer());
    }

    public bool Validate(Board boardSent, Move placement)
    {
        if (placement.Row < 1 || placement.Column < 1 || placement.Row >= 9 || placement.Column >= 9)
        {
            return false;
        }

        //if the space is occupied
        if (boardSent.GameBoard[placement.Row, placement.Column] != 0)
        {
            return false;
        }
        //if wall is horizontal, check left & right for horizontal walls
        else if (placement.Value == 1 &&
            (boardSent.GameBoard[placement.Row, placement.Column - 1] == 1 ||
            boardSent.GameBoard[placement.Row, placement.Column + 1] == 1))
        {
            return false;
        }
        //if wall is vertical, check up & down for vertical walls
        else if (placement.Value == -1 &&
            (boardSent.GameBoard[placement.Row - 1, placement.Column] == -1 ||
            boardSent.GameBoard[placement.Row + 1, placement.Column] == -1))
        {
            return false;
        }

        //check both player positions
        for (int i = 0; i <= 1; i++)
        {
            State startingState = new State(boardSent.PlayerPositions[0], boardSent.PlayerPositions[1]);
            Node startingNode = new Node(boardSent, placement, startingState);
            startingNode.gameboard.MakeMove(placement);
            startingNode.gameboard.Current = (byte)i;

            if (PathToGoal(startingNode, i) == false)
            {
                return false;
            }
            PlacesBeen.Clear();
        }

        return true;
    }

    //depth-first recursive search
    private bool PathToGoal(Node node, int playerNumber)
    {
        Move moveRight;
        Move moveLeft;
        Move moveDown;
        Move moveUp;

        bool result = false;

        if (PlacesBeen.Count == 0)
        {
            PlacesBeen.Add(node.gameboard.PlayerPositions[playerNumber]);
        }

        moveUp = new Move((byte)(node.gameboard.PlayerPositions[playerNumber].Row - 1), node.gameboard.PlayerPositions[playerNumber].Column, 0);
        moveDown = new Move((byte)(node.gameboard.PlayerPositions[playerNumber].Row + 1), node.gameboard.PlayerPositions[playerNumber].Column, 0);
        moveLeft = new Move(node.gameboard.PlayerPositions[playerNumber].Row, (byte)(node.gameboard.PlayerPositions[playerNumber].Column - 1), 0);
        moveRight = new Move(node.gameboard.PlayerPositions[playerNumber].Row, (byte)(node.gameboard.PlayerPositions[playerNumber].Column + 1), 0);


        //if current player can reach other side, wall is legal
        if ((playerNumber == 0 && node.gameboard.PlayerPositions[0].Row == 1) ||
            (playerNumber == 1 && node.gameboard.PlayerPositions[1].Row == 9))
        {
            return true;
        }

        if (PlacesBeen.Count == 71)
        {
            playerNumber = playerNumber;
        }

        //check each direction
        if (node.gameboard.ValidateMove(moveUp) &&
            !PlacesBeen.Contains(moveUp) &&
            result != true)
        {
            Node newNode = new Node(node.gameboard, node.mv, node.currentstate);
            newNode.gameboard.MakeMove(moveUp);
            newNode.gameboard.Current = (byte)playerNumber;

            PlacesBeen.Add(node.gameboard.PlayerPositions[playerNumber]);
            result = PathToGoal(newNode, playerNumber);
        }
        if (node.gameboard.ValidateMove(moveLeft) &&
            !PlacesBeen.Contains(moveLeft) &&
            result != true)
        {
            Node newNode = new Node(node.gameboard, node.mv, node.currentstate);
            newNode.gameboard.MakeMove(moveLeft);
            newNode.gameboard.Current = (byte)playerNumber;

            PlacesBeen.Add(node.gameboard.PlayerPositions[playerNumber]);
            result = PathToGoal(newNode, playerNumber);
        }
        if (node.gameboard.ValidateMove(moveRight) &&
            !PlacesBeen.Contains(moveRight) &&
            result != true)
        {
            Node newNode = new Node(node.gameboard, node.mv, node.currentstate);
            newNode.gameboard.MakeMove(moveRight);
            newNode.gameboard.Current = (byte)playerNumber;

            PlacesBeen.Add(node.gameboard.PlayerPositions[playerNumber]);
            result = PathToGoal(newNode, playerNumber);
        }
        if (node.gameboard.ValidateMove(moveDown) &&
            !PlacesBeen.Contains(moveDown) &&
            result != true)
        {
            Node newNode = new Node(node.gameboard, node.mv, node.currentstate);
            newNode.gameboard.MakeMove(moveDown);
            newNode.gameboard.Current = (byte)playerNumber;

            PlacesBeen.Add(node.gameboard.PlayerPositions[playerNumber]);
            result = PathToGoal(newNode, playerNumber);
        }

        //if no more possible moves, then wall is illegal
        // if other side was reached, wall is legal
        return result;
    }
}