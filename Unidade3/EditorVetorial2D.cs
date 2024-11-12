#define CG_Debug

using System.Collections.Generic;
using System.Linq;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
    internal class EditorVetorial2D : Objeto
    {
        private List<Poligono> _poligonos;
        private int _indicePoligonoSelecionado;
        private bool _estaEditandoPoligono;

        public EditorVetorial2D(Objeto _paiRef, ref char _rotulo) : base(_paiRef, ref _rotulo)
        {
            PrimitivaTipo = PrimitiveType.Points;
            PrimitivaTamanho = 1;

            _poligonos = new List<Poligono>();
            _indicePoligonoSelecionado = -1;
            _estaEditandoPoligono = false;

            Atualizar();
        }

        public bool EstaEditandoPoligono()
        {
            return _estaEditandoPoligono;
        }

        public void AdicionarNovoPoligono(Poligono poligono)
        {
            if (EstaEditandoPoligono())
                return;

            _poligonos.Add(poligono);
            _indicePoligonoSelecionado = _poligonos.Count - 1;
            _estaEditandoPoligono = true;

            Atualizar();
        }

        public void AdicionarPontoPoligono(Ponto4D ponto)
        {
            if (!EstaEditandoPoligono())
                return;

            var poligono = _poligonos.Last();

            // ponto de rastro
            if (poligono.pontosLista.Count == 0)
                poligono.PontosAdicionar(ponto);

            poligono.PontosAdicionar(ponto);

            Atualizar();
        }

        public void AlterarPontoRastroPoligono(Ponto4D ponto)
        {
            if (!EstaEditandoPoligono())
                return;

            var poligono = _poligonos.Last();
            poligono.PontosAlterar(ponto, poligono.pontosLista.Count - 1);
        }

        public void FinalizarPoligono()
        {
            if (!EstaEditandoPoligono())
                return;

            // ponto de rastro
            var poligono = _poligonos.Last();
            poligono.PontosRemover(poligono.PontosListaTamanho - 1);

            _estaEditandoPoligono = false;

            Atualizar();
        }

        public Poligono ObterPoligonoSelecionado()
        {
            if (_indicePoligonoSelecionado < 0 || _indicePoligonoSelecionado >= _poligonos.Count)
                return null;

            return _poligonos[_indicePoligonoSelecionado];
        }

        public Poligono SelecionarPoligono(Ponto4D pontoClique)
        {
            for (var indicePoligono = 0; indicePoligono < _poligonos.Count; indicePoligono++)
            {
                var poligono = _poligonos[indicePoligono];

                var flagBBoxPoligono = poligono.Bbox().Dentro(pontoClique);

                if (!flagBBoxPoligono)
                    continue;

                var pontosPoligono = poligono.pontosLista.Append(poligono.pontosLista.First()).ToArray();
                var qtdFlagScanLineTrue = 0;

                for (var indicePonto = 0; indicePonto < poligono.pontosLista.Count; indicePonto++)
                {
                    var ponto1 = pontosPoligono[indicePonto];
                    var ponto2 = pontosPoligono[indicePonto + 1];

                    var flagScanLine = Matematica.ScanLine(pontoClique, ponto1, ponto2);

                    if (flagScanLine)
                        qtdFlagScanLineTrue++;
                }

                if (qtdFlagScanLineTrue % 2 != 0)
                {
                    _indicePoligonoSelecionado = indicePoligono;
                    return poligono;
                }
            }

            _indicePoligonoSelecionado = -1;
            return null;
        }

        public void RemoverPoligonoSelecionado()
        {
            var poligonoSelecionado = ObterPoligonoSelecionado();

            if (poligonoSelecionado == null)
                return;

            var poligonosRemover = new List<Poligono>(_poligonos.Count) { poligonoSelecionado };

            foreach (var poligono in _poligonos)
            {
                var paiPoligonoFoiRemovido = poligonosRemover.Contains(poligono.ObterPai());

                if (paiPoligonoFoiRemovido)
                    poligonosRemover.Add(poligono);
            }

            _poligonos.RemoveAll(x => poligonosRemover.Contains(x));
            _indicePoligonoSelecionado = -1;

            base.FilhoRemover(poligonoSelecionado);

            Atualizar();
        }

        public void AlterarPontoMaisProximoPoligonoSelecionado(Ponto4D pontoClique)
        {
            var poligonoSelecionado = ObterPoligonoSelecionado();

            if (poligonoSelecionado == null || poligonoSelecionado.pontosLista.Count == 0)
                return;

            var indicePontoMenorDistancia = SelecionarPontoMaisProximoPoligonoSelecionado(poligonoSelecionado, pontoClique);

            poligonoSelecionado.PontosAlterar(pontoClique, indicePontoMenorDistancia);

            Atualizar();
        }

        public void RemoverPontoMaisProximoPoligonoSelecionado(Ponto4D pontoClique)
        {
            var poligonoSelecionado = ObterPoligonoSelecionado();

            if (poligonoSelecionado == null || poligonoSelecionado.pontosLista.Count == 0)
                return;

            var indicePontoMenorDistancia = SelecionarPontoMaisProximoPoligonoSelecionado(poligonoSelecionado, pontoClique);

            poligonoSelecionado.PontosRemover(indicePontoMenorDistancia);

            Atualizar();
        }

        private int SelecionarPontoMaisProximoPoligonoSelecionado(Poligono poligonoSelecionado, Ponto4D pontoClique)
        {
            (double Distancia, int Indice) pontoMenorDistancia = (double.MaxValue, -1);

            for (var i = 0; i < poligonoSelecionado.pontosLista.Count; i++)
            {
                var pontoPoligono = poligonoSelecionado.pontosLista[i];

                var distancia = Matematica.Distancia(pontoClique, pontoPoligono);

                if (distancia < pontoMenorDistancia.Distancia)
                    pontoMenorDistancia = (distancia, i);
            }

            return pontoMenorDistancia.Indice;
        }

        public void TranslacaoPoligonoSelecionado(double tx, double ty, double tz)
        {
            var poligonoSelecionado = ObterPoligonoSelecionado();

            if (poligonoSelecionado == null)
                return;

            poligonoSelecionado.MatrizTranslacaoXYZ(tx, ty, tz);

            Atualizar();
        }

        public void EscalaPoligonoSelecionado(double tx, double ty, double tz)
        {
            var poligonoSelecionado = ObterPoligonoSelecionado();

            if (poligonoSelecionado == null)
                return;

            // 1 opcao
            var cloneCentroBBox = new Ponto4D(poligonoSelecionado.Bbox().ObterCentro);
            poligonoSelecionado.MatrizTranslacaoXYZ(-cloneCentroBBox.X, -cloneCentroBBox.Y, -cloneCentroBBox.Z);
            poligonoSelecionado.MatrizEscalaXYZ(tx, ty, tz);
            poligonoSelecionado.MatrizTranslacaoXYZ(cloneCentroBBox.X, cloneCentroBBox.Y, cloneCentroBBox.Z);

            // 2 opcao
            // poligonoSelecionado.MatrizEscalaXYZBBox(tx, ty, tz);

            Atualizar();
        }

        public void RotacaoPoligonoSelecionado(double angulo)
        {
            var poligonoSelecionado = ObterPoligonoSelecionado();

            if (poligonoSelecionado == null)
                return;

            // 1 opcao
            var cloneCentroBBox = new Ponto4D(poligonoSelecionado.Bbox().ObterCentro);
            poligonoSelecionado.MatrizTranslacaoXYZ(-cloneCentroBBox.X, -cloneCentroBBox.Y, -cloneCentroBBox.Z);
            poligonoSelecionado.MatrizRotacao(angulo);
            poligonoSelecionado.MatrizTranslacaoXYZ(cloneCentroBBox.X, cloneCentroBBox.Y, cloneCentroBBox.Z);

            // 2 opcao
            // poligonoSelecionado.MatrizRotacaoZBBox(angulo);

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
        retorno = "__ Objeto EditorVetorial2D _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
        retorno += base.ImprimeToString();
        return retorno;
    }
#endif

    }
}