using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public UIAutoAnimation uiAnimation;
    private CanvasGroup canvasGroup;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake(){
        canvasGroup = GetComponent<CanvasGroup>();
        uiAnimation = GetComponent<UIAutoAnimation>();
    }
    void Start()
    {
        uiAnimation.EntranceAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
