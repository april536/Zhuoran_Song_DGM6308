Exception? exception = null;//catches exceptions and displays them

Encoding encoding = Console.OutputEncoding;//initializes the console encoding

try {
    Console.OutputEncoding = Encoding.UTF8;
    Game game = ShowIntroScreenAndGetOption();//stores the current console encoding UTF8
    Console.Clear();
    RunGameLoop(game);//runs the game loop and pass the game object
    RenderGameState(game, promptPressKey: true);//renders the game state
    Console.ReadKey(true);
} catch (Exception e) {
    exception = e;
    throw;
} finally {//set the game ending conditions
    Console.OutputEncoding = encoding;
    Console.CursorVisible = true;
    Console.Clear();
    Console.WriteLine(exception?.ToString() ?? "Checkers was closed.");
}

Game ShowIntroScreenAndGetOption() {//show the game introduction screen
    Console.Clear();
    Console.WriteLine();
    Console.WriteLine("  Checkers");
    Console.WriteLine();
    Console.WriteLine("  Checkers is played on an 8x8 board between two sides commonly known as black");
    Console.WriteLine("  and white. The objective is simple - capture all your opponent's pieces. An");
    Console.WriteLine("  alternative way to win is to trap your opponent so that they have no valid");
    Console.WriteLine("  moves left.");
    Console.WriteLine();
    Console.WriteLine("  Black starts first and players take it in turns to move their pieces forward");
    Console.WriteLine("  across the board diagonally. Should a piece reach the other side of the board");
    Console.WriteLine("  the piece becomes a king and can then move diagonally backwards as well as");
    Console.WriteLine("  forwards.");
    Console.WriteLine();
    Console.WriteLine("  Pieces are captured by jumping over them diagonally. More than one enemy piece");
    Console.WriteLine("  can be captured in the same turn by the same piece. If you can capture a piece");
    Console.WriteLine("  you must capture a piece.");
    Console.WriteLine();
    Console.WriteLine("  Moves are selected with the arrow keys. Use the [enter] button to select the");
    Console.WriteLine("  from and to squares. Invalid moves are ignored.");
    Console.WriteLine();
    Console.WriteLine("  Press a number key to choose number of human players:");
    Console.WriteLine("    [0] Black (computer) vs White (computer)");
    Console.WriteLine("    [1] Black (human) vs White (computer)");
    Console.Write("    [2] Black (human) vs White (human)");

    int? humanPlayerCount = null;//initialize the human player count
    while (humanPlayerCount is null) {//only get out of the loop when the human player count is not null
        Console.CursorVisible = false;
        switch (Console.ReadKey(true).Key) {//only get out of the loop when the player pressed 012
            case ConsoleKey.D0 or ConsoleKey.NumPad0: humanPlayerCount = 0; break;
            case ConsoleKey.D1 or ConsoleKey.NumPad1: humanPlayerCount = 1; break;
            case ConsoleKey.D2 or ConsoleKey.NumPad2: humanPlayerCount = 2; break;
        }
    }
    return new Game(humanPlayerCount.Value);//return the game object with the human player count
}

