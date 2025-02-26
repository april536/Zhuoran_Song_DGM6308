namespace Checkers;
//the class about game core mechanics
public class Game {
    private const int PiecesPerColor = 12;//initial number of pieces per color as private constant
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
        
        //if the current player has stamina, and the piece to move is not the aggressor, save the piece state
        if (currentPlayer.Stamina != 0 && move.PieceToMove != game.Board.Aggressor) {
            currentPlayer.PieceSaves.Push(game.SavePieceState());//add the current state of the pieces to the stack
        }

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
            if(currentPlayer.Stamina == 1) Turn = Turn is Black ? White : Black;//switch the turn to the other player
        }
        CheckForWinner();//check if there's a winner after the move
        if(currentPlayer.Stamina != 0 && move.PieceToMove != game.Board.Aggressor) {
            currentPlayer.Stamina --;//reduce the player's stamina by 1n when no more pieces can be captured
            currentPlayer.MovedNum ++;//increase the player's moved number by 1, to prevent the player from undo the other player's piece
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

    public List<Piece> SavePieceState() {//save the current state of the pieces on the board， return the list with information of all pieces
        return Board.Pieces.Select//a methoud to give all the pieces on the board a new copy using the transform function
        (p => new Piece {
            X = p.X,
            Y = p.Y,
            Color = p.Color,
            Promoted = p.Promoted
        })//transform function to give the piece a new copy, p is the piece now on the board
        .ToList();//transform the sequence containing the pieces to a list
    }

    public void RestorePieceState(List<Piece> savedPieces) {//method to restore the pieces' state
        Board.Pieces.Clear();//clear the pieces on the board
        Board.Pieces.AddRange(savedPieces);//add the saved pieces to the board using AddRange method, which adds the elements of the specified collection to the end of the List<T>
        Board.Aggressor = null;//clear the aggressor
    }
}