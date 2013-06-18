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
        private TypeDeplacement _MovingType { get; set; }
        private bool _DeplacementJoueurSeul { get; set; }

        public SpriteFont WIN;

        /// <summary>
        /// Joueur actuel
        /// </summary>
        public short _CurrentPlayer { get; set; }

        /// <summary>
        /// Nombre de joueurs possible dans le niveau
        /// </summary>
        public int NbJoueurs { get { return Joueurs.Count; } }

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
            foreach (PlainBloc pb in Movers)
                pb.Initialize();
            foreach (PlayerBloc pb in Joueurs)
                pb.Initialize();
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

            var pBloc = GetPlayerBloc(_CurrentPlayer);
            if (!_FinJeu && !pBloc.InMoving && _MovingType != TypeDeplacement.None && !_DeplacementJoueurSeul)
            {
                bool OnlyPlayeur = false;
                if (IsDeplacementPossible(_MovingType, out OnlyPlayeur, true))
                    MoveBloc(_MovingType, OnlyPlayeur);
                else
                {
                    if (HasEndBloc(pBloc.PosX, pBloc.PosY))
                        _FinJeu = true;
                    else
                        ChangementJoueur();
                }
            }
        }

        /// <summary>
        /// Change le joueur courant
        /// </summary>
        private void ChangementJoueur()
        {
            if (_CurrentPlayer == NbJoueurs)
                _CurrentPlayer = 1;
            else
                _CurrentPlayer++;

            _MovingType = TypeDeplacement.None;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyboardState"></param>
        public void HandleInput(KeyboardState keyboardState)
        {
            var pBloc = GetPlayerBloc(_CurrentPlayer);
            if (pBloc.InMoving || _FinJeu)
                return;

            bool OnlyPlayeur = false;
            if (IsKeyPress(keyboardState, Keys.Down) && IsDeplacementPossible(TypeDeplacement.Bas, out OnlyPlayeur))
                _MovingType = TypeDeplacement.Bas;
            if (IsKeyPress(keyboardState, Keys.Up) && IsDeplacementPossible(TypeDeplacement.Haut, out OnlyPlayeur))
                _MovingType = TypeDeplacement.Haut;
            if (IsKeyPress(keyboardState, Keys.Left) && IsDeplacementPossible(TypeDeplacement.Gauche, out OnlyPlayeur))
                _MovingType = TypeDeplacement.Gauche;
            if (IsKeyPress(keyboardState, Keys.Right) && IsDeplacementPossible(TypeDeplacement.Droite, out OnlyPlayeur))
                _MovingType = TypeDeplacement.Droite;

            if (_MovingType != TypeDeplacement.None)
            {
                _DeplacementJoueurSeul = OnlyPlayeur;
                MoveBloc(_MovingType, OnlyPlayeur);
            }

            _oldKeyboardState = keyboardState;
        }

        /// <summary>
        /// Vérifie qu'un déplacement est possible
        /// </summary>
        /// <param name="typeDeplacement">type de déplacement</param>
        /// <param name="inmoving">Le joueur était-il déjà en déplacement</param>
        /// <returns></returns>
        private bool IsDeplacementPossible(TypeDeplacement typeDeplacement, out bool OnlyPlayeur, bool inmoving = false)
        {
            OnlyPlayeur = false;
            var X = GetPlayerBloc(_CurrentPlayer).PosX;
            var Y = GetPlayerBloc(_CurrentPlayer).PosY;

            switch (typeDeplacement)
            {
                case TypeDeplacement.Haut:
                    Y--;
                    break;
                case TypeDeplacement.Bas:
                    Y++;
                    break;
                case TypeDeplacement.Gauche:
                    X--;
                    break;
                case TypeDeplacement.Droite:
                    X++;
                    break;
            }

            if (!HasBloc(X, Y))
                return false;

            var plainBloc = GetPlainBloc(X, Y);
            if (plainBloc == null)
                return true;

            if (HasPlayerBloc(X, Y) || inmoving)
                return false;

            OnlyPlayeur = true;
            return true;
        }

        /// <summary>
        /// Vérifie qu'une touche est appuyée mais pas maintenue
        /// </summary>
        /// <param name="keyboardState">Etat actuel du clavier</param>
        /// <param name="keys">touche appuyée</param>
        /// <returns>true si déclenchement possible ; false sinon</returns>
        private bool IsKeyPress(KeyboardState keyboardState, Keys keys)
        {
            return keyboardState.IsKeyDown(keys) && _oldKeyboardState.IsKeyUp(keys);
        }

        /// <summary>
        /// Déplace le joueur et son bloc
        /// </summary>
        /// <param name="MovingType"></param>
        private void MoveBloc(TypeDeplacement MovingType, bool OnlyPlayeur)
        {
            var pBloc = GetPlayerBloc(_CurrentPlayer);

            if (OnlyPlayeur)
                _MovingType = TypeDeplacement.None;
            else
                GetPlainBloc(pBloc.PosX, pBloc.PosY).Move(MovingType);

            pBloc.Move(MovingType);
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
        /// Vérifie si un joueur est présent sur le bloc
        /// </summary>
        private bool HasPlayerBloc(int X, int Y)
        {
            return Joueurs.Any(b => b.PosX == X && b.PosY == Y);
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
    }
}
