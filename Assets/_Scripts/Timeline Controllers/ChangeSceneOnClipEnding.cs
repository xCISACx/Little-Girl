using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class ChangeSceneOnClipEnding : PlayTimelineOnClipEnding
{
    public int NextSceneBuildIndex;
    
    public override void StartOtherTimeline()
    {
        _ThisTimeline.Stop();

        //Load the next scene
        GameObject.FindGameObjectWithTag("GameLoader").GetComponent<GameStateAndLoader>().ActivateLoadingScreen();
        StartCoroutine(GameObject.FindGameObjectWithTag("GameLoader").GetComponent<GameStateAndLoader>().LoadGame(NextSceneBuildIndex, true));
        //SceneManager.LoadScene(NextSceneBuildIndex);
    }
}
