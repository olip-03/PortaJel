namespace Portajel.Terminal.Struct;

public class FormItem(string title, bool protect = false)
{
    private string Title { get; } = title;
    public string? UserResponse { get; private set; }

    public void Query()
    {
        Console.Write($" {Title}: ");
        UserResponse = Console.ReadLine();
    }
}