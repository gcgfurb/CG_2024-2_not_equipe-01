#define CG_Debug

using System;
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;

namespace gcgcg
{
  internal class SrPalito : Objeto
  {
    private Ponto4D _inicio;
    private Ponto4D _fim;
    private double _angulo;
    private double _raio;
    private double _moverX;

    public SrPalito(Objeto paiRef, ref char rotulo) : base(paiRef, ref rotulo)
    {
      PrimitivaTipo = PrimitiveType.Lines;
      PrimitivaTamanho = 1;
      InicializarValores();
      AdicionarPontos();
    }

    private void InicializarValores()
    {
      _inicio = new Ponto4D(0, 0);
      _fim = new Ponto4D(0.35, 0.35);
      _moverX = 0;
      _angulo = 45;
      _raio = Matematica.Distancia(_inicio, _fim);
    }

    private void AdicionarPontos()
    {
      base.PontosAdicionar(_inicio);
      base.PontosAdicionar(_fim);
      base.ObjetoAtualizar();
    }

    private void Atualizar()
    {
      _fim = Matematica.GerarPtosCirculo(_angulo, _raio);
      _inicio.X = _moverX;
      _fim.X += _moverX;
      base.PontosAlterar(_inicio, 0);
      base.PontosAlterar(_fim, 1);
      base.ObjetoAtualizar();
    }

    public void AtualizarPe(double peInc)
    {
      _moverX += peInc;
      Atualizar();
    }

    public void AtualizarRaio(double raioInc)
    {
      _raio += raioInc;
      Atualizar();
    }

    public void AtualizarAngulo(double anguloInc)
    {
      _angulo += anguloInc;
      Atualizar();
    }

#if CG_Debug
    public override string ToString()
    {
      return $"__ Objeto SrPalito _ Tipo: {PrimitivaTipo} _ Tamanho: {PrimitivaTamanho}\n" + ImprimeToString();
    }
#endif
  }
}
