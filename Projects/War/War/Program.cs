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


try {
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
    Console.WriteLine("Game End");
}

static void Shuffle(List<Card> cards) { // Method to shuffle the cards
    Random rand = new Random();
    for (int i = 0; i < cards.Count; i++) {
        int swap = rand.Next(cards.Count);
        (cards[i], cards[swap]) = (cards[swap], cards[i]);
    }
}

class Card
{
    // A class of cards with two kinds of properties.
    public Suit Suit { get; set; }
    public Value Value { get; set; }
    public const int RenderHeight = 7;

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