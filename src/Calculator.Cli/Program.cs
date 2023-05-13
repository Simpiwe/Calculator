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

    string? line = Console.ReadLine()?.Trim() ?? "";

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
        double result = interpreter.Evaluate<double>(line);

        Console.WriteLine(result.ToString(CultureInfo.InvariantCulture));
    }
    catch (InvalidSyntaxException ex)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;

        foreach (string error in ex.Errors)
        {
            Console.WriteLine(error);
        }
    }
    catch(Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;

        Console.WriteLine(ex.Message);
    }
    finally
    {
        Console.ResetColor();
    }
}
