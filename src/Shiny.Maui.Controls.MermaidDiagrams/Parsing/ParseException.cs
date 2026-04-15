namespace Shiny.Maui.Controls.MermaidDiagrams.Parsing;

public sealed class ParseException : Exception
{
    public int Line { get; }
    public int Column { get; }

    public ParseException(string message, int line, int column)
        : base($"Line {line}, Col {column}: {message}")
    {
        Line = line;
        Column = column;
    }
}