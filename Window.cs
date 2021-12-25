using System;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using Rubiks;
using Common.Shader;
using Common.Texture;
using Common.CameraTTC;
using System.Collections.Generic;

namespace Window
{
    public class GLRubiksCube
    {
        public struct TexturePos
        {
            public float X1,Y1,X2,Y2;
            public TexturePos(float x1, float y1, float x2, float y2 )
            { X1 = x1; Y1 = y1; X2 = x2; Y2 = y2; }
        }
        public static readonly TexturePos[] texturePos =
            {
                new(0.0f, 0.66f, 0.25f, 1.0f),   // 0  - Белый
                new(0.25f, 0.66f, 0.5f, 1.0f),   // 1  - Белый Литер
                new(0.5f, 0.66f, 0.75f, 1.0f),   // 2  - Жёлтый
                new(0.75f, 0.66f, 1.0f, 1.0f),   // 3  - Жёлтый Литер

                new(0.0f, 0.33f, 0.25f, 0.66f),  // 4  - Зелёный
                new(0.25f, 0.33f, 0.5f, 0.66f),  // 5  - Зелёный Литер
                new(0.5f, 0.33f, 0.75f, 0.66f),  // 6  - Ораньжевый
                new(0.75f, 0.33f, 1.0f, 0.66f),  // 7  - Ораньжевый Литер

                new(0.0f, 0.0f, 0.25f, 0.33f),   // 8  - Синий
                new(0.25f, 0.0f, 0.5f, 0.33f),   // 9  - Синий Литер
                new(0.5f, 0.0f, 0.75f, 0.33f),   // 10 - Красный
                new(0.75f, 0.0f, 1.0f, 0.33f),   // 11 - Красный Литер
        };

        private static readonly Texture _texture = Texture.LoadFromFile("Resources/RubiksCube.png");
        public static Matrix4 View { set; get; }
        public static Matrix4 Projection { set; get; }

        public GLPart[] ArrGLParts;
        private List<GLPart> GLParts = new();
        public GLRubiksCube(RubiksCube rubiksCube)
        {
            for (int i = 0; i < rubiksCube.Part.GetLength(0); i++)
            {
                for (int j = 0; j < rubiksCube.Part.GetLength(1); j++)
                {
                    for (int k = 0; k < rubiksCube.Part.GetLength(2); k++)
                    {
                        GLParts.Add(new(rubiksCube.Part[i,j,k]));
                    }
                }
            }
            ArrGLParts = GLParts.ToArray();
        }
        public void Draw()
        {
            for (int i = 0; i < ArrGLParts.Length; i++)
            {
                ArrGLParts[i].Draw();
            }
        }
        public class GLPart
        {
            private readonly uint[] _indices =
            {
                0, 1, 3,    1, 2, 3,
                4, 5, 7,    5, 6, 7,
                8, 9,11,    9,10,11,
                12,13,15,   13,14,15,
                16,17,19,   17,18,19,
                20,21,23,   21,22,23
            };

            private readonly int _elementBufferObject;
            private readonly int _vertexBufferObject;
            private readonly int _vertexArrayObject;
            private readonly Shader _shader;
            public PartOfCube _part;
            private Matrix4 _model;
            public GLPart(PartOfCube part)
            {
                float[] _vertices =
                {
                    // Position         Texture coordinates
                     1.0f,  1.0f, 1.0f, texturePos[part.Front].X2, texturePos[part.Front].Y2,       // top right        //  0 - фасад
                     1.0f, -1.0f, 1.0f, texturePos[part.Front].X2, texturePos[part.Front].Y1,       // bottom right
                    -1.0f, -1.0f, 1.0f, texturePos[part.Front].X1, texturePos[part.Front].Y1,       // bottom left
                    -1.0f,  1.0f, 1.0f, texturePos[part.Front].X1, texturePos[part.Front].Y2,       // top left         //  3

                    -1.0f,  1.0f,  1.0f, texturePos[part.Left].X2, texturePos[part.Left].Y2,        // 4 - лево
                    -1.0f, -1.0f,  1.0f, texturePos[part.Left].X2, texturePos[part.Left].Y1,
                    -1.0f, -1.0f, -1.0f, texturePos[part.Left].X1, texturePos[part.Left].Y1,
                    -1.0f,  1.0f, -1.0f, texturePos[part.Left].X1, texturePos[part.Left].Y2,        // 7

                    -1.0f,  1.0f, -1.0f, texturePos[part.Rear].X2, texturePos[part.Rear].Y2,        // 8 - тыл
                    -1.0f, -1.0f, -1.0f, texturePos[part.Rear].X2, texturePos[part.Rear].Y1,
                     1.0f, -1.0f, -1.0f, texturePos[part.Rear].X1, texturePos[part.Rear].Y1,
                     1.0f,  1.0f, -1.0f, texturePos[part.Rear].X1, texturePos[part.Rear].Y2,        // 11

                     1.0f,  1.0f, -1.0f, texturePos[part.Right].X2, texturePos[part.Right].Y2,      // 12 - право
                     1.0f, -1.0f, -1.0f, texturePos[part.Right].X2, texturePos[part.Right].Y1,
                     1.0f, -1.0f,  1.0f, texturePos[part.Right].X1, texturePos[part.Right].Y1,
                     1.0f,  1.0f,  1.0f, texturePos[part.Right].X1, texturePos[part.Right].Y2,      // 15

                     1.0f,  1.0f,  1.0f, texturePos[part.Top].X2, texturePos[part.Top].Y2,          // 16 - верх
                    -1.0f,  1.0f,  1.0f, texturePos[part.Top].X2, texturePos[part.Top].Y1,
                    -1.0f,  1.0f, -1.0f, texturePos[part.Top].X1, texturePos[part.Top].Y1,
                     1.0f,  1.0f, -1.0f, texturePos[part.Top].X1, texturePos[part.Top].Y2,          // 19

                     1.0f, -1.0f,  1.0f, texturePos[part.Bottom].X2, texturePos[part.Bottom].Y2,    // 20 - низ
                    -1.0f, -1.0f,  1.0f, texturePos[part.Bottom].X2, texturePos[part.Bottom].Y1,
                    -1.0f, -1.0f, -1.0f, texturePos[part.Bottom].X1, texturePos[part.Bottom].Y1,
                     1.0f, -1.0f, -1.0f, texturePos[part.Bottom].X1, texturePos[part.Bottom].Y2,    // 23
                };

                _part = part;
                _model = part.Position * Matrix4.CreateScale(1/3.0f);

                _vertexArrayObject = GL.GenVertexArray();
                GL.BindVertexArray(_vertexArrayObject);

                _vertexBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

                _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
                _shader.Use();

                var vertexLocation = _shader.GetAttribLocation("aPosition");
                GL.EnableVertexAttribArray(vertexLocation);
                GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

                var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
                GL.EnableVertexAttribArray(texCoordLocation);
                GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

                _texture.Use(TextureUnit.Texture0);
                _shader.SetInt("texture0", 0);
            }
            public void Draw()
            {
                _model = _part.Position * Matrix4.CreateScale(1 / 3.0f) * _part.Rotation;

                GL.BindVertexArray(_vertexArrayObject);

                _texture.Use(TextureUnit.Texture0);
                _shader.Use();

                _shader.SetMatrix4("model", _model);
                _shader.SetMatrix4("view", View);
                _shader.SetMatrix4("projection", Projection);

                GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
            }
        }
    }
    public class Window : GameWindow
    {
        public RubiksCube cube = new();
        public GLRubiksCube GLCube ;

