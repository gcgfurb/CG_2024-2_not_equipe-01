using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;

namespace VectorEditor
{
    class Program : GameWindow
    {
        List<Polygon> polygons = new List<Polygon>();
        Polygon selectedPolygon = null;
        bool drawing = false;
        Vector2 lastMousePosition;

        public Program(int width, int height) : base(width, height) { }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            GL.ClearColor(0f, 0f, 0f, 1f);  // Cor de fundo preta
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetCursorState();

            Vector2 mousePosition = GetMousePosition();

            // 2. Adicionar polígonos
            if (mouseState.RightButton == ButtonState.Pressed && !drawing)
            {
                StartDrawingPolygon(mousePosition);
            }

            if (keyState.IsKeyDown(Key.Enter) && drawing)
            {
                EndDrawingPolygon();
            }

            // 3. Remover polígono
            if (keyState.IsKeyDown(Key.D) && selectedPolygon != null)
            {
                polygons.Remove(selectedPolygon);
                selectedPolygon = null;
            }

            // 4. Mover vértice
            if (keyState.IsKeyDown(Key.V) && selectedPolygon != null)
            {
                MoveClosestVertex(mousePosition);
            }

            // 5. Remover vértice
            if (keyState.IsKeyDown(Key.E) && selectedPolygon != null)
            {
                RemoveClosestVertex(mousePosition);
            }

            // 6. Exibir rastro
            if (drawing)
            {
                DrawTrail(mousePosition);
            }

            // 7. Alternar polígono entre aberto e fechado
            if (keyState.IsKeyDown(Key.P) && selectedPolygon != null)
            {
                selectedPolygon.ToggleOpenClose();
            }

            // 8. Alterar cor do polígono
            if (keyState.IsKeyDown(Key.R) && selectedPolygon != null)
            {
                selectedPolygon.SetColor(1f, 0f, 0f);
            }
            else if (keyState.IsKeyDown(Key.G) && selectedPolygon != null)
            {
                selectedPolygon.SetColor(0f, 1f, 0f);
            }
            else if (keyState.IsKeyDown(Key.B) && selectedPolygon != null)
            {
                selectedPolygon.SetColor(0f, 0f, 1f);
            }

            // 9. Selecionar polígono pelo clique do mouse
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                SelectPolygon(mousePosition);
            }

            // 10. Translação do polígono
            if (selectedPolygon != null)
            {
                if (keyState.IsKeyDown(Key.Up))
                    selectedPolygon.Translate(0, 1);
                if (keyState.IsKeyDown(Key.Down))
                    selectedPolygon.Translate(0, -1);
                if (keyState.IsKeyDown(Key.Left))
                    selectedPolygon.Translate(-1, 0);
                if (keyState.IsKeyDown(Key.Right))
                    selectedPolygon.Translate(1, 0);
            }

            // 11. Escalar o polígono
            if (selectedPolygon != null)
            {
                if (keyState.IsKeyDown(Key.Home))
                    selectedPolygon.Scale(1.1f);
                if (keyState.IsKeyDown(Key.End))
                    selectedPolygon.Scale(0.9f);
            }

            // 12. Rotacionar o polígono
            if (selectedPolygon != null)
            {
                if (keyState.IsKeyDown(Key.Number3))
                    selectedPolygon.Rotate(5f);
                if (keyState.IsKeyDown(Key.Number4))
                    selectedPolygon.Rotate(-5f);
            }

            // 13. Adicionar polígonos filhos (grafo de cena)
            if (keyState.IsKeyDown(Key.F) && selectedPolygon != null)
            {
                StartAddingChildPolygon(mousePosition);
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Desenhar todos os polígonos
            foreach (var polygon in polygons)
            {
                polygon.Draw(Matrix4.Identity);  // Inicia com a matriz identidade
            }

            if (selectedPolygon != null)
            {
                selectedPolygon.DrawBoundingBox();
            }

            SwapBuffers();
        }

        private void StartDrawingPolygon(Vector2 mousePosition)
        {
            drawing = true;
            selectedPolygon = new Polygon();
            selectedPolygon.AddVertex(mousePosition);
            polygons.Add(selectedPolygon);
        }

        private void EndDrawingPolygon()
        {
            drawing = false;
        }

        private void StartAddingChildPolygon(Vector2 mousePosition)
        {
            Polygon childPolygon = new Polygon();
            selectedPolygon.AddChild(childPolygon);
        }

        private void MoveClosestVertex(Vector2 mousePosition)
        {
            selectedPolygon.MoveClosestVertex(mousePosition);
        }

        private void RemoveClosestVertex(Vector2 mousePosition)
        {
            selectedPolygon.RemoveClosestVertex(mousePosition);
        }

        private void DrawTrail(Vector2 mousePosition)
        {
            selectedPolygon.AddVertex(mousePosition);  // Desenhar vértices enquanto o polígono está sendo criado
        }

        private void SelectPolygon(Vector2 mousePosition)
        {
            foreach (var polygon in polygons)
            {
                if (polygon.IsPointInside(mousePosition))
                {
                    selectedPolygon = polygon;
                    return;
                }
            }
            selectedPolygon = null;
        }

