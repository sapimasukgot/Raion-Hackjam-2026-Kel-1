using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    

    // =========================
    // RATION
    // =========================

    public bool UseRation(int amount = 1)
    {
        if (GameManager.Instance.ration < amount)
        {
            return false;
        }

        GameManager.Instance.ration -= amount;
        return true;
    }

    public void AddRation(int amount)
    {
        GameManager.Instance.ration += amount;
    }


    // =========================
    // MEDKIT
    // =========================

    public bool UseMedkit(int amount = 1)
    {
        if (GameManager.Instance.medkit < amount)
        {
            return false;
        }

        GameManager.Instance.medkit -= amount;
        return true;
    }

    public void AddMedkit(int amount)
    {
        GameManager.Instance.medkit += amount;
    }


    // =========================
    // TOOLS
    // =========================

    public bool UseTools(int amount = 1)
    {
        if (GameManager.Instance.tools < amount)
        {
            return false;
        }

        GameManager.Instance.tools -= amount;
        return true;
    }

    public void AddTools(int amount)
    {
        GameManager.Instance.tools += amount;
    }


    // =========================
    // KNIFE
    // =========================

    public bool UseKnife()
    {
        if (!GameManager.Instance.knife)
        {
            return false;
        }

        GameManager.Instance.knife = false;
        return true;
    }

    public void AddKnife()
    {
        GameManager.Instance.knife = true;
    }
}