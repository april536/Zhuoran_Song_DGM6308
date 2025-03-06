using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
List<Card> deck = new List<Card>();//build a new list to store all the cards
List<Card> discardPile = new();//a new list to store all the discarded cards
Card playerHand = new();//player’s card
Card dealerHand = new();//dealer’s card
int playerScore = 0;//player’s score
int dealerScore = 0;//dealer’s score
bool closeRequested = false;
State state = State.MainMenu;
Player player = new Player();
Player dealer = new Player();

try {
    while (!closeRequested) {
        Gameloop();
    }
    // Add 52 cards with 4 different suits combined with 13 values
    foreach (Suit suit in Enum.GetValues<Suit>()) {
        foreach (Value value in Enum.GetValues<Value>()) {
            deck.Add(new Card { Suit = suit, Value = value });
        }
    }

    Shuffle(deck);

    while (true) {
        start:
        Console.Clear();
        if (deck.Count ==0) {// Shuffle the discard pile back into the deck when deck is empty
            deck.AddRange(discardPile);
            discardPile.Clear();
            Shuffle(deck);
            Console.WriteLine("Deck is empty! press enter to continue or esc to exit"); 
            if (Console.ReadKey(true).Key == ConsoleKey.Escape) return; 
        }

        Console.Clear();
        Console.WriteLine("Press Enter to draw a card:");
        Console.WriteLine($"{deck.Count} cards left in the deck"); // Render the hint
    
        switch (Console.ReadKey(true).Key) {
            case ConsoleKey.Enter:
                playerHand = deck[^1]; // Get the last card in the list as player's hand
                deck.RemoveAt(deck.Count - 1); // Remove it from the deck
                discardPile.Add(playerHand); // Add it into the discard pile
                Console.WriteLine("You draw:");
                Console.WriteLine(playerHand.Render());
                Console.WriteLine($"Player Score: {playerScore}");
                break;

            case ConsoleKey.Escape:
                return;

            default:
                Console.WriteLine("Press Enter only.");
                goto start;
        }

        Console.WriteLine("Dealer draws a card:"); // Do the same for dealer
        dealerHand = deck[^1];
        deck.RemoveAt(deck.Count - 1); // Remove it from the deck
        discardPile.Add(dealerHand);
        Console.WriteLine(dealerHand.Render());
        Console.WriteLine($"Dealer Score: {dealerScore}");

        // Compare hands for both player and dealer
        if (playerHand.Value > dealerHand.Value) {
            Console.WriteLine("You win");
            playerScore++;
        } else if (playerHand.Value < dealerHand.Value) {
            Console.WriteLine("You lose");
            dealerScore++;
        } else Console.WriteLine("Draw");
                     
        Console.WriteLine("Press Enter to continue");
        if (Console.ReadKey(true).Key == ConsoleKey.Escape) return;
    }
} finally {
    Console.WriteLine("Esc pressed. Game closed.");
}

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
    }
}

static void Shuffle(List<Card> cards) { // Method to shuffle the cards
    Random rand = new Random();
    for (int i = 0; i < cards.Count; i++) {
        int swap = rand.Next(cards.Count);
        (cards[i], cards[swap]) = (cards[swap], cards[i]);
    }
}

void ShowMainMenu() {
    Console.Clear();
    Console.WriteLine("War Game");
    Console.WriteLine("This is a doule-player stratigic card game!");
    Console.WriteLine("Both players draw 2 cards at start of the turn.");
    Console.WriteLine("The first card's value shall be count into the value of this turn.");
    Console.WriteLine("The second card can be used either as value card or skill card.");
    Console.WriteLine("   [1. Value card would be added to the value of this turn.]");
    Console.WriteLine("   [2. Skill card would trigger the skill related to it's suit.]");
    Console.WriteLine("Player who holds the higher value wins the turn and earn score.");
    Console.WriteLine("Game ends when one player's score reaches 10!");
    Console.WriteLine("    1. Press 1 to stard PVP Mode");
    Console.WriteLine("    2. Press 2 to start PVE Mode");
    Console.WriteLine("    3. Press 3 to check the skills of cards");
    Console.WriteLine("    Press Esc to exit");
    switch (Console.ReadKey(true).Key) {
        case ConsoleKey.D1:
            state = State.RandomizeTurn;
            break;
        case ConsoleKey.D2:
            state = State.RandomizeTurn;
            break;
        case ConsoleKey.D3:
            state = State.RandomizeTurn;
            break;
        case ConsoleKey.Escape:
            closeRequested = true;
            break;
    }
}

void ShowCardSkills() {
    Console.Clear();
    Console.WriteLine("Card Skills");
    Console.WriteLine("Wait for the next update!");
    Console.WriteLine("Press Enter to return to main menu");
    if (Console.ReadKey(true).Key == ConsoleKey.Enter) {
        state = State.MainMenu;
    }
    if (Console.ReadKey(true).Key == ConsoleKey.Escape) closeRequested = true;
}

void RandomizeTurn() {
    Console.Clear();
    Console.WriteLine("Randomizing turn...");
    Console.WriteLine("Press Enter to continue");
    if (Console.ReadKey(true).Key == ConsoleKey.Enter) {
        state = State.DealCards;
    }
    if (Console.ReadKey(true).Key == ConsoleKey.Escape) closeRequested = true;
}

class Player {
    public bool Gofirst { get; set; }
    public bool isHuman { get; set; }
    public int Score { get; set; }
    public List<Card> Hand { get; set; } = new List<Card>();
}

class Card
{
    // A class of cards with two kinds of properties.
    public Suit Suit { get; set; }
    public Value Value { get; set; }

    public string Render()
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
}

enum Suit
{
    // The card’s suit.
    Hearts,
    Clubs,
    Spades,
    Diamonds
}

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

enum State {
    // The game’s state.
    MainMenu,
    ShowCardSkills,
    RandomizeTurn,
    DealCards,
    HumanTurn,
    CPUTurn,
    ScoreCounter,
    ShowWinner,
}

enum GameMode {
    // The game’s mode.
    PVP,
    PVE
}