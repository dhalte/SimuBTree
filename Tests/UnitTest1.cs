using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimuBTree;
using System.Diagnostics;

namespace Tests
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void TestSplit()
    {
      int order;
      int idxK, K;
      BTreeNode N, child;
      string sTmp;

      for (order = 7; order < 9; order++)
      {
        // on teste order à 7 (impair) et 8 (pair)
        for (idxK = 0; idxK < order; idxK++)
        {
          // Préparation de l'environnement de test
          // N : noeud plein d'entiers de 0 à order-2
          // N possède order-1 children, chaque child a un seul entier = 100 + son indice de 0 à order-1
          // RQ : on ne respecte pas l'ordre des clés, le processus de split ne faisant pas de vérifications
          N = new BTreeNode(order);
          N.AlloueChildren();
          for (int i = 0; i < order - 1; i++)
          {
            N.AddKey(i);
            child = new BTreeNode(order);
            child.AddKey(i + 100);
            N.AddChild(child);
          }
          child = new BTreeNode(order);
          child.AddKey(order - 1 + 100);
          N.AddChild(child);
          // Valeur à insérer (avec son lien supérieur)
          K = 1000;
          child = new BTreeNode(order);
          child.AddKey(K + 1);
          Debug.Print($"Order={order}, N={DumpKeys(N)}, K={K}, child keys={DumpKeys(child)}, idxK={idxK}");
          sTmp =" clés des children de N";
          for (int i = 0; i < N.NbChildren; i++)
          {
            sTmp += $" [{DumpKeys(N.Child(i))}]";
          }
          Debug.Print(sTmp);
          // Opération de split
          N.Split(idxK, ref K, ref child);
          // K contient maintenant la clé à remonter au parent, child contient le nouveau noeud NN
          Debug.Print("résultat du split");
          Debug.Print($"Kup={K}");
          Debug.Print($"clés de N : {DumpKeys(N)}");
          sTmp = " clés des children de N";
          for (int i = 0; i < N.NbChildren; i++)
          {
            sTmp += $" [{DumpKeys(N.Child(i))}]";
          }
          Debug.Print(sTmp);
          Debug.Print($"clés de NN : {DumpKeys(child)}");
          sTmp=" clés des children de NN";
          for (int i = 0; i < child.NbChildren; i++)
          {
            sTmp += $" [{DumpKeys(child.Child(i))}]";
          }
          Debug.Print(sTmp);
          Debug.Print("-------------------------------------------------------------");
        }
      }
    }

    private string DumpKeys(BTreeNode n)
    {
      string s = $"({n.NbKeys}) :";
      for (int i = 0; i < n.NbKeys; i++)
      {
        s += $" {n.Key(i)}";
      }
      return s;
    }
  }
}
