using System.Collections.Generic;
using UnityEngine;

public class Vector2IntComparer : IEqualityComparer<Vector2Int> {
    public bool Equals(Vector2Int a, Vector2Int b) {
        return a.x == b.x && a.y == b.y;
    }

    public int GetHashCode(Vector2Int obj) {
        // http://www.lsi.upc.edu/~alvarez/calculabilitat/enumerabilitat.pdf
        int temp = (obj.y + (obj.x + 1) / 2);
        return obj.x + temp * temp;
    }
}
