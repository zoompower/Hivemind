using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float MovementSpeed;
    public int EdgeSize;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource != null && PlayerPrefs.HasKey("Volume"))
        {
            audioSource.volume = PlayerPrefs.GetFloat("Volume");
            SettingsScript.OnVolumeChanged += UpdateVolume;
        }
    }

    void Update()
    {
        if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height) return;

        var movement = new Vector3();

        movement.x += (Input.GetAxis("Horizontal") * MovementSpeed) * Time.deltaTime;
        movement.z += (Input.GetAxis("Vertical") * MovementSpeed) * Time.deltaTime;

        if (Input.mousePosition.x < EdgeSize)
        {
            movement.x -= MovementSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x > Screen.width - EdgeSize)
        {
            movement.x += MovementSpeed * Time.deltaTime;
        }

        if (Input.mousePosition.y < EdgeSize)
        {
            movement.z -= MovementSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y > Screen.height - EdgeSize)
        {
            movement.z += MovementSpeed * Time.deltaTime;
        }
        if (Time.timeScale == 0)
        {
            movement *= Time.timeScale;
        }

        transform.position += movement;
    }

    private void UpdateVolume(object sender, System.EventArgs e)
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            audioSource.volume = PlayerPrefs.GetFloat("Volume");
        }
    }
}
