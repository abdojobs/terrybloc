using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TerryBloc
{
    public class Sprite
    {
        #region Properties

        /// <summary>
        /// Récupère ou définit l'image du sprite
        /// </summary>
        public Texture2D Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }
        private Texture2D _texture;

        /// <summary>
        /// Récupère ou définit la position du Sprite
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        private Vector2 _position;

        /// <summary>
        /// Récupère ou définit la direction du sprite. Lorsque la direction est modifiée, elle est automatiquement normalisée.
        /// </summary>
        public Vector2 Direction
        {
            get { return _direction; }
            set { _direction = Vector2.Normalize(value); }
        }
        private Vector2 _direction;

        /// <summary>
        /// Récupère ou définit la vitesse de déplacement du sprite.
        /// </summary>
        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }
        private float _speed;

        /// <summary>
        /// Position en X sur le plateau
        /// </summary>
        public int PosX
        {
            get { return _posX; }
            set { _posX = value; }
        }
        private int _posX;

        /// <summary>
        /// Position en Y sur le plateau
        /// </summary>
        public int PosY
        {
            get { return _posY; }
            set { _posY = value; }
        }
        private int _posY;

        #endregion Properties

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        public Sprite(int X, int Y)
        {
            _posX = X;
            _posY = Y;
        }

        /// <summary>
        /// Initialise les variables du Sprite
        /// </summary>
        public virtual void Initialize()
        {
            _position = Vector2.Zero;
            _direction = Vector2.Zero;
            _speed = 0;
        }

        /// <summary>
        /// Charge l'image voulue grâce au ContentManager donné
        /// </summary>
        /// <param name="content">Le ContentManager qui chargera l'image</param>
        /// <param name="assetName">L'asset name de l'image à charger pour ce Sprite</param>
        public virtual void LoadContent(ContentManager content, string assetName)
        {
            _texture = content.Load<Texture2D>(assetName);
            _position = new Vector2(_posX * Texture.Width, _posY * Texture.Height);
        }

        /// <summary>
        /// Met à jour les variables du sprite
        /// </summary>
        /// <param name="gameTime">Le GameTime associé à la frame</param>
        public virtual void Update(GameTime gameTime)
        {
            _position += _direction * _speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        /// <summary>
        /// Permet de gérer les entrées du joueur
        /// </summary>
        /// <param name="keyboardState">L'état du clavier à tester</param>
        public virtual void HandleInput(KeyboardState keyboardState)
        {
        }

        /// <summary>
        /// Dessine le sprite en utilisant ses attributs et le spritebatch donné
        /// </summary>
        /// <param name="spriteBatch">Le spritebatch avec lequel dessiner</param>
        /// <param name="gameTime">Le GameTime de la frame</param>
        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(_texture, _position, Color.White);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public virtual void SetPosition(int X, int Y)
        {
            _posX = X;
            _posY = Y;
            _position = new Vector2(_posX * Texture.Width, _posY * Texture.Height);
        }

        #endregion Methods
    }
}
