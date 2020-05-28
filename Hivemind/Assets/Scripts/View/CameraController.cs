using System.Linq;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float MovementSpeed;
    public int EdgeSize;
    private Vector3 collisionSize = new Vector3(0.01f, 0.01f, 0.01f);
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

        var newPositionX = transform.position + new Vector3(movement.x, 0, 0);
        if (ValidatePosition(newPositionX))
        {
            transform.position = newPositionX;
        }
        var newPositionZ = transform.position + new Vector3(0, 0, movement.z);
        if (ValidatePosition(newPositionZ))
        {
            transform.position = newPositionZ;
        }
    }

    private void OnDestroy()
    {
        SettingsScript.OnVolumeChanged -= UpdateVolume;
    }

    private void UpdateVolume(object sender, System.EventArgs e)
    {
        if (audioSource != null)
        {
            if (PlayerPrefs.HasKey("Volume"))
            {
                audioSource.volume = PlayerPrefs.GetFloat("Volume");
            }
        }
    }

    private bool ValidatePosition(Vector3 newPos)
    {
        Collider[] colliders = Physics.OverlapBox(newPos, collisionSize);
        colliders = colliders.Where(x => x.CompareTag("MapBoundaries")).ToArray();
        if (colliders.Length < 1)
        {
            return false;
        }
        return true;
    }
}
