using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace gcgcg
{
    internal class Spline : Objeto
    {
        private List<Ponto4D> pontosControle;
        private List<Ponto4D> pontosSpline;
        private int qtdPontosSpline = 20; // Número inicial de pontos calculados na spline

        public Spline(Objeto _paiRef, ref char _rotulo, List<Ponto4D> pontosControle) : base(_paiRef, ref _rotulo)
        {
            this.pontosControle = pontosControle;
            CalcularSpline();
        }

        public void AlterarQtdPontosSpline(int incremento)
        {
            qtdPontosSpline += incremento;
            if (qtdPontosSpline < 2) qtdPontosSpline = 2; // Mínimo de 2 pontos
            CalcularSpline();
        }

        public void AtualizarPontoControle(int indice, double deslocX, double deslocY)
        {
            // Atualiza a posição do ponto de controle selecionado
            pontosControle[indice].X += deslocX;
            pontosControle[indice].Y += deslocY;
            CalcularSpline();
        }

        private void CalcularSpline()
        {
            // Limpa os pontos anteriores da spline
            pontosSpline = new List<Ponto4D>();

            // Cálculo da spline (exemplo de uma curva de Bezier usando a fórmula geral)
            for (int i = 0; i <= qtdPontosSpline; i++)
            {
                double t = i / (double)qtdPontosSpline;
                Ponto4D pontoCalculado = CalcularBezier(t);
                pontosSpline.Add(pontoCalculado);
            }

            Atualizar();
        }

        // Exemplo de cálculo de uma spline (Bezier)
        private Ponto4D CalcularBezier(double t)
        {
            int n = pontosControle.Count - 1;
            Ponto4D ponto = new Ponto4D(0, 0);
            for (int i = 0; i <= n; i++)
            {
                double coefBinomial = Binomial(n, i);
                double termo = coefBinomial * Math.Pow(1 - t, n - i) * Math.Pow(t, i);
                ponto.X += termo * pontosControle[i].X;
                ponto.Y += termo * pontosControle[i].Y;
            }
            return ponto;
        }

        private double Binomial(int n, int k)
        {
            double resultado = 1;
            for (int i = 1; i <= k; i++)
            {
                resultado *= (n - (k - i)) / (double)i;
            }
            return resultado;
        }

        public void Atualizar()
        {
            // Limpa os pontos anteriores
            base.pontosLista.Clear();

            // Adiciona os pontos da spline
            foreach (var ponto in pontosSpline)
            {
                base.PontosAdicionar(ponto);
            }

            PrimitivaTipo = PrimitiveType.LineStrip; // Spline desenhada como uma linha conectada
            base.ObjetoAtualizar();
        }

#if CG_Debug
        public override string ToString()
        {
            string retorno = "__ Objeto Spline _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + pontosSpline.Count + "\n";
            retorno += base.ImprimeToString();
            return retorno;
        }
#endif
    }
}