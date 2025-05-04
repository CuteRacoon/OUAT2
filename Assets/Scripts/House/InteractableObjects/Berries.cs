using UnityEngine;
using System.Collections;

public class Berries : Interactable
{
    public GameObject berriesObject; // Refactor berries -> berriesObject
    private Vector3[] initialChildPositions;
    private Animation anime;
    private AnimationsControl animationsControl;

    private float timeToWait = 1.07f;

    private int objectIndicator = 1;

    protected override void Start()
    {
        base.Start();

        anime = GetComponent<Animation>();
        if (anime == null)
        {
            Debug.LogWarning("Нет компонента Animation на объекте Berries");
        }

        animationsControl = FindAnyObjectByType<AnimationsControl>();

        if (berriesObject != null)
        {
            Transform[] childTransforms = berriesObject.GetComponentsInChildren<Transform>();
            initialChildPositions = new Vector3[childTransforms.Length - 1];

            int i = 0;
            foreach (Transform child in childTransforms)
            {
                if (child != berriesObject.transform)
                {
                    initialChildPositions[i] = child.position;
                    i++;
                }
            }
        }

        if (tag != "berries") Debug.LogWarning("Тэг объекта не Berries");
    }
    protected override void PickupObject()
    {
        base.PickupObject();
        if (animationsControl.isFull)
        {
            animationsControl.CleanDust();
        }
    }
    public void ResetBerries()
    {
        berriesObject.SetActive(true);
        gameObject.GetComponent<Berries>().enabled = true;
    }
    protected override IEnumerator HandleObjectRelease()
    {
        // Если у объекта тег "berries", проигрываем анимацию и ждем ее завершения
        if (anime != null && animationsControl.IsNearCorrectBowl(this.gameObject))
        {
            anime.Play("BerriesAnimation");

            StartCoroutine(SetActiveBerries());
            // Ждем пока анимация не закончит проигрываться.
            yield return new WaitForSeconds(timeToWait);

            if (berriesObject != null && this.index > 0)
            {
                if (animationsControl.isFull)
                {
                    animationsControl.CleanDust();
                }
                animationsControl.ObjectsOn(this.index, objectIndicator);

            }
            DropObject();

            yield return StartCoroutine(animationsControl.PlayMortarAnimation(this.index, this.objectIndicator));
            gameLogic.AddToObjectsList(index, objectIndicator);
            gameObject.GetComponent<Berries>().enabled = false;
        }
        else
        {
            DropObject();
        }
    }
    private IEnumerator SetActiveBerries()
    {
        yield return new WaitForSeconds(timeToWait);

        berriesObject.SetActive(false);
    }
}
