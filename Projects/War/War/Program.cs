﻿//Reference
//Card value and patern and rendering: https://github.com/dotnet/dotnet-console-games/blob/main/Projects/Blackjack/Program.cs
/**********************************************************************************************************
                                            Preset
**********************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Security.Cryptography.X509Certificates;
List<Card> remainDeck = new List<Card>();//build a new list to store all the cards
List<Card> discardDeck = new();//a new list to store all the discarded cards
bool closeRequested = false;//a bool to check if the game is closed
bool endOfRound = false;//a bool to check if the round is ended
int winningScore = 5;//the score to win the game
int RoundWinnerBonus = 1;//the bonus for the winner of the round
int RoundLoserPenalty = 0;//the penalty for the loser of the round
State state = State.MainMenu;//the state of the game
GameMode gameMode = GameMode.PVP;//the mode of the game
Player player1 = new Player();
Player player2 = new Player();
List<Player> PlayerTurns = new();//a list to store the players

/**********************************************************************************************************
                                        main game flow
**********************************************************************************************************/
//The main game loop
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
    Console.Clear();
    InitializeDeck();
    Console.WriteLine("Randomizing turn...");
    DefinePlayers();
    DecideTurns();
    Console.WriteLine($"{PlayerTurns[0].Name} goes first!");
    Console.WriteLine("Press Enter to continue");
    ChangeState(State.DealCards);
}

//deal cards for both players
void DealCards() {
    Console.Clear();
    Console.WriteLine("Dealing cards for both players...");
    DealAndRenderCards();
    Console.WriteLine($"{remainDeck.Count} cards left in the deck");
    Console.WriteLine("Press Enter to continue");
    ChangeState(State.PlayCards);
}

//have both of the player to play the cards
void PlayCards() {
    while (player1.Score < winningScore && player2.Score < winningScore) {
        Console.Clear();
        GameRound();
    }
}

//show the winner of the game
void ShowWinner() {
    Console.Clear();
    if (player1.Score > player2.Score) Console.WriteLine($"{player1.Name} wins!");
    else if (player1.Score == player2.Score) Console.WriteLine($"Draw!");
    else Console.WriteLine($"{player2.Name} wins!");
    Console.WriteLine("Final Score:");
    Console.WriteLine($"{player1.Name} : {player1.Score}");
    Console.WriteLine($"{player2.Name} : {player2.Score}");
    Console.WriteLine();
    ClearAll();
    Console.WriteLine("Press Enter to return to the main menu");
    ChangeState(State.MainMenu);
}
/**********************************************************************************************************
                                    Secondary Methods
**********************************************************************************************************/
//define the players
void DefinePlayers() {
 if (gameMode == GameMode.PVP) {
        player1.Name = "Player 1";
        player1.isHuman = true;
        player2.Name = "Player 2";
        player2.isHuman = true;
    } else {
        player1.Name = "Player";
        player1.isHuman = true;
        player2.Name = "CPU";
        player2.isHuman = false;
    }
}

//decide the turns of the players
void DecideTurns() {
    player1.Gofirst = new Random().Next(2) == 0;
    player2.Gofirst = !player1.Gofirst;

    Player firstPlayer = player1.Gofirst ? player1 : player2;
    Player secondPlayer = player1.Gofirst ? player2 : player1;
    PlayerTurns.Add(firstPlayer);
    PlayerTurns.Add(secondPlayer);
}

//loop for the game round
void GameRound() {
    for(int i = 0; i < 2; i++) {
        if (PlayerTurns[i].isHuman) HumanTurn(PlayerTurns[i]);
        else CPUTurn(PlayerTurns[i]);
        Console.WriteLine($"{remainDeck.Count} cards left in the deck");
    }
    RefreshHandCards(player1);
    RefreshHandCards(player2);
    CountScore();
}

