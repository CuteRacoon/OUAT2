using UnityEngine;
using System.Collections;

public class Bowls : Interactable
{
    private Animation anime;
    private AnimationsManager animationsControl;

    protected void Awake()
    {
        base.Start();
        base.OutlineOn(false);
        anime = GetComponent<Animation>();
        if (anime == null)
        {
            Debug.LogWarning("Нет компонента Animation на объекте bowl");
        }

        animationsControl = FindAnyObjectByType<AnimationsManager>();

        if (tag != "bowl") Debug.LogWarning("Тэг объекта не bowl");
    }
    protected override IEnumerator HandleObjectRelease()
    {
        string animationName = null;
        switch (index)
        {
            case 1:
                animationName = "WaterBowlAnimation";
                break;
            case 3:
                animationName = "GrindBowlAnimation";
                break;
        }
        if (anime != null && animationsControl.IsNearCorrectBowl(this.gameObject))
        {
            anime.Play(animationName);
            yield return new WaitForSeconds(anime[animationName].length - 0.5f);
            animationsControl.CleanDust();

            if (index == 3)
            {
                if (!gameLogic.CheckNumberOfObjects())
                {
                    gameLogic.AccessHerbsAndBerriesInteraction(true);
                }
                else
                {
                    gameLogic.isGameOver = true;
                    gameLogic.CheckNumberOfObjects();
                }
                gameLogic.AccessBowls(3, false);
            }
            if (index == 1)
            {
                gameLogic.EndGame();
                Debug.Log("Вызываю завершение игры из Bowls");
            }
        }
        else
        {
            isReturning = true;
            DropObject();
        }
        DropObject();
    }
}
