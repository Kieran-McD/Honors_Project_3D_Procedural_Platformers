using UnityEngine;
using UnityEngine.UI;
public class ToggleButton : MonoBehaviour
{
    public ObstacleStorage obstacles;
    public int index;
    public Image image;

    private void Start()
    {
        UpdateImage();
    }

    public void UpdateImage()
    {
        if (obstacles.obstacles[index].active)
        {
            image.color = Color.green;
        }
        else
        {
            image.color = Color.red;
        }
    }
}