//count the score of the player
void CountScore() {
    endOfRound = true;
    Console.Clear();
    RenderCards();
    Console.WriteLine("");
    foreach (var player in PlayerTurns) {
        if (player.usedSkill) Console.WriteLine($"{player.Name} used the skill card!");
        else Console.WriteLine($"{player.Name} didn't use the skill card.");
    }
    Console.WriteLine("");
    foreach (var player in PlayerTurns) Console.WriteLine($"{player.Name}: {player.RoundValue}");
    CheckForWinner();
    PressEnterToContinue();
    endOfRound = false;
}

//check for the winner of the round
void CheckForWinner() {
    if (player1.RoundValue > player2.RoundValue) {
        Console.WriteLine($"{player1.Name} wins the round!");
        player1.Score += RoundWinnerBonus;
        player2.Score -= RoundLoserPenalty;
    } else if (player1.RoundValue < player2.RoundValue) {
        Console.WriteLine($"{player2.Name} wins the round!");
        player2.Score += RoundWinnerBonus;
        player1.Score -= RoundLoserPenalty;
    } else {
        Console.WriteLine("It's a tie!");
    }
    Console.WriteLine("Press Enter to continue");

    if (player1.Score < 0) player1.Score = 0;
    if (player2.Score < 0) player2.Score = 0;

    if (player1.Score >= winningScore || player2.Score >= winningScore) {
        ChangeState(State.ShowWinner);
    } else {
        Console.WriteLine("Game goes on! Press Enter to continue");
        foreach (var player in PlayerTurns) Console.WriteLine($"{player.Name}: {player.Score}");
    }
}

//if the player is human, let the player to choose the skill
void HumanTurn(Player currentPlayer) {
    Console.Clear();
    
    if (gameMode == GameMode.PVP) Console.WriteLine($"{currentPlayer.Name}'s turn! opponent please avert eyes\n");
    else Console.WriteLine("Your turn!");
    if (currentPlayer.Hand.Any(card => card.Value == (Value)15) && currentPlayer.Hand.Any(card => card.Value == (Value)16)) {
        Console.WriteLine($"{currentPlayer.Name} drew both Jokers! Instant victory!");
        currentPlayer.Score += 10;
        ChangeState(State.ShowWinner);
        return;
    } else {
        Console.WriteLine($"Your skill card is {currentPlayer.Hand[1].Suit}{currentPlayer.Hand[1].Value}.");
        Console.WriteLine("The card skill is : " + currentPlayer.Hand[1].SkillDescription());
        Console.WriteLine();

        currentPlayer.isPlaying = true;
        currentPlayer.RoundValue = (int)currentPlayer.Hand[0].Value;
        RenderCards();
        UseSkill(currentPlayer);
        currentPlayer.isPlaying = false;
    }
}

//randomize the skill for the CPU
void CPUTurn(Player currentPlayer) {
    Console.Clear();
    currentPlayer.usedSkill = false;

    Player opponentPlayer;
    if (currentPlayer == player1) opponentPlayer = player2;
    else opponentPlayer = player1; 

    if (currentPlayer.Hand.Any(card => card.Value == (Value)15) && currentPlayer.Hand.Any(card => card.Value == (Value)16)) {
        Console.WriteLine($"{currentPlayer.Name} drew both Jokers! Instant victory!");
        currentPlayer.Score += 10;
        ChangeState(State.ShowWinner);
        return;
    } else {
        currentPlayer.RoundValue = (int)currentPlayer.Hand[0].Value;
        RenderCards();

        Random random = new Random();
        if (random.Next(2) == 0) {
            ExecuteSkill(currentPlayer, opponentPlayer);
            currentPlayer.usedSkill = true;
        } else {
            currentPlayer.RoundValue += (int)currentPlayer.Hand[1].Value;
        }
    }

    Console.WriteLine($"CPU is deciding, please wait...");
    Thread.Sleep(1000);
}

