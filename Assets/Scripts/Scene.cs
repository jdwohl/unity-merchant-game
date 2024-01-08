using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MerchantGame/Scene")]
public class Scene : ScriptableObject
{
    public int sceneIndex;
    public string sceneRoute = "a";

    public string[] sceneDialogue = new string[40];
    public int sceneDialogueIndex = 0;
    public int sceneQuestionIndex = 99;
    public bool sceneQuestionFlag = false;
    public string[] sceneQuestionOptions = new string[2];

    public int lockedOptionIndex = 99;
    public string statToCheck = "";
    public int statValue;

    public string lockedRouteChange = "";

    public int[] sceneJumps = new int[2];
    public int[] sceneEndIndices = new int[2];
    public bool sceneEndFlag = false;
    public bool rightBeforeBattle;
    public bool endOfDay = false;

    public bool changeMusicFlag = false;
    public int[] changeMusicIndices = new int[2];
    public string musicToPlay = "";

    public Customer[] characters = new Customer[7];
    public int[] characterTalking = new int[40];
    public int[] characterPortrait = new int[40];
    // the numbers correspond to who is talking -- customer number

    public string AdvanceSceneDialogue()
    {
        sceneDialogueIndex++;
        for (int i = 0; i < sceneEndIndices.Length; i++)
        {
            if(sceneDialogueIndex == sceneEndIndices[i])
            {
                sceneEndFlag = true;
            }
        }

        for (int i = 0; i < changeMusicIndices.Length; i++)
        {
            if (sceneDialogueIndex == changeMusicIndices[i])
            {
                changeMusicFlag = true;
            }
        }

        if (sceneDialogueIndex == sceneQuestionIndex)
        {
            sceneQuestionFlag = true;
        }


        return sceneDialogue[sceneDialogueIndex];
    }
}
