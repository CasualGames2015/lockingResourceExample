using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Sprites
{
    public class SimpleSprite
    {
        public Texture2D Image;
        public Vector2 Position;
        private Rectangle boundingRect;
        public bool Visible = true;
        public bool Moving = false;
        public Vector2 Target;

        public Rectangle BoundingRect
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y,
                Image.Width, Image.Height); ;
            }
            
        }

        // Constructor epects to see a loaded Texture
        // and a start position
        public SimpleSprite(Texture2D spriteImage,
                            Vector2 startPosition)
        {
            // Take a copy of the texture passed down
            Image = spriteImage;
            // Take a copy of the start position
            Target = Position = startPosition;
            // Calculate the bounding rectangle
        }

        public void draw(SpriteBatch sp)
        {
            if(Visible)
                sp.Draw(Image, Position, Color.White);
        }

        public void Update(GameTime gameTime)
        {
            MouseState mState = Mouse.GetState();
            if(mState.LeftButton == ButtonState.Pressed)
            {
                if(BoundingRect.Contains(mState.Position))
                {
                    Visible = false;
                }
            }
            float distance = Vector2.Distance(Target,Position);

            if (Moving && Visible && distance > 0.1f)
            {
                Position = Vector2.Lerp(Position, Target, 0.01f);
            }
            else
            {
                Position = Target;
                Moving = false;
            }
        }

        public void startMove(int x, int y)
        {
            Moving = true;
            Target = new Vector2(x, y);
        }


        public void Move(Vector2 delta)
        {
            Position += delta;
        }
    }
}
