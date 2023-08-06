using System;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using KeyEngine;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

class MainClass
{
    public static List<IGraphic> renderObjects = new List<IGraphic>();
    public static List<Component> Components = new List<Component>();
    public static Scene[] Scenes = new Scene[] { new Game(), new Scene2() };

    public static RenderWindow window;
    public static View Camera;

    private static Stopwatch deltaTimer;
    public static int currentSceneIndex;

    static void Main(string[] args)
    {
        ContextSettings appSettings = new ContextSettings()
        {
            AntialiasingLevel = 4,
        };

        //Инициализация окна
        window = new RenderWindow(new VideoMode(800, 600), "2D MineCamp", Styles.Default, appSettings);

        //Настройки окна
        window.SetFramerateLimit(120);
        window.SetVerticalSyncEnabled(false);
        window.SetKeyRepeatEnabled(false);

        //Эвенты окна
        window.Closed += OnWindowClosed;
        window.Resized += OnWindowResized;
        window.KeyPressed += OnKeyPressed;
        window.KeyReleased += OnKeyUp;
        window.TextEntered += OnTextEntered;

        //Инициализация разного

        deltaTimer = new Stopwatch();
        Camera = new View(new Vector2f(0, 0), new Vector2f(window.Size.X, window.Size.Y));

        //Линк камеры к окну
        window.SetView(Camera);

        Scenes[currentSceneIndex].Start();
        CallStart();

        while (window.IsOpen)
        {
            deltaTimer.Restart();

            window.DispatchEvents();
            window.Clear(new Color(63, 230, 58));

            CallUpdate();

            Render();

            window.Display();

            KeyTime.deltaTime = deltaTimer.ElapsedMilliseconds;
            //Console.WriteLine(KeyTime.deltaTime);
        }
    }

    private static void CallStart()
    {
        //Компоненты

        for (int i = 0; i < Components.Count; i++)
        {
            Components[i].Start();
        }

        //Другое
    }

    private static void CallUpdate()
    {
        Input.UpdateAxis();

        Scenes[currentSceneIndex].Update();

        for (int i = 0; i < Components.Count; i++)
        {
            Components[i].Update();
        }
    }

    private static void Render()
    {
        for (int i = 0; i < renderObjects.Count; i++)
        {
            IGraphic graphic = renderObjects[i];

            if (renderObjects[i].isActive == true)
                window.Draw(graphic.sharedShape);
        }
    }

    private static void OnWindowResized(object sender, SizeEventArgs e)
    {
        View view = new View(new Vector2f(0, 0), new Vector2f(e.Width, e.Height));
        Camera = view;
        window.SetView(view);
    }

    private static void OnWindowClosed(object sender, EventArgs e)
    {
        window.Close();
    }

    private static void OnKeyPressed(object sender, KeyEventArgs e)
    {

    }

    private static void OnKeyUp(object sender, KeyEventArgs e)
    {
        Input.systemKeyUp(e.Code);
    }

    private static void OnTextEntered(object sender, TextEventArgs e)
    {
        
    }

    public static void LoadScene(int index)
    {
        Console.Clear();

        currentSceneIndex = index;

        for (int i = 0; i < MainClass.renderObjects.Count; i++)
        {
            GameObject obj = MainClass.renderObjects[i] as GameObject;
            GameObject.Destroy(ref obj);
        }

        MainClass.renderObjects.Clear();
        MainClass.Components.Clear();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        Scenes[currentSceneIndex].Start();
        CallStart();
    }
}
