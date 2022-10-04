namespace Destuff.Client.Services;

internal static class Utilities
{
    internal static async void FireAndForget(this Task task)
    {
        try
        {
            await task;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
            throw;
        }
    }
}