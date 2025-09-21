using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Windows.Forms;


namespace EngineGDI
{
    public static class Engine
    {

    // Motor 2D básico propio con GDI+, con:
    // Soporte de imágenes(texturas)
    // Sonido con SoundPlayer
    // Captura de teclado continua(IsKeyDown)
    // Captura de teclado puntual(IsKeyPressed)
    // Estructura lista para escalar(game loop separado, render con doble buffer, etc.)

        private class DrawCommand
        {
            public string TexturePath;
            public float X, Y, ScaleX, ScaleY;
        }

        private static Dictionary<string, Image> textures = new Dictionary<string, Image>();
        private static Dictionary<string, SoundPlayer> sounds = new Dictionary<string, SoundPlayer>();
        private static List<DrawCommand> drawQueue = new List<DrawCommand>();

        private static GameForm window;

        public static bool IsWindowOpen { get; private set; } = false;
        public static Form Window => window;

        private static HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private static HashSet<Keys> handledKeys = new HashSet<Keys>();


        public static void Initialize(string title = "Game", int width = 800, int height = 600, bool fullscreen = false)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            window = new GameForm
            {
                Text = title,
                ClientSize = new Size(width, height),
                StartPosition = FormStartPosition.CenterScreen
            };

            if (fullscreen)
                window.WindowState = FormWindowState.Maximized;

            window.FormClosed += (s, e) => IsWindowOpen = false;

            window.KeyDown += (s, e) =>
            {
                if (!pressedKeys.Contains(e.KeyCode))
                {
                    pressedKeys.Add(e.KeyCode);
                    handledKeys.Remove(e.KeyCode); 
                }
            };

            window.KeyUp += (s, e) =>
            {
                pressedKeys.Remove(e.KeyCode);
                handledKeys.Remove(e.KeyCode);
            };

            window.Show();
            window.Focus();
            window.KeyPreview = true;
            IsWindowOpen = true;
        }

        public static void UpdateWindow()
        {
            if (window != null && window.Created)
                Application.DoEvents();
        }


        public static void PlaySound(string path)
        {
            if (!sounds.ContainsKey(path))
                sounds[path] = new SoundPlayer(path);

            sounds[path].Play();
        }

        public static void Draw(string path, float x, float y, float scaleX = 1f, float scaleY = 1f)
        {
            if (!textures.ContainsKey(path))
                textures[path] = Image.FromFile(path);

            drawQueue.Add(new DrawCommand
            {
                TexturePath = path,
                X = x,
                Y = y,
                ScaleX = scaleX,
                ScaleY = scaleY
            });
        }

        public static void Clear(Color color)
        {
            window.ClearColor = color;
        }

        public static void OnKeyDown(KeyEventHandler handler)
        {
            window.KeyDown += handler;
        }

        public static void OnKeyUp(KeyEventHandler handler)
        {
            window.KeyUp += handler;
        }

        public static bool IsKeyDown(Keys key)
        {
            return pressedKeys.Contains(key);
        }

        public static bool IsKeyPressed(Keys key)
        {
            if (pressedKeys.Contains(key) && !handledKeys.Contains(key))
            {
                handledKeys.Add(key);
                return true;
            }

            return false;
        }

        private class GameForm : Form
        {
            public Color ClearColor = Color.Black;

            public GameForm()
            {
                this.DoubleBuffered = true;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                e.Graphics.Clear(ClearColor);

                foreach (var cmd in drawQueue)
                {
                    if (textures.ContainsKey(cmd.TexturePath))
                    {
                        var img = textures[cmd.TexturePath];
                        e.Graphics.DrawImage(img, cmd.X, cmd.Y, img.Width * cmd.ScaleX, img.Height * cmd.ScaleY);
                    }
                }

                drawQueue.Clear();
            }
        }
    }

}
