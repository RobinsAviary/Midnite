using SFML.Graphics;
using SFML.Window;

using static Midnite;
public class WindowEvent
{
    private List<Keyboard.Key> pressedKeys = [];
    private List<Keyboard.Key> releasedKeys = [];

    public List<Keyboard.Key> PressedKeys
    {
        get
        {
            return pressedKeys;
        }
    }

    public List<Keyboard.Key> ReleasedKeys
    {
        get
        {
            return releasedKeys;
        }
    }

    string typedText = "";

    public string TypedText
    {
        get
        {
            return typedText;
        }
    }

    public void ClearTypedText()
    {
        typedText = "";
    }

    public void ClearKeys()
    {
        releasedKeys.Clear();
        pressedKeys.Clear();
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
        typedText += e.Unicode;
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