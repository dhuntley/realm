public struct MapTile {
    public bool traversable;

    public bool hardCorners;

    public MapTile(bool isTraversable, bool hasHardCorners) {
        traversable = isTraversable;
        hardCorners = hasHardCorners;
    }
}
