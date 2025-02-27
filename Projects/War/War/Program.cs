using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
List<Card> deck = new List<Card>();//build a new list to store all the cards
List<Card> discardPile = new();//a new list to store all the discarded cards
Card playerHand = new();//player’s card
Card dealerHand = new();//dealer’s card


try{
        foreach (Suit suit in Enum.GetValues<Suit>())
    {
        foreach (Value value in Enum.GetValues<Value>())
        {
            deck.Add(new()
            {
                Suit = suit,
                Value = value,
            });
        }
    }//add 52 cards with 4 different patterns combined with 13 numbers
    Shuffle(deck);
    while(deck.Count > 0){
        start:
        Console.Clear();
        Console.WriteLine("Press Enter to draw a card(escape to quit):");
        Console.WriteLine(deck.Count + "cards left in the deck");//render the hint
        switch (Console.ReadKey(true).Key)
        {
            case ConsoleKey.Enter:
                playerHand = deck[^1];//get the last variable in the list as player’s hand
                deck.RemoveAt(deck.Count - 1);//remove it from the list
                discardPile.Add(playerHand);//add it into the discard pile
                Console.WriteLine("You draw:");
                Console.WriteLine(playerHand.Render());
                break;

            case ConsoleKey.Escape:
                return;

            default:
                Console.WriteLine("Press Enter only.");
                goto start;
        }

        Console.WriteLine("Dealer Draw a card:");//do the same for dealer
        dealerHand = deck[^1];
        deck.RemoveAt(deck.Count - 1);//compare hands for both player
        Console.WriteLine("Dealer draws:");
        Console.WriteLine(dealerHand.Render());

        if(playerHand.Value > dealerHand.Value) Console.WriteLine("You win");
        else if(playerHand.Value < dealerHand.Value) Console.WriteLine("You lose");
        else Console.WriteLine("Draw");

        if(deck.Count == 0) {//shuffle the card after the deck pile was run out
            Shuffle(discardPile);
            deck = discardPile;
        }
        Console.WriteLine("Press any key to Continue");
        Console.ReadKey();
    }

    void Shuffle(List<Card> cards) {//method to shuffle the card
        for (int i = 0; i < cards.Count; i++)
        {
            int swap = Random.Shared.Next(cards.Count);
            (cards[i], cards[swap]) = (cards[swap], cards[i]);
        }
    }
}
finally{
    Console.WriteLine("Game End");
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
        builder.AppendLine($"Score: {score}");

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