
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

using OpenTKShaders.Graphics;

using ErrorCode = OpenTK.Graphics.OpenGL4.ErrorCode;
using OpenTK.Platform.Windows;



namespace OpenTKShaders.Graphics;



internal class ShaderWindow : GameWindow
{
    private Quad windowQuad;

    private Camera camera;

    private int windowWidth;
    private int windowHeight;

    private ShaderProgram shaderProgram;



    public ShaderWindow(int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
    {
        windowWidth = width;
        windowHeight = height;

        CenterWindow(new Vector2i(windowWidth, windowHeight));

        windowQuad = new();
        camera = new(windowWidth, windowHeight, Vector3.Zero);
        shaderProgram = new();
    }


    
    public void LoadShaders(string fragmentShaderPath, string vertexShaderPath)
    {
        if (!File.Exists(fragmentShaderPath))
        {
            throw new FileNotFoundException("Fragment shader not found!", fragmentShaderPath);
        }

        if (!File.Exists(vertexShaderPath))
        {
            throw new FileNotFoundException("Vertex shader not found!", fragmentShaderPath);
        }

        shaderProgram.CreateNewProgram(fragmentShaderPath, vertexShaderPath);
    }



    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        windowWidth = e.Width;
        windowHeight = e.Height;
        GL.Viewport(0, 0, windowWidth, windowHeight);
        camera.ScreenWidth = windowWidth;
        camera.ScreenHeight = windowHeight;
    }



    protected override void OnLoad()
    {
        base.OnLoad();
        
        GL.Enable(EnableCap.DepthTest);
    }



    protected override void OnUnload()
    {
        base.OnUnload();

        windowQuad.Delete();
        shaderProgram.Delete();
    }



    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        MouseState mouseState = MouseState;
        KeyboardState keyboardState = KeyboardState;

        if (keyboardState.IsKeyReleased(Keys.Escape))
        {
            CursorState = CursorState == CursorState.Grabbed ? CursorState.Normal : CursorState.Grabbed;
        }
        camera.Update(keyboardState, mouseState, args);
    }



    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        GL.ClearColor(0.0627f, 0.0666f, 0.1019f, 1.0f);
        CheckGLError();
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        CheckGLError();

        if (shaderProgram.IsCompiled)
        {
            shaderProgram.SetUniform2("windowSize", new Vector2(windowWidth, windowHeight));
            shaderProgram.SetCameraUniforms(camera);

            windowQuad.Render(shaderProgram);
        }

        Context.SwapBuffers();
    }



    public static string LoadShaderSource(string filePath)
    {
        string shaderSource = "";

        try
        {
            using (StreamReader reader = new(filePath))
            {
                shaderSource = reader.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to load shader!!\nFilepath: {filePath}\n{e.Message}");
        }

        return shaderSource;
    }



    private static void CheckGLError()
    {
        ErrorCode error = GL.GetError();
        if (error != ErrorCode.NoError)
        {
            // throw new Exception($"OpenGL error: {error}");
            Console.WriteLine($"OpenGL error: {error}");
        }
    }
}

