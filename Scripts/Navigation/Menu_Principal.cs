using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Principal : MonoBehaviour
{
    public void Retourner()
    {
        Debug.Log("Coucou");
        SceneManager.LoadSceneAsync("Menu Principal");
    }
}
