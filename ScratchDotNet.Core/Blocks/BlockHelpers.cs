using System.Text;

namespace ScratchDotNet.Core.Blocks;

/// <summary>
/// general helper methods for blocks
/// </summary>
public static class BlockHelpers
{
    /// <summary>
    /// Generates a random block id
    /// </summary>
    /// <returns>The generated id</returns>
    public static string GenerateBlockId()
    {
        const string characters = "#abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_=+[]{}|;:'<>,.?/";

        StringBuilder randomString = new(20);
        for (int i = 0; i < 20; i++)
        {
            int randomIndex = Random.Shared.Next(characters.Length);
            randomString.Append(characters[randomIndex]);
        }

        return randomString.ToString();
    }
}
