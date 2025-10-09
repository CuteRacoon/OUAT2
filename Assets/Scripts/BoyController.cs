using UnityEngine;

public class BoyController : MonoBehaviour
{
    public static BoyController Instance { get; private set; }

    private Animator animator;
    Transform animTransform;
    private void Awake()
    {
        // Реализация синглтона
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Находим аниматор у вложенного объекта "boy_anim"
        animTransform = transform.Find("boy_anim");
        if (animTransform != null)
        {
            animator = animTransform.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("Не найден объект 'boy_anim' внутри " + gameObject.name);
        }
    }

    // Метод для перевода мальчика в сидячее положение
    public void SitDown()
    {
         animTransform.gameObject.SetActive(false);
         animator.SetBool("sitting", true);
    }
    public void HideBoy()
    {
        animTransform.gameObject.SetActive(false);
    }

    // Метод для перевода мальчика в лежачее положение
    public void LieDown()
    {
        if (animator != null)
        {
            animTransform.gameObject.SetActive(true);
            animator.SetBool("sitting", false);
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
}
