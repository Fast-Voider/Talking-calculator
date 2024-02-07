using System;
using System.Collections.Generic;
using System.Speech.Recognition;
using System.Speech.Synthesis;

class CalculatorApp
{
    private static Dictionary<string, int> numberMappings = new Dictionary<string, int>
{
  {"one", 1}, {"two", 2}, {"three", 3}, {"four", 4}, {"five", 5},
  {"six", 6}, {"seven", 7}, {"eight", 8}, {"nine", 9}, {"ten", 10},
  {"eleven", 11}, {"twelve", 12}, {"thirteen", 13}, {"fourteen", 14}, {"fifteen", 15},
  {"sixteen", 16}, {"seventeen", 17}, {"eighteen", 18}, {"nineteen", 19}, {"twenty", 20},
  {"twenty-one", 21}, {"twenty-two", 22}, {"twenty-three", 23}, {"twenty-four", 24}, {"twenty-five", 25},
  {"twenty-six", 26}, {"twenty-seven", 27}, {"twenty-eight", 28}, {"twenty-nine", 29}, {"thirty", 30},
  {"thirty-one", 31}, {"thirty-two", 32}, {"thirty-three", 33}, {"thirty-four", 34}, {"thirty-five", 35},
  {"thirty-six", 36}, {"thirty-seven", 37}, {"thirty-eight", 38}, {"thirty-nine", 39}, {"forty", 40},
  {"forty-one", 41}, {"forty-two", 42}, {"forty-three", 43}, {"forty-four", 44}, {"forty-five", 45},
  {"forty-six", 46}, {"forty-seven", 47}, {"forty-eight", 48}, {"forty-nine", 49}, {"fifty", 50},
    };

    static void Main()
    {
        // Create a SpeechRecognitionEngine
        SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine();

        // Create a GrammarBuilder with predefined choices for numbers and operators
        Choices numbers = new Choices(numberMappings.Keys.ToArray());
        Choices operators = new Choices("plus", "minus", "times", "divided by", "exit"); // Added "exit" as a valid operator
        GrammarBuilder grammarBuilder = new GrammarBuilder();
        grammarBuilder.Append(numbers);
        grammarBuilder.Append(operators);
        grammarBuilder.Append(numbers);

        // Create a Grammar object from the GrammarBuilder
        Grammar grammar = new Grammar(grammarBuilder);

        // Load the grammar into the recognizer
        recognizer.LoadGrammar(grammar);

        // Set up an event handler for the SpeechRecognized event
        recognizer.SpeechRecognized += Recognizer_SpeechRecognized;

        // Start listening
        recognizer.SetInputToDefaultAudioDevice();
        recognizer.RecognizeAsync(RecognizeMode.Multiple);

        Console.WriteLine("Listening for equations. Say 'exit' to close.");
        Console.ReadLine(); // Keep the application running until Enter is pressed
    }

    static void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
    {
        // Check if the recognized words include "exit"
        if (e.Result.Words.Any(word => word.Text.ToLower() == "exit"))
        {
            Console.WriteLine("Exiting the application. Press Enter to close.");
            // Stop the recognizer
            ((SpeechRecognitionEngine)sender).RecognizeAsyncStop();
            return;
        }

        // Handle the recognized speech for equations
        string equation = $"{e.Result.Words[0].Text} {e.Result.Words[1].Text} {e.Result.Words[2].Text}";

        // Evaluate the equation and get the result
        double result = EvaluateEquation(equation);

        // Speak the result
        SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        synthesizer.Speak($"The answer is {result}");

        // Print the result to the console
        Console.WriteLine($"Result: {result}");
    }


    static double EvaluateEquation(string equation)
    {
        string[] parts = equation.Split(' ');

        // Map words to numeric values using the dictionary
        int operand1 = numberMappings[parts[0]];
        int operand2 = numberMappings[parts[2]];
        int result = 0;

        switch (parts[1])
        {
            case "plus":
                result = operand1 + operand2;
                break;
            case "minus":
                result = operand1 - operand2;
                break;
            case "times":
                result = operand1 * operand2;
                break;
            case "divided":
                if (operand2 != 0)
                    result = operand1 / operand2;
                else
                    Console.WriteLine("Cannot divide by zero.");
                break;
            default:
                Console.WriteLine("Invalid operator.");
                break;
        }

        return result;
    }
}
