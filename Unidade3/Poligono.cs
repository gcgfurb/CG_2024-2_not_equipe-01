using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;

namespace gcgcg
{
    internal class Poligono : Objeto
    {
        // Construtor principal que inicializa o polígono com a lista de pontos fornecida
        public Poligono(Objeto paiRef, ref char rotulo, List<Ponto4D> pontosPoligono) : base(paiRef, ref rotulo)
        {
            InicializarPoligono(pontosPoligono);
        }

        // Método para inicializar o polígono com os pontos fornecidos
        private void InicializarPoligono(List<Ponto4D> pontos)
        {
            PrimitivaTipo = PrimitiveType.LineLoop;
            PrimitivaTamanho = 1;
            pontosLista = new List<Ponto4D>(pontos);
            Atualizar();
        }

        // Atualiza o objeto e força uma atualização da estrutura interna
        private void Atualizar()
        {
            ObjetoAtualizar();
        }

#if CG_Debug
        // Método sobrescrito para imprimir informações detalhadas do polígono
        public override string ToString()
        {
            Console.WriteLine("__________________________________ \n");
            return $"__ Objeto Poligono __ Tipo: {PrimitivaTipo} __ Tamanho: {PrimitivaTamanho}\n" +
                   base.ImprimeToString();
        }
#endif

        // Método adicional para adicionar um ponto ao polígono
        public void AdicionarPonto(Ponto4D novoPonto)
        {
            pontosLista.Add(novoPonto);
            Atualizar();
        }

        // Método adicional para remover o último ponto do polígono
        public void RemoverUltimoPonto()
        {
            if (pontosLista.Count > 0)
            {
                pontosLista.RemoveAt(pontosLista.Count - 1);
                Atualizar();
            }
        }

        // Método para definir o tipo de primitiva (aberto ou fechado)
        public void AlternarPrimitiva()
        {
            PrimitivaTipo = (PrimitivaTipo == PrimitiveType.LineLoop) ? PrimitiveType.LineStrip : PrimitiveType.LineLoop;
            Atualizar();
        }

        // Método para redefinir todos os pontos do polígono
        public void RedefinirPontos(List<Ponto4D> novosPontos)
        {
            pontosLista.Clear();
            pontosLista.AddRange(novosPontos);
            Atualizar();
        }
    }
}
