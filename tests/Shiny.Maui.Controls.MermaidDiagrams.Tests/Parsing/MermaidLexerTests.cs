using Shiny.Maui.Controls.MermaidDiagrams.Parsing;
using Shouldly;
using Xunit;

namespace Shiny.Maui.Controls.MermaidDiagrams.Tests.Parsing;

public class MermaidLexerTests
{
    static List<MermaidToken> Tokenize(string source)
        => new MermaidLexer(source).Tokenize();

    // -----------------------------------------------------------------
    // Tokenize_SimpleGraph_ReturnsCorrectTokens
    // -----------------------------------------------------------------
    [Fact]
    public void Tokenize_SimpleGraph_ReturnsCorrectTokens()
    {
        var tokens = Tokenize("graph TD");

        tokens[0].Type.ShouldBe(MermaidTokenType.Graph);
        tokens[1].Type.ShouldBe(MermaidTokenType.Direction);
        tokens[1].Value.ShouldContain("TD");
    }

    // -----------------------------------------------------------------
    // Tokenize_Arrow_ReturnsArrowToken
    // -----------------------------------------------------------------
    [Fact]
    public void Tokenize_Arrow_ReturnsArrowToken()
    {
        var tokens = Tokenize("A --> B");

        tokens.ShouldContain(t => t.Type == MermaidTokenType.Arrow);
    }

    // -----------------------------------------------------------------
    // Tokenize_DottedArrow_ReturnsDottedArrowToken
    // -----------------------------------------------------------------
    [Fact]
    public void Tokenize_DottedArrow_ReturnsDottedArrowToken()
    {
        var tokens = Tokenize("A -.-> B");

        tokens.ShouldContain(t => t.Type == MermaidTokenType.DottedArrow);
    }

    // -----------------------------------------------------------------
    // Tokenize_ThickArrow_ReturnsThickArrowToken
    // -----------------------------------------------------------------
    [Fact]
    public void Tokenize_ThickArrow_ReturnsThickArrowToken()
    {
        var tokens = Tokenize("A ==> B");

        tokens.ShouldContain(t => t.Type == MermaidTokenType.ThickArrow);
    }

    // -----------------------------------------------------------------
    // Tokenize_Comment_IsSkipped
    // -----------------------------------------------------------------
    [Fact]
    public void Tokenize_Comment_IsSkipped()
    {
        var tokens = Tokenize("%% comment\nA");

        tokens.ShouldNotContain(t => t.Type == MermaidTokenType.Comment);
        tokens.ShouldContain(t => t.Type == MermaidTokenType.Identifier && t.Value == "A");
    }

    // -----------------------------------------------------------------
    // Tokenize_QuotedString_ReturnsValue
    // -----------------------------------------------------------------
    [Fact]
    public void Tokenize_QuotedString_ReturnsValue()
    {
        var tokens = Tokenize("A[\"hello world\"]");

        var quoted = tokens.FirstOrDefault(t => t.Type == MermaidTokenType.QuotedString);
        quoted.Type.ShouldBe(MermaidTokenType.QuotedString);
        quoted.Value.ShouldBe("hello world");
    }

    // -----------------------------------------------------------------
    // Tokenize_BracketsAndParens
    // -----------------------------------------------------------------
    [Fact]
    public void Tokenize_BracketsAndParens()
    {
        var tokens = Tokenize("A[text](round){diamond}");

        tokens.ShouldContain(t => t.Type == MermaidTokenType.OpenBracket);
        tokens.ShouldContain(t => t.Type == MermaidTokenType.CloseBracket);
        tokens.ShouldContain(t => t.Type == MermaidTokenType.OpenParen);
        tokens.ShouldContain(t => t.Type == MermaidTokenType.CloseParen);
        tokens.ShouldContain(t => t.Type == MermaidTokenType.OpenBrace);
        tokens.ShouldContain(t => t.Type == MermaidTokenType.CloseBrace);
    }
}