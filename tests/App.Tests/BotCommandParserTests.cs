using App.Core;
using Xunit;

namespace App.Tests;

public sealed class BotCommandParserTests
{
    [Fact]
    public void Parse_Returns_Command_With_Value()
    {
        var parser = new BotCommandParser('!');

        var result = parser.Parse("!kofre 12345");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("kofre", result.Value!.Name);
        Assert.Equal("12345", result.Value.Arguments["value"]);
    }
}
