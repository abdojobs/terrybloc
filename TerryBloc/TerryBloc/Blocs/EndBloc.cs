using Microsoft.Xna.Framework.Content;

namespace TerryBloc
{
    public class EndBloc : Bloc
    {
        public EndBloc(int X, int Y)
            : base(X, Y)
        { }

        public override void LoadContent(ContentManager content)
        {
            base.LoadContent(content, "EndBloc");
        }
    }
}
