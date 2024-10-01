#define CG_Debug

using System;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
    internal class SrPalito : Objeto
    {
        public Ponto4D Start { get; set; }
        public Ponto4D End { get; set; }

        private double _angulo;
        private double _raio;
        private double _dislocaX;

        public SrPalito(Objeto paiRef, ref char rotulo) : base(paiRef, ref rotulo)
        {
            PrimitivaTipo = PrimitiveType.Lines;
            PrimitivaTamanho = 1;
            Start = new Ponto4D(0, 0);
            End = new Ponto4D(0.35, 0.35);
            _dislocaX = 0;
            _angulo = 45;
            _raio = Matematica.distancia(Start, End);

            AdicionarPontosIniciais();
        }

        private void AdicionarPontosIniciais()
        {
            base.PontosAdicionar(Start);
            base.PontosAdicionar(End);
            base.ObjetoAtualizar();
        }

        private void AtualizarPontos()
        {
            End = Matematica.GerarPtosCirculo(_angulo, _raio);
            Start.X = _dislocaX;
            End.X += _dislocaX;

            base.PontosAlterar(Start, 0);
            base.PontosAlterar(End, 1);
            base.ObjetoAtualizar();
        }

        public void Movimentar(double valor)
        {
            _dislocaX += valor;
            AtualizarPontos();
        }

        public void MudaTamanho(double valor)
        {
            _raio += valor;
            AtualizarPontos();
        }

        public void Girar(double valor)
        {
            _angulo += valor;
            AtualizarPontos();
        }

#if CG_Debug
        public override string ToString()
        {
            return $"__ Objeto SrPalito _ Tipo: {PrimitivaTipo} _ Tamanho: {PrimitivaTamanho}\n{ImprimeToString()}";
        }
#endif
    }
}
