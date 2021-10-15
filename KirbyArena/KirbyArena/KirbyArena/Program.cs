using System;

namespace KirbyArena
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (KirbyGame game = new KirbyGame())
            {
                game.Run();
            }
        }
    }
#endif
}

