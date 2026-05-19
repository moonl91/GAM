using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace HorrorGame;

public class Game1 : Game
{
    private Vector2 _cameraPosition;
    private int _screenWidth = 1780;
    private int _screenHeight = 900;
    
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private List<Scene> _scenes;
    private int _currentSceneIndex = 0;
    private Scene _currentScene;
    
    private Player _player;

    private RenderTarget2D _darknessLayer;
    private int _lightSize = 650;  

    private Texture2D _playerIdle;
    private Texture2D _playerWalk;
    private Texture2D _lockerTexture;
    private Texture2D _lockerOccupiedTexture;
    private Texture2D _enemyTexture;
    private Texture2D _lightTexture;
    
    private Texture2D _background0;  
    private Texture2D _background1;  
    private Texture2D _background2;  
    private Texture2D _foreground2;
    private Texture2D _lockerTexture2;
    
    private SoundEffect _hideSound;
    private SoundEffect _footstepSound;
    private SoundEffect _enemySpawnSound;
    private Song _backgroundMusic;
    
    private SpriteFont _font;
    private ModalWindow _modalWindow;
    private Texture2D _modalImage1;
    private Texture2D _modalImageStart;  
    
    private InteractionZone _interactionZone;
    private bool _interactionUsed = false;
    
    private bool _enemySpawned = false;
    private bool _enemySpawning = false;
    private float _enemySpawnTimer = 0f;
    private float _enemySpawnDelay = 3f;
    
    private bool _isTransitioning = false;
    
    private bool _startDialogShown = false;
    private int _worldWidth;


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        _graphics.PreferredBackBufferWidth = _screenWidth;
        _graphics.PreferredBackBufferHeight = _screenHeight;
        _graphics.ApplyChanges();
        
