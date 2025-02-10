namespace Checkers;
//a class about the properties of a piece
public class Piece
{
	public int X { get; set; }//x coordinate

	public int Y { get; set; }//y coordinate

	public string NotationPosition {//set the method to get the position or parse it of the piece
		get => Board.ToPositionNotationString(X, Y);
		set => (X, Y) = Board.ParsePositionNotation(value);
	}

	public PieceColor Color { get; init; }//color of the piece

	public bool Promoted { get; set; }//if the piece is promoted as king
}
