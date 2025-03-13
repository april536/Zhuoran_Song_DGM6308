/**********************************************************************************************************
                                            Preset
**********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
List<Card> remainDeck = new List<Card>();//build a new list to store all the cards
List<Card> discardDeck = new();//a new list to store all the discarded cards
bool closeRequested = false;
int winningScore = 0;
State state = State.MainMenu;
GameMode gameMode = GameMode.PVP;
Player player1 = new Player();
Player player2 = new Player();
List<Player> PlayerTurns = new();
/**********************************************************************************************************
                                     main game flow
**********************************************************************************************************/
try {
    while (!closeRequested) {
        Gameloop();
    }
} finally {
    Console.WriteLine("Esc pressed. Game closed.");
}

//The main game loop
void Gameloop() {
    switch (state) {
        case State.MainMenu:
            ShowMainMenu();
            break;
        case State.ShowCardSkills:
            ShowCardSkills();
            break;
        case State.RandomizeTurn:
            RandomizeTurn();
            break;
        case State.DealCards:
            DealCards();
            break;
        case State.PlayCards:
            PlayCards();
            break;
        case State.ShowWinner:
            ShowWinner();
            break;
    }
}

//randomize the turn
void RandomizeTurn() {
    InitializeDeck();
    if (remainDeck.Count < 4) RefreshDeck();
    Console.Clear();
    Console.WriteLine("Randomizing turn...");

    if (gameMode == GameMode.PVP) {
        player1.Name = "Player 1";
        player2.Name = "Player 2";
    } else {
        player1.Name = "Player";
        player2.Name = "CPU";
    }
    player1.Gofirst = new Random().Next(2) == 0;
    player2.Gofirst = !player1.Gofirst;

    Player firstPlayer = player1.Gofirst ? player1 : player2;
    Player secondPlayer = player1.Gofirst ? player2 : player1;
    PlayerTurns.Add(firstPlayer);
    PlayerTurns.Add(secondPlayer);
    Console.WriteLine($"{firstPlayer.Name} goes first!");

    Console.WriteLine("Press Enter to continue");
    ChangeState(State.DealCards);
}

//deal cards for both players
void DealCards() {
    Console.Clear();
    Console.WriteLine("Dealing cards for both players...");

    for(int i = 0; i < 2; i++) {
        PlayerDrawCards(PlayerTurns[i].Hand);
        RenderCards(PlayerTurns[i].Hand);
    }

    Console.WriteLine($"{remainDeck.Count} cards left in the deck"); // Render the hint
    Console.WriteLine("Press Enter to continue");
    ChangeState(State.PlayCards);
}

//have both of the player to play the cards
void PlayCards() {
    while (player1.Score < winningScore && player2.Score < winningScore) {
        Console.Clear();
        Console.WriteLine("Playing cards...while the game is not over");
        for(int i = 0; i < 2; i++) {
            if (PlayerTurns[i].isHuman) HumanTurn(PlayerTurns[i]);
            else CPUTurn(PlayerTurns[i]);
        }
        CountScore();
    }
    Console.WriteLine("Playing cards...and there's a winner");
    ChangeState(State.ShowWinner);
}

//if the player is human, let the player to choose the skill
void HumanTurn(Player currentPlayer) {
    Console.Clear();
    if (gameMode == GameMode.PVP) Console.WriteLine($"{currentPlayer}'s turn! opponent please avert eyes");
    else Console.WriteLine("Your turn! press Y to execute skill, or press N to use it as value card");

    currentPlayer.isPlayering = true;
    RenderCards(currentPlayer.Hand);
    UseSkill(currentPlayer);
    currentPlayer.isPlayering = false;
}

//randomize the skill for the CPU
void CPUTurn(Player currentPlayer) {
    Console.Clear();
    Console.WriteLine("{currentPlayer}'s turn! Player please avert eyes");

    RenderCards(currentPlayer.Hand);
    Thread.Sleep(1000);
}

//testing the skill of the card
void UseSkill(Player currentPlayer) {
    Console.WriteLine("Press 1 to score + 1, or press 2 keep the value");
    var key = Console.ReadKey(true).Key;
    if (key == ConsoleKey.D1) {
        currentPlayer.Score += 1;
    } else if (key == ConsoleKey.D2) {
        currentPlayer.Score += 0;
    }
}

