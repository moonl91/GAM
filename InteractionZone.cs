using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HorrorGame;

public class InteractionZone
{
    public Rectangle Bounds { get; private set; }
    public bool IsActive { get; set; } = true;
    public string InteractionText { get; set; } = "Нажми F, чтобы осмотреть";
    
    //private Texture2D _debugTexture; 
    
    public InteractionZone(Rectangle bounds)
    {
        Bounds = bounds;
    }
    
    public bool IsPlayerInZone(Player player)
    {
        return IsActive && player.Bounds.Intersects(Bounds);
    }

    internal void DrawDebug(SpriteBatch spriteBatch)
    {
        throw new NotImplementedException();
    }

    //public void DrawDebug(SpriteBatch spriteBatch)
    //{
    // Для отладки: рисуем зону взаимодействия (можно закомментировать)
    //if (_debugTexture == null)
    //{
    //_debugTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
    //_debugTexture.SetData(new[] { Color.Yellow * 0.3f });
    //}
    //spriteBatch.Draw(_debugTexture, Bounds, Color.Yellow * 0.3f);
    //}
}