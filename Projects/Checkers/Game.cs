namespace Checkers;
//the class about game core mechanics
public class Game {
    private const int PiecesPerColor = 12;//initial number of pieces per color as private constant

    //authorized method to get and set properties of the class Game
    public PieceColor Turn { get; private set; }//color of the current player
    public Board Board { get; }//game board
    public PieceColor? Winner { get; private set; }//color of the winner
    public List<Player> Players { get; }//list of players

    public Game(int humanPlayerCount) {//initialize the game player, board, turn, and winner
        if (humanPlayerCount < 0 || 2 < humanPlayerCount) throw new ArgumentOutOfRangeException(nameof(humanPlayerCount));
        //throw an exception if the human player count is less than 0 or more than 2
        Board = new Board();
        Players = new() {//initialize the list of players
            new Player(humanPlayerCount >= 1, Black),//add a black piece player to the list if the human player count equals 1 or more
            new Player(humanPlayerCount >= 2, White),//add a white piece player to the list if the human player count equals 2 or more
        };
        Turn = Black;
        Winner = null;
    }

    public void PerformMove(Move move, Game game) {//perform the move of the player from the move list from logic
        Player currentPlayer = game.Players.First(player => player.Color == game.Turn);//get the player who's color is same as the current turn
        //move the piece to the target position, giving the target position's x,y to the piece's x,y
        (move.PieceToMove.X, move.PieceToMove.Y) = move.To;

        //if the piece reaches the end of the board, promote the piece
        if ((move.PieceToMove.Color is Black && move.To.Y is 7) ||
            (move.PieceToMove.Color is White && move.To.Y is 0)) {
            move.PieceToMove.Promoted = true;
        }
        //if the piece captures the other's piece, remove the captured piece from the board
        if (move.PieceToCapture is not null) {
            Board.Pieces.Remove(move.PieceToCapture);
        }
        //if the piece captures the other's piece, and the piece can capture another piece, set the aggressor to the piece
        if (move.PieceToCapture is not null &&
            Board.GetPossibleMoves(move.PieceToMove).Any(m => m.PieceToCapture is not null)) {
            Board.Aggressor = move.PieceToMove;
            //set the aggressor to the piece that can capture another piece, make it can gather the most pieces
        } 
        //if there's no more pieces to be captured, set the aggressor to null and change the turn to the other player
        else {
            Board.Aggressor = null;
            if(currentPlayer.Stamina == 1) Turn = Turn is Black ? White : Black;
        }
        CheckForWinner();//check if there's a winner after the move
        if(currentPlayer.Stamina != 0 && move.PieceToMove != game.Board.Aggressor) {
            currentPlayer.Stamina --;//reduce the player's stamina by 1n when no more pieces can be captured
        }
    }

    public void CheckForWinner() {//check if there's a winner after the move        
        if (!Board.Pieces.Any(piece => piece.Color is Black)) {//if there's no black piece on the board, white wins
            Winner = White;
        }
        if (!Board.Pieces.Any(piece => piece.Color is White)) {//if there's no white piece on the board, black wins
            Winner = Black;
        }
        if (Winner is null && Board.GetPossibleMoves(Turn).Count is 0) {
            //if there's no winner, and the current player has no possible moves, the other player wins
            Winner = Turn is Black ? White : Black;
        }
    }

    public int TakenCount(PieceColor colour) =>//method to count the number of pieces taken by the otjer player
        PiecesPerColor - Board.Pieces.Count(piece => piece.Color == colour);
}