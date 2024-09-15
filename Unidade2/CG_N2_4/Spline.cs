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
        private int qtdPontosSpline = 20;

        public Spline(Objeto _paiRef, ref char _rotulo, List<Ponto4D> pontosControle) : base(_paiRef, ref _rotulo)
        {
            this.pontosControle = pontosControle;
            CalcularSpline();
        }

        public void AlterarQtdPontosSpline(int incremento)
        {
            qtdPontosSpline += incremento;
            if (qtdPontosSpline < 2) qtdPontosSpline = 2;
            CalcularSpline();
        }

        public void AtualizarPontoControle(int indice, double deslocX, double deslocY)
        {
            pontosControle[indice].X += deslocX;
            pontosControle[indice].Y += deslocY;
            CalcularSpline();
        }

        private void CalcularSpline()
        {
            pontosSpline = new List<Ponto4D>();

            for (int i = 0; i <= qtdPontosSpline; i++)
            {
                double t = i / (double)qtdPontosSpline;
                Ponto4D pontoCalculado = CalcularBezier(t);
                pontosSpline.Add(pontoCalculado);
            }

            Atualizar();
        }

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
            base.pontosLista.Clear();

            foreach (var ponto in pontosSpline)
            {
                base.PontosAdicionar(ponto);
            }

            PrimitivaTipo = PrimitiveType.LineStrip;
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