//count the score of the player
void CountScore() {
    Console.Clear();
    Console.WriteLine("Counting score...");
    Console.WriteLine("Press Enter to continue");
}
/**********************************************************************************************************
                                    Secondary Methods
**********************************************************************************************************/
//change the state of the game
void ChangeState(State newState) {
    var key = Console.ReadKey(true).Key;
    if (key == ConsoleKey.Enter) {
        state = newState;
    } else if (key == ConsoleKey.Escape) {
        closeRequested = true;
    }
}

//initialize the deck for 52 cards
void InitializeDeck() {
        foreach (Suit suit in Enum.GetValues<Suit>()) {
        foreach (Value value in Enum.GetValues<Value>()) {
            remainDeck.Add(new Card { Suit = suit, Value = value });
        }
    }
}

//refresh the deck
void RefreshDeck() {
    // Shuffle the discard pile back into the remainDeck when remainDeck is empty
        remainDeck.AddRange(discardDeck);
        discardDeck.Clear();
        Shuffle(remainDeck);
}

//shuffle the cards
static void Shuffle(List<Card> cards) { // Method to shuffle the cards
    Random rand = new Random();
    for (int i = 0; i < cards.Count; i++) {
        int swap = rand.Next(cards.Count);
        (cards[i], cards[swap]) = (cards[swap], cards[i]);
    }
}

//draw cards for the player
void PlayerDrawCards(List<Card> playerHand) {
    for (int i = 0; i < 2; i++){
        var card = remainDeck[^1]; // Get the last card in the list
        remainDeck.RemoveAt(remainDeck.Count - 1); // Remove it from the remainDeck
        playerHand.Add(card);
    }
}
/**********************************************************************************************************
                                    Render
**********************************************************************************************************/
//show the winner of the game
void ShowWinner() {
    Console.Clear();
    if (player1.Score > player2.Score) Console.WriteLine($"{player1.Name} wins!");
    else if (player1.Score == player2.Score) Console.WriteLine($"Draw!");
    else Console.WriteLine($"{player2.Name} wins!");
    Console.WriteLine("Final Score:");
    Console.WriteLine($"{player1.Name}: {player1.Score}");
    Console.WriteLine($"{player2.Name}: {player2.Score}");
    Console.WriteLine();
    Console.WriteLine("Press Enter to continue");
    ChangeState(State.MainMenu);
}

//show the main menu
void ShowMainMenu() {
    Console.Clear();
    Console.WriteLine("War Game");
    Console.WriteLine("This is a doule-player stratigic card game!");
    Console.WriteLine("Both players draw 2 cards at start of the turn.");
    Console.WriteLine("The first card's is value card, it's value shall be count into the total value of this turn.");
    Console.WriteLine("The second card can be used as either a value card or a skill card.");
    Console.WriteLine("   [1. Value card would be added to the value of this turn.]");
    Console.WriteLine("   [2. Skill card would trigger the skill related to it's suit.]");
    Console.WriteLine("Player who holds the higher total value wins the turn and earns score.");
    Console.WriteLine("Game ends when one player's score reaches 10!");
    Console.WriteLine("   [1. Press 1 to stard PVP Mode]");
    Console.WriteLine("   [2. Press 2 to start PVE Mode]");
    Console.WriteLine("   [3. Press 3 to check the skills of cards]");
    Console.WriteLine();
    Console.WriteLine("Press Esc to exit");
    switch (Console.ReadKey(true).Key) {
        case ConsoleKey.D1:
            gameMode = GameMode.PVP;
            state = State.RandomizeTurn;
            break;
        case ConsoleKey.D2:
            gameMode = GameMode.PVE;
            state = State.RandomizeTurn;
            break;
        case ConsoleKey.D3:
            state = State.ShowCardSkills;
            break;
        case ConsoleKey.Escape:
            closeRequested = true;
            break;
    }
}

