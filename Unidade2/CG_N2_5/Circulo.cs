using System;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
    internal class Circulo : Objeto
    {
        public double raio;
        public Ponto4D centro;

        public Circulo(Objeto paiRef, ref char rotulo, double _raio) : base(paiRef, ref rotulo)
        {
            PrimitivaTipo = PrimitiveType.LineLoop;
            raio = _raio;
            centro = new Ponto4D(0, 0);
            DesenharCirculo();
        }

        public Circulo(Objeto paiRef, ref char rotulo, double _raio, Ponto4D ptoDeslocamento) : base(paiRef, ref rotulo)
        {
            PrimitivaTipo = PrimitiveType.LineLoop;
            raio = _raio;
            centro = new Ponto4D(ptoDeslocamento.X, ptoDeslocamento.Y);
            DesenharCirculo(ptoDeslocamento);
        }

        public void Atualizar(Ponto4D ptoDeslocamento)
        {
            centro.X = ptoDeslocamento.X;
            centro.Y = ptoDeslocamento.Y;
            DesenharCirculo(ptoDeslocamento);
        }

        public bool EstaDentro(Circulo circuloMaior)
        {
            double dist = Matematica.DistanciaEuclidiana(this.centro, circuloMaior.centro);
            return dist + this.raio <= circuloMaior.raio;
        }

        private void DesenharCirculo(Ponto4D? ptoDeslocamento = null)
        {
            base.pontosLista.Clear();
            for (int i = 0; i < 360; i += 1)
            {
                Ponto4D ponto = Matematica.GerarPtosCirculo(i, raio);
                if (ptoDeslocamento != null)
                {
                    ponto.X += ptoDeslocamento.Value.X;
                    ponto.Y += ptoDeslocamento.Value.Y;
                }
                base.PontosAdicionar(ponto);
            }
            base.ObjetoAtualizar();
        }

#if CG_Debug
        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto Círculo _ Tipo: " + PrimitivaTipo + "\n";
            retorno += base.ToString();
            return (retorno);
        }
#endif
    }
}
