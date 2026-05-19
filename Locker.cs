using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HorrorGame;

public class Locker
{
    private Texture2D _emptyTexture;
    private Texture2D _occupiedTexture;
    public Rectangle Bounds { get; private set; }
    public bool IsOccupied { get; set; } = false;

    public Locker(Texture2D empty, Texture2D occupied, Rectangle bounds)
    {
        _emptyTexture = empty;
        _occupiedTexture = occupied;
        Bounds = bounds;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Texture2D currentTexture = IsOccupied ? _occupiedTexture : _emptyTexture;
        spriteBatch.Draw(currentTexture, Bounds, Color.White);
    }
}