//show the explaination of the cards skills
void ShowCardSkills() {
    Console.Clear();
    Console.WriteLine(@"
Skill Cards:
Hearts: 
During scoring, the player with the higher total value gains 2 points instead of 1. This bonus is not stackable and has no effect during ties.
Spades: 
During scoring, the player with the lower total value loses 1 point. This penalty is not stackable and has no effect during ties.
Diamonds: 
Doubles the face value of the player's other card.
Clubs: 
Discards this skill card and draws a replacement card. The new card must be used for its face value and added to the other card for the final total.

Special Cards:
Hearts 8: 
During scoring, if your value is higher, gain 2 points. No effect during ties.
Spades 8: 
During scoring, if your opponent's value is lower, they lose 2 points. No effect during ties.
Diamonds 8: 
Triples the face value of the player's other card.
Clubs 8: 
Discards this skill card and draws two replacement cards. The higher-value card must be used for its face value and added to the other card for the final total.
Joker (Small): 
Adds -1 point to your opponent during scoring, with a fixed face value of 15.
Joker (Big): 
Adds +2 points to yourself during scoring, with a fixed face value of 16.
Both Jokers Drawn in One Round: 
The player instantly wins with 10 points, regardless of other rules.
    ");
    Console.WriteLine("Press Enter to return to the main menu");
    ChangeState(State.MainMenu);
}

//render the cards
void RenderCards(List<Card> playerHands) {
    foreach (var card in playerHands) {
        if (state == State.DealCards) Console.WriteLine(card.RenderFacedown());
        if (state == State.PlayCards) {
            foreach (var player in PlayerTurns) {
                if (player.isPlayering) Console.WriteLine(card.RenderFaceUp());
                else Console.WriteLine(card.RenderFacedown());
            }
        }
    }
}

//class to store the player's information
class Player {
    public string Name { get; set; } = string.Empty;
    public bool Gofirst { get; set; }
    public bool isHuman { get; set; } = true;
    public bool isPlayering { get; set; } = false;
    public int Score { get; set; } = 0;
    public List<Card> Hand { get; set; } = new List<Card>();
}

//class to store the card's information
class Card {
    // A class of cards with two kinds of properties.
    public Suit Suit { get; set; }
    public Value Value { get; set; }

    public string RenderFaceUp()
    {
        char suit = Suit.ToString()[0];
        string value = Value switch
        {
            Value.Ace => "A",
            Value.Ten => "10",
            Value.Jack => "J",
            Value.Queen => "Q",
            Value.King => "K",
            _ => ((int)Value).ToString(CultureInfo.InvariantCulture),
        };
        string card = $"{value}{suit}";
        string a = card.Length < 3 ? $"{card} " : card;
        string b = card.Length < 3 ? $" {card}" : card;
        int score = (int)Value;

        var builder = new StringBuilder();
        builder.AppendLine("┌───────┐");
        builder.AppendLine($"│{a}    │");
        builder.AppendLine("│       │");
        builder.AppendLine("│       │");
        builder.AppendLine("│       │");
        builder.AppendLine($"│    {b}│");
        builder.AppendLine("└───────┘");

        return builder.ToString();
    }
    
    public string RenderFacedown() {
        var builder = new StringBuilder();
        builder.AppendLine("┌───────┐");
        builder.AppendLine("│███████│");
        builder.AppendLine("│███████│");
        builder.AppendLine("│███████│");
        builder.AppendLine("│███████│");
        builder.AppendLine("│███████│");
        builder.AppendLine("└───────┘");
        return builder.ToString();
    }
}

// The card’s suit.
enum Suit
{
    // The card’s suit.
    Hearts,
    Clubs,
    Spades,
    Diamonds
}

// The card’s value.
enum Value
{
    // The card’s value.
    Ace = 14,
    Two = 02,
    Three = 03,
    Four = 04,
    Five = 05,
    Six = 06,
    Seven = 07,
    Eight = 08,
    Nine = 09,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13
}

//The game’s state.
enum State {
    // The game’s state.
    MainMenu,
    ShowCardSkills,
    RandomizeTurn,
    DealCards,
    PlayCards,
    ShowWinner,
}

//decide the game’s mode.
enum GameMode {
    // The game’s mode.
    PVP,
    PVE
}