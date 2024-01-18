using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{

    public GameObject blackOutSquare;
    public TextMeshProUGUI fadeText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator FadeInAndOutBlackSquare(int fadeSpeed = 2, string textToDisplay = "")
    {
        blackOutSquare.SetActive(true);
        fadeText.text = textToDisplay;
        fadeText.alpha = 255f;

        Color objectColor = blackOutSquare.GetComponent<Image>().color;
        float fadeAmount;

        while(blackOutSquare.GetComponent<Image>().color.a < 1)
        {
            fadeAmount = objectColor.a + (fadeSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            blackOutSquare.GetComponent<Image>().color = objectColor;
        }

        yield return new WaitForSeconds(1.5f);
        
        FadeText();

        yield return new WaitForSeconds(1);


        while (blackOutSquare.GetComponent<Image>().color.a > 0)
        {
            fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            blackOutSquare.GetComponent<Image>().color = objectColor;
        }

        blackOutSquare.SetActive(false);
    }

    public void FadeText()
    {
        fadeText.CrossFadeAlpha(0, 1f, true);
    }

}