        private CameraTTC _camera;

        private bool _firstMove = true;

        private Vector2 _lastPos;

        private double _time;
        private float FremeTime;
        private float FPS;

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            GLCube = new(cube);

            // Мы инициализируем камеру так, чтобы она находилась на 3 единицы от прямоугольника.
            // Мы также задаем правильное соотношение сторон.
            _camera = new CameraTTC( 8, Size.X / (float)Size.Y);

            // Мы делаем курсор мыши невидимым и фиксируемым, чтобы можно было правильно перемещать камеру FPS.
            CursorGrabbed = true;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GLRubiksCube.View = _camera.GetViewMatrix();
            GLRubiksCube.Projection = _camera.GetProjectionMatrix();

            GLCube.Draw();

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            _time += 96.0 * e.Time;


            if (!IsFocused) // Check to see if the window is focused
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }

            const float cameraSpeed = 1.5f;
            const float sensitivity = 0.2f;

            
            if (input.IsKeyDown(Keys.KeyPadAdd))
            {
                _camera.Distance -= cameraSpeed * (float)e.Time; // Forward
            }
            if (input.IsKeyDown(Keys.KeyPadSubtract))
            {
                _camera.Distance += cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyPressed(Keys.O))
            {
                cube.Rotacion( input.IsKeyDown(Keys.LeftControl) ? cube.OrangeMiddle : cube.Orange, !input.IsKeyDown(Keys.LeftShift));
            }
            if (input.IsKeyPressed(Keys.G))
            {
                cube.Rotacion(input.IsKeyDown(Keys.LeftControl) ? cube.GreenMiddle : cube.Green, !input.IsKeyDown(Keys.LeftShift));
            }
            if (input.IsKeyPressed(Keys.B))
            {
                cube.Rotacion(input.IsKeyDown(Keys.LeftControl) ? cube.BlueMiddle : cube.Blue, !input.IsKeyDown(Keys.LeftShift));
            }
            if (input.IsKeyPressed(Keys.Y))
            {
                cube.Rotacion(input.IsKeyDown(Keys.LeftControl) ? cube.YellowMiddle : cube.Yellow, !input.IsKeyDown(Keys.LeftShift));
            }
            if (input.IsKeyPressed(Keys.W))
            {
                cube.Rotacion(input.IsKeyDown(Keys.LeftControl) ? cube.WhiteMiddle : cube.White, !input.IsKeyDown(Keys.LeftShift));
            }
            if (input.IsKeyPressed(Keys.R))
            {
                cube.Rotacion(input.IsKeyDown(Keys.LeftControl) ? cube.RedMiddle : cube.Red, !input.IsKeyDown(Keys.LeftShift));
            }
            if (input.IsKeyPressed(Keys.Space))
            {
                cube.Confuse(20);
            }
            if (input.IsKeyPressed(Keys.F1))
            {
                cube.Untangle();
            }
            else
            {
                cube.Rotacion();
            }

            // Get the mouse state
            var mouse = MouseState;

            if (_firstMove) // This bool variable is initially set to true.
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                // Calculate the offset of the mouse position
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }

            FremeTime += (float)e.Time;
            FPS++;
            if (FremeTime >= 1.0f)
            {
                Title = $"PracticeOpenTK - FPS {FPS}";
                FremeTime = 0;
                FPS = 0;
            }

        }

        // В функции колеса мыши мы управляем всем масштабированием камеры.
        // Это просто делается путем изменения FOV камеры.
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _camera.Fov -= e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            // Нам нужно обновить соотношение сторон после изменения размера окна.
            _camera.AspectRatio = Size.X / (float)Size.Y;
        }
    }
}
