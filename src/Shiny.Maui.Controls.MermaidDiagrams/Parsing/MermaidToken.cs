namespace Shiny.Maui.Controls.MermaidDiagrams.Parsing;

public readonly record struct MermaidToken(MermaidTokenType Type, string Value, int Line, int Column);

public enum MermaidTokenType
{
    // Keywords
    Graph,
    Flowchart,
    Subgraph,
    End,
    Direction,

    // Identifiers and literals
    Identifier,
    QuotedString,

    // Node shapes
    OpenBracket, // [
    CloseBracket, // ]
    OpenParen, // (
    CloseParen, // )
    OpenBrace, // {
    CloseBrace, // }

    // Edge types
    Arrow, // -->
    Line, // ---
    DottedArrow, // -.->
    DottedLine, // -.-
    ThickArrow, // ==>
    ThickLine, // ===

    // Edge labels
    Pipe, // |

    // Separators
    Semicolon,
    Newline,

    // Comment
    Comment,

    // End of input
    Eof
}