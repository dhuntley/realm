using System.Collections;

using UnityEngine;

public class Worker : Unit {

    public StructureModel fortModel;

    public StructureModel outpostModel;

    public float treeCutTime = 2.0f;

    private Vector2Int lastTreeCut;

    private bool carryingLumber = false;

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

    //TODO: Should we handle this with a Coroutine, or should it be handled with a state and Update?
    private IEnumerator CutDownTree(Vector2Int targetCell) {
        float startTime = Time.time;
        SetProgress(0.0f);
        SetProgressTotal(treeCutTime);
        while (Time.time < startTime + treeCutTime) {
            SetProgress(Time.time - startTime);
            SetProgressBarEnabled(true);
            yield return null;
        }
        SetProgressBarEnabled(false);
        MapController.Instance.RemoveTree(targetCell);
        carryingLumber = true;
        lastTreeCut = targetCell;

        ReturnLumber();
    }

    private void ReturnLumber() {
        // Return to nearest structure
        Structure closestStructure = MapController.Instance.mapModel.GetClosestStructure(this.cell);
        if (closestStructure) {
            HandleInteractionRequest(closestStructure);
        }
    }

    public override void HandleInteractionRequest(Vector2Int targetCell) {
        // 1. Identify which kind of object we will be interacting with
        // 2. Move to one of the interaction nodes of the target object
        // 3. Start the interaction

        if (MapController.Instance.mapModel.ContainsTree(targetCell)) {
            Vector2Int interactionNode = MapController.Instance.mapModel.GetInteractionNode(targetCell, this.cell);
            HandleMoveRequest(interactionNode, () => {
                if (carryingLumber) {
                    ReturnLumber();
                } else {
                    StartCoroutine(CutDownTree(targetCell));
                }
            });
        } else if (MapController.Instance.mapModel.ContainsStructure(targetCell)) {
            Structure structure = MapController.Instance.mapModel.GetStructure(targetCell);
            HandleInteractionRequest(structure);
        }
    }

    public override void HandleInteractionRequest(Structure structure) {
        Vector2Int interactionNode = MapController.Instance.mapModel.GetInteractionNode(structure, this.cell);
        HandleMoveRequest(interactionNode, () => {
            if (carryingLumber) {
                carryingLumber = false;
                // Cut down next tree
                HandleInteractionRequest(MapController.Instance.mapModel.GetNextTreePosition(lastTreeCut, this.cell));
            }
        });
    }
}
