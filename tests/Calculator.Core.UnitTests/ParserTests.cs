using Moq;
using System.Globalization;
using System.Linq.Expressions;

namespace Calculator.Core.UnitTests
{
    public class ParserTests
    {
        [Theory]
        [InlineData("0")]
        [InlineData("5")]
        [InlineData("119")]
        [InlineData("-119")]
        [InlineData("-9")]
        public void Parse_ParsesNumbers(string numberText)
        {
            double number = double.Parse(numberText, CultureInfo.InvariantCulture);

            var token = new Token()
            {
                Kind = TokenKind.Number,
                Length = 0,
                Position = 0,
                Text = numberText,
                Value = number
            };

            Mock<ITokenizer> tokenizerMock = new Mock<ITokenizer>();
            tokenizerMock.Setup(x => x.GetTokens(It.IsAny<string>()))
                .Returns(new[] { token });

            Parser sut = new Parser(tokenizerMock.Object);

            ConstantExpression exp = (ConstantExpression)sut.Parse(numberText);

            Assert.Equal(number, exp.Value);
        }

        [Fact]
        public void Parse_ThrowsWhenThereAreUnknownTokens()
        {
            var token = new Token()
            {
                Kind = TokenKind.Unknown,
                Length = 0,
                Position = 0,
            };

            Mock<ITokenizer> tokenizerMock = new Mock<ITokenizer>();
            tokenizerMock.Setup(x => x.GetTokens(It.IsAny<string>()))
                .Returns(new[] { token });

            Parser sut = new Parser(tokenizerMock.Object);

            Assert.Throws<InvalidSyntaxException>(() => sut.Parse(string.Empty));
        }

        [Theory]
        [InlineData(TokenKind.Add, 2, 2)]
        [InlineData(TokenKind.Subtract, 2, 2)]
        [InlineData(TokenKind.Multiply, 2, 2)]
        [InlineData(TokenKind.Divide, 2, 2)]
        public void Parse_ParsesSimpleBinaryOperation(TokenKind operatorKind, double left, double right)
        {
            Token[] tokens = new[]
            {
                new Token()
                {
                    Kind = TokenKind.Number,
                    Length = 0,
                    Position = 0,
                    Value = left
                },
                new Token()
                {
                    Kind = operatorKind,
                    Length = 0,
                    Position = 0,
                },
                new Token()
                {
                    Kind = TokenKind.Number,
                    Length = 0,
                    Position = 0,
                    Value = right
                },
            };

            Mock<ITokenizer> tokenizerMock = new Mock<ITokenizer>();
            tokenizerMock.Setup(x => x.GetTokens(It.IsAny<string>()))
                .Returns(tokens);

            Parser sut = new Parser(tokenizerMock.Object);

            BinaryExpression result = (BinaryExpression)sut.Parse(string.Empty);

            Assert.Equal(left, ((ConstantExpression)result.Left).Value);
            Assert.Equal(right, ((ConstantExpression)result.Right).Value);
        }

        [Theory]
        [MemberData(nameof(BinaryExpressionsWithBrackets))]
        public void Parse_ParsesSimpleBinaryOperationWithBrackets(params Token[] tokens)
        {
            //Token[] tokens = new[]
            //{
            //    new Token()
            //    {
            //        Kind = TokenKind.Number,
            //        Length = 0,
            //        Position = 0,
            //        Value = left
            //    },
            //    new Token()
            //    {
            //        Kind = operatorKind,
            //        Length = 0,
            //        Position = 0,
            //    },
            //    new Token()
            //    {
            //        Kind = TokenKind.Number,
            //        Length = 0,
            //        Position = 0,
            //        Value = right
            //    },
            //};

            Mock<ITokenizer> tokenizerMock = new Mock<ITokenizer>();
            tokenizerMock.Setup(x => x.GetTokens(It.IsAny<string>()))
                .Returns(tokens);

            Parser sut = new Parser(tokenizerMock.Object);

            Expression result = sut.Parse(string.Empty);

            Assert.IsAssignableFrom<BinaryExpression>(result);
        }

        private static IEnumerable<Token[]> BinaryExpressionsWithBrackets()
        {
            //2 + (3 - 2)
            yield return new[]
            {
                new Token
                {
                    Kind = TokenKind.Number,
                    Position = 0,
                    Length = 0,
                    Value = 2D 
                },
                new Token
                {
                    Kind = TokenKind.Add,
                    Position = 0,
                    Length = 0
                },
                new Token
                {
                    Kind = TokenKind.OpenBracket,
                    Position = 0,
                    Length = 0
                },
                new Token
                {
                    Kind = TokenKind.Number,
                    Position = 0,
                    Length = 0,
                    Value = 3D
                },
                new Token
                {
                    Kind = TokenKind.Subtract,
                    Position = 0,
                    Length = 0
                },
                new Token
                {
                    Kind = TokenKind.Number,
                    Position = 0,
                    Length = 0,
                    Value = 2D
                },
                new Token
                {
                    Kind = TokenKind.CloseBracket,
                    Position = 0,
                    Length = 0
                }
            };

            //2 + 3 - (2 + 3)
            yield return new[]
            {
                new Token
                {
                    Kind = TokenKind.Number,
                    Position = 0,
                    Length = 0,
                    Value = 2D
                },
                new Token
                {
                    Kind = TokenKind.Add,
                    Position = 0,
                    Length = 0
                },
                new Token
                {
                    Kind = TokenKind.Number,
                    Position = 0,
                    Length = 0,
                    Value = 3D
                },
                new Token
                {
                    Kind = TokenKind.Subtract,
                    Position = 0,
                    Length = 0
                },
                new Token
                {
                    Kind = TokenKind.OpenBracket,
                    Position = 0,
                    Length = 0
                },
                new Token
                {
                    Kind = TokenKind.Number,
                    Position = 0,
                    Length = 0,
                    Value = 2D
                },
                new Token
                {
                    Kind = TokenKind.Add,
                    Position = 0,
                    Length = 0
                },
                new Token
                {
                    Kind = TokenKind.Number,
                    Position = 0,
                    Length = 0,
                    Value = 3D
                },
                new Token
                {
                    Kind = TokenKind.CloseBracket,
                    Position = 0,
                    Length = 0
                }
            };
        }
    }
}
