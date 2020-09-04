/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GreatMachine.Models.ScreenSystem
{
    public class SpriteFonts
    {
        public SpriteFont DetailsFont { get; set; }        
        public SpriteFont MenuSpriteFont { get; set; }

        public SpriteFonts(ContentManager contentManager)
        {
            MenuSpriteFont = contentManager.Load<SpriteFont>("Fonts/menuFont");            
            DetailsFont = contentManager.Load<SpriteFont>("Fonts/detailsFont");
        }
    }
}
