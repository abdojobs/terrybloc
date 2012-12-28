using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TerryBloc
{
    public class Level
    {
        public SpriteFont WIN;

        /// <summary>
        /// Joueur actuel
        /// </summary>
        public short _CurrentPlayer { get; set; }

        /// <summary>
        /// Nombre de joueurs possible dans le niveau
        /// </summary>
        public int NbJoueurs() { return Joueurs.Count; }

        /// <summary>
        /// true si un joueur est arrivé ; false sinon
        /// </summary>
        public bool _FinJeu { get; set; }

        /// <summary>
        /// Ancien statut du clavier
        /// </summary>
        public KeyboardState _oldKeyboardState;

        /// <summary>
        /// Blocs du plateau (tout bloc)
        /// </summary>
        public List<Bloc> Plateau;

        /// <summary>
        /// Blocs déplacables
        /// </summary>
        public List<PlainBloc> Movers;

        /// <summary>
        /// Joueurs
        /// </summary>
        public List<PlayerBloc> Joueurs;

        public Level()
        {
            Plateau = new List<Bloc>();
            Movers = new List<PlainBloc>();
            Joueurs = new List<PlayerBloc>();
        }

        public void Initialize()
        {
            _oldKeyboardState = Keyboard.GetState();
            _CurrentPlayer = 1;

            Loadlevel(1);

            foreach (Bloc b in Plateau)
                b.Initialize();
        }

        /// <summary>
        /// Chargement d'un niveau de test à partir d'un fichier
        /// </summary>
        private bool Loadlevel(short niveau)
        {
            string LevelPath = String.Concat(CST.PATH_LEVEL, niveau);

            if (File.Exists(LevelPath))
            {
                FileStream fs = null;
                try
                {
                    fs = File.OpenRead(LevelPath);

                    short ligne = 1;
                    short colonne = 1;
                    int code;
                    char c;

                    do
                    {
                        code = fs.ReadByte();
                        if (code != -1)
                        {
                            c = Convert.ToChar(code);

                            switch (c)
                            {
                                case CST.BLOC:
                                    Plateau.Add(new Bloc(colonne, ligne));
                                    break;
                                case CST.END:
                                    Plateau.Add(new EndBloc(colonne, ligne));
                                    break;
                                case CST.PLAIN:
                                    Plateau.Add(new Bloc(colonne, ligne));
                                    Movers.Add(new PlainBloc(colonne, ligne));
                                    break;
                                case CST.FINLIGNE:
                                    ligne++;
                                    colonne = 0;
                                    break;
                                default:
                                    short playeur = 0;
                                    if (short.TryParse(c.ToString(), out playeur))
                                    {
                                        Plateau.Add(new Bloc(colonne, ligne));
                                        Movers.Add(new PlainBloc(colonne, ligne));
                                        Joueurs.Add(new PlayerBloc(colonne, ligne, playeur));
                                    }
                                    break;
                            }
                            colonne++;
                        }
                    } while (code != -1);
                }
                catch
                {
                    // Fault
                }
                finally
                {
                    if (fs != null)
                        fs.Close();
                }
                return IsValidLevel();
            }
            return false;
        }

        /// <summary>
        /// Vérifie qu'un niveau chargé est valide : 
        ///     - une seule case d'arrivée
        ///     - au moins un joueur
        /// </summary>
        /// <returns>true si le niveau est valide ; false sinon</returns>
        private bool IsValidLevel()
        {
            return (Plateau.Count(b => b is EndBloc) == 1) && Joueurs.Any();
        }

        /// <summary>
        /// Chargement des textures
        /// </summary>
        /// <param name="content">ContentManager initialisé</param>
        public void LoadContent(ContentManager content)
        {
            foreach (Bloc b in Plateau)
                b.LoadContent(content);
            foreach (PlainBloc pb in Movers)
                pb.LoadContent(content);
            foreach (PlayerBloc pb in Joueurs)
                pb.LoadContent(content);

            WIN = content.Load<SpriteFont>("default");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            HandleInput(Keyboard.GetState());

            foreach (Bloc b in Plateau)
                b.Update(gameTime);
            foreach (PlainBloc pb in Movers)
                pb.Update(gameTime);
            foreach (PlayerBloc pb in Joueurs)
                pb.Update(gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyboardState"></param>
        public void HandleInput(KeyboardState keyboardState)
        {
            var stop = false;
            var deplacement = false;
            var pBloc = GetPlayerBloc(_CurrentPlayer);

            if (!_FinJeu)
            {
                if (keyboardState.IsKeyDown(Keys.Down) && _oldKeyboardState.IsKeyUp(Keys.Down))
                {
                    do
                    {
                        stop = GestionPosition(pBloc, pBloc.PosX, pBloc.PosY + 1, ref deplacement);
                    } while (!stop);
                }
                if (keyboardState.IsKeyDown(Keys.Up) && _oldKeyboardState.IsKeyUp(Keys.Up))
                {
                    do
                    {
                        stop = GestionPosition(pBloc, pBloc.PosX, pBloc.PosY - 1, ref deplacement);
                    } while (!stop);
                }
                if (keyboardState.IsKeyDown(Keys.Left) && _oldKeyboardState.IsKeyUp(Keys.Left))
                {
                    do
                    {
                        stop = GestionPosition(pBloc, pBloc.PosX - 1, pBloc.PosY, ref deplacement);
                    } while (!stop);
                }
                if (keyboardState.IsKeyDown(Keys.Right) && _oldKeyboardState.IsKeyUp(Keys.Right))
                {
                    do
                    {
                        stop = GestionPosition(pBloc, pBloc.PosX + 1, pBloc.PosY, ref deplacement);
                    } while (!stop);
                }

                if (deplacement)
                {
                    if (HasEndBloc(pBloc.PosX, pBloc.PosY))
                        _FinJeu = true;
                    else
                        ChangementJoueur();
                }
            }
            _oldKeyboardState = keyboardState;
        }

        /// <summary>
        /// Change le joueur courant
        /// </summary>
        private void ChangementJoueur()
        {
            if (_CurrentPlayer == NbJoueurs())
                _CurrentPlayer = 1;
            else
                _CurrentPlayer++;
        }

        /// <summary>
        /// Gestion de la position
        /// </summary>
        /// <param name="pBloc"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="deplacement"></param>
        /// <returns></returns>
        private bool GestionPosition(PlayerBloc pBloc, int X, int Y, ref bool deplacement)
        {
            var stop = false;

            if (HasBloc(X, Y))
            {
                var plainBloc = GetPlainBloc(X, Y);
                if (plainBloc == null)
                {
                    GetPlainBloc(pBloc.PosX, pBloc.PosY).SetPosition(X, Y);
                    pBloc.SetPosition(X, Y);
                    deplacement = true;
                }
                else if (!deplacement && !HasPlayerBloc(X, Y))
                {
                    pBloc.SetPosition(X, Y);
                    stop = true;
                }
                else
                    stop = true;
            }
            else
                stop = true;

            return stop;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (Bloc b in Plateau)
                b.Draw(spriteBatch, gameTime);
            foreach (PlainBloc pb in Movers)
                pb.Draw(spriteBatch, gameTime);
            foreach (PlayerBloc pb in Joueurs)
                pb.Draw(spriteBatch, gameTime);
            if (_FinJeu)
                spriteBatch.DrawString(WIN, String.Format("Le joueur {0} gagne !", _CurrentPlayer), Vector2.Zero, Color.White);
        }

        /// <summary>
        /// Vérifie si un bloc existe à un emplacement donné
        /// </summary>
        /// <param name="X">Position en X</param>
        /// <param name="Y">Position en Y</param>
        /// <returns>true si bloc trouvé ; false sinon</returns>
        private bool HasBloc(int X, int Y)
        {
            return Plateau.Any(b => b.PosX == X && b.PosY == Y);
        }

        /// <summary>
        /// Vérifie si un bloc de fin existe à un emplacement donné
        /// </summary>
        /// <param name="X">Position en X</param>
        /// <param name="Y">Position en Y</param>
        /// <returns>true si bloc trouvé ; false sinon</returns>
        private bool HasEndBloc(int X, int Y)
        {
            return Plateau.Any(b => b is EndBloc && b.PosX == X && b.PosY == Y);
        }

        /// <summary>
        /// Vérifie qu'un bloc plein existe
        /// </summary>
        /// <param name="X">Position en X</param>
        /// <param name="Y">Position en Y</param>
        /// <returns>true si trouvé ; false sinon</returns>
        private bool HasPlainBloc(int X, int Y)
        {
            return Movers.Any(b => b.PosX == X && b.PosY == Y);
        }

        /// <summary>
        /// Récupère un bloc plein
        /// </summary>
        /// <param name="X">Position en X</param>
        /// <param name="Y">Position en Y</param>
        /// <returns>Bloc plein si trouvé sinon null</returns>
        private PlainBloc GetPlainBloc(int X, int Y)
        {
            return Movers.FirstOrDefault(b => b.PosX == X && b.PosY == Y);
        }

        /// <summary>
        /// Récupère le bloc où se trouve le joueur
        /// </summary>
        /// <param name="pJoueur">Numéro du joueur</param>
        /// <returns>Bloc du joueur</returns>
        private PlayerBloc GetPlayerBloc(short pJoueur)
        {
            return Joueurs.FirstOrDefault(b => b.NumJoueur == pJoueur);
        }

        /// <summary>
        /// Vérifie si un joueur est présent sur le bloc
        /// </summary>
        private bool HasPlayerBloc(int X, int Y)
        {
            return Joueurs.Any(b => b.PosX == X && b.PosY == Y);
        }
    }
}
