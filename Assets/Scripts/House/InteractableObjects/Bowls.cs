using UnityEngine;
using System.Collections;

public class Bowls : Interactable
{
    private Animation anime;
    private AnimationsControl animationsControl;

    protected void Awake()
    {
        base.Start();
        base.OutlineOn(false);
        anime = GetComponent<Animation>();
        if (anime == null)
        {
            Debug.LogWarning("��� ���������� Animation �� ������� bowl");
        }

        animationsControl = FindAnyObjectByType<AnimationsControl>();

        if (tag != "bowl") Debug.LogWarning("��� ������� �� bowl");
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

            }
            if (index == 1)
            {
                gameLogic.EndGame();
                Debug.Log("������� ���������� ���� �� Bowls");
            }
        }
        else
        {
            isReturning = true;
        }
        DropObject();
        if (index == 3) gameLogic.AccessBowls(3, false);
    }
}