//testing the skill of the card
void UseSkill(Player currentPlayer) {
    currentPlayer.usedSkill = false;
    Player opponentPlayer;
    if (currentPlayer == player1) opponentPlayer = player2;
    else opponentPlayer = player1; 

    Console.WriteLine("Your current value: " + currentPlayer.RoundValue);
    Console.WriteLine("Press 1 to use skill, or press 2 to keep the value");
    while (true) {
        var key = Console.ReadKey(true).Key;

        if (key == ConsoleKey.D1) {
            ExecuteSkill(currentPlayer, opponentPlayer);
            currentPlayer.usedSkill = true;
            Console.WriteLine("You chosed to use the skill!");
            break;
        } else if (key == ConsoleKey.D2) {
            Console.WriteLine("You chosed to use it as a normal value card!");
            currentPlayer.RoundValue += (int)currentPlayer.Hand[1].Value;
            break;
        } else if (key == ConsoleKey.Escape) {
            closeRequested = true;
            Console.WriteLine("Close requested.");
            break;
        } else {
            Console.WriteLine("Invalid key. Please press 1, 2, or Esc.");
        }
    }
}

//execute the skill of the skill card
void ExecuteSkill(Player player, Player opponent) {
    Card skillCard = player.Hand[1];
    switch (skillCard.Suit) {
        case Suit.Hearts:
            if (skillCard.Value == Value.Eight) {
                if (player.RoundValue > opponent.RoundValue) RoundWinnerBonus = 2;
            } else {
                RoundWinnerBonus = 2;
            }
        break;
        case Suit.Spades:
            if (skillCard.Value == Value.Eight) {
                if (player.RoundValue < opponent.RoundValue) RoundLoserPenalty = 1;
            } else {
                RoundLoserPenalty = 1;
            }
        break;
        case Suit.Diamonds:
            if (skillCard.Value == Value.Eight) {
                player.RoundValue += (int)player.Hand[0].Value * 2;
            } else if (skillCard.Value == (Value)15) {
                opponent.Score -= 1;
            } else if (skillCard.Value == (Value)16) {
                player.Score += 2;
            } else {
                player.RoundValue += (int)player.Hand[0].Value;
            }
        break;
        case Suit.Clubs:
            if (skillCard.Value == Value.Eight) {
                if (remainDeck.Count < 2) RefreshDeck();

                discardDeck.Add(skillCard);
                player.Hand.RemoveAt(player.Hand.Count - 1);
                Card newCard1 = remainDeck[^1];
                player.Hand.Add(newCard1);
                remainDeck.RemoveAt(remainDeck.Count - 1);
                Card newCard2 = remainDeck[^1];
                remainDeck.RemoveAt(remainDeck.Count - 1);
                player.Hand.Add(newCard2);
                Console.WriteLine($"Your new card is: {newCard1.Suit}{newCard1.Value} and {newCard2.Suit}{newCard2.Value}");

                int higherValue = Math.Max((int)newCard1.Value, (int)newCard2.Value);
                Console.WriteLine($"Higher value card is: {higherValue}, added to your total value in this round.");
                player.RoundValue += higherValue;
            } else {
                if (remainDeck.Count < 1) RefreshDeck();

                discardDeck.Add(skillCard);
                player.Hand.RemoveAt(player.Hand.Count - 1);

                Card newCard = remainDeck[^1];
                player.Hand.Add(newCard);
                remainDeck.RemoveAt(remainDeck.Count - 1);
                Console.WriteLine($"Your new card is: {newCard.Suit}{newCard.Value}");

                player.RoundValue += (int)player.Hand[1].Value;
            }
        break;
    }
}
/**********************************************************************************************************
                                    Other Methods
**********************************************************************************************************/
//change the state of the game
void ChangeState(State newState) {
    var key = Console.ReadKey(true).Key;
    if (key == ConsoleKey.Enter) {
        state = newState;
    } else if (key == ConsoleKey.Escape) {
        closeRequested = true;
    } else {
        Console.WriteLine("Invalid key. Please press Enter or Esc.");
    }
}

