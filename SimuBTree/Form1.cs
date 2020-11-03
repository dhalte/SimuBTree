using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimuBTree
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void btAJouter_Click(object sender, EventArgs e)
    {
      bool bOK = BTree.Add(BTree.NbKeys);
      Helper.Assert(bOK);
      ShowTreeView();
      ScanBTree();
    }

    private void ShowTreeView()
    {
      BTreeView.SuspendLayout();
      BTreeNode root = BTree.Root;
      bool children = root.Children.Count > 0;
      BTreeView.Nodes.Clear();
      // TODO : 
      if (children)
      {
        TreeNode node = new TreeNode("-");
        BTreeView.Nodes.Add(node);
        ShowTreeView(node, root.Children[0]);
      }
      for (int i = 0; i < root.Keys.Count; i++)
      {
        TreeNode node = new TreeNode(root.Keys[i].ToString());
        BTreeView.Nodes.Add(node);
        if (children)
        {
          ShowTreeView(node, root.Children[i + 1]);
        }
      }
      BTreeView.ExpandAll();
      BTreeView.ResumeLayout();
    }

    private void ShowTreeView(TreeNode parent, BTreeNode root)
    {
      bool children = root.Children.Count > 0;
      if (children)
      {
        TreeNode node = new TreeNode("-");
        parent.Nodes.Add(node);
        ShowTreeView(node, root.Children[0]);
      }
      for (int i = 0; i < root.Keys.Count; i++)
      {
        TreeNode node = new TreeNode(root.Keys[i].ToString());
        parent.Nodes.Add(node);
        if (children)
        {
          ShowTreeView(node, root.Children[i + 1]);
        }
      }
    }

    class BTreeNode
    {
      internal List<int> Keys = new List<int>();
      internal List<BTreeNode> Children = new List<BTreeNode>();
      // Keys est trié
      // Si Keys vide ou value < Keys[0] return -1
      // sinon si Keys[Keys.count-1] < value, return Keys.count-1
      // sinon return idx tel que Keys[idx] soit la plus grande clé <= value
      internal (int idx, bool exists) FindValue(int value)
      {
        for (int idxK = 0; idxK < Keys.Count; idxK++)
        {
          if (Keys[idxK] == value)
          {
            return (idxK, true);
          }
          if (Keys[idxK] > value)
          {
            return (idxK - 1, false);
          }
        }
        return (Keys.Count - 1, false);
      }

      internal void InsertValue(int idxK, int value, BTreeNode newNode)
      {
        Keys.Insert(idxK, value);
        if (Children.Count > 0)
        {
          Helper.Assert(newNode != null);
          Children.Insert(idxK + 1, newNode);
        }
        else
        {
          Helper.Assert(newNode == null);
        }
      }
    }
    class BTreeClass
    {
      internal readonly int Order;
      internal int NbKeys = 0;
      internal int Deep;
      // On s'arrange pour que lors du split de la racine, root reste la racine.
      // Ce n'est pas obligatoire, mais sera utile lorsque on gèrera un BTree persistant.
      internal readonly BTreeNode Root = new BTreeNode();
      internal BTreeClass(int order)
      {
        Order = order;
      }

      // recherche value dans Root
      // si trouvé, return false
      // si Root n'a pas d'enfant, 
      //   insertion dans Root de value
      // sinon 
      //   recherche dans les enfants
      //   si value trouvée dans enfants, return false
      //   si enfant a été scindé, insertion dans Root de keyUp et newNode retourné par la recherche
      // si Root sursaturé, split root
      // return true
      public bool Add(int value)
      {
        (int idxK, bool exists) item = Root.FindValue(value);
        if (item.exists)
        {
          return false;
        }
        int idxK = item.idxK;
        if (Root.Children.Count != 0)
        {
          (bool success, bool split, int keyUP, BTreeNode newNode) result = Add(idxK + 1, value, Root);
          if (!result.success)
          {
            return false;
          }
          if (result.split)
          {
            Root.InsertValue(idxK + 1, result.keyUP, result.newNode);
          }
        }
        else
        {
          Root.InsertValue(idxK + 1, value, null);
        }
        if (Root.Keys.Count >= Order)
        {
          Helper.Assert(Root.Keys.Count == Order);
          SplitRoot();
        }
        NbKeys++;
        return true;
      }

      // recherche de value dans node = parent.Children[idxChild]
      // si trouvé, return false
      // si node a des children, return recherche de value dans le child de node concerné
      // insertion de value dans node
      // si node sursaturé, return resultat du split de node
      // return true
      private (bool success, bool split, int keyUP, BTreeNode newNode) Add(int idxChild, int value, BTreeNode parent)
      {
        BTreeNode node = parent.Children[idxChild];
        (int idxK, bool exists) item = node.FindValue(value);
        int idxK = item.idxK;
        if (item.exists)
        {
          return (false, false, int.MinValue, null);
        }
        if (node.Children.Count != 0)
        {
          (bool success, bool split, int keyUP, BTreeNode newNode) result = Add(idxK + 1, value, node);
          if (!result.success)
          {
            return (false, false, int.MinValue, null);
          }
          if (result.split)
          {
            node.InsertValue(idxK + 1, result.keyUP, result.newNode);
          }
        }
        else
        {
          node.InsertValue(idxK + 1, value, null);
        }

        if (node.Keys.Count >= Order)
        {
          Helper.Assert(node.Keys.Count == Order);
          return SplitNode(node);
        }

        return (true, false, int.MinValue, null);
      }

      private void SplitRoot()
      {
        Deep++;
        bool bChildren = Root.Children.Count > 0;
        int idxPivot = Order / 2;
        int pivot = Root.Keys[idxPivot];
        BTreeNode gauche = new BTreeNode();
        BTreeNode droite = new BTreeNode();
        for (int i = 0; i < idxPivot; i++)
        {
          gauche.Keys.Add(Root.Keys[i]);
        }
        for (int i = idxPivot + 1; i < Order; i++)
        {
          droite.Keys.Add(Root.Keys[i]);
        }
        if (bChildren)
        {
          for (int i = 0; i <= idxPivot; i++)
          {
            gauche.Children.Add(Root.Children[i]);
          }
          for (int i = idxPivot + 1; i <= Order; i++)
          {
            droite.Children.Add(Root.Children[i]);
          }
        }
        Root.Keys.Clear();
        Root.Keys.Add(pivot);
        Root.Children.Clear();
        Root.Children.Add(gauche);
        Root.Children.Add(droite);
      }

      private (bool success, bool split, int keyUP, BTreeNode newNode) SplitNode(BTreeNode node)
      {
        bool bChildren = node.Children.Count > 0;
        int idxPivot = Order / 2;
        int pivot = node.Keys[idxPivot];
        BTreeNode newNode = new BTreeNode();
        newNode.Keys.AddRange(node.Keys.GetRange(idxPivot + 1, Order - idxPivot - 1));
        node.Keys.RemoveRange(idxPivot, Order - idxPivot);
        if (bChildren)
        {
          newNode.Children.AddRange(node.Children.GetRange(idxPivot + 1, Order - idxPivot));
          node.Children.RemoveRange(idxPivot + 1, Order - idxPivot);
        }
        return (true, true, pivot, newNode);
      }
    }

    // vérification que les valeurs sont bien triées
    // vérification que tous les chemins sont de même longueur
    // décompte du nombre de keys max, du nombre de keys actuel
    // On parcourt le btree en profondeur
    bool previousValueSet = false;
    int previousValue = int.MinValue;
    bool bLgCheminSet = false;
    int lgChemin = int.MinValue;
    int nbKeysMax = 0;
    int nbKeysActual = 0;
    private void ScanBTree()
    {
      if (BTree.Root.Keys.Count == 0)
      {
        return;
      }
      previousValueSet = false;
      previousValue = int.MinValue;
      bLgCheminSet = false;
      lgChemin = int.MinValue;
      nbKeysMax = 0;
      nbKeysActual = 0;
      ScanBTree(BTree.Root, 0);
      Debug.Print($"lgChemin={lgChemin}, nbkeysmax={nbKeysMax}, nbkeysactual={nbKeysActual}, remplissage={100.0 * nbKeysActual / nbKeysMax} %");
      Helper.Assert(nbKeysActual == BTree.NbKeys);
    }

    private void ScanBTree(BTreeNode node, int profondeur)
    {
      nbKeysMax += BTree.Order - 1;
      nbKeysActual += node.Keys.Count;
      bool bchildren = node.Children.Count > 0;
      Helper.Assert(node == BTree.Root || node.Keys.Count >= (BTree.Order - 1) / 2);
      if (bchildren)
      {
        // On descend en bas à gauche
        Helper.Assert(node.Keys.Count == node.Children.Count - 1);
        ScanBTree(node.Children[0], profondeur + 1);
      }
      else if (bLgCheminSet)
      {
        Helper.Assert(profondeur == lgChemin);
      }
      else
      {
        lgChemin = profondeur;
        bLgCheminSet = true;
      }
      for (int i = 0; i < node.Keys.Count; i++)
      {
        int value = node.Keys[i];
        if (!previousValueSet)
        {
          Helper.Assert(i == 0 && !bchildren);
          previousValue = node.Keys[0];
          previousValueSet = true;
        }
        else
        {
          // previousValue est censée être la plus grande key dans le sous-arbre à gauche 
          Helper.Assert(previousValue < value);
          previousValue = value;
        }
        if (bchildren)
        {
          ScanBTree(node.Children[i + 1], profondeur + 1);
        }
      }

    }
    const int Ordre = 4;
    BTreeClass BTree = new BTreeClass(Ordre);

    private void btMassAdd_Click(object sender, EventArgs e)
    {
      BTree = new BTreeClass(Ordre);
      int max = 100_000;
      int nb = 6_000;
//      int seed = 0;
      Random random = new Random();
      Stopwatch stopwatch = Stopwatch.StartNew();      
      for (int i = 0; i < nb; i++)
      {
        BTree.Add( random.Next(max));
      }
      stopwatch.Stop();
      Debug.Print($"test insertion de {nb} valeurs en {stopwatch.Elapsed} ms");
      stopwatch.Restart();
      ScanBTree();
      Debug.Print($"vérification en {stopwatch.Elapsed} ms");
    }
  }
  static class Helper
  {
    internal static void Assert(bool b)
    {
      if (!b)
      {
        Debugger.Break();
      }
    }
  }
}
