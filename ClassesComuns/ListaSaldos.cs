using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassesComuns
{
    /// <summary>
    /// Esta classe e apenas instancia no servidor de contabilidade
    /// Para alem de guardar a lista de saldos vai guardar tambem todas as operacoes relativas a contabilidade
    /// Operacoes de contabilidade a implementar:
    /// VerSaldo
    /// DepositarDinheiro
    /// LevantarDinheiro
    /// AcrecentarSaldo
    /// RemoverSaldo
    /// </summary>
    public class ListaSaldos
    {
        ///// <summary>
        ///// Lista de saldos de todos os utilizadores.
        ///// </summary>

        ////public static List<float> ListaSaldos = new List<float> {
        ////    0.00f,
        ////    0.00f
        ////};

        //public static List<float> ListaSaldo;

        ///// <summary>
        ///// Para obter o saldo de um determinado utlizador
        ///// </summary>
        ///// <param name="num">numero de utilizador</param>
        ///// <returns>retorna o saldo do utilizador</returns>
        //public float VerSaldo(int num)
        //{
        //    return ListaSaldos[num];
        //}

        ///// <summary>
        ///// Depositar dinheiro para um determinado utilizador
        ///// </summary>
        ///// <param name="num">numero de utilizador</param>
        ///// <param name="saldo">valor a acrescentar ao saldo</param>
        //public void DepositarSaldo(int num, float saldo)
        //{
        //    ListaSaldos[num]+=saldo;
        //}
        
        ///// <summary>
        ///// Levantar dinheiro de um determinado utilizador
        ///// </summary>
        ///// <param name="num">numero de utilizador</param>
        ///// <param name="valor">valor a levantar do saldo</param>
        ///// <returns></returns>
        //public bool LevantarDinheiro(int num,float valor)
        //{
        //    if ((ListaSaldos[num] - valor) >= 0.00f)
        //    {
        //        ListaSaldos[num] -= valor;
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// Acrescentar entrada na lista quando um novo utilizador é criado no servidor de leiloes
        ///// </summary>
        ///// <param name="saldo">saldo inicial</param>
        //public void AcrescentarSaldo(float saldo)
        //{
        //    ListaSaldos.Add(saldo);
        //}

        ///// <summary>
        ///// Acrescentar entrada na lisa quando um novo utilizador é criado no servidor de leiloes
        ///// </summary>
        //public void AcrecentarSaldo()
        //{
        //    ListaSaldos.Add(0.00f);
        //}

        ///// <summary>
        ///// Remove uma dada entrada quando um novo utilizador é removido do servidor de leiloes
        ///// </summary>
        ///// <param name="num"></param>
        //public void RemoverSaldo(int num)
        //{
        //    ListaSaldos.RemoveAt(num);
        //}
    }
}
