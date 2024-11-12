using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;

namespace gcgcg
{
    public class Mundo : GameWindow
    {
        private static Objeto mundo;
        private char rotuloAtual = '?';
        private Dictionary<char, Objeto> grafoLista = new();
        private Objeto objetoSelecionado;
        private Poligono poligonoEmAndamento;
        private Transformacao4D matrizGrafo = new();
        private int qtdObjetos = 0;
        private int idxNewPto = 1;

        private Shader[] shaders = new Shader[7];
        private readonly string[] shaderPaths = 
        {
            "Shaders/shaderBranca.frag",
            "Shaders/shaderVermelha.frag",
            "Shaders/shaderVerde.frag",
            "Shaders/shaderAzul.frag",
            "Shaders/shaderCiano.frag",
            "Shaders/shaderMagenta.frag",
            "Shaders/shaderAmarela.frag"
        };

        public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) 
            : base(gameWindowSettings, nativeWindowSettings)
        {
            mundo ??= new Objeto(null, ref rotuloAtual);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            InicializarShaders();
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            InicializarPoligonos();
        }

        private void InicializarShaders()
        {
            for (int i = 0; i < shaders.Length; i++)
                shaders[i] = new Shader("Shaders/shader.vert", shaderPaths[i]);
        }

        private void InicializarPoligonos()
        {
            var pontosA = new List<Ponto4D>
            {
                new Ponto4D(0.25, 0.25),
                new Ponto4D(0.75, 0.25),
                new Ponto4D(0.75, 0.75),
                new Ponto4D(0.50, 0.50),
                new Ponto4D(0.25, 0.75)
            };
            objetoSelecionado = new Poligono(mundo, ref rotuloAtual, pontosA);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            matrizGrafo.AtribuirIdentidade();
            mundo.Desenhar(matrizGrafo, objetoSelecionado);
            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            var estadoTeclado = KeyboardState;
            if (estadoTeclado.IsKeyPressed(Keys.Escape))
                Close();

            ProcessarTeclado(estadoTeclado);
            ProcessarMouse();
        }

        private void ProcessarTeclado(KeyboardState estadoTeclado)
        {
            if (estadoTeclado.IsKeyPressed(Keys.Space))
                SelecionarProximoObjeto();

            if (estadoTeclado.IsKeyPressed(Keys.Enter))
                FinalizarPoligono();

            if (estadoTeclado.IsKeyPressed(Keys.D))
                RemoverObjetoSelecionado();

            if (estadoTeclado.IsKeyPressed(Keys.P) && objetoSelecionado != null)
                AlternarTipoPrimitiva();

            ProcessarTeclasCores(estadoTeclado);
            ProcessarTransformacoes(estadoTeclado);
        }

        private void ProcessarTeclasCores(KeyboardState estadoTeclado)
        {
            if (objetoSelecionado == null) return;

            if (estadoTeclado.IsKeyPressed(Keys.R))
                objetoSelecionado.ShaderObjeto = shaders[1]; // Vermelho
            if (estadoTeclado.IsKeyPressed(Keys.G))
                objetoSelecionado.ShaderObjeto = shaders[2]; // Verde
            if (estadoTeclado.IsKeyPressed(Keys.B))
                objetoSelecionado.ShaderObjeto = shaders[3]; // Azul
        }

        private void ProcessarTransformacoes(KeyboardState estadoTeclado)
        {
            if (objetoSelecionado == null) return;

            if (estadoTeclado.IsKeyPressed(Keys.Left))
                objetoSelecionado.MatrizTranslacaoXYZ(-0.05, 0, 0);
            if (estadoTeclado.IsKeyPressed(Keys.Right))
                objetoSelecionado.MatrizTranslacaoXYZ(0.05, 0, 0);
            if (estadoTeclado.IsKeyPressed(Keys.Up))
                objetoSelecionado.MatrizTranslacaoXYZ(0, 0.05, 0);
            if (estadoTeclado.IsKeyPressed(Keys.Down))
                objetoSelecionado.MatrizTranslacaoXYZ(0, -0.05, 0);
        }

        private void ProcessarMouse()
        {
            Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
            Ponto4D newPto = Utilitario.NDC_TelaSRU(ClientSize.X, ClientSize.Y, mousePonto);

            if (MouseState.IsButtonDown(MouseButton.Right))
            {
                if (poligonoEmAndamento != null)
                {
                    poligonoEmAndamento.PontosAlterar(newPto, idxNewPto);
                }
                else
                {
                    List<Ponto4D> pontosPoligono = new() { newPto, newPto };
                    poligonoEmAndamento = CriarNovoPoligono(pontosPoligono);
                    grafoLista.Add(poligonoEmAndamento.Rotulo, poligonoEmAndamento);
                }
            }

            if (MouseState.IsButtonReleased(MouseButton.Right))
            {
                if (poligonoEmAndamento != null)
                {
                    poligonoEmAndamento.PontosAdicionar(newPto);
                    idxNewPto++;
                }
            }

            if (MouseState.IsButtonPressed(MouseButton.Left))
            {
                SelecionarObjetoComMouse(newPto);
            }
        }

        private void SelecionarObjetoComMouse(Ponto4D pontoMouse)
        {
            foreach (var obj in grafoLista.Values)
            {
                if (obj.Bbox().Dentro(pontoMouse))
                {
                    objetoSelecionado = obj;
                    break;
                }
            }
        }

        private Poligono CriarNovoPoligono(List<Ponto4D> pontosPoligono)
        {
            return new Poligono(mundo, ref rotuloAtual, pontosPoligono);
        }

        private void SelecionarProximoObjeto()
        {
            objetoSelecionado = Grafocena.GrafoCenaProximo(mundo, objetoSelecionado, grafoLista);
        }

        private void FinalizarPoligono()
        {
            objetoSelecionado = poligonoEmAndamento;
            qtdObjetos++;
            poligonoEmAndamento = null;
            idxNewPto = 1;
        }

        private void RemoverObjetoSelecionado()
        {
            if (objetoSelecionado != null)
            {
                objetoSelecionado.ObjetoRemover();
                objetoSelecionado = null;
            }
        }

        private void AlternarTipoPrimitiva()
        {
            objetoSelecionado.PrimitivaTipo = (objetoSelecionado.PrimitivaTipo == PrimitiveType.LineStrip)
                ? PrimitiveType.LineLoop
                : PrimitiveType.LineStrip;
        }

        protected override void OnUnload()
        {
            mundo.OnUnload();
            foreach (var shader in shaders)
                GL.DeleteProgram(shader.Handle);
            base.OnUnload();
        }
    }
}
