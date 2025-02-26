namespace Checkers;

//the class about board and related methods
public class Board {
    public List<Piece> Pieces { get; set; }//The list of pieces

    public Piece? Aggressor { get; set; }//The piece that can be moving and eat the other's piece

    public Piece? this[int x, int y] =>//A method of board to get the piece at the position x,y
        Pieces.FirstOrDefault(piece => piece.X == x && piece.Y == y);
        //use the method FirstOrDefault to get the first piece at the position x,y

    public Board() {//constructor of class Board
        Aggressor = null;//preset the Aggressor to null
        Pieces = new List<Piece> {//preset the Pieces on the board
                new() { NotationPosition ="A3", Color = Black},
                new() { NotationPosition ="A1", Color = Black},
                new() { NotationPosition ="B2", Color = Black},
                new() { NotationPosition ="C3", Color = Black},
                new() { NotationPosition ="C1", Color = Black},
                new() { NotationPosition ="D2", Color = Black},
                new() { NotationPosition ="E3", Color = Black},
                new() { NotationPosition ="E1", Color = Black},
                new() { NotationPosition ="F2", Color = Black},
                new() { NotationPosition ="G3", Color = Black},
                new() { NotationPosition ="G1", Color = Black},
                new() { NotationPosition ="H2", Color = Black},
                
                new() { NotationPosition ="A7", Color = White},
                new() { NotationPosition ="B8", Color = White},
                new() { NotationPosition ="B6", Color = White},
                new() { NotationPosition ="C7", Color = White},
                new() { NotationPosition ="D8", Color = White},
                new() { NotationPosition ="D6", Color = White},
                new() { NotationPosition ="E7", Color = White},
                new() { NotationPosition ="F8", Color = White},
                new() { NotationPosition ="F6", Color = White},
                new() { NotationPosition ="G7", Color = White},
                new() { NotationPosition ="H8", Color = White},
                new() { NotationPosition ="H6", Color = White}
            };
    }

    //static method to turn the position x,y to the string of position notation
    public static string ToPositionNotationString(int x, int y) {
        if (!IsValidPosition(x, y)) throw new ArgumentException("Not a valid position!");
        //throw an exception if the position is not valid
        return $"{(char)('A' + x)}{y + 1}";//return the string of position notation
    }

    //reverse of the method ToPositionNotationString
    public static (int X, int Y) ParsePositionNotation(string notation) {
        if (notation is null) throw new ArgumentNullException(nameof(notation));
        //throw an exception if the notation is null
        notation = notation.Trim().ToUpper();//trim the space of the notation and turn it to uppercase
        if (notation.Length is not 2 ||
            notation[0] < 'A' || 'H' < notation[0] ||
            notation[1] < '1' || '8' < notation[1])
            throw new FormatException($@"{nameof(notation)} ""{notation}"" is not valid");
            //throw an format exception if the notation is not valid
        return (notation[0] - 'A', notation[1] - '1');//return the position x,y of the notation
    }

    //static method to check if the position x,y is valid
    public static bool IsValidPosition(int x, int y) =>
        0 <= x && x < 8 &&
        0 <= y && y < 8;

    //method to check all nearest pieces of the rival color for the player's piece in this turn
    public (Piece A, Piece B) GetClosestRivalPieces(PieceColor priorityColor) {
        double minDistanceSquared = double.MaxValue;//initialize the min distance to the max value double can present
        (Piece A, Piece B) closestRivals = (null!, null!);//initialize the closest rivals to null
        foreach (Piece a in Pieces.Where(piece => piece.Color == priorityColor)) {
            //go through all pieces of the given player's pieces color
            foreach (Piece b in Pieces.Where(piece => piece.Color != priorityColor)) {
                //go through all pieces of the rival player's pieces color
                (int X, int Y) vector = (a.X - b.X, a.Y - b.Y);//calculate the vector of the two pieces to calculate the distance
                double distanceSquared = vector.X * vector.X + vector.Y * vector.Y;//calculate the distance squared
                if (distanceSquared < minDistanceSquared) {
                    minDistanceSquared = distanceSquared;//update the min distance to min distance squared when it is smaller
                    closestRivals = (a, b);
                }
            }
        }
        return closestRivals;//return the position of closest rivals
    }

