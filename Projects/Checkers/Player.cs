namespace Checkers;
//class about player's information
public class Player {
	public bool IsHuman { get; }//if the player is human
	public PieceColor Color { get; }//the color of the player's pieces
	public int Stamina { get; set; }//the stamina of the player
	public int Stimulant { get; set; }//the stamina of the player
	public int UndoChances { get; set; }//the undo chance of the player
	public int MovedNum { get; set; }
	//the time tha player moved in the turn, to prevent player from undo the other player's piece
	public Stack<List<Piece>> PieceSaves { get; } = new Stack<List<Piece>>();//use the LIFO stack to save the pieces' state

	public Player(bool isHuman, PieceColor color) {//constructor about the player's information
		IsHuman = isHuman;
		Color = color;
		Stamina = 2;
		Stimulant = isHuman ? 1 : 0;
		UndoChances = isHuman ? 5 : 0;
		MovedNum = 0;
	}
}