using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System;

namespace gcgcg
{
    internal class SrPalito : Objeto
    {
        private double raio = 0.5; 
        private double angulo = 45;  
        private Ponto4D origem;     

        public SrPalito(Objeto _paiRef, ref char _rotulo) : base(_paiRef, ref _rotulo)
        {
            origem = new Ponto4D(0, 0); 
            AtualizarPosicao();
        }

        private void AtualizarPosicao()
        {
            base.pontosLista.Clear();

            double x = origem.X + raio * Math.Cos(Math.PI * angulo / 180.0);
            double y = origem.Y + raio * Math.Sin(Math.PI * angulo / 180.0);
            Ponto4D cabeca = new Ponto4D(x, y);

            base.PontosAdicionar(origem);
            base.PontosAdicionar(cabeca);

            Atualizar();
        }

        public void MoverLados(double deslocamento)
        {
            origem.X += deslocamento;
            AtualizarPosicao();
        }

        public void AlterarTamanho(double fator)
        {
            raio += fator;
            if (raio < 0.1) raio = 0.1;
            AtualizarPosicao();
        }

        public void Girar(double incrementoAngulo)
        {
            angulo += incrementoAngulo;
            AtualizarPosicao();
        }

        private void Atualizar()
        {
            PrimitivaTipo = PrimitiveType.Lines; 
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