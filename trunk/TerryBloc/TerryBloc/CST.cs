using System.Reflection;
using System.IO;
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
    }
}
