using System;
using System.Collections.Generic;
using System.Text;

namespace SimuBTree
{
  class BTreeNode
  {
    private List<int> Keys = new List<int>();
    private List<BTreeNode> Children = new List<BTreeNode>();
    // Keys est trié, ne contient pas de doublons
    // si value trouvée en idx, alors renvoie (idx, true)
    // Si Keys vide ou value < Keys[0] return (-1, false)
    // sinon si Keys[Keys.count-1] < value, return (Keys.count-1, false)
    // sinon soit idx tel que Keys[idx] soit la plus grande clé < value, renvoie (idx, false)
    // Attention : si value non trouvée, le child à rechercher est à l'indice idx+1
    internal (int idx, bool exists) FindValue(int value)
    {
      //Recherche dichotomique
      int borneMin = 0, borneMax = Keys.Count - 1;
      // Pour le seul cas de la racine vide !
      if (borneMax < 0)
      {
        return (borneMax, false);
      }
      for (; ; )
      {
        int milieu = (borneMin + borneMax) / 2;
        int cmp = value.CompareTo(Keys[milieu]);
        if (cmp < 0)
        {
          if (borneMin == milieu)
          {
            return (borneMin - 1, false);
          }
          borneMax = milieu - 1;
        }
        else if (0 < cmp)
        {
          if (milieu == borneMax)
          {
            return (borneMax, false);
          }
          borneMin = milieu + 1;
        }
        else
        {
          return (milieu, true);
        }
      }
    }

    // Insertion d'une value à l'endroit indiqué, 
    // et de l'éventuel sous-arbre de clés > value issu de la scission en cas de débordement précédent
    // On doit avoir 0 <= idxK <= Keys.Count
    // On doit avoir idxK==0 || Keys[idxK-1] < value
    // On doit avoir idxK==Keys.Count || value < Keys[idxK]
    internal void InsertValue(int idxK, int value, BTreeNode newNode)
    {
      Helper.Assert(0 <= idxK && idxK <= Keys.Count);
      Helper.Assert(idxK == 0 || Keys[idxK - 1] < value);
      Helper.Assert(idxK == Keys.Count || value < Keys[idxK]);
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

    internal bool Empty { get => Keys.Count == 0; }
    internal bool Leaf { get => Children.Count == 0; }
    internal int NbKeys { get => Keys.Count; }
    internal int NbChildren { get => Children.Count; }

    internal int Key(int idxKey) => Keys[idxKey];
    internal void SetKey(int idxKey, int value) => Keys[idxKey] = value;
    internal BTreeNode Child(int idxChild) => Children[idxChild];
    internal void AddKey(int key) => Keys.Add(key);
    internal void InsertKey(int idxKey, int key) => Keys.Insert(idxKey, key);
    internal void AddChild(BTreeNode child) => Children.Add(child);
    internal void InitNewRoot(int pivot, BTreeNode gauche, BTreeNode droite)
    {
      Keys.Clear();
      Keys.Add(pivot);
      Children.Clear();
      Children.Add(gauche);
      Children.Add(droite);
    }

    internal void AddKeys(BTreeNode node, int idxPivot, int order)
    {
      Keys.AddRange(node.Keys.GetRange(idxPivot + 1, order - idxPivot - 1));
    }
    internal void RemoveKey(int idxKey) => Keys.RemoveAt(idxKey);
    internal void RemoveKeys(int idxPivot, int order)
    {
      Keys.RemoveRange(idxPivot, order - idxPivot);
    }

    internal void AddChildren(BTreeNode node, int idxPivot, int order)
    {
      Children.AddRange(node.Children.GetRange(idxPivot + 1, order - idxPivot));
    }
    internal void InsertChild(int idxInsert, BTreeNode btreeNode) => Children.Insert(idxInsert, btreeNode);

    internal void RemoveChildren(int idxPivot, int order)
    {
      Children.RemoveRange(idxPivot + 1, order - idxPivot);
    }

    internal BTreeNode Child(object p)
    {
      throw new NotImplementedException();
    }

    internal void RemoveChild(int idxChild)
    {
      Children.RemoveAt(idxChild);
    }

    internal void AddKeys(BTreeNode droite) => Keys.AddRange(droite.Keys);

    internal void AddChildren(BTreeNode droite)
    {
      Children.AddRange(droite.Children);
    }

    internal void InitNewRoot(BTreeNode gauche)
    {
      Keys = gauche.Keys;
      Children = gauche.Children;
    }
  }
}
