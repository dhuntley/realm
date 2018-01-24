using UnityEngine;

public class UnitManager : Singleton<UnitManager> {

    public T[] GetAllUnits<T>() where T: Unit {
        return FindObjectsOfType<T>();
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
