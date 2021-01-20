using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayLevel1 : MonoBehaviour
{
    // Start is called before the first frame update
    public void PlayFirstGame(string scenename)
    {

        SceneManager.LoadScene(scenename);
    }

    public void RestartFirstGame(string scenename)
    {

        CharacterController2D.coin = 0;
        CharacterController2D.continuelives = 3;
        CharacterController2D.current_level = 1;
        //CharacterController2D.lives_text.text = "x" + " " + CharacterController2D.continuelives;
        SceneManager.LoadScene(scenename);
    }
}
