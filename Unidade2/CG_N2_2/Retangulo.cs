using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace gcgcg
{
    internal class Retangulo : Objeto
    {
        private List<PrimitiveType> primitivas;
        private int indicePrimitivaAtual = 0;

        public Retangulo(Objeto _paiRef, ref char _rotulo) : this(_paiRef, ref _rotulo, new Ponto4D(-0.5, -0.5), new Ponto4D(0.5, 0.5))
        {

        }

        public Retangulo(Objeto _paiRef, ref char _rotulo, Ponto4D ptoInfEsq, Ponto4D ptoSupDir) : base(_paiRef, ref _rotulo)
        {
            primitivas = new List<PrimitiveType>
            {
                PrimitiveType.Points,
                PrimitiveType.Lines,
                PrimitiveType.LineLoop,
                PrimitiveType.LineStrip,
                PrimitiveType.Triangles,
                PrimitiveType.TriangleStrip,
                PrimitiveType.TriangleFan
            };

            PrimitivaTipo = primitivas[indicePrimitivaAtual];
            PrimitivaTamanho = 10;

            base.PontosAdicionar(ptoInfEsq);
            base.PontosAdicionar(new Ponto4D(ptoSupDir.X, ptoInfEsq.Y));
            base.PontosAdicionar(ptoSupDir);
            base.PontosAdicionar(new Ponto4D(ptoInfEsq.X, ptoSupDir.Y));
            Atualizar();
        }

        public void AlternarPrimitiva()
        {
            indicePrimitivaAtual = (indicePrimitivaAtual + 1) % primitivas.Count;
            PrimitivaTipo = primitivas[indicePrimitivaAtual];
            Atualizar();
        }

        private void Atualizar()
        {
            base.ObjetoAtualizar();
        }

#if CG_Debug
        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto Retangulo _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
            retorno += base.ImprimeToString();
            return retorno;
        }
#endif
    }
}