        private Vector2 GetMousePosition()
        {
            var mouse = Mouse.GetCursorState();
            var point = PointToClient(new System.Drawing.Point(mouse.X, mouse.Y));
            return new Vector2(point.X, Height - point.Y);  // Inverter coordenadas Y
        }

        static void Main(string[] args)
        {
            using (Program window = new Program(800, 600))
            {
                window.Run(60.0);
            }
        }
    }

    public class Polygon
    {
        private List<Vector2> vertices = new List<Vector2>();
        private bool isClosed = false;
        private Vector3 color = new Vector3(1f, 1f, 1f); // Cor branca por padrão
        private List<Polygon> children = new List<Polygon>();  // Polígonos filhos
        private Matrix4 transformMatrix = Matrix4.Identity;

        public void Draw(Matrix4 parentMatrix)
        {
            // Acumular transformações globais do pai
            Matrix4 globalMatrix = transformMatrix * parentMatrix;

            GL.PushMatrix();
            GL.MultMatrix(ref globalMatrix);

            GL.Color3(color.X, color.Y, color.Z);
            GL.Begin(isClosed ? PrimitiveType.Polygon : PrimitiveType.LineLoop);

            foreach (var vertex in vertices)
            {
                GL.Vertex2(vertex);
            }

            GL.End();

            // Desenhar os filhos, acumulando a transformação global
            foreach (var child in children)
            {
                child.Draw(globalMatrix);
            }

            GL.PopMatrix();
        }

        public void AddVertex(Vector2 vertex)
        {
            vertices.Add(vertex);
        }

        public void MoveClosestVertex(Vector2 mousePosition)
        {
            float minDistance = float.MaxValue;
            int closestVertex = -1;

            for (int i = 0; i < vertices.Count; i++)
            {
                float distance = (vertices[i] - mousePosition).LengthSquared;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestVertex = i;
                }
            }

            if (closestVertex != -1)
            {
                vertices[closestVertex] = mousePosition;
            }
        }

        public void RemoveClosestVertex(Vector2 mousePosition)
        {
            float minDistance = float.MaxValue;
            int closestVertex = -1;

            for (int i = 0; i < vertices.Count; i++)
            {
                float distance = (vertices[i] - mousePosition).LengthSquared;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestVertex = i;
                }
            }

            if (closestVertex != -1)
            {
                vertices.RemoveAt(closestVertex);
            }
        }

        public void ToggleOpenClose()
        {
            isClosed = !isClosed;
        }

        public void SetColor(float r, float g, float b)
        {
            color = new Vector3(r, g, b);
        }

        public void Translate(float dx, float dy)
        {
            transformMatrix = Matrix4.CreateTranslation(dx, dy, 0) * transformMatrix;
        }

        public void Scale(float factor)
        {
            Vector2 center = GetCenter();
            Matrix4 scaleMatrix = Matrix4.CreateScale(factor);
            transformMatrix = Matrix4.CreateTranslation(-center.X, -center.Y, 0) * scaleMatrix * Matrix4.CreateTranslation(center.X, center.Y, 0) * transformMatrix;
        }

        public void Rotate(float angleDegrees)
        {
            Vector2 center = GetCenter();
            Matrix4 rotationMatrix = Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(angleDegrees));
            transformMatrix = Matrix4.CreateTranslation(-center.X, -center.Y, 0) * rotationMatrix * Matrix4.CreateTranslation(center.X, center.Y, 0) * transformMatrix;
        }

        public void DrawBoundingBox()
        {
            Vector2 min = GetMinBounds();
            Vector2 max = GetMaxBounds();

            GL.Color3(1f, 1f, 0f); // BBox na cor amarela
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex2(min.X, min.Y);
            GL.Vertex2(max.X, min.Y);
            GL.Vertex2(max.X, max.Y);
            GL.Vertex2(min.X, max.Y);
            GL.End();
        }

        public void AddChild(Polygon child)
        {
            children.Add(child);
        }

        private Vector2 GetCenter()
        {
            Vector2 sum = Vector2.Zero;
            foreach (var vertex in vertices)
            {
                sum += vertex;
            }
            return sum / vertices.Count;
        }

        private Vector2 GetMinBounds()
        {
            Vector2 min = vertices[0];
            foreach (var vertex in vertices)
            {
                min = Vector2.ComponentMin(min, vertex);
            }
            return min;
        }

        private Vector2 GetMaxBounds()
        {
            Vector2 max = vertices[0];
            foreach (var vertex in vertices)
            {
                max = Vector2.ComponentMax(max, vertex);
            }
            return max;
        }

        public bool IsPointInside(Vector2 point)
        {
            // Algoritmo simples de interseção de Scan Line para seleção
            int intersections = 0;
            for (int i = 0; i < vertices.Count; i++)
            {
                int next = (i + 1) % vertices.Count;
                if ((vertices[i].Y > point.Y) != (vertices[next].Y > point.Y) &&
                    point.X < (vertices[next].X - vertices[i].X) * (point.Y - vertices[i].Y) / (vertices[next].Y - vertices[i].Y) + vertices[i].X)
                {
                    intersections++;
                }
            }
            return (intersections % 2 != 0);
        }
    }
}