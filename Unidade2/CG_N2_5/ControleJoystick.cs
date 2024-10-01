using System;
using CG_Biblioteca;
using OpenTK.Input;

namespace gcgcg
{
    internal class ControleJoystick
    {
        private Circulo _circuloMaior;
        private Circulo _circuloMenor;
        private double _passo = 0.05;

        public ControleJoystick(Circulo circuloMaior, Circulo circuloMenor)
        {
            _circuloMaior = circuloMaior;
            _circuloMenor = circuloMenor;
        }

        public void MovimentarCirculo(Key tecla)
        {
            Ponto4D novaPosicao = new Ponto4D(_circuloMenor.centro.X, _circuloMenor.centro.Y);

            switch (tecla)
            {
                case Key.C: // Mover para cima
                    novaPosicao.Y += _passo;
                    break;
                case Key.B: // Mover para baixo
                    novaPosicao.Y -= _passo;
                    break;
                case Key.E: // Mover para esquerda
                    novaPosicao.X -= _passo;
                    break;
                case Key.D: // Mover para direita
                    novaPosicao.X += _passo;
                    break;
            }

            // Teste de distância euclidiana e limite da BBox do círculo maior
            double dist = Matematica.DistanciaEuclidiana(novaPosicao, _circuloMaior.centro);

            if (dist + _circuloMenor.raio <= _circuloMaior.raio)
            {
                // Atualiza posição se estiver dentro do círculo maior
                _circuloMenor.Atualizar(novaPosicao);
            }
        }
    }
}
