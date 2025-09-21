using System;
using System.Drawing;
using System.Windows.Forms;

namespace EngineGDI
{
    static class Program
    {
        static int x = 100;
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Engine.Initialize("IERVA ENGINE", 800, 600, false);

            while (Engine.IsWindowOpen)
            {
                Engine.UpdateWindow();

                if (Engine.IsKeyPressed(Keys.Space))
                {
                    Engine.PlaySound("sound.wav");
                }

                if (Engine.IsKeyDown(Keys.Left))
                    x -= 5;

                Engine.Clear(Color.Black);
                Engine.Draw("test.png", x, 200);

                Engine.Window.Invalidate(); 
            }
        }
    }
}
