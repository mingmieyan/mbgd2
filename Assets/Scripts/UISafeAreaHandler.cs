using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class UISafeAreaHandler : MonoBehaviour
{
    RectTransform panel;

    IEnumerator Start()
    {
        panel = GetComponent<RectTransform>();
        while(true)
        {
            Rect area = Screen.safeArea;

            /* Pixel size in screen space of the whole screen */
            Vector2 screenSize = new Vector2(Screen.width, Screen.height);

            /* Set anchors to percentages of the screen used. */
            panel.anchorMin = area.position / screenSize;
            panel.anchorMax = (area.position + area.size) / screenSize;

            yield return new WaitForSeconds(0.25f);
        }
    }
 }
