using System;
using System.Linq;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
    internal class Circulo : Objeto
    {
        public Circulo(Objeto paiRef, ref char rotulo, double tamanho) : base(paiRef, ref rotulo)
        {
            PrimitivaTipo = PrimitiveType.Points;
            GerarCirculo(tamanho);
        }

        private void GerarCirculo(double tamanho)
        {
            // Usando LINQ para gerar os pontos do círculo
            var pontos = Enumerable.Range(0, 360 / 5) // Intervalos de 5 graus
                                   .Select(i => Matematica.GerarPtosCirculo(i * 5, tamanho));

            foreach (var ponto in pontos)
            {
                base.PontosAdicionar(ponto);
            }

            base.ObjetoAtualizar();
        }

        #if CG_Debug
        public override string ToString()
        {
            return $"__ Objeto Círculo _ Tipo: {PrimitivaTipo}\n{base.ToString()}";
        }
        #endif
    }
}
