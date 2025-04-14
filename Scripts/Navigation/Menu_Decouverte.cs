using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Decouverte : MonoBehaviour
{
    public void Decouvrir()
    {
        SceneManager.LoadSceneAsync("Page Decouverte");
    }
}
