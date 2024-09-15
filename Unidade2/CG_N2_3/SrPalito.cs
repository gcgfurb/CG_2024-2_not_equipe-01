using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;

namespace gcgcg
{
    internal class SrPalito : Objeto
    {
        private double raio = 0.5;   // Tamanho inicial
        private double angulo = 45;  // Ângulo inicial
        private Ponto4D origem;      // Ponto inicial para os pés (na origem)

        public SrPalito(Objeto _paiRef, ref char _rotulo) : base(_paiRef, ref _rotulo)
        {
            origem = new Ponto4D(0, 0); // Sr. Palito "nasce" com os pés na origem
            AtualizarPosicao();
        }

        private void AtualizarPosicao()
        {
            // Limpa os pontos anteriores
            base.pontosLista.Clear();

            // Calcula a nova posição da cabeça usando seno e cosseno (raio e ângulo)
            double x = origem.X + raio * Math.Cos(Math.PI * angulo / 180.0);
            double y = origem.Y + raio * Math.Sin(Math.PI * angulo / 180.0);
            Ponto4D cabeca = new Ponto4D(x, y);

            // Adiciona os pés na origem e a cabeça na posição calculada
            base.PontosAdicionar(origem);   // Pés
            base.PontosAdicionar(cabeca);   // Cabeça

            // Atualiza o objeto na tela
            Atualizar();
        }

        // Mover para os lados (Q e W)
        public void MoverLados(double deslocamento)
        {
            origem.X += deslocamento;
            AtualizarPosicao();
        }

        // Alterar o tamanho (A e S)
        public void AlterarTamanho(double fator)
        {
            raio += fator;
            if (raio < 0.1) raio = 0.1; // Limite mínimo para evitar tamanhos negativos ou muito pequenos
            AtualizarPosicao();
        }

        // Girar (Z e X)
        public void Girar(double incrementoAngulo)
        {
            angulo += incrementoAngulo;
            AtualizarPosicao();
        }

        private void Atualizar()
        {
            PrimitivaTipo = PrimitiveType.Lines; // Sr. Palito é representado como uma linha
            base.ObjetoAtualizar();
        }

#if CG_Debug
        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto SrPalito _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + raio + " _ Ângulo: " + angulo + "\n";
            retorno += base.ImprimeToString();
            return retorno;
        }
#endif
    }
}