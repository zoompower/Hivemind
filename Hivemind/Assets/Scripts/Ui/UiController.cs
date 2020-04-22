using TMPro;
using UnityEngine;

public class UiController : MonoBehaviour
{
    public TextMeshProUGUI resourceTextBox;

    int tmp = 0;
    int tmp2 = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            tmp++;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            tmp2++;
        }

        resourceTextBox.text = FormatResource("1", tmp) + FormatResource("2", tmp2);
    }

    private string FormatResource(string name, int val)
    {
        return $" <sprite={name}> ({val}/999)";
    }
}
