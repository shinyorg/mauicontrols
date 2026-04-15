using Shiny.Maui.Controls.MermaidDiagrams.Models;

namespace Shiny.Maui.Controls.MermaidDiagrams.Parsing;

public sealed class MermaidParser
{
    readonly List<MermaidToken> tokens;
    int pos;

    MermaidParser(List<MermaidToken> tokens)
    {
        this.tokens = tokens;
    }

    public static DiagramModel Parse(string mermaidText)
    {
        var lexer = new MermaidLexer(mermaidText);
        var tokens = lexer.Tokenize();
        var parser = new MermaidParser(tokens);
        return parser.ParseDiagram();
    }

    MermaidToken Current => pos < tokens.Count ? tokens[pos] : tokens[^1];

    MermaidToken Advance()
    {
        var token = Current;
        if (pos < tokens.Count - 1)
            pos++;
        return token;
    }

    bool Check(MermaidTokenType type) => Current.Type == type;

    bool Match(MermaidTokenType type)
    {
        if (!Check(type))
            return false;
        Advance();
        return true;
    }

    MermaidToken Expect(MermaidTokenType type)
    {
        if (Check(type))
            return Advance();

        throw new ParseException(
            $"Expected {type}, got {Current.Type} '{Current.Value}'",
            Current.Line,
            Current.Column);
    }

    void SkipNewlinesAndSemicolons()
    {
        while (Check(MermaidTokenType.Newline) || Check(MermaidTokenType.Semicolon))
            Advance();
    }

    DiagramModel ParseDiagram()
    {
        var model = new DiagramModel();

        SkipNewlinesAndSemicolons();

        // Parse "graph" or "flowchart" keyword
        if (Check(MermaidTokenType.Graph) || Check(MermaidTokenType.Flowchart))
        {
            Advance();
        }
        else
        {
            throw new ParseException(
                $"Expected 'graph' or 'flowchart', got '{Current.Value}'",
                Current.Line,
                Current.Column);
        }

        // Parse optional direction
        if (Check(MermaidTokenType.Direction))
        {
            model.Direction = ParseDirection(Current.Value);
            Advance();
        }

        SkipNewlinesAndSemicolons();

        // Parse statements
        ParseStatements(model, null);

        return model;
    }

    void ParseStatements(DiagramModel model, string? currentSubgraphId)
    {
        while (!Check(MermaidTokenType.Eof) && !Check(MermaidTokenType.End))
        {
            SkipNewlinesAndSemicolons();

            if (Check(MermaidTokenType.Eof) || Check(MermaidTokenType.End))
                break;

            if (Check(MermaidTokenType.Subgraph))
            {
                ParseSubgraph(model, currentSubgraphId);
            }
            else if (Check(MermaidTokenType.Identifier) || Check(MermaidTokenType.QuotedString))
            {
                ParseNodeOrEdge(model, currentSubgraphId);
            }
            else
            {
                Advance(); // skip unknown
            }

            SkipNewlinesAndSemicolons();
        }
    }

    void ParseSubgraph(DiagramModel model, string? parentSubgraphId)
    {
        Advance(); // consume 'subgraph'

        // Read subgraph id/title
        var id = ReadText();
        var title = id;

        // Check for bracket syntax: subgraph id [title]
        if (Check(MermaidTokenType.OpenBracket))
        {
            Advance();
            title = ReadTextUntil(MermaidTokenType.CloseBracket);
            Expect(MermaidTokenType.CloseBracket);
        }

        var subgraph = new SubgraphModel
        {
            Id = id,
            Title = title,
            ParentSubgraphId = parentSubgraphId
        };
        model.Subgraphs.Add(subgraph);

        SkipNewlinesAndSemicolons();

        // Check for optional "direction" inside subgraph
        if (Check(MermaidTokenType.Identifier) &&
            Current.Value.Equals("direction", StringComparison.OrdinalIgnoreCase))
        {
            Advance(); // skip "direction"
            if (Check(MermaidTokenType.Direction) || Check(MermaidTokenType.Identifier))
                Advance(); // skip the direction value
            SkipNewlinesAndSemicolons();
        }

        ParseStatements(model, id);

        // Assign nodes discovered inside this subgraph
        foreach (var node in model.Nodes)
        {
            if (node.SubgraphId == id)
                subgraph.NodeIds.Add(node.Id);
        }

        if (Check(MermaidTokenType.End))
            Advance();
    }