        Window.Title = "HIDE";
    }

    protected override void LoadContent()

    {
        _lightTexture = new Texture2D(GraphicsDevice, 1, 1);
        _lightTexture.SetData(new[] { Color.White });
        _lightSize = 300;
        _lightTexture = Content.Load<Texture2D>("light");  
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _darknessLayer = new RenderTarget2D(GraphicsDevice, _screenWidth, _screenHeight);
        
        _playerIdle = Content.Load<Texture2D>("player_idle");
        _playerWalk = Content.Load<Texture2D>("player_walk");
        
        _lockerTexture = Content.Load<Texture2D>("locker");
        _lockerOccupiedTexture = Content.Load<Texture2D>("locker_closed");
        _enemyTexture = Content.Load<Texture2D>("enemy");
        
        _background0 = Content.Load<Texture2D>("background0"); 
        _background1 = Content.Load<Texture2D>("background");   
        _background2 = Content.Load<Texture2D>("background2"); 
        _foreground2 = Content.Load<Texture2D>("background2_UP");
        
        _lockerTexture2 = _lockerTexture;
        
        _backgroundMusic = Content.Load<Song>("song");
        _hideSound = Content.Load<SoundEffect>("shkaf");
        _footstepSound = Content.Load<SoundEffect>("footstep");
        _enemySpawnSound = Content.Load<SoundEffect>("enemy_spawn");
        
        _font = Content.Load<SpriteFont>("Font");
        
        _modalImageStart = Content.Load<Texture2D>("modal_start"); 
        _modalImage1 = Content.Load<Texture2D>("modal_image");
        
        _modalWindow = new ModalWindow(_screenWidth, _screenHeight);
        _modalWindow.SetFont(_font);
        _modalWindow.AddPage(_modalImageStart, "После того как я проснулась, стало ясно, что это не то место");
        
        _modalWindow.OnWindowClosed += () => 
        {
            _interactionUsed = false;
        };
        
        _interactionZone = new InteractionZone(new Rectangle(2400, 0, 100, 900));
        
        MediaPlayer.IsRepeating = true;
        MediaPlayer.Play(_backgroundMusic);
        MediaPlayer.Volume = 0.5f;
        
        _scenes = new List<Scene>();
        
        int worldWidth0 = 2500;     
        int transitionX0 = 1500;     

        Scene scene0 = new Scene("Начало", 
            _background0, 
            null, 
            worldWidth0, transitionX0);
        _scenes.Add(scene0);

        int worldWidth1 = 2500;
        int transitionX1 = 1500;

        Scene scene1 = new Scene("Коридор", 
            _background1, 
            null, 
            worldWidth1, transitionX1);
        scene1.SetLocker(_lockerTexture, _lockerOccupiedTexture, 1380, 60, 500, 740);
        _scenes.Add(scene1);

        int worldWidth2 = 2500;
        int transitionX2 = -1;  

        Scene scene2 = new Scene("Комната", 
            _background2, 
            _foreground2, 
            worldWidth2, transitionX2);
        scene2.SetLocker(_lockerTexture2, _lockerOccupiedTexture, 1380, 60, 500, 740);
        _scenes.Add(scene2);
        
        int playerWidth = 800;
        int playerHeight = 700;
        float playerY = 160;
        float playerX = 60;
        
        _player = new Player(_playerIdle, _playerWalk, 
            new Vector2(playerX, playerY), playerWidth, playerHeight, 
            _hideSound, _footstepSound);
        
        _currentScene = _scenes[0];
        _currentSceneIndex = 0;
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
            keyboard.IsKeyDown(Keys.Escape))
            Exit();
        
        if (!_startDialogShown)
        {
            _startDialogShown = true;
            _modalWindow.Open();
        }
        
        if (!_isTransitioning && _currentScene.TransitionX > 0 && 
            _player.Position.X >= _currentScene.TransitionX && 
            _currentSceneIndex < _scenes.Count - 1 && keyboard.IsKeyDown(Keys.E))
        {
            _isTransitioning = true;
            
            _currentSceneIndex++;
            _currentScene = _scenes[_currentSceneIndex];
            
            _player.Position = new Vector2(60, _player.Position.Y);
            _player.UpdateBounds();
            
            _cameraPosition.X = 0;
            
            _enemySpawned = false;
            _enemySpawning = false;
            _enemySpawnTimer = 0f;
            
            _isTransitioning = false;
        }
        
        _player.Update(gameTime, _currentScene.Locker, _currentScene.WorldWidth);
        
        if (!_enemySpawned && !_enemySpawning && _player.IsHiding && _currentScene.Locker != null)
        {
            _enemySpawning = true;
            _enemySpawnTimer = _enemySpawnDelay;
            _enemySpawnSound?.Play();
        }
        
        if (_enemySpawning)
        {
            _enemySpawnTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (_enemySpawnTimer <= 0f)
            {
                _enemySpawning = false;
                _enemySpawned = true;
                
                int enemyX = _currentScene.Locker.Bounds.X + _currentScene.Locker.Bounds.Width + 100;
                _currentScene.SpawnEnemy(_enemyTexture, enemyX, 50);
            }
        }
        
        if (_currentScene.Enemy != null && _currentScene.Enemy.IsActive)
        {
            _currentScene.Enemy.Update(gameTime, _player, _currentScene.Locker);
            
            if (_player.CheckCollisionWithEnemy(_currentScene.Enemy))
            {
                _player.TakeDamage();
            }
        }
        
        if (_currentSceneIndex == 2 && !_interactionUsed && _interactionZone != null)
        {
            if (_interactionZone.IsPlayerInZone(_player))
            {
                if (keyboard.IsKeyDown(Keys.F))
                {
                    _interactionUsed = true;
                    _modalWindow.Open();
                }
            }
        }
        
        if (_modalWindow != null)
        {
            _modalWindow.Update(gameTime);
        }
        
        _cameraPosition.X = _player.Position.X - _screenWidth / 2;
        _cameraPosition.X = MathHelper.Clamp(_cameraPosition.X, 0, _currentScene.WorldWidth - _screenWidth);
        _cameraPosition.Y = 0;
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        
        _spriteBatch.Begin(transformMatrix: Matrix.CreateTranslation(-_cameraPosition.X, -_cameraPosition.Y, 0));
        _currentScene.DrawBackground(_spriteBatch);
        if (_currentScene.Locker != null) _currentScene.Locker.Draw(_spriteBatch);
        if (_currentScene.Enemy != null && _currentScene.Enemy.IsActive) _currentScene.Enemy.Draw(_spriteBatch);
        _player.Draw(_spriteBatch);
        _currentScene.DrawForeground(_spriteBatch);
        _spriteBatch.End();
        
        _spriteBatch.Begin();
        
        Texture2D pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
        _spriteBatch.Draw(pixel, new Rectangle(0, 0, _screenWidth, _screenHeight), new Color(0, 0, 0, 200));
        
        if (_lightTexture != null)
        {
            float offsetX = _player.IsFacingRight ? 200 : -150;  

            Vector2 screenLightPos = new Vector2(
                _player.Position.X + _player.Bounds.Width / 2 + offsetX - _cameraPosition.X,
                _player.Position.Y + _player.Bounds.Height / 2 - _cameraPosition.Y
            );
            
            float rotation = _player.IsFacingRight ? 0f : MathHelper.Pi;
            _spriteBatch.Draw(_lightTexture, screenLightPos, null, Color.White, rotation,
                new Vector2(_lightTexture.Width / 2, _lightTexture.Height / 2),
                (float)_lightSize / _lightTexture.Width, SpriteEffects.None, 0f);
        }
        
        _spriteBatch.End();
        
        if (_modalWindow != null && _modalWindow.IsVisible)
        {
            _spriteBatch.Begin();
            _modalWindow.Draw(_spriteBatch);
            _spriteBatch.End();
        }
        
        DrawStaminaBar();
        base.Draw(gameTime);
    }
    
    private void DrawStaminaBar()
    {
        if (_player == null) return;
        _spriteBatch.Begin();
        
        Texture2D whiteTexture = new Texture2D(GraphicsDevice, 1, 1);
        whiteTexture.SetData(new[] { Color.White });
        
        int barWidth = 200;
        int barHeight = 20;
        int barX = 20;
        int barY = 50;
        
        _spriteBatch.Draw(whiteTexture, new Rectangle(barX, barY, barWidth, barHeight), Color.Gray);
        
        float staminaPercent = _player.Stamina / _player.MaxStamina;
        int staminaWidth = (int)(barWidth * staminaPercent);
        _spriteBatch.Draw(whiteTexture, new Rectangle(barX, barY, staminaWidth, barHeight), Color.Maroon);
        
        _spriteBatch.End();
    }

}