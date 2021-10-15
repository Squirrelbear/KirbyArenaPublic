using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace KirbyArena
{
    public class IslandBackground : LevelBackground
    {
        public ShipEnemy shipLeft, shipRight;

        public IslandBackground(Texture2D backgroundSprite, Rectangle displayRect, Level levelRef)
            : base(backgroundSprite, displayRect)
        {
            Texture2D shipBottomSprite = levelRef.loadTexture("Sprites\\ExtraContent\\ship_bottom");
            Texture2D shipTopSprite = levelRef.loadTexture("Sprites\\ExtraContent\\ship_top");
            Texture2D cannonBallSprite = levelRef.loadTexture("Sprites\\ExtraContent\\cannonball");
            Texture2D cannonSprite = levelRef.loadTexture("Sprites\\ExtraContent\\cannon");
            shipLeft = new ShipEnemy(displayRect, true, shipBottomSprite, shipTopSprite,
                                    cannonSprite, cannonBallSprite, levelRef);
            shipRight = new ShipEnemy(displayRect, false, shipBottomSprite, shipTopSprite,
                                    cannonSprite, cannonBallSprite, levelRef);
        }

        public override void update(GameTime gameTime)
        {
            base.update(gameTime);
            shipLeft.update(gameTime);
            shipRight.update(gameTime);
        }

        public override void drawBackground(SpriteBatch spriteBatch)
        {
            base.drawBackground(spriteBatch);
        }

        public override void drawForeground(SpriteBatch spriteBatch)
        {
            base.drawForeground(spriteBatch);
            shipLeft.drawCannonBalls(spriteBatch);
            shipRight.drawCannonBalls(spriteBatch);
            shipLeft.draw(spriteBatch);
            shipRight.draw(spriteBatch);
        }
    }
}
