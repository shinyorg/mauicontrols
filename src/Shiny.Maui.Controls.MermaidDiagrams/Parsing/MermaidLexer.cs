namespace Shiny.Maui.Controls.MermaidDiagrams.Parsing;

public sealed class MermaidLexer
{
    readonly string source;
    int pos;
    int line = 1;
    int col = 1;

    public MermaidLexer(string source)
    {
        this.source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public List<MermaidToken> Tokenize()
    {
        var tokens = new List<MermaidToken>();

        while (pos < source.Length)
        {
            SkipWhitespaceExceptNewline();

            if (pos >= source.Length)
                break;

            var ch = source[pos];

            // Comments
            if (ch == '%' && Peek(1) == '%')
            {
                SkipLineComment();
                continue;
            }

            // Newlines
            if (ch == '\n')
            {
                tokens.Add(new MermaidToken(MermaidTokenType.Newline, "\n", line, col));
                Advance();
                continue;
            }

            if (ch == '\r')
            {
                Advance();
                if (pos < source.Length && source[pos] == '\n')
                    Advance();
                tokens.Add(new MermaidToken(MermaidTokenType.Newline, "\n", line - 1, col));
                continue;
            }

            // Semicolons
            if (ch == ';')
            {
                tokens.Add(new MermaidToken(MermaidTokenType.Semicolon, ";", line, col));
                Advance();
                continue;
            }

            // Pipes for edge labels
            if (ch == '|')
            {
                tokens.Add(new MermaidToken(MermaidTokenType.Pipe, "|", line, col));
                Advance();
                continue;
            }

            // Quoted strings
            if (ch == '"')
            {
                tokens.Add(ReadQuotedString());
                continue;
            }

            // Edge detection: ==> or ===
            if (ch == '=' && Peek(1) == '=' && Peek(2) == '>')
            {
                tokens.Add(new MermaidToken(MermaidTokenType.ThickArrow, "==>", line, col));
                Advance();
                Advance();
                Advance();
                continue;
            }

            if (ch == '=' && Peek(1) == '=' && Peek(2) == '=')
            {
                tokens.Add(new MermaidToken(MermaidTokenType.ThickLine, "===", line, col));
                Advance();
                Advance();
                Advance();
                continue;
            }

            // Edge detection: -.-> or -.-
            if (ch == '-' && Peek(1) == '.' && Peek(2) == '-' && Peek(3) == '>')
            {
                tokens.Add(new MermaidToken(MermaidTokenType.DottedArrow, "-.->", line, col));
                Advance();
                Advance();
                Advance();
                Advance();
                continue;
            }

            if (ch == '-' && Peek(1) == '.' && Peek(2) == '-')
            {
                tokens.Add(new MermaidToken(MermaidTokenType.DottedLine, "-.-", line, col));
                Advance();
                Advance();
                Advance();
                continue;
            }

            // Edge detection: --> or ---
            if (ch == '-' && Peek(1) == '-' && Peek(2) == '>')
            {
                tokens.Add(new MermaidToken(MermaidTokenType.Arrow, "-->", line, col));
                Advance();
                Advance();
                Advance();
                continue;
            }

            if (ch == '-' && Peek(1) == '-' && Peek(2) == '-')
            {
                tokens.Add(new MermaidToken(MermaidTokenType.Line, "---", line, col));
                Advance();
                Advance();
                Advance();
                continue;
            }

            // Brackets
            if (ch == '[')
            {
                tokens.Add(new MermaidToken(MermaidTokenType.OpenBracket, "[", line, col));
                Advance();
                continue;
            }

            if (ch == ']')
            {
                tokens.Add(new MermaidToken(MermaidTokenType.CloseBracket, "]", line, col));
                Advance();
                continue;
            }

            if (ch == '(')
            {
                tokens.Add(new MermaidToken(MermaidTokenType.OpenParen, "(", line, col));
                Advance();
                continue;
            }

            if (ch == ')')
            {
                tokens.Add(new MermaidToken(MermaidTokenType.CloseParen, ")", line, col));
                Advance();
                continue;
            }

            if (ch == '{')
            {
                tokens.Add(new MermaidToken(MermaidTokenType.OpenBrace, "{", line, col));
                Advance();
                continue;
            }

            if (ch == '}')
            {
                tokens.Add(new MermaidToken(MermaidTokenType.CloseBrace, "}", line, col));
                Advance();
                continue;
            }

            // Identifiers and keywords
            if (IsIdentStart(ch))
            {
                tokens.Add(ReadIdentifierOrKeyword());
                continue;
            }

            // Skip unknown characters
            Advance();
        }

        tokens.Add(new MermaidToken(MermaidTokenType.Eof, "", line, col));
        return tokens;
    }

    char Peek(int offset = 0)
    {
        var i = pos + offset;
        return i < source.Length ? source[i] : '\0';
    }

    void Advance()
    {
        if (pos < source.Length)
        {
            if (source[pos] == '\n')
            {
                line++;
                col = 1;
            }
            else
            {
                col++;
            }

            pos++;
        }
    }

    void SkipWhitespaceExceptNewline()
    {
        while (pos < source.Length && source[pos] is ' ' or '\t')
            Advance();
    }

    void SkipLineComment()
    {
        while (pos < source.Length && source[pos] != '\n')
            Advance();
    }

    MermaidToken ReadQuotedString()
    {
        var startLine = line;
        var startCol = col;
        Advance(); // skip opening "

        var start = pos;
        while (pos < source.Length && source[pos] != '"')
            Advance();

        var value = source[start..pos];

        if (pos < source.Length)
            Advance(); // skip closing "

        return new MermaidToken(MermaidTokenType.QuotedString, value, startLine, startCol);
    }

    MermaidToken ReadIdentifierOrKeyword()
    {
        var startLine = line;
        var startCol = col;
        var start = pos;

        while (pos < source.Length && IsIdentPart(source[pos]))
        {
            // Stop before '-' if it looks like an edge (-- or -.)
            if (source[pos] == '-' && pos + 1 < source.Length && source[pos + 1] is '-' or '.')
                break;
            Advance();
        }

        var value = source[start..pos];

        var type = value.ToLowerInvariant() switch
        {
            "graph" => MermaidTokenType.Graph,
            "flowchart" => MermaidTokenType.Flowchart,
            "subgraph" => MermaidTokenType.Subgraph,
            "end" => MermaidTokenType.End,
            "td" or "tb" => MermaidTokenType.Direction,
            "bt" => MermaidTokenType.Direction,
            "lr" => MermaidTokenType.Direction,
            "rl" => MermaidTokenType.Direction,
            _ => MermaidTokenType.Identifier
        };

        return new MermaidToken(type, value, startLine, startCol);
    }

    static bool IsIdentStart(char c) => char.IsLetter(c) || c == '_';
    static bool IsIdentPart(char c) => char.IsLetterOrDigit(c) || c == '_' || c == '-';
}