    public List<Move> GetPossibleMoves(PieceColor color) {//method to get all possible pieces to move in this turn
        List<Move> moves = new();//create a list of moves
        if (Aggressor is not null) {
            if (Aggressor.Color != color) {
                throw new Exception($"{nameof(Aggressor)} is not null && {nameof(Aggressor)}.{nameof(Aggressor.Color)} != {nameof(color)}");
            }//throw an exception if the aggressor is not null, but the aggressor is not player
            moves.AddRange(GetPossibleMoves(Aggressor).Where(move => move.PieceToCapture is not null));
            //move the aggressor to the list of moves if the aggressor can capture the other's piece
        } 
        
        else {//add all possible moves of the player's pieces to the list, by another method that takes a piece as a parameter
            foreach (Piece piece in Pieces.Where(piece => piece.Color == color)) {
                moves.AddRange(GetPossibleMoves(piece));
            }
        }

        return moves.Any(move => move.PieceToCapture is not null) ? moves.Where(move => move.PieceToCapture is not null).ToList() : moves;
            //return the list of moves either can capture the other's piece, or moving nearby when no piece can capture
    }

    public List<Move> GetPossibleMoves(Piece piece) {//method to get all possible moves diagonal for the piece
        List<Move> moves = new();
        ValidateDiagonalMove(-1, -1);
        ValidateDiagonalMove(-1,  1);
        ValidateDiagonalMove( 1, -1);
        ValidateDiagonalMove( 1,  1);
        return moves.Any(move => move.PieceToCapture is not null) ? moves.Where(move => move.PieceToCapture is not null).ToList() : moves;
        //return the list of moves either can capture the other's piece, or moving nearby when no piece can capture

        void ValidateDiagonalMove(int dx, int dy) {//method to check if the diagonal move is valid, and add the move to the list of moves
            if (!piece.Promoted && piece.Color is Black && dy is -1) return;
            if (!piece.Promoted && piece.Color is White && dy is 1) return;
            //normal piece can only move forward diagonaled, else the move is invalid

            (int X, int Y) target = (piece.X + dx, piece.Y + dy);//initialize target position of the piece
            if (!IsValidPosition(target.X, target.Y)) return;//if the target position is not valid, return

            //if the target position is empty, add the move to the list of moves
            PieceColor? targetColor = this[target.X, target.Y]?.Color;//get the color of the target position
            if (targetColor is null) {//only can move an empty position
                if (!IsValidPosition(target.X, target.Y)) return;//if the target position is not valid, return
                Move newMove = new(piece, target);
                moves.Add(newMove);//add the move to the list of moves
            } 
            //add an attack move to the list of moves if the target position is \empty and valid
            else if (targetColor != piece.Color) {
                (int X, int Y) jump = (piece.X + 2 * dx, piece.Y + 2 * dy);//if the target position is not empty, calculate the jump position
                if (!IsValidPosition(jump.X, jump.Y)) return;//if the jump position is not valid, return
                PieceColor? jumpColor = this[jump.X, jump.Y]?.Color;//get the color of the piece at the jump position
                if (jumpColor is not null) return;//if the jump position is not empty, return
                Move attack = new(piece, jump, this[target.X, target.Y]);//add an attack move to the list of moves
                moves.Add(attack);
            }
        }
    }

    //method to validate the move of the player
    public Move? ValidateMove(PieceColor color, (int X, int Y) from, (int X, int Y) to) {
        Piece? piece = this[from.X, from.Y];
        if (piece is null) {
            return null;
        }//if there's no piece at the from position, return null
        foreach (Move move in GetPossibleMoves(color)) {//go through all possible moves of the current player
            if ((move.PieceToMove.X, move.PieceToMove.Y) == from && move.To == to) {
                return move;//if the move selected by the user is valid, return the move
            }
        }
        return null;
    }

    //method to check if the move is towards the given position of the piece
    public static bool IsTowards(Move move, Piece piece) {
        (int Dx, int Dy) a = (move.PieceToMove.X - piece.X, move.PieceToMove.Y - piece.Y);//vector of the piece to move
        int a_distanceSquared = a.Dx * a.Dx + a.Dy * a.Dy;//distance squared of the piece to move
        (int Dx, int Dy) b = (move.To.X - piece.X, move.To.Y - piece.Y);
        int b_distanceSquared = b.Dx * b.Dx + b.Dy * b.Dy;//distance squared of the target position
        return b_distanceSquared < a_distanceSquared;//return if the target position is closer to the piece
    }
}