void RunGameLoop(Game game) {//the main game loop
    while (game.Winner is null) {//run the loop until there is a winner
        Player currentPlayer = game.Players.First(player => player.Color == game.Turn);//get the player who's color is same as the current turn
        if (currentPlayer.IsHuman) {
            while (game.Turn == currentPlayer.Color) {//run the loop until the current player's turn is over
                (int X, int Y)? selectionStart = null;
                (int X, int Y)? from = game.Board.Aggressor is not null ? (game.Board.Aggressor.X, game.Board.Aggressor.Y) : null;//get the aggressor piece

                List<Move> moves = game.Board.GetPossibleMoves(game.Turn);//get the possible moves from the games cs in this turn
                if (moves.Select(move => move.PieceToMove).Distinct().Count() is 1) {//check if there is only one piece that can move
                    Move must = moves.First();//define the dirst move the player can take as must
                    from = (must.PieceToMove.X, must.PieceToMove.Y);//define the "must" move as the piece that going to move
                    selectionStart = must.To;//giving the default first move as the must move's destination
                }

                while (from is null) {//let the player select the piece to move
                    from = HumanMoveSelection(game);
                    selectionStart = from;
                }

                //let the player select the destination to move, take the game information, selection start and from position as parameter
                (int X, int Y)? to = HumanMoveSelection(game, selectionStart: selectionStart, from: from);
                Piece? piece = null;//initialize the piece as null
                piece = game.Board[from.Value.X, from.Value.Y];//get the position selected by the player
                
                if (piece is null || piece.Color != game.Turn) {//if the piece is null or the piece's color is not the same as the current player's turn
                    from = null;
                    to = null;
                }//reset the from and to position

                if (from is not null && to is not null) {//if this move is valid, perform the move
                    Move? move = game.Board.ValidateMove(game.Turn, from.Value, to.Value);//check if the move is valid
                    if (move is not null &&//if the move is not null and the piece to move is the aggressor or the aggressor is null
                        (game.Board.Aggressor is null || move.PieceToMove == game.Board.Aggressor)) {
                        game.PerformMove(move);//perform the move
                    }
                }
            }
        } else {//if it is computer's turn
            List<Move> moves = game.Board.GetPossibleMoves(game.Turn);//get the possible moves from the games cs in this turn
            List<Move> captures = moves.Where(move => move.PieceToCapture is not null).ToList();//initialize the possible move with captures list
            
            if (captures.Count > 0) {//if there is a capture move, get a random move from the capture list
                game.PerformMove(captures[Random.Shared.Next(captures.Count)]);
            } 
            else if(!game.Board.Pieces.Any(piece => piece.Color == game.Turn && !piece.Promoted)) {
                //if there is no capture move, and there's no current player piece have been promoted
                var (a, b) = game.Board.GetClosestRivalPieces(game.Turn);//get the closest rival pieces
                Move? priorityMove = moves.FirstOrDefault(move => move.PieceToMove == a && Board.IsTowards(move, b));
                //the priority move is the first move in the list that can move the piece to the closest rival piece
                game.PerformMove(priorityMove ?? moves[Random.Shared.Next(moves.Count)]);
                //perform the move defined, if it is null, get a random move from the moves list
            } 
            else {
                game.PerformMove(moves[Random.Shared.Next(moves.Count)]);
                //if there is no capture move, and there's a current player piece have been promoted, get a random move from the moves list
            }
        }

        RenderGameState(game, playerMoved: currentPlayer, promptPressKey: true);//render the game state with the current player moved
        Console.ReadKey(true);
    }
}

