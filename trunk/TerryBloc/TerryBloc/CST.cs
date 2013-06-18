using System.IO;
using System.Reflection;
namespace TerryBloc
{
    public class CST
    {
        public const char BLOC = 'X';
        public const char PLAIN = 'N';
        public const char END = 'O';
        public const char JOUEUR = 'J';
        public const char FINLIGNE = '\n';

        /// <summary>
        /// Dossier contenant les niveaux
        /// </summary>
        public static string PATH_LEVEL = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Content\\Levels\\");

        public const int LARGEUR_BLOC = 50;

        public static int DECALAGE_PLAINBLOC = -5;
        public static int DECALAGE_PLAYEUR = -10;
    }

    public enum TypeDeplacement
    {
        None,
        Haut,
        Bas,
        Gauche,
        Droite
    }
}
