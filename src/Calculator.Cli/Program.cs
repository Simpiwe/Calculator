// See https://aka.ms/new-console-template for more information

using Calculator.Core;
using System.Globalization;

HashSet<string> exitCommands = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { "exit", "close" };
HashSet<string> clearCommands = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase) { "clear", "cls" };

Parser parser = new Parser(new Tokenizer());
Interpreter interpreter = new Interpreter(parser);

while (true)
{
    Console.Write("> ");

    string? line = Console.ReadLine()?.Trim();

    if (string.IsNullOrEmpty(line))
    {
        continue;
    }

    if (exitCommands.Contains(line))
    {
        break;
    }

    if (clearCommands.Contains(line))
    {
        Console.Clear();
        continue;
    }


    try
    {
        object? result = interpreter.Evaluate<object>(line);

        if (result is double num)
        {
            Console.WriteLine(num.ToString(CultureInfo.InvariantCulture));
        }
        else 
        {
            Console.WriteLine("NaN");
        }
    }
    catch (InvalidSyntaxException ex)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;

        foreach (string error in ex.Errors)
        {
            Console.WriteLine(error);
        }
    }
    catch(ApplicationException ex)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;

        Console.WriteLine(ex.Message);
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;

        Console.WriteLine("Uhmm... That's awkward. Something unexpected happened while parsing or evaluating your input. Please report the issue to dev. If you are dev, I'm really disappointed in you. -_-");
    }
    finally
    {
        Console.ResetColor();
    }
}
