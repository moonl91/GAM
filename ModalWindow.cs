using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HorrorGame;

public class ModalWindow
{
    public bool IsVisible { get; private set; } = false;
    public Action OnWindowClosed { get; internal set; }

    private Texture2D _image;
    private string _text;
    private int _screenWidth;
    private int _screenHeight;
    private SpriteFont _font; 
    
    private int _currentPage = 0;
    private List<ModalPage> _pages;
    
    public ModalWindow(int screenWidth, int screenHeight)
    {
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        _pages = new List<ModalPage>();
    }
    
    public void SetFont(SpriteFont font)
    {
        _font = font;
    }
    
    public void AddPage(Texture2D image, string text)
    {
        _pages.Add(new ModalPage { Image = image, Text = text });
    }
    
    public void Open()
    {
        IsVisible = true;
        _currentPage = 0;
    }
    
    public void Close()
    {
        IsVisible = false;
        OnWindowClosed?.Invoke();
    }
    
    public void Update(GameTime gameTime)
    {
        if (!IsVisible) return;
        
        KeyboardState keyboard = Keyboard.GetState();
        
        if (keyboard.IsKeyDown(Keys.E))
        {
            Close();
        }
        
        if (keyboard.IsKeyDown(Keys.F))
        {
            if (_currentPage < _pages.Count - 1)
            {
                _currentPage++;
            }
        }
        
        if (keyboard.IsKeyDown(Keys.Back))
        {
            if (_currentPage > 0)
            {
                _currentPage--;
            }
        }
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        if (!IsVisible || _pages.Count == 0) return;
        
        ModalPage current = _pages[_currentPage];
        
        Texture2D overlay = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        overlay.SetData(new[] { Color.Black * 0.8f });
        spriteBatch.Draw(overlay, new Rectangle(0, 0, _screenWidth, _screenHeight), Color.White);
        
        int imageWidth = _screenWidth - 200;
        int imageHeight = _screenHeight - 200;
        int imageX = (_screenWidth - imageWidth) / 2;
        int imageY = 50;
        
        if (current.Image != null)
        {
            spriteBatch.Draw(current.Image, 
                new Rectangle(imageX, imageY, imageWidth, imageHeight), 
                Color.White);
        }
        
        if (_font != null && !string.IsNullOrEmpty(current.Text))
        {
            string displayText = current.Text;
            if (_pages.Count > 1)
            {
                displayText += $" (страница {_currentPage + 1}/{_pages.Count})";
            }
            
            Vector2 textSize = _font.MeasureString(displayText);
            float textX = (_screenWidth - textSize.X) / 2;
            float textY = _screenHeight - 150;
            
            Texture2D textBg = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            textBg.SetData(new[] { Color.Black * 0.7f });
            spriteBatch.Draw(textBg, 
                new Rectangle((int)textX - 20, (int)textY - 10, 
                (int)textSize.X + 40, (int)textSize.Y + 30), 
                Color.White);
            
            spriteBatch.DrawString(_font, displayText, 
                new Vector2(textX, textY), Color.White);
        }
        
        //DrawControlsHint(spriteBatch);
    }
    
    //private void DrawControlsHint(SpriteBatch spriteBatch)
    //{
    //    if (_font == null) return;
        
    //    string hint = "E - закрыть";
    //    if (_currentPage > 0)
    //        hint += " | Backspace - назад";
    //    if (_currentPage < _pages.Count - 1)
    //        hint += " | F - далее";
        
    //    Vector2 hintSize = _font.MeasureString(hint);
    //    float hintX = (_screenWidth - hintSize.X) / 2;
    //    float hintY = _screenHeight - 50;
        
    //    spriteBatch.DrawString(_font, hint, new Vector2(hintX, hintY), Color.LightGray);
    //}
    
    private class ModalPage
    {
        public Texture2D Image { get; set; }
        public string Text { get; set; } = "";
    }
}