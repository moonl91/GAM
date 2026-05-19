using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HorrorGame;

public class Background
{
    private Texture2D _texture;
    private int _worldWidth;
    private int _worldHeight = 900;
    
    public Background(Texture2D texture, int worldWidth)
    {
        _texture = texture;
        _worldWidth = worldWidth;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, 
            new Rectangle(0, 0, _worldWidth, _worldHeight), 
            Color.White);
    }
}