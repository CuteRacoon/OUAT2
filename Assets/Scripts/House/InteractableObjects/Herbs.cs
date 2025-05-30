using UnityEngine;
using System.Collections;

public class Herbs : Interactable
{
    private Animation anime;
    private AnimationsManager animationsControl;

    private MeshRenderer meshRenderer;
    private Collider boxCollider;

    private int objectIndicator = 2;
    protected override void Start()
    {
        base.Start();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        boxCollider = GetComponent<Collider>();

        anime = GetComponent<Animation>();
        if (anime == null)
        {
            Debug.LogWarning("Нет компонента Animation на объекте Herbs");
        }

        this.animationsControl = FindAnyObjectByType<AnimationsManager>();

        if (tag != "herbs") Debug.LogWarning("Тэг объекта не Herbs");

    }
    protected override void PickupObject()
    {
        base.PickupObject();
        if (animationsControl.isFull)
        {
            animationsControl.CleanDust();
        }
    }
    protected override IEnumerator HandleObjectRelease()
    {
        if (anime != null && animationsControl.IsNearCorrectBowl(this.gameObject))
        {
            anime.Play("HerbsAnimation");

            yield return new WaitForSeconds(anime["HerbsAnimation"].length);

            animationsControl.ObjectsOn(this.index, objectIndicator);
            gameLogic.AddToObjectsList(index, objectIndicator);

            //gameLogic.AccessHerbsAndBerriesInteraction(false);

            DropObject();

            meshRenderer.enabled = false;
            boxCollider.enabled = false;
            base.ReturnToInitialPosition();


            yield return StartCoroutine(animationsControl.PlayMortarAnimation(this.index, this.objectIndicator));

            gameObject.SetActive(false);
            meshRenderer.enabled = true;
            boxCollider.enabled = true;
        }
        else
        {
            isReturning = true;
            DropObject();
        }
    }
}