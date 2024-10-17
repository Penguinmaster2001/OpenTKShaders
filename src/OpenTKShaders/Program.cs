
using OpenTKShaders.Graphics;



namespace OpenTKShaders;



public class Program
{
    private static string shadersDirectory = "Shaders";
    private static string defaultVertCameraShader = "defaultCamera";
    private static string defaultVertShader = "default";
    private static string defaultFragShader = "default";



    static void Main(string[] args)
    {
        for (int i = 0; i < 5; i++)
        {
            using(ShaderWindow shaderWindow = new(1440, 900))
            {
                LoadShaders(shaderWindow);

                shaderWindow.Run();
            }
        }
    }



    private static void LoadShaders(ShaderWindow shaderWindow)
    {
        Console.WriteLine("Enter name of shader: ");
        string shaderName = Console.ReadLine() ?? "Null string";

        string fragmentShaderPath = $"{shadersDirectory}/{shaderName}.frag";
        string vertexShaderPath = $"{shadersDirectory}/{shaderName}.vert";

        if (!File.Exists(vertexShaderPath))
        {
            Console.WriteLine("Using default vertex shader");
            vertexShaderPath = $"{shadersDirectory}/{defaultVertShader}.vert";
        }

        shaderWindow.LoadShaders(fragmentShaderPath, vertexShaderPath);
    }
}
