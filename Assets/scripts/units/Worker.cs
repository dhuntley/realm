using System.Collections;

using UnityEngine;

public class Worker : Unit {

    public StructureModel fortModel;

    public StructureModel outpostModel;

    public float treeCutTime = 2.0f;

    private Vector2Int lastTreeCut;

    private bool carryingLumber = false;

    public bool isCuttingTree = false;

    private Vector2Int _currentTree;

    public Vector2Int currentCuttingTree {
        get {
            return isCuttingTree ? _currentTree : new Vector2Int(int.MinValue, int.MinValue);
        }
    }

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
        isCuttingTree = true;
        _currentTree = targetCell;
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
        isCuttingTree = false;
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
                    lastTreeCut = interactionNode;
                    ReturnLumber();
                } else if (!MapController.Instance.mapModel.ContainsTree(targetCell) || UnitManager.Instance.TreeIsBeingCut(targetCell)) {
                    // The tree has already been cut down, or someone else is cutting down the tree
                    HandleInteractionRequest(MapController.Instance.mapModel.GetNextTreePosition(targetCell, this.cell));
                } else {
                    // The tree is free. Let's chop it down!
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
        }, () => {
            // Select a new interaction node. Our desired one is blocked.
            return MapController.Instance.mapModel.GetInteractionNode(structure, this.cell);
        });
    }

    protected override void StopAllRequests() {
        base.StopAllRequests();
        isCuttingTree = false;
    }
}
