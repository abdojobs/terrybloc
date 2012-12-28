using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace TerryBloc
{
    public class Bloc : Sprite
    {
        public Bloc(int X, int Y)
            : base(X, Y)
        { }

        public virtual void LoadContent(ContentManager content)
        {
            base.LoadContent(content, "Bloc");
        }

        public override void Update(GameTime gameTime)
        {
            // Un bloc ne bouge pas
        }
    }
}
