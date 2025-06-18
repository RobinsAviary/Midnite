using SFML.Graphics;
using SFML.Window;

using static SaverToy;
public class WindowEvent
{
    private List<Keyboard.Key> pressedKeys = [];
    private List<Keyboard.Key> releasedKeys = [];

    List<Keyboard.Key> FlushPressedKeys()
    {
        List<Keyboard.Key> result = pressedKeys; // We have to make a copy so we can clear everything before returning.
        pressedKeys.Clear();
        return result;
    }

    List<Keyboard.Key> FlushReleasedKeys()
    {
        List<Keyboard.Key> result = releasedKeys;
        releasedKeys.Clear();
        return result;
    }

    public void Closed(object sender, EventArgs e)
    {
        ((WindowBase)sender).Close();
    }

    public void KeyReleased(object sender, KeyEventArgs e)
    {
        Keyboard.Key key = e.Code;
    }

    public void Resized(object sender, SizeEventArgs e)
    {
        FloatRect visible = new(0, 0, e.Width, e.Height);

        ((RenderWindow)sender).SetView(new(visible));
    }

    public void TextEntered(object sender, TextEventArgs e)
    {
        enteredText += e.Unicode;
    }

    public void KeyPressed(object sender, KeyEventArgs e)
    {
        Keyboard.Key key = e.Code;

        pressedKeys.Add(key);
    }

    public void FocusLost(object sender, EventArgs e)
    {

    }

    public void FocusGained(object sender, EventArgs e)
    {

    }

    public void MouseEntered(object sender, EventArgs e)
    {

    }

    public void MouseLeft(object sender, EventArgs e)
    {

    }

    public void MouseButtonPressed(object sender, MouseButtonEventArgs e)
    {

    }

    public void MouseButtonReleased(object sender, MouseButtonEventArgs e)
    {

    }
}