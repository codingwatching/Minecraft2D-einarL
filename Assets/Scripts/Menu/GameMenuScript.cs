using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuScript : MonoBehaviour
{
    CanvasScript canvasScript;

    // Start is called before the first frame update
    void Start()
    {
        canvasScript = GameObject.Find("Canvas").GetComponent<CanvasScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void backToGame()
    {
        canvasScript.closeMenuAndResumeGame();
    }

    public void options()
    {
		SceneManager.LoadScene("Settings", LoadSceneMode.Additive);
	}

    public void SaveAndQuitToTitle()
    {
		canvasScript.closeMenuAndResumeGame(); // so that we unpause the game
		GameObject.Find("Main Camera").GetComponent<SaveScript>().save(); // save 
        unrenderAndSaveChunks(); // save chunks
		ArmorOutfitScript.removeInstance(); // this singleton needs to be reset when we go to another world, so we will do this
        if(!SpawningChunkData.lightingEnabled)GameObject.Find("Canvas").transform.Find("Chat").GetComponent<ChatScript>().toggleLights(true);
        SpawnTreeScript.resetVariables();

		JsonDataService.Instance.resetWorldFolder(); // reset world folder so we can later play another world
        SaveChunk.resetWorldFolder(); // reset world folder so we can later play another world
		SceneManager.LoadScene("TitleScreen"); // go to title screen
    }

    private void unrenderAndSaveChunks()
    {
        spawnChunkScript scScript = GameObject.Find("Main Camera").transform.GetComponent<spawnChunkScript>();
        scScript.saveWorldBeforeExiting();
	}
}