//initialize the deck for 52 cards
void InitializeDeck() {
    remainDeck.Clear();
    discardDeck.Clear();
    foreach (Suit suit in Enum.GetValues<Suit>()) {
        foreach (Value value in Enum.GetValues<Value>()) {
            remainDeck.Add(new Card { Suit = suit, Value = value });
        }
    }

    Card smallJoker = new Card { Suit = Suit.Diamonds, Value = (Value)15 }; 
    Card bigJoker = new Card { Suit = Suit.Diamonds, Value = (Value)16 };
    remainDeck.Add(smallJoker);
    remainDeck.Add(bigJoker);

    Shuffle(remainDeck);
}

//refresh the deck
void RefreshDeck() {
    // Shuffle the discard pile back into the remainDeck when remainDeck is empty
        remainDeck.AddRange(discardDeck);
        discardDeck.Clear();
        Shuffle(remainDeck);
}

//refresh the hand cards
void RefreshHandCards(Player player) {
    discardDeck.AddRange(player.Hand);
    if (remainDeck.Count < 2) RefreshDeck();// Shuffle the discard pile back into the remainDeck when remainDeck is empty
    player.Hand.Clear();
    player.Hand.Add(remainDeck[^1]);
    remainDeck.RemoveAt(remainDeck.Count - 1);
    player.Hand.Add(remainDeck[^1]);
    remainDeck.RemoveAt(remainDeck.Count - 1);
}

//shuffle the cards
static void Shuffle(List<Card> cards) { // Method to shuffle the cards
    Random rand = new Random();
    for (int i = 0; i < cards.Count; i++) {
        int swap = rand.Next(cards.Count);
        (cards[i], cards[swap]) = (cards[swap], cards[i]);
    }
}
//deal and render the cards
void DealAndRenderCards() {
    for(int i = 0; i < 2; i++) {
        PlayerTurns[i].Hand.Clear();
        PlayerDrawCards(PlayerTurns[i].Hand);
    }
    RenderCards();
}

//draw cards for the player
void PlayerDrawCards(List<Card> playerHand) {
    for (int i = 0; i < 2; i++){
        var card = remainDeck[^1]; // Get the last card in the list
        remainDeck.RemoveAt(remainDeck.Count - 1); // Remove it from the remainDeck
        playerHand.Add(card);
    }
}

//press enter to continue
void PressEnterToContinue() {
    var key = Console.ReadKey(true).Key;
    if (key == ConsoleKey.Enter) {
        return;
    } else if (key == ConsoleKey.Escape) {
        closeRequested = true;
    }
}

