using UnityEngine;
using UnityEngine.UI;
using ScreenCaptureBlocker;

public class Sample : MonoBehaviour
{
    public GameObject text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ToggleUpdate(bool toggled)
    {
        if (toggled)
        {
            Capture.ProtectWindowContent();
        }
        else
        {
            Capture.UnprotectWindowContent();
        }
        text.SetActive(toggled);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += ((this.transform.forward * Input.GetAxis("Vertical")) + (this.transform.right * Input.GetAxis("Horizontal"))) * 10f * Time.deltaTime;
        transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0f);
    }
}