    void ParseNodeOrEdge(DiagramModel model, string? currentSubgraphId)
    {
        var (nodeId, label, shape) = ParseNodeDefinition();
        var node = model.GetOrAddNode(nodeId, label, shape);
        if (currentSubgraphId != null && node.SubgraphId == null)
            node.SubgraphId = currentSubgraphId;

        // Check for edge
        while (IsEdgeToken(Current.Type))
        {
            var (style, arrowType) = ParseEdgeType();

            // Check for edge label: -->|label| or edge label after edge
            string? edgeLabel = null;
            if (Check(MermaidTokenType.Pipe))
            {
                Advance();
                edgeLabel = ReadTextUntil(MermaidTokenType.Pipe);
                Expect(MermaidTokenType.Pipe);
            }

            // Parse target node
            var (targetId, targetLabel, targetShape) = ParseNodeDefinition();
            var targetNode = model.GetOrAddNode(targetId, targetLabel, targetShape);
            if (currentSubgraphId != null && targetNode.SubgraphId == null)
                targetNode.SubgraphId = currentSubgraphId;

            model.Edges.Add(new DiagramEdge
            {
                SourceId = nodeId,
                TargetId = targetId,
                Label = edgeLabel,
                Style = style,
                ArrowType = arrowType
            });

            // For chaining: A --> B --> C
            nodeId = targetId;
        }
    }

    (string Id, string Label, NodeShape Shape) ParseNodeDefinition()
    {
        var id = ReadText();
        var label = id;
        var shape = NodeShape.Rectangle;

        // Check for node shape definition
        if (Check(MermaidTokenType.OpenBracket))
        {
            Advance();
            // Check for stadium: ([text])
            if (Check(MermaidTokenType.OpenParen))
            {
                Advance();
                label = ReadTextUntil(MermaidTokenType.CloseParen);
                Expect(MermaidTokenType.CloseParen);
                Expect(MermaidTokenType.CloseBracket);
                shape = NodeShape.Stadium;
            }
            else
            {
                label = ReadTextUntil(MermaidTokenType.CloseBracket);
                Expect(MermaidTokenType.CloseBracket);
                shape = NodeShape.Rectangle;
            }
        }
        else if (Check(MermaidTokenType.OpenParen))
        {
            Advance();
            // Check for circle: ((text))
            if (Check(MermaidTokenType.OpenParen))
            {
                Advance();
                label = ReadTextUntil(MermaidTokenType.CloseParen);
                Expect(MermaidTokenType.CloseParen);
                Expect(MermaidTokenType.CloseParen);
                shape = NodeShape.Circle;
            }
            else
            {
                label = ReadTextUntil(MermaidTokenType.CloseParen);
                Expect(MermaidTokenType.CloseParen);
                shape = NodeShape.RoundedRect;
            }
        }
        else if (Check(MermaidTokenType.OpenBrace))
        {
            Advance();
            // Check for hexagon: {{text}}
            if (Check(MermaidTokenType.OpenBrace))
            {
                Advance();
                label = ReadTextUntil(MermaidTokenType.CloseBrace);
                Expect(MermaidTokenType.CloseBrace);
                Expect(MermaidTokenType.CloseBrace);
                shape = NodeShape.Hexagon;
            }
            else
            {
                label = ReadTextUntil(MermaidTokenType.CloseBrace);
                Expect(MermaidTokenType.CloseBrace);
                shape = NodeShape.Diamond;
            }
        }

        return (id, label, shape);
    }

    (EdgeStyle Style, ArrowType ArrowType) ParseEdgeType()
    {
        var token = Advance();
        return token.Type switch
        {
            MermaidTokenType.Arrow => (EdgeStyle.Solid, ArrowType.Arrow),
            MermaidTokenType.Line => (EdgeStyle.Solid, ArrowType.Open),
            MermaidTokenType.DottedArrow => (EdgeStyle.Dotted, ArrowType.Arrow),
            MermaidTokenType.DottedLine => (EdgeStyle.Dotted, ArrowType.Open),
            MermaidTokenType.ThickArrow => (EdgeStyle.Thick, ArrowType.Arrow),
            MermaidTokenType.ThickLine => (EdgeStyle.Thick, ArrowType.Open),
            _ => (EdgeStyle.Solid, ArrowType.Arrow)
        };
    }

    string ReadText()
    {
        if (Check(MermaidTokenType.QuotedString))
            return Advance().Value;

        if (Check(MermaidTokenType.Identifier))
            return Advance().Value;

        return Advance().Value;
    }

    string ReadTextUntil(MermaidTokenType stopType)
    {
        var parts = new List<string>();
        while (!Check(stopType) && !Check(MermaidTokenType.Eof))
        {
            if (Check(MermaidTokenType.QuotedString))
                parts.Add(Advance().Value);
            else
                parts.Add(Advance().Value);
        }

        return parts.Count > 0 ? string.Join(" ", parts) : "";
    }

    static bool IsEdgeToken(MermaidTokenType type) =>
        type is MermaidTokenType.Arrow
            or MermaidTokenType.Line
            or MermaidTokenType.DottedArrow
            or MermaidTokenType.DottedLine
            or MermaidTokenType.ThickArrow
            or MermaidTokenType.ThickLine;

    static DiagramDirection ParseDirection(string value) =>
        value.ToUpperInvariant() switch
        {
            "TD" or "TB" => DiagramDirection.TopToBottom,
            "BT" => DiagramDirection.BottomToTop,
            "LR" => DiagramDirection.LeftToRight,
            "RL" => DiagramDirection.RightToLeft,
            _ => DiagramDirection.TopToBottom
        };
}