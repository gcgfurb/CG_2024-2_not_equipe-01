#define CG_DEBUG
#define CG_Gizmo
#define CG_OpenGL

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using OpenTK.Mathematics;

namespace gcgcg
{
    public class Mundo : GameWindow
    {
        private static Objeto mundo = null;
        private char rotuloAtual = '?';
        private Dictionary<char, Objeto> grafoLista = new Dictionary<char, Objeto>();
        private Objeto objetoSelecionado = null;

#if CG_Gizmo
        private readonly float[] _sruEixos =
        {
            0.0f,  0.0f,  0.0f,
            0.0f,  0.0f,  0.0f,
            0.0f,  0.0f,  0.0f,
        };
        private int _vertexBufferObject_sruEixos;
        private int _vertexArrayObject_sruEixos;
#endif

        private Shader _shaderVermelha, _shaderVerde, _shaderAzul, _shaderCiano;
        private bool mouseMovtoPrimeiro = true;
        private Ponto4D mouseMovtoUltimo;

        public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            mundo ??= new Objeto(null, ref rotuloAtual);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            Utilitario.Diretivas();

#if CG_DEBUG
            Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            #region Cores
            _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
            _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
            _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
            _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
            #endregion

#if CG_Gizmo
            #region Eixos: SRU
            _vertexBufferObject_sruEixos = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
            GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
            _vertexArrayObject_sruEixos = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject_sruEixos);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            #endregion
#endif

            #region Objeto: SrPalito
            objetoSelecionado = new SrPalito(mundo, ref rotuloAtual);
            #endregion
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            mundo.Desenhar(new Transformacao4D(), objetoSelecionado);

#if CG_Gizmo
            Gizmo_Sru3D();
#endif

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var input = KeyboardState;
            if (input.IsKeyPressed(Keys.Escape))
            {
                Close();
            }
            else
            {
                AtualizarObjetoSelecionado(input);
            }
        }

        private void AtualizarObjetoSelecionado(KeyboardState input)
        {
            if (input.IsKeyPressed(Keys.W))
            {
                (objetoSelecionado as SrPalito)?.AtualizarPe(0.05);
            }
            if (input.IsKeyPressed(Keys.Q))
            {
                (objetoSelecionado as SrPalito)?.AtualizarPe(-0.05);
            }
            if (input.IsKeyPressed(Keys.S))
            {
                (objetoSelecionado as SrPalito)?.AtualizarRaio(0.05);
            }
            if (input.IsKeyPressed(Keys.A))
            {
                (objetoSelecionado as SrPalito)?.AtualizarRaio(-0.05);
            }
            if (input.IsKeyPressed(Keys.X))
            {
                (objetoSelecionado as SrPalito)?.AtualizarAngulo(5);
            }
            if (input.IsKeyPressed(Keys.Z))
            {
                (objetoSelecionado as SrPalito)?.AtualizarAngulo(-5);
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

#if CG_DEBUG
            Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        }

        protected override void OnUnload()
        {
            mundo.OnUnload();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

#if CG_Gizmo
            GL.DeleteBuffer(_vertexBufferObject_sruEixos);
            GL.DeleteVertexArray(_vertexArrayObject_sruEixos);
#endif

            GL.DeleteProgram(_shaderVermelha.Handle);
            GL.DeleteProgram(_shaderVerde.Handle);
            GL.DeleteProgram(_shaderAzul.Handle);
            GL.DeleteProgram(_shaderCiano.Handle);

            base.OnUnload();
        }

#if CG_Gizmo
        private void Gizmo_Sru3D()
        {
#if CG_OpenGL && !CG_DirectX
            var transform = Matrix4.Identity;
            GL.BindVertexArray(_vertexArrayObject_sruEixos);

            _shaderVermelha.SetMatrix4("transform", transform);
            _shaderVermelha.Use();
            GL.DrawArrays(PrimitiveType.Lines, 0, 2);

            _shaderVerde.SetMatrix4("transform", transform);
            _shaderVerde.Use();
            GL.DrawArrays(PrimitiveType.Lines, 2, 2);

            _shaderAzul.SetMatrix4("transform", transform);
            _shaderAzul.Use();
            GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#elif CG_DirectX && !CG_OpenGL
            Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
            Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
        }
#endif
    }
}
