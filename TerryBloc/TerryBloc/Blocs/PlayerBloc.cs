﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TerryBloc
{
    /// <summary>
    /// Pion représentant un joueur.
    /// </summary>
    public class PlayerBloc : Sprite
    {
        public short NumJoueur;

        public PlayerBloc(int X, int Y, short Joueur = 1)
            : base(X, Y)
        {
            NumJoueur = Joueur;
        }

        public void LoadContent(ContentManager content)
        {
            base.LoadContent(content, "PlayerBloc");
            Position = new Vector2(PosX * CST.LARGEUR_BLOC + (CST.LARGEUR_BLOC / 2 - Texture.Width / 2), PosY * CST.LARGEUR_BLOC + (CST.LARGEUR_BLOC / 2 - Texture.Height / 2));
        }

        public override void SetPosition(int X, int Y)
        {
            PosX = X;
            PosY = Y;
            Position = new Vector2(PosX * CST.LARGEUR_BLOC + (CST.LARGEUR_BLOC / 2 - Texture.Width / 2), PosY * CST.LARGEUR_BLOC + (CST.LARGEUR_BLOC / 2 - Texture.Height / 2));
        }
    }
}
