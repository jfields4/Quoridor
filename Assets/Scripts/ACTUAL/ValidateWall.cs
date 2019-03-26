using System.Collections.Generic;
using static AI.Board;
using static Board.Move;

public class WallValidation
{
    //state and node are used to track board state
    public struct State
    {
        public Board.Move position1;
        public Board.Move position2;

        public State(Board.Move first, Board.Move second)
        {
            position1 = new Board.Move(first);
            position2 = new Board.Move(second);
        }
    }
    public struct Node
    {
        public Node(AI.Board currentboard, Board.Move currentmove, State state)
        {
            gameboard = new AI.Board(currentboard);
            mv = new Board.Move(currentmove);
            currentstate = state;
        }

        public State currentstate;
        public readonly AI.Board gameboard;
        public readonly Board.Move mv;
    }

    //this is necessary for the HashSet to not put duplicates in the set
    public class MoveComparer : EqualityComparer<Board.Move>
    {
        public override bool Equals(Board.Move x, Board.Move y)
        {
            return x == y;
        }

        public override int GetHashCode(Board.Move obj)
        {
            int hash = obj.Row ^ obj.Column ^ obj.Value;
            return hash.GetHashCode();
        }
    }

    //this set is used to make sure we don't back track and get infinite recursion
    HashSet<Board.Move> PlacesBeen;

    public WallValidation()
    {
        PlacesBeen = new HashSet<Board.Move>(new MoveComparer());
    }

    public bool Validate(AI.Board boardSent, Board.Move placement)
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
        Board.Move moveRight;
        Board.Move moveLeft;
        Board.Move moveDown;
        Board.Move moveUp;

        bool result = false;

        if (PlacesBeen.Count == 0)
        {
            PlacesBeen.Add(node.gameboard.PlayerPositions[playerNumber]);
        }

        moveUp = new Board.Move((byte)(node.gameboard.PlayerPositions[playerNumber].Row - 1), node.gameboard.PlayerPositions[playerNumber].Column, 0);
        moveDown = new Board.Move((byte)(node.gameboard.PlayerPositions[playerNumber].Row + 1), node.gameboard.PlayerPositions[playerNumber].Column, 0);
        moveLeft = new Board.Move(node.gameboard.PlayerPositions[playerNumber].Row, (byte)(node.gameboard.PlayerPositions[playerNumber].Column - 1), 0);
        moveRight = new Board.Move(node.gameboard.PlayerPositions[playerNumber].Row, (byte)(node.gameboard.PlayerPositions[playerNumber].Column + 1), 0);


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