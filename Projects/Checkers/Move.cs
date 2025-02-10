namespace Checkers;
//class about possible moves
public class Move
{
	public Piece PieceToMove { get; set; }//piece to move

	public (int X, int Y) To { get; set; }//the position to move to

	public Piece? PieceToCapture { get; set; }//piece to capture

	public Move(Piece pieceToMove, (int X, int Y) to, Piece? pieceToCapture = null)//a constructor about properties mentioned above
	{
		PieceToMove = pieceToMove;
		To = to;
		PieceToCapture = pieceToCapture;
	}
}
