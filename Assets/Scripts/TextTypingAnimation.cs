using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TextTypingAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AlertText = AnimatableText.text;
        briefingTextLength = AlertText.Length;
        AnimatableText.text = "";
        textAnimator = TextTypeSlowly(0.05f);
    }


    public int briefingTextLength = 0;
    public GameObject Panel;
    public Text AnimatableText;
    public string AlertText;

    private IEnumerator textAnimator;

    // Update is called once per frame
    void Update()
    {

    }

    public bool _animate;
    public bool Animate
    {
        get { return _animate; }
        set
        {
            _animate = value;
            if (_animate)
            {
                StartCoroutine(textAnimator);
            }
            else
            {
                StopCoroutine(textAnimator);
            }
        }
    }

    IEnumerator TextTypeSlowly(float timeToWait)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < briefingTextLength; i++)
        {
            sb.Append(AlertText.Substring(i, 1));
            AnimatableText.text = sb.ToString();
            if (AlertText.Substring(i, 1) == "\n")
                yield return new WaitForSeconds(timeToWait * 10);
            else
                yield return new WaitForSeconds(timeToWait);
        }
    }

}
