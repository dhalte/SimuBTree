using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SimuBTree
{
  // Cette classe n'est pas Thread Safe
  class BTreeClass
  {
    internal readonly int Order, HalfFull;
    internal int NbKeys = 0;
    internal int Deep;
    internal bool IsFull(BTreeNode node) => node.NbKeys >= Order;
    internal bool IsAtLeastHalfFull(BTreeNode node) => node.NbKeys >= HalfFull;
    internal bool IsMoreThanHalfFull(BTreeNode node) => node.NbKeys > HalfFull;

    internal BTreeNode Root;
    internal BTreeClass(int order)
    {
      Order = order;
      HalfFull = (order - 1) / 2;
      Root = new BTreeNode(order);
      Stack = new StackList();
      Stack.Add(new StackItem());
    }

    #region Ajout clé
    // Trace de la recherche de la clé dans le BTree à une profondeur donnée
    private class StackItem
    {
      // Le noeud courant dans lequel on a cherché key
      internal BTreeNode Node;
      // L'indice de la plus grande clé < key dans ce noeud (ou -1 s'il n'y en a pas)
      internal int Idx;

      internal void Set(BTreeNode node, int idxK)
      {
        Node = node;
        Idx = idxK;
      }

      internal void Get(out BTreeNode node, out int idxK)
      {
        node = Node;
        idxK = Idx;
      }
    }
    // Pile des traces de la recherche (débute toujours par Root, et se termine par la Leaf qui accueillera key)
    private class StackList : List<StackItem>
    {
    }
    // Cette pile est allouée de la même taille que la profondeur de l'arbre et n'est pas réallouée à chaque insertion
    private StackList Stack;

    // Recherche de la Leaf L qui doit accueillir key (avec trace des étapes dans Stack).
    // Renvoie false si key déjà présente.
    // child = null
    // Boucle depuis node=L en remontant la Stack
    //   Si node n'est pas Full
    //     insert key et child dans node
    //     return true
    //   transfert moitié de node (keys et children) dans newChild=new Child()
    //   insertion de key et child dans node ou newChild suivant la valeur de key
    //   key devient une des clés du centre de node avant son split, et child devient newChild
    //   si node==Root
    //     Root = new Child()
    //     insertion de key dans Root
    //     insertion de node et child dans Root
    // return true
    public bool Add(int key)
    {
      //if (key == 273704457)
      //{
      //  Debugger.Break();
      //}
      #region BTree vide
      // Cas particulier du BTree vide
      if (Root.NbKeys == 0)
      {
        Root.AddKey(key);
        NbKeys++;
        return true;
      }
      #endregion BTree vide

      #region Recherche de la leaf qui accueillera key
      // Boucle depuis Root en descendant les children pour trouver la Leaf qui accueillera key
      // Trace conservée dans Stack (qui a une taille suffisante pour éviter les push() et pop())
      // Début de la recherche
      BTreeNode node = Root;
      // indice dans Stack
      int idxProfondeur = 0;
      // indice de la plus grande key < key à insérer (ou -1 s'il n'y en a pas)
      int idxK;
      for (; ; )
      {
        if (node.FindValue(key, out idxK))
        {
          // key déjà présente, on n'insère pas de doublon
          return false; // <==================== RETURN
        }
        Stack[idxProfondeur].Set(node, idxK);
        if (node.Leaf)
        {
          // Leaf trouvée, on va procéder à l'insertion et aux splits éventuels en résultant
          break;
        }
        // On descend dans le sous-arbre susceptible de contenir ou d'accueillir key
        node = node.Child(idxK + 1);
        // Gestion de la profondeur
        idxProfondeur++;
      }
      #endregion Recherche de la leaf qui accueillera key

      #region Insertion dans la leaf et gestion du split en remontant dans l'arbre
      BTreeNode child = null;
      for (; idxProfondeur >= 0; idxProfondeur--)
      {
        Stack[idxProfondeur].Get(out node, out idxK);
        if (!node.IsFull)
        {
          node.Insert(idxK + 1, key, child);
          break;
        }
        // node est saturé, on va le splitter 
        node.Split(idxK + 1, ref key, ref child);
        if (node == Root)
        {
          Root = new BTreeNode(Order);
          Root.Init(key, node, child);
          Stack.Add(new StackItem());
          Deep++;
        }
      }
      #endregion Insertion dans la leaf et gestion du split en remontant dans l'arbre

      NbKeys++;
      return true;
    }
    #endregion Ajout clé

    #region Suppression clé
    public bool Remove(int value)
    {
      //if (value == 2222)
      //{
      //  Debugger.Break();
      //}
      int idxProfondeur = 0;
      // Recherche de la valeur
      // Recevra le noeud qui contient la clé
      BTreeNode node = Root;
      // index de la clé cherchée, ou du sous-arbre à suivre pour la trouver
      int idx;
      for (; ; )
      {
        bool exists = node.FindValue(value, out idx);
        if (exists)
        {
          // On a trouvé la clé dans node
          break;
        }
        if (node.Leaf)
        {
          // On est arrivé en bas de l'arbre sans avoir trouvé la clé
          return false;
        }
        Stack[idxProfondeur++].Set(node, idx);
        // idx est l'index de la + grande clé < value (ou -1)
        // idx+1 est l'index du sous-arbre pouvant contenir value
        node = node.Child(idx + 1);
      }

      if (!node.Leaf)
      {
        // On n'est pas dans une feuille. 
        // On va chercher la feuille contenant les plus grandes clés < value
        // node.Keys[idx] est la value à supprimer, node.Children[idx] est le sous-arbre qui contient la + grande clé < value
        BTreeNode nodeMax = node.Child(idx);
        // on ajoute à la liste des ancètres une indication semblable à celles 
        // ajoutées dans la boucle précédente, lorsqu'on ne trouvait pas value
        Stack[idxProfondeur++].Set(node, idx - 1);
        int idxMax;
        while (!nodeMax.Leaf)
        {
          // on simule le résultat de nodeMax.FindValue, pour cette value > à toute clé
          idxMax = nodeMax.NbKeys - 1;
          Stack[idxProfondeur++].Set(nodeMax, idxMax);
          nodeMax = nodeMax.Child(idxMax + 1);
        }
        // On remplace value par la plus grande de ces clés
        idxMax = nodeMax.NbKeys - 1;
        int valueMax = nodeMax.Key(idxMax);
        node.SetKey(idx, valueMax);
        // Et maintenant, on va faire comme si on avait trouvé value dans nodeMax
        node = nodeMax;
        idx = idxMax;
      }
      // Maintenant node est une leaf, et la clé qu'on va supprimer est
      //  soit la clé recherchée parce qu'elle était dans une leaf
      //  soit la clé de la leaf dont la valeur a remplacé la clé recherchée, trouvée plus haut.
      node.RemoveKey(idx);
      NbKeys--;

      if (!IsAtLeastHalfFull(node) && node != Root)
      {
        // La feuille est dépeuplée, débute le processus de balance ou de fusion
        BalanceOuFusionne(node, idxProfondeur - 1);
      }
      return true;
    }

    // Le node n'est pas la racine (il a un parent) et a un déficit de clés
    // On cherche un sibling qui pourrait en fournir un (balance), 
    // ou sinon avec lequel il pourrait fusionner.
    // quatre possibilités (sachant que node a au moins un voisin, à gauche ou à droite) : 
    // le voisin de gauche de node existe et a suffisamment de clés : 
    //   il fournit sa + grande clé pour compenser node
    // sinon le voisin de droite de node existe et a suffisamment de clés : 
    //   il fournit sa + petite clé pour compenser node
    // sinon le voisin de gauche de node existe, il fusionne avec node
    // sinon le voisin de droite de node existe, il fusionne avec node
    private void BalanceOuFusionne(BTreeNode node, int idxProfondeur)
    {
      // Il nous faut le parent de ce node
      // idx est le résultat de parent.FindValue lors de la recherche de la valeur à supprimer.
      // idx = -1 ou désigne la case de la clé dans parent juste inférieure à la value à supprimer.
      // idx désigne aussi l'arbre des clés < value
      // idx+1 est l'arbre suivi lors de la recherche de value, c'est node
      Stack[idxProfondeur].Get(out BTreeNode parent, out int idx);
      BTreeNode sibling;
      if (idx >= 0)
      {
        // Le voisin de gauche existe
        sibling = parent.Child(idx);
        if (IsMoreThanHalfFull(sibling))
        {
          // sibling a suffisamment de clés, il fournit la plus grande qui remonte dans parent,
          // à la place d'une clé de parent qui descend dans node
          // Balance
          // Tip : les .NbChildren sont mis à jour lors des .Addkey, .InsertKey, .RemoveKey, 
          // donc on met à jour les liens avant les clefs
          if (!node.Leaf)
          {
            node.InsertChild(0, sibling.Child(sibling.NbChildren - 1));
            sibling.RemoveChild(sibling.NbChildren - 1);
          }
          node.InsertKey(0, parent.Key(idx));
          parent.SetKey(idx, sibling.Key(sibling.NbKeys - 1));
          sibling.RemoveKey(sibling.NbKeys - 1);
          return; // =====================================================> RETURN
        }
      }
      // Le voisin de gauche n'existe pas, ou il est dépeuplé
      if (idx < parent.NbKeys - 1)
      {
        // Le voisin de droite existe
        sibling = parent.Child(idx + 2);
        if (IsMoreThanHalfFull(sibling))
        {
          // sibling a suffisamment de clés, il fournit la plus petite qui remonte dans parent,
          // à la place d'une clé de parent qui descend dans node
          // Balance
          if (!node.Leaf)
          {
            node.AddChild(sibling.Child(0));
            sibling.RemoveChild(0);
          }
          node.AddKey(parent.Key(idx + 1));
          parent.SetKey(idx + 1, sibling.Key(0));
          sibling.RemoveKey(0);
          return; // =====================================================> RETURN
        }
      }
      // aucun des voisins ne convient pour effectuer une balance, on réalise donc une fusion
      BTreeNode gauche, droite;
      if (idx >= 0)
      {
        // Le voisin de gauche existe, on va le fusionner avec node qui sera le voisin de droite, et la clé de parent qui les sépare
        gauche = parent.Child(idx);
        droite = node;
      }
      else
      {
        gauche = node;
        idx += 1;
        droite = parent.Child(idx + 1);
      }
      // Tip : les .NbChildren sont mis à jour lors des .Addkey, .InsertKey, .RemoveKey, 
      //   donc on met à jour les liens avant les clefs
      // On retire le pointeur sur "droite"
      parent.RemoveChild(idx + 1);
      // on ajoute à "gauche" les pointeurs qui étaient auparavant dans "droite"
      gauche.AddChildren(droite);
      // On ajoute à "gauche" la clé qui descend du père
      gauche.AddKey(parent.Key(idx));
      // on enlève cette clé dans le père
      parent.RemoveKey(idx);
      // on ajoute à "gauche" les clés de "droite"
      gauche.AddKeys(droite);
      if (!IsAtLeastHalfFull(parent) && parent != Root)
      {
        BalanceOuFusionne(parent, idxProfondeur - 1);
      }
      // traiter cas particulier du parent == Root && parent.Keys.Count == 0 (après fusion)
      if (parent == Root && Root.NbKeys == 0)
      {
        Root = gauche;
        Stack.RemoveAt(Stack.Count - 1);
        Deep--;
      }

    }
    #endregion Suppression clé

    #region Vérification arbre
    // vérification que les valeurs sont bien triées
    // vérification que tous les chemins sont de même longueur
    // décompte du nombre de keys max, du nombre de keys actuel
    // On parcourt le btree en profondeur
    private class ScanDataClass
    {
      public bool previousValueSet = false;
      public int previousValue = int.MinValue;
      public bool bLgCheminSet = false;
      public int lgChemin = int.MinValue;
      public int nbKeysMax = 0;
      public int nbKeysActual = 0;
    }
    internal void Scan()
    {
      if (Root.NbKeys == 0)
      {
        return;
      }
      ScanDataClass scanData = new ScanDataClass();
      ScanBTree(Root, 0, scanData);
      Helper.Trace($"lgChemin={scanData.lgChemin}, nbkeysmax={scanData.nbKeysMax}, nbkeysactual={scanData.nbKeysActual}, remplissage={100.0 * scanData.nbKeysActual / scanData.nbKeysMax} %");
      Helper.Assert(scanData.nbKeysActual == NbKeys);
    }
    private void ScanBTree(BTreeNode node, int profondeur, ScanDataClass scanData)
    {
      scanData.nbKeysMax += Order - 1;
      scanData.nbKeysActual += node.NbKeys;
      bool bchildren = !node.Leaf;
      Helper.Assert(node == Root || IsAtLeastHalfFull(node));
      if (bchildren)
      {
        // On descend en bas à gauche
        Helper.Assert(node.NbKeys == node.NbChildren - 1);
        ScanBTree(node.Child(0), profondeur + 1, scanData);
      }
      else if (scanData.bLgCheminSet)
      {
        Helper.Assert(profondeur == scanData.lgChemin);
      }
      else
      {
        scanData.lgChemin = profondeur;
        scanData.bLgCheminSet = true;
      }
      for (int i = 0; i < node.NbKeys; i++)
      {
        int value = node.Key(i);
        if (!scanData.previousValueSet)
        {
          Helper.Assert(i == 0 && !bchildren);
          scanData.previousValue = node.Key(0);
          scanData.previousValueSet = true;
        }
        else
        {
          // previousValue est censée être la plus grande key dans le sous-arbre à gauche 
          Helper.Assert(scanData.previousValue < value);
          scanData.previousValue = value;
        }
        if (bchildren)
        {
          ScanBTree(node.Child(i + 1), profondeur + 1, scanData);
        }
      }
    }
    #endregion Vérification arbre

  }
}
