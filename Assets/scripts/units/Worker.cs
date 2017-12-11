using UnityEngine;

public class Worker : Unit {

    public StructureModel fortModel;

    public StructureModel outpostModel;

    [GUIPanel("Build Fort")]
    public void BuildFort() {
        StructurePlacer structurePlacer = StructurePlacer.Instance;
        structurePlacer.structureModel = fortModel;
        structurePlacer.enabled = true;
    }

    [GUIPanel("Build Outpost")]
    public void BuildOutpost() {
        StructurePlacer structurePlacer = StructurePlacer.Instance;
        structurePlacer.structureModel = outpostModel;
        structurePlacer.enabled = true;
    }

}
