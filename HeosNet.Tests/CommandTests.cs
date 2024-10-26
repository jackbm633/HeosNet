using FluentAssertions;
using HeosNet.Models;

namespace HeosNet.Tests;

[TestClass]
public class CommandTests
{
    /// <summary>
    /// Test to see if parsing and generating a heos command works.
    /// </summary>
    [TestMethod]
    public void HeosCommand_ParseAndGenerate_SimpleCommand()
    {
        // Arrange
        string command = "system/heart_beat";

        // Act
        HeosCommand result = HeosCommand.ParseHeosCommandString(command);

        // Assert
        result
            .Should()
            .BeEquivalentTo(new HeosCommand { CommandGroup = "system", Command = "heart_beat" });
    }
}
