namespace GUI;
class Button
{
    public Rectangle Rectangle { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public string Label { get; set; }
    public ButtonAction Action { get; set; }

    public Button(string label, float x, float y, float width, float height, ButtonAction action)
    {
        this.Label = label;

        this.Position = new Vector2(x, y);
        this.Size = new Vector2(width, height);

        this.Rectangle = new Rectangle(Position.X, Position.Y, Size.X, Size.Y);
        Action = action;
    }

    public bool IsClickedOn()
    {
        return CheckCollisionPointRec(GetMousePosition(), this.Rectangle) && IsMouseButtonReleased(MouseButton.Left);
    }
}