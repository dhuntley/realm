using UnityEngine;
using System.Collections.Generic;

public class UnitManager : Singleton<UnitManager> {
    public T[] GetAllUnits<T>() where T: Unit {
        return FindObjectsOfType<T>();
    }

    public Unit[] GetAllUnits() {
        return FindObjectsOfType<Unit>();
    }

    public T[] GetAllSelectedUnits<T>() where T : Unit {
        T[] units = FindObjectsOfType<T>();

        LinkedList<T> selectedUnits = new LinkedList<T>();
        foreach (T unit in units) {
            Selectable selectable = unit.GetComponent<Selectable>();
            if (selectable && selectable.selected) {
                selectedUnits.AddLast(unit);
            }
        }
        T[] selectedUnitArray = new T[selectedUnits.Count];
        selectedUnits.CopyTo(selectedUnitArray, 0);
        return selectedUnitArray;
    }

    public Unit[] GetAllSelectedUnits() {
        Unit[] units = FindObjectsOfType<Unit>();

        LinkedList<Unit> selectedUnits = new LinkedList<Unit>();
        foreach (Unit unit in units) {
            Selectable selectable = unit.GetComponent<Selectable>();
            if (selectable && selectable.selected) {
                selectedUnits.AddLast(unit);
            }
        }
        Unit[] selectedUnitArray = new Unit[selectedUnits.Count];
        selectedUnits.CopyTo(selectedUnitArray, 0);
        return selectedUnitArray;
    }

    public bool TreeIsBeingCut(Vector2Int treeCell) {
        Worker[] workers = GetAllUnits<Worker>();
        foreach (Worker worker in workers) {
            if (worker.isCuttingTree && worker.currentCuttingTree.Equals(treeCell)) {
                return true;
            }
        }
        return false;
    }
}
