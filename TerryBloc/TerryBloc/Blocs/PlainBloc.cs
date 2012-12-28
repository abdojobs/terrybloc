using Microsoft.Xna.Framework.Content;

namespace TerryBloc
{
    public class PlainBloc : Sprite
    {
        public PlainBloc(int X, int Y)
            : base(X, Y)
        { }

        public override void Initialize()
        {
        }

        public void LoadContent(ContentManager content)
        {
            base.LoadContent(content, "PlainBloc");
        }
    }
}
