using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HorrorGame;

public class Scene
{
    public string Name { get; private set; }
    
    // Слои фона
    public Texture2D BackgroundLayer { get; private set; } 
    public Texture2D ForegroundLayer { get; private set; }  
     public InteractionZone InteractionZone { get; set; }
    public Locker Locker { get; set; }
    public Enemy Enemy { get; set; }
    
    public int TransitionX { get; private set; }
    public int NextSceneIndex { get; set; } = -1; 
    
    public int WorldWidth { get; private set; }
    public int WorldHeight { get; private set; } = 900;
    
    public Scene(string name, Texture2D backgroundLayer, Texture2D foregroundLayer, 
                 int worldWidth, int transitionX)
    {
        Name = name;
        BackgroundLayer = backgroundLayer;
        ForegroundLayer = foregroundLayer;
        WorldWidth = worldWidth;
        TransitionX = transitionX;
    }
    
    public void SetLocker(Texture2D emptyTexture, Texture2D occupiedTexture, 
                          int x, int y, int width, int height)
    {
        Locker = new Locker(emptyTexture, occupiedTexture, 
            new Rectangle(x, y, width, height));
    }
    
    public void SpawnEnemy(Texture2D enemyTexture, int x, int y)
    {
        Enemy = new Enemy(enemyTexture, new Vector2(x, y));
        Enemy.IsActive = true;
    }
    
    public void DespawnEnemy()
    {
        Enemy = null;
    }
    
    public void DrawBackground(SpriteBatch spriteBatch)
    {
        if (BackgroundLayer != null)
        {
            spriteBatch.Draw(BackgroundLayer, 
                new Rectangle(0, 0, WorldWidth, WorldHeight), 
                Color.White);
        }
    }
    
    public void DrawForeground(SpriteBatch spriteBatch)
    {
        if (ForegroundLayer != null)
        {
            spriteBatch.Draw(ForegroundLayer, 
                new Rectangle(0, 0, WorldWidth, WorldHeight), 
                Color.White);
        }
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        DrawBackground(spriteBatch);
        
        if (Locker != null)
            Locker.Draw(spriteBatch);
        
        if (Enemy != null && Enemy.IsActive)
            Enemy.Draw(spriteBatch);
    }
}