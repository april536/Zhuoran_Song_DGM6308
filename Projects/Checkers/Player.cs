namespace Checkers;
//class about player's information
public class Player {
	public bool IsHuman { get; }//if the player is human
	public PieceColor Color { get; }//the color of the player's pieces
	public int Stamina { get; set; }//the stamina of the player
	public int Stimulant { get; set; }//the stamina of the player

	public Player(bool isHuman, PieceColor color) {//constructor about the player's information
		IsHuman = isHuman;
		Color = color;
		Stamina = 2;
		Stimulant = isHuman ? 1 : 0;
	}
}