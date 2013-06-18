using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TerryBloc
{
    /// <summary>
    /// Pion représentant un joueur.
    /// </summary>
    public class PlayerBloc : Sprite
    {
        public short NumJoueur { get; private set; }

        public PlayerBloc(int X, int Y, short Joueur = 1)
            : base(X, Y)
        {
            NumJoueur = Joueur;
        }

        public void LoadContent(ContentManager content)
        {
            base.LoadContent(content, "PlayerBloc");
            base.Position = GetPositionVoulu();
        }

        /// <summary>
        /// Donne la position voulu 
        /// </summary>
        /// <returns></returns>
        public override Vector2 GetPositionVoulu()
        {
            return new Vector2(PosX * CST.LARGEUR_BLOC + (CST.LARGEUR_BLOC / 2 - Texture.Width / 2) + CST.DECALAGE_PLAINBLOC, PosY * CST.LARGEUR_BLOC + (CST.LARGEUR_BLOC / 2 - Texture.Height / 2) + CST.DECALAGE_PLAYEUR);
        }
    }
}
