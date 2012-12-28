using System;

namespace TerryBloc
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Terrybloc game = new Terrybloc())
            {
                game.Run();
            }
        }
    }
#endif
}

