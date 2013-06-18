using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace TerryBloc
{
    public class PlainBloc : Sprite
    {
        public PlainBloc(int X, int Y)
            : base(X, Y)
        { }

        public void LoadContent(ContentManager content)
        {
            base.LoadContent(content, "PlainBloc");
            base.Position = GetPositionVoulu();
        }

        /// <summary>
        /// Donne la position voulu 
        /// </summary>
        /// <returns></returns>
        public override Vector2 GetPositionVoulu()
        {
            return new Vector2(PosX * CST.LARGEUR_BLOC + CST.DECALAGE_PLAINBLOC, PosY * CST.LARGEUR_BLOC + CST.DECALAGE_PLAINBLOC);
        }
    }
}
