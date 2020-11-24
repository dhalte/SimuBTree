using System;
using System.Collections.Generic;
using System.Text;

namespace SimuBTree
{
  class BTreeNode
  {
    internal List<int> Keys = new List<int>();
    internal List<BTreeNode> Children = new List<BTreeNode>();
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
  }
}
