using SFML.Graphics;
using SFML.Window;

using static SaverToy;
public static class WindowEvent
{
    public static void Closed(object sender, EventArgs e)
    {
        ((WindowBase)sender).Close();
    }

    public static void KeyReleased(object sender, KeyEventArgs e)
    {
        Keyboard.Key key = e.Code;
    }

    public static void Resized(object sender, SizeEventArgs e)
    {
        FloatRect visible = new(0, 0, e.Width, e.Height);

        ((RenderWindow)sender).SetView(new(visible));
    }

    public static void TextEntered(object sender, TextEventArgs e)
    {
        enteredText += e.Unicode;
    }

    public static void KeyPressed(object sender, KeyEventArgs e)
    {
        Keyboard.Key key = e.Code;

        if (key == Keyboard.Key.Escape)
        {
            ((WindowBase)sender).Close();
        }

        if (key == Keyboard.Key.Right)
        {

        }

        if (key == Keyboard.Key.Left)
        {

        }

        if (key == Keyboard.Key.Up)
        {

        }

        if (key == Keyboard.Key.Down)
        {

        }

        if (key == Keyboard.Key.Enter)
        {

        }

        if (key == Keyboard.Key.Delete)
        {

        }
    }

    public static void FocusLost(object sender, EventArgs e)
    {

    }

    public static void FocusGained(object sender, EventArgs e)
    {

    }

    public static void MouseEntered(object sender, EventArgs e)
    {

    }

    public static void MouseLeft(object sender, EventArgs e)
    {

    }

    public static void MouseButtonPressed(object sender, MouseButtonEventArgs e)
    {

    }

    public static void MouseButtonReleased(object sender, MouseButtonEventArgs e)
    {

    }
}