//clear all the information
void ClearAll() {
    PlayerTurns.Clear();
    player1.Hand.Clear();
    player2.Hand.Clear();
    player1.Score = 0;
    player2.Score = 0; 
    RoundWinnerBonus = 1;
    RoundLoserPenalty = 0;
}
/**********************************************************************************************************
                                    Render
**********************************************************************************************************/
//show the main menu
void ShowMainMenu() {
    Console.Clear();
    Console.WriteLine("War Game");
    Console.WriteLine("This is a doule-player stratigic card game!");
    Console.WriteLine("Both players draw 2 cards at start of the turn.");
    Console.WriteLine("The first card's is value card, it's value shall be count into the total value of this turn.");
    Console.WriteLine("The second card can be used as either a value card or a skill card.");
    Console.WriteLine("   Value card would be added to the value of this turn.");
    Console.WriteLine("   Skill card would trigger the skill related to it's suit.");
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
Discards skill card, draws another card, add the new card's value in the total value of the round.

Special Cards:
Hearts 8: 
During scoring, if your value is higher, gain 2 points. No effect during ties.
Spades 8: 
During scoring, if your opponent's value is lower, they lose 2 points. No effect during ties.
Diamonds 8: 
Triples the face value of the player's other card.
Clubs 8: 
Discards skill card, draws two other cards, add the higher vlue of these two cards in the total value of the round.
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
void RenderCards() {
    foreach (var player in PlayerTurns) {
        switch (state) {
            case State.DealCards:
                for (int i = 0; i < 2; i++) {
                    Console.WriteLine(player.Hand[i].RenderFaceDown());
                }
                break;
            case State.PlayCards:
                if (endOfRound) {
                    Console.WriteLine($"{player.Name}'s value card:");
                    Console.WriteLine(player.Hand[0].RenderFaceUp());
                    Console.WriteLine($"{player.Name}'s skill card:");
                    Console.WriteLine(player.Hand[1].RenderFaceUp());
                } else if (player.isPlaying) {
                    Console.WriteLine($"{player.Name}'s value card:");
                    Console.WriteLine(player.Hand[0].RenderFaceUp());
                    Console.WriteLine($"{player.Name}'s skill card:");
                    Console.WriteLine(player.Hand[1].RenderFaceUp());
                } else {
                    Console.WriteLine($"{player.Name}'s value card:");
                    Console.WriteLine(player.Hand[0].RenderFaceDown());
                    Console.WriteLine($"{player.Name}'s skill card:");
                    Console.WriteLine(player.Hand[1].RenderFaceDown());
                }
                break;
            default:
                Console.WriteLine("State not recognized for rendering.");
                break;
        }
    }
}

//class to store the player's information
class Player {
    public string Name { get; set; } = string.Empty;
    public bool Gofirst { get; set; }
    public bool isHuman { get; set; } = true;
    public bool isPlaying { get; set; } = false;
    public bool usedSkill { get; set; } = false;
    public int Score { get; set; } = 0;
    public List<Card> Hand { get; set; } = new List<Card>();
    public int RoundValue { get; set; } = 0;
}

//class to store the card's information
class Card {
    // A class of cards with two kinds of properties.
    public Suit Suit { get; set; }
    public Value Value { get; set; }

    public string SkillDescription() {
        if (Value == (Value)15) {
            return "Adds -1 point to your opponent during scoring, with a fixed face value of 15.";
        } else if (Value == (Value)16) {
            return "Adds +2 points to yourself during scoring, with a fixed face value of 16.";
        } else if (Value == Value.Eight) {
            return Suit switch {
                Suit.Hearts => "During scoring, if your value is higher, gain 2 points. No effect during ties.",
                Suit.Spades => "During scoring, if your opponent's value is lower, they lose 2 points. No effect during ties.",
                Suit.Diamonds => "Triples the face value of the player's other card.",
                Suit.Clubs => "Discards this skill card and draws two replacement cards. The higher-value card must be used for its face value and added to the other card for the final total.",
                _ => "No special skill."
            };
        } else {
            return Suit switch {
                Suit.Hearts => "During scoring, the player with the higher total value gains 2 points instead of 1. This bonus is not stackable and has no effect during ties.",
                Suit.Spades => "During scoring, the player with the lower total value loses 1 point. This penalty is not stackable and has no effect during ties.",
                Suit.Diamonds => "Doubles the face value of the player's other card.",
                Suit.Clubs => "Discards skill card, draws another card, add the new card's value in the total value of the round.",
                _ => "No skill"
            };
        }
    }

    public string RenderFaceUp() {
        if (Value == (Value)15) {
            return @"
┌───────┐
│ Small │
│       │
│ Joker │
│       │
│       │
└───────┘
            ";
        }
    else if (Value == (Value)16) {
        return @"
┌───────┐
│  Big  │
│       │
│ Joker │
│       │
│       │
└───────┘
            ";
    } else {
        char suit = Suit.ToString()[0];
        string value = Value switch {
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
    }
    
    public string RenderFaceDown() {
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