
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;



namespace OpenTKShaders;



internal class Camera
{
    // TODO: These should be from another class that manages the screen
    private float _screenWidth;
    public float ScreenWidth
    {
        get => _screenWidth;
        set
        {
            _screenWidth = value;
            UpdateProjectionMatrix();
        }
    }

    private float _screenHeight;
    public float ScreenHeight
    {
        get => _screenHeight;
        set
        {
            _screenHeight = value;
            UpdateProjectionMatrix();
        }
    }
    
    private float _fov = 110;
    public float FOV
    {
        get => _fov;
        set
        {
            _fov = MathHelper.Clamp(value, 0.1f, 179.9f);
            UpdateProjectionMatrix();
        }
    }

    private float nearClip = 0.1f;
    private float farClip = 10_000.0f;
    private float sensitivity = 100.0f;
    private float MaxPitch = 89.99f;
    private float MinPitch = -89.99f;

    public float MovementSpeed { get; private set; } = 10.0f;
    public Vector3 Velocity { get; private set; }
    private bool firstMove = true;
    public Vector3 mouseLastPos;

    public Matrix4 ViewMatrix => Matrix4.LookAt(Position, Position + forward, up);

    private Matrix4 _projectionMatrix;
    public Matrix4 ProjectionMatrix { get => _projectionMatrix; private set => _projectionMatrix = value; }

    // TODO: This should be an affine matrix
    public Vector3 Position { get; private set; }
    
    /// <summary>
    /// Yaw, pitch, and roll in degrees
    /// </summary>
    public Vector3 RotationEuler;
    public Vector3 up = Vector3.UnitY;
    public Vector3 forward = Vector3.UnitZ;
    public Vector3 right = Vector3.UnitX;



    public Camera(float screenWidth, float screenHeight, Vector3 position)
    {
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
        UpdateProjectionMatrix();
        Velocity = Vector3.Zero;
        Position = position;
        RotationEuler = new(0.0f, 0.0f, -90.0f);
    }



    private void UpdateProjectionMatrix()
    {
        ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOV),
                                                                ScreenWidth / ScreenHeight,
                                                                nearClip,
                                                                farClip);
    }



    public void InputController(KeyboardState keyboardState, MouseState mouseState, float frameDelta)
    {
        Vector3 keyboardDirection = Vector3.Zero;

        if (keyboardState.IsKeyDown(Keys.W))
        {
            keyboardDirection += forward;
        }
        if (keyboardState.IsKeyDown(Keys.S))
        {
            keyboardDirection += -forward;
        }
        if (keyboardState.IsKeyDown(Keys.D))
        {
            keyboardDirection += right;
        }
        if (keyboardState.IsKeyDown(Keys.A))
        {
            keyboardDirection += -right;
        }
        if (keyboardState.IsKeyDown(Keys.E))
        {
            keyboardDirection += up;
        }
        if (keyboardState.IsKeyDown(Keys.Q))
        {
            keyboardDirection += -up;
        }

        if (keyboardDirection.LengthSquared > 0.0)
        {
            keyboardDirection.Normalize();
        }

        float scrollAmount = frameDelta * -mouseState.ScrollDelta.Y;
        if (keyboardState.IsKeyDown(Keys.LeftControl))
        {
            FOV += 50.0f * scrollAmount;
        }
        else
        {
            MovementSpeed += 100.0f * scrollAmount;
        }

        Velocity = MovementSpeed * keyboardDirection;

        Vector3 mouseCurPos = new(mouseState.X, -mouseState.Y, 0.0f);
        if (firstMove || frameDelta > 0.05f)
        {
            mouseLastPos = mouseCurPos;
            firstMove = false;
        }
        else
        {
            Vector3 mouseDelta = mouseCurPos - mouseLastPos;
            mouseLastPos = mouseCurPos;

            RotationEuler += frameDelta * (FOV / 180.0f) *  sensitivity * mouseDelta;

            if (RotationEuler.Y > MaxPitch) RotationEuler.Y = MaxPitch;
            else if (RotationEuler.Y < MinPitch) RotationEuler.Y = MinPitch;
        }
    }



    private void UpdateVectors(float frameDelta)
    {
        Position += frameDelta * Velocity;

        forward.X = MathF.Cos(MathHelper.DegreesToRadians(RotationEuler.Y)) * MathF.Cos(MathHelper.DegreesToRadians(RotationEuler.X));
        forward.Y = MathF.Sin(MathHelper.DegreesToRadians(RotationEuler.Y));
        forward.Z = MathF.Cos(MathHelper.DegreesToRadians(RotationEuler.Y)) * MathF.Sin(MathHelper.DegreesToRadians(RotationEuler.X));
        forward.Normalize();

        right = Vector3.Normalize(Vector3.Cross(forward, Vector3.UnitY));

        up = Vector3.Normalize(Vector3.Cross(right, forward));
    }



    public void Update(KeyboardState keyboardState, MouseState mouseState, FrameEventArgs e)
    {
        float frameDelta = (float) e.Time;

        InputController(keyboardState, mouseState, frameDelta);

        UpdateVectors(frameDelta);
    }
}