//render the game state with the game information, player moved, selection, from position, and prompt press key as parameter
void RenderGameState(Game game, Player? playerMoved = null, (int X, int Y)? selection = null, (int X, int Y)? from = null, bool promptPressKey = false) {
    const char BlackPiece = '○';
    const char BlackKing  = '☺';
    const char WhitePiece = '◙';
    const char WhiteKing  = '☻';
    const char Vacant     = '·';//the token of the vacant position

    Console.CursorVisible = false;
    Console.SetCursorPosition(0, 0);
    StringBuilder sb = new();//initialize the string builder
    sb.AppendLine();
    sb.AppendLine("  Checkers");
    sb.AppendLine();
    sb.AppendLine($"    ╔═══════════════════╗");
    sb.AppendLine($"  8 ║  {B(0, 7)} {B(1, 7)} {B(2, 7)} {B(3, 7)} {B(4, 7)} {B(5, 7)} {B(6, 7)} {B(7, 7)}  ║ {BlackPiece} = Black");
    sb.AppendLine($"  7 ║  {B(0, 6)} {B(1, 6)} {B(2, 6)} {B(3, 6)} {B(4, 6)} {B(5, 6)} {B(6, 6)} {B(7, 6)}  ║ {BlackKing} = Black King");
    sb.AppendLine($"  6 ║  {B(0, 5)} {B(1, 5)} {B(2, 5)} {B(3, 5)} {B(4, 5)} {B(5, 5)} {B(6, 5)} {B(7, 5)}  ║ {WhitePiece} = White");
    sb.AppendLine($"  5 ║  {B(0, 4)} {B(1, 4)} {B(2, 4)} {B(3, 4)} {B(4, 4)} {B(5, 4)} {B(6, 4)} {B(7, 4)}  ║ {WhiteKing} = White King");
    sb.AppendLine($"  4 ║  {B(0, 3)} {B(1, 3)} {B(2, 3)} {B(3, 3)} {B(4, 3)} {B(5, 3)} {B(6, 3)} {B(7, 3)}  ║");
    sb.AppendLine($"  3 ║  {B(0, 2)} {B(1, 2)} {B(2, 2)} {B(3, 2)} {B(4, 2)} {B(5, 2)} {B(6, 2)} {B(7, 2)}  ║ Taken:");
    sb.AppendLine($"  2 ║  {B(0, 1)} {B(1, 1)} {B(2, 1)} {B(3, 1)} {B(4, 1)} {B(5, 1)} {B(6, 1)} {B(7, 1)}  ║ {game.TakenCount(White),2} x {WhitePiece}");
    sb.AppendLine($"  1 ║  {B(0, 0)} {B(1, 0)} {B(2, 0)} {B(3, 0)} {B(4, 0)} {B(5, 0)} {B(6, 0)} {B(7, 0)}  ║ {game.TakenCount(Black),2} x {BlackPiece}");
    sb.AppendLine($"    ╚═══════════════════╝");
    sb.AppendLine($"       A B C D E F G H");
    sb.AppendLine();
    if (selection is not null) {//use the bracket to show the selected position if the selection is not null
        sb.Replace(" $ ", $"[{ToChar(game.Board[selection.Value.X, selection.Value.Y])}]");
    }
    if (from is not null) {//search for the selected position and give a <> at the outside of the position
        char fromChar = ToChar(game.Board[from.Value.X, from.Value.Y]);
        sb.Replace(" @ ", $"<{fromChar}>");
        sb.Replace("@ ",  $"{fromChar}>");
        sb.Replace(" @",  $"<{fromChar}");
    }
    PieceColor? wc = game.Winner;//get the color of all needed parameters
    PieceColor? mc = playerMoved?.Color;
    PieceColor? tc = game.Turn;
    // Note: these strings need to match in length
    // so they overwrite each other.
    string w = $"  *** {wc} wins ***";
    string m = $"  {mc} moved       ";
    string t = $"  {tc}'s turn      ";
    sb.AppendLine(//show the string needed in different conditions
        game.Winner is not null ? w :
        playerMoved is not null ? m :
        t);
    string p = "  Press any key to continue...";
    string s = "                              ";
    sb.AppendLine(promptPressKey ? p : s);
    Console.Write(sb);

    char B(int x, int y) =>
        (x, y) == selection ? '$' ://if the x,y is selected position, the position character is rendered as $
        (x, y) == from ? '@' ://make the selected from position character substituted as @, to perform the <>
        ToChar(game.Board[x, y]);//show it on the board

    static char ToChar(Piece? piece) =>//render the pieces on the board with give token
        piece is null ? Vacant :
        (piece.Color, piece.Promoted) switch {
            (Black, false) => BlackPiece,
            (Black, true)  => BlackKing,
            (White, false) => WhitePiece,
            (White, true)  => WhiteKing,
            _ => throw new NotImplementedException(),
        };
}

//a position deal with human selected moving destination that takes game information, selection start and from position as parameter
(int X, int Y)? HumanMoveSelection(Game game, (int X, int y)? selectionStart = null, (int X, int Y)? from = null) {
    (int X, int Y) selection = selectionStart ?? (3, 3);//initislize the selection position as 3,3 if it is null
    while (true) {
        RenderGameState(game, selection: selection, from: from);//render game state with the given parameter
        switch (Console.ReadKey(true).Key) {//get the uers key input to find the user's selection
            case ConsoleKey.DownArrow:  selection.Y = Math.Max(0, selection.Y - 1); break;
            case ConsoleKey.UpArrow:    selection.Y = Math.Min(7, selection.Y + 1); break;
            case ConsoleKey.LeftArrow:  selection.X = Math.Max(0, selection.X - 1); break;
            case ConsoleKey.RightArrow: selection.X = Math.Min(7, selection.X + 1); break;
            case ConsoleKey.Enter:      return selection;//only return the selection when the user press enter
            case ConsoleKey.Escape:     return null;
        }
    }
}