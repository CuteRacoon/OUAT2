using UnityEngine;
using System.Collections;

public class Roots : Interactable
{
    private Animation anime;
    private AnimationsControl animationsControl;
    private bool needToDelete = false;

    private int objectIndicator = 0;

    private MeshRenderer meshRenderer;
    private Collider boxCollider;

    protected void Awake()
    {
        base.Start();
        base.pickupHeight = 1f;

        anime = GetComponent<Animation>();
        if (anime == null)
        {
            Debug.LogWarning("Нет компонента Animation на объекте Roots");
        }

        animationsControl = FindAnyObjectByType<AnimationsControl>();

        if (tag != "roots") Debug.LogWarning("Тэг объекта не Roots");
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        boxCollider = GetComponent<Collider>();

    }
    protected override IEnumerator HandleObjectRelease()
    {
        if (anime != null && animationsControl.IsNearCorrectBowl(this.gameObject))
        {
            needToDelete = true;
            anime.Play("RootsAnimation");

            yield return new WaitForSeconds(anime["RootsAnimation"].length);

            animationsControl.ObjectsOn(this.index, objectIndicator);
            gameLogic.AddToObjectsList(index, objectIndicator);
            int indexToDisable = (index == 1) ? 2 : 1;

            gameLogic.AccessRoots(false);
            gameLogic.AccessHerbsAndBerriesInteraction(true);
        }
        else
        {
            isReturning = true;
        }
        DropObject();
        if (needToDelete)
        {
            meshRenderer.enabled = false;
            boxCollider.enabled = false;
            base.ReturnToInitialPositionIfHide();
            gameObject.SetActive(false);
            needToDelete = false;
            meshRenderer.enabled = true;
            boxCollider.enabled = true;
        }
    }
}
