namespace Calculator.Core.UnitTests
{
    public class TokenizerTests
    {
        [Theory]
        [InlineData("0", TokenKind.Number)]
        [InlineData("1", TokenKind.Number)]
        [InlineData("44", TokenKind.Number)]
        [InlineData("ff", TokenKind.Identifier)]
        [InlineData("@", TokenKind.Unknown)]
        [InlineData("43", TokenKind.Number)]
        [InlineData("2", TokenKind.Number)]
        [InlineData("+", TokenKind.Add)]
        [InlineData("*", TokenKind.Multiply)]
        [InlineData("/", TokenKind.Divide)]
        [InlineData("-", TokenKind.Subtract)]
        public void GetTokens_ReturnsTheCorrectToken(string text, TokenKind kind)
        {
            Tokenizer sut = new Tokenizer();

            Token token = sut.GetTokens(text).First();

            Assert.Equal(text, token.Text);
            Assert.Equal(kind, token.Kind);
        }

        [Theory]
        [InlineData("0", TokenKind.Number)]
        [InlineData("1", TokenKind.Number)]
        [InlineData("44", TokenKind.Number)]
        [InlineData("ff", TokenKind.Unknown)]
        [InlineData("43", TokenKind.Number)]
        [InlineData("2", TokenKind.Number)]
        [InlineData("+", TokenKind.Number)]
        [InlineData("*", TokenKind.Number)]
        [InlineData("/", TokenKind.Number)]
        [InlineData("-", TokenKind.Number)]
        public void GetTokens_ReturnsTheCorrectTokenType(string text, TokenKind expectedType)
        {
            Tokenizer sut = new Tokenizer();

            Token token = sut.GetTokens(text).First();

            Assert.Equal(text, token.Text);
        }

        [Theory]
        [InlineData("   ff")]
        [InlineData("43     ")]
        [InlineData("  2  ")]
        [InlineData(" + ")]
        [InlineData("  *     \t")]
        [InlineData("  5\n")]
        public void GetTokens_ShouldIgnoreWhiteSpace(string text)
        {
            Tokenizer sut = new Tokenizer();

            Token token = sut.GetTokens(text).First();

            Assert.Equal(text.Trim(), token.Text);
        